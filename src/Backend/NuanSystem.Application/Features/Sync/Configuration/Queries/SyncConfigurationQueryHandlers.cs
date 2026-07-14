using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Sync.Configuration.Queries;

public sealed class GetSyncProfilesQueryHandler(ISyncProfileRepository repository)
    : IQueryHandler<GetSyncProfilesQuery, PagedResultDto<SyncProfileListItemDto>>
{
    public async Task<Result<PagedResultDto<SyncProfileListItemDto>>> Handle(
        GetSyncProfilesQuery request,
        CancellationToken cancellationToken)
    {
        var result = await repository.SearchAsync(
            new SyncProfileListFilter(
                request.Search,
                request.CompanyId,
                request.IsActive,
                request.ExecutionMode,
                request.PageNumber,
                request.PageSize,
                request.UserId),
            cancellationToken);

        return Result<PagedResultDto<SyncProfileListItemDto>>.Success(result);
    }
}

public sealed class GetSyncProfileByIdQueryHandler(ISyncProfileRepository repository)
    : IQueryHandler<GetSyncProfileByIdQuery, SyncProfileApiDetailDto>
{
    public async Task<Result<SyncProfileApiDetailDto>> Handle(
        GetSyncProfileByIdQuery request,
        CancellationToken cancellationToken)
    {
        var profile = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (profile is null)
        {
            return Result<SyncProfileApiDetailDto>.Failure(
                "Perfil de sincronizacion no encontrado.",
                [new ApiError("SyncProfileNotFound", "El perfil no existe.", nameof(request.Id))]);
        }

        if (!await UserCanAccessCompanyAsync(repository, request.UserId, profile.CompanyId, cancellationToken))
        {
            return Result<SyncProfileApiDetailDto>.Failure(
                "No tiene acceso a la empresa maestra del perfil.",
                [new ApiError("SyncProfileCompanyAccessDenied", "La empresa maestra no esta permitida para el usuario.", nameof(profile.CompanyId))]);
        }

        return Result<SyncProfileApiDetailDto>.Success(SyncProfileMapper.ToApiDetail(profile));
    }

    private static async Task<bool> UserCanAccessCompanyAsync(
        ISyncProfileRepository repository,
        int? userId,
        int companyId,
        CancellationToken cancellationToken)
    {
        var companies = await repository.GetCompanyLookupsAsync(userId, cancellationToken);
        return companies.Any(company => company.Id == companyId);
    }
}

public sealed class GetSyncConfigurationCatalogQueryHandler(ISyncProfileRepository repository)
    : IQueryHandler<GetSyncConfigurationCatalogQuery, SyncConfigurationCatalogDto>
{
    public async Task<Result<SyncConfigurationCatalogDto>> Handle(
        GetSyncConfigurationCatalogQuery request,
        CancellationToken cancellationToken)
    {
        var companies = await repository.GetCompanyLookupsAsync(request.UserId, cancellationToken);
        var catalog = new SyncConfigurationCatalogDto
        {
            MasterCompanies = companies
                .Where(company => company.IsMaster && company.SyncEnabled)
                .Select(company => new CompanyLookupDto(company.Id, company.Code, company.Name, company.IsActive))
                .ToArray(),
            BranchCompanies = companies
                .Where(company => !company.IsMaster && company.SyncEnabled)
                .Select(company => new CompanyLookupDto(company.Id, company.Code, company.Name, company.IsActive))
                .ToArray(),
            Entities = SyncProfileMapper.ToEntityCatalog(),
            Directions = [new LookupItemDto("MasterToBranch", "Maestro a sucursal")],
            ExecutionModes =
            [
                new LookupItemDto("Incremental", "Incremental"),
                new LookupItemDto("Full", "Completa"),
                new LookupItemDto("Manual", "Manual")
            ],
            ConflictStrategies = [new LookupItemDto("MasterWins", "Prevalece maestro")],
            ScheduleTypes =
            [
                new LookupItemDto("Manual", "Manual"),
                new LookupItemDto("Interval", "Intervalo"),
                new LookupItemDto("Daily", "Diaria")
            ],
            DefaultTimeZoneId = "America/Guayaquil"
        };

        return Result<SyncConfigurationCatalogDto>.Success(catalog);
    }
}
