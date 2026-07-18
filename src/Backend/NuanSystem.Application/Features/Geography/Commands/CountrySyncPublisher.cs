using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Geography.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Geography.Commands;

internal static class CountrySyncPublisher
{
    private const string EntityName = SyncMasterBranchEntityCodes.Countries;

    public static async Task<Result<SyncPublishResult>?> PublishAsync(
        ISyncEventPublisher syncEventPublisher,
        ICompanyContext companyContext,
        CountryDto country,
        SyncOperation operation,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany || companyContext.CurrentCompany is null)
        {
            return null;
        }

        var payload = new CountrySyncPayload(
            country.GlobalId,
            country.Code,
            country.Name,
            country.Iso2,
            country.Iso3,
            country.PhonePrefix,
            operation is not SyncOperation.Disabled and not SyncOperation.Deleted && country.IsActive,
            country.CreatedAt,
            country.UpdatedAt);

        return await syncEventPublisher.PublishAsync(
            new SyncPublishRequest(
                companyContext.CurrentCompany.CompanyId,
                EntityName,
                country.GlobalId,
                country.Code,
                operation,
                payload,
                SourceSystem: null,
                SourceReference: country.Id.ToString()),
            cancellationToken);
    }
}
