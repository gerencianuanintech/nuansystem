using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Profiles.Services;

namespace NuanSystem.Application.Features.SapSync.Profiles.Queries;

public sealed class GetSapSyncProfilesQueryHandler(
    ISapSyncProfileRepository repository,
    ISapSyncProfileValidationService validationService)
    : IQueryHandler<GetSapSyncProfilesQuery, SapSyncPagedResult<SapSyncProfileListItemDto>>
{
    public async Task<Result<SapSyncPagedResult<SapSyncProfileListItemDto>>> Handle(
        GetSapSyncProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        if (filter.CompanyId is not null)
        {
            var access = await validationService.ValidateCompanyAsync(
                filter.CompanyId.Value,
                request.UserId,
                requireSapReady: false,
                cancellationToken);
            if (!access.IsSuccess)
            {
                return SapSyncProfileResults.FromFailure<
                    SapSyncProfileCompanyAccessDto,
                    SapSyncPagedResult<SapSyncProfileListItemDto>>(access);
            }

            var result = await repository.SearchAsync(ToRepositoryFilter(filter), cancellationToken);
            return Result<SapSyncPagedResult<SapSyncProfileListItemDto>>.Success(result);
        }

        var allowedCompanies = (await repository.GetCompanyAccessAsync(
                request.UserId,
                cancellationToken: cancellationToken))
            .Where(company => company.IsUserAuthorized)
            .Select(company => company.CompanyId)
            .ToArray();
        if (allowedCompanies.Length == 0)
        {
            return Result<SapSyncPagedResult<SapSyncProfileListItemDto>>.Success(
                new SapSyncPagedResult<SapSyncProfileListItemDto>(
                    Array.Empty<SapSyncProfileListItemDto>(),
                    0,
                    filter.PageNumber,
                    filter.PageSize));
        }

        var allItems = new List<SapSyncProfileListItemDto>();
        foreach (var companyId in allowedCompanies)
        {
            var pageNumber = 1;
            SapSyncPagedResult<SapSyncProfileListItemDto> page;
            do
            {
                page = await repository.SearchAsync(
                    new SapSyncProfileFilter(
                        companyId,
                        filter.Search,
                        filter.IsActive,
                        filter.EntityCode,
                        pageNumber,
                        500),
                    cancellationToken);
                allItems.AddRange(page.Items);
                pageNumber++;
            }
            while (allItems.Count(item => item.CompanyId == companyId) < page.TotalCount);
        }

        var ordered = allItems
            .OrderByDescending(item => item.CreatedAtUtc)
            .ThenByDescending(item => item.Id)
            .ToArray();
        var items = ordered
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToArray();

        return Result<SapSyncPagedResult<SapSyncProfileListItemDto>>.Success(
            new SapSyncPagedResult<SapSyncProfileListItemDto>(
                items,
                ordered.Length,
                filter.PageNumber,
                filter.PageSize));
    }

    private static SapSyncProfileFilter ToRepositoryFilter(SapSyncProfileListRequest filter) =>
        new(
            filter.CompanyId,
            filter.Search,
            filter.IsActive,
            filter.EntityCode,
            filter.PageNumber,
            filter.PageSize);
}

public sealed class GetSapSyncProfileByIdQueryHandler(
    ISapSyncProfileRepository repository,
    ISapSyncProfileValidationService validationService)
    : IQueryHandler<GetSapSyncProfileByIdQuery, SapSyncProfileDto>
{
    public async Task<Result<SapSyncProfileDto>> Handle(
        GetSapSyncProfileByIdQuery request,
        CancellationToken cancellationToken)
    {
        var profile = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (profile is null)
        {
            return SapSyncProfileResults.NotFound<SapSyncProfileDto>(request.Id);
        }

        var access = await validationService.ValidateCompanyAsync(
            profile.CompanyId,
            request.UserId,
            requireSapReady: false,
            cancellationToken);
        return access.IsSuccess
            ? Result<SapSyncProfileDto>.Success(profile.ToApiDto())
            : SapSyncProfileResults.FromFailure<SapSyncProfileCompanyAccessDto, SapSyncProfileDto>(access);
    }
}

public sealed class GetSapSyncProfileCatalogQueryHandler(ISapSyncProfileRepository repository)
    : IQueryHandler<GetSapSyncProfileCatalogQuery, SapSyncProfileCatalogDto>
{
    public async Task<Result<SapSyncProfileCatalogDto>> Handle(
        GetSapSyncProfileCatalogQuery request,
        CancellationToken cancellationToken)
    {
        var companies = (await repository.GetCompanyAccessAsync(
                request.UserId,
                cancellationToken: cancellationToken))
            .Where(company =>
                company.IsUserAuthorized
                && company.IsCompanyActive
                && company.SapIntegrationMode != 0
                && company.HasSapSettings
                && company.IsSapEnabled
                && company.SapSettingsIntegrationMode != 0)
            .Select(company => new SapSyncProfileCompanyDto(
                company.CompanyId,
                company.CompanyCode,
                company.CompanyName))
            .ToArray();
        var capabilities = (await repository.GetHandlerCapabilitiesAsync(
                activeOnly: true,
                cancellationToken))
            .Where(capability =>
                capability.IsImplemented
                && !capability.EntityCode.Equals("PurchaseOrders", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var directions = new List<SapSyncProfileCatalogItemDto>();
        if (capabilities.Any(capability => capability.SupportsSapToErp))
        {
            directions.Add(new SapSyncProfileCatalogItemDto("SapToErp", "SAP a ERP"));
        }
        if (capabilities.Any(capability => capability.SupportsErpToSap))
        {
            directions.Add(new SapSyncProfileCatalogItemDto("ErpToSap", "ERP a SAP"));
        }

        return Result<SapSyncProfileCatalogDto>.Success(new SapSyncProfileCatalogDto(
            companies,
            capabilities,
            directions,
            [
                new SapSyncProfileCatalogItemDto(SapSyncModes.Full, "Completa"),
                new SapSyncProfileCatalogItemDto(SapSyncModes.Incremental, "Incremental")
            ],
            [
                new SapSyncProfileCatalogItemDto(SapSyncScheduleTypes.Manual, "Manual"),
                new SapSyncProfileCatalogItemDto(SapSyncScheduleTypes.Interval, "Intervalo"),
                new SapSyncProfileCatalogItemDto(SapSyncScheduleTypes.Daily, "Diaria")
            ],
            SapSyncProfileValidationService.DefaultTimeZoneId));
    }
}
