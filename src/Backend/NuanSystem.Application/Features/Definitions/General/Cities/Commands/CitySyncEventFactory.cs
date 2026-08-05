using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.General.Cities.Commands;

internal static class CitySyncEventFactory
{
    public static SyncPublishRequest Create(int companyId, CityDto city, SyncOperation operation)
    {
        if (city.GlobalId == Guid.Empty || city.CountryGlobalId == Guid.Empty || city.ProvinceGlobalId == Guid.Empty)
        {
            throw new InvalidOperationException("City requiere GlobalId, CountryGlobalId y ProvinceGlobalId para sincronizacion.");
        }

        var isDeleted = operation == SyncOperation.Deleted;
        var payload = new CitySyncPayload(
            city.GlobalId,
            city.CountryGlobalId,
            city.CountryCode,
            city.ProvinceGlobalId,
            city.ProvinceCode,
            city.Code,
            city.Name,
            !isDeleted && operation != SyncOperation.Disabled && city.IsActive,
            isDeleted,
            city.ExternalSystem,
            city.ExternalCode,
            city.CreatedAt,
            city.UpdatedAt);

        return new SyncPublishRequest(
            companyId,
            SyncMasterBranchEntityCodes.Cities,
            city.GlobalId,
            $"{city.CountryCode}|{city.ProvinceCode}|{city.Code}",
            operation,
            payload,
            SourceSystem: city.ExternalSystem,
            SourceReference: city.Id.ToString());
    }
}
