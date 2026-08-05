using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.General.Countries.Commands;

internal static class CountrySyncEventFactory
{
    public static SyncPublishRequest Create(int companyId, CountryDto country, SyncOperation operation)
    {
        if (country.GlobalId == Guid.Empty)
        {
            throw new InvalidOperationException("Country requiere GlobalId para sincronizacion.");
        }

        var isDeleted = operation == SyncOperation.Deleted;
        var payload = new CountrySyncPayload(
            country.GlobalId,
            country.Code,
            country.Name,
            country.Iso2,
            country.Iso3,
            country.PhonePrefix,
            !isDeleted && operation != SyncOperation.Disabled && country.IsActive,
            isDeleted,
            country.ExternalSystem,
            country.ExternalCode,
            country.CreatedAt,
            country.UpdatedAt);

        return new SyncPublishRequest(
            companyId,
            SyncMasterBranchEntityCodes.Countries,
            country.GlobalId,
            country.Code,
            operation,
            payload,
            SourceSystem: country.ExternalSystem,
            SourceReference: country.Id.ToString());
    }
}
