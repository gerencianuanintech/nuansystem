using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Geography.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Geography.Commands;

internal static class CitySyncPublisher
{
    private const string EntityName = SyncMasterBranchEntityCodes.Cities;

    public static async Task<Result<SyncPublishResult>?> PublishAsync(
        ISyncEventPublisher syncEventPublisher,
        ICompanyContext companyContext,
        CityDto city,
        SyncOperation operation,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany || companyContext.CurrentCompany is null)
        {
            return null;
        }

        var payload = new CitySyncPayload(
            city.GlobalId,
            city.CountryGlobalId,
            city.CountryCode,
            city.ProvinceGlobalId,
            city.ProvinceCode,
            city.Code,
            city.Name,
            operation is not SyncOperation.Disabled and not SyncOperation.Deleted && city.IsActive,
            city.CreatedAt,
            city.UpdatedAt);

        return await syncEventPublisher.PublishAsync(
            new SyncPublishRequest(
                companyContext.CurrentCompany.CompanyId,
                EntityName,
                city.GlobalId,
                $"{city.CountryCode}|{city.ProvinceCode}|{city.Code}",
                operation,
                payload,
                SourceSystem: null,
                SourceReference: city.Id.ToString()),
            cancellationToken);
    }
}
