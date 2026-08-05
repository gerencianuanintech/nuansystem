using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.General.Provinces.Commands;

internal static class ProvinceSyncEventFactory
{
    public static SyncPublishRequest Create(int companyId, ProvinceDto province, SyncOperation operation)
    {
        if (province.GlobalId == Guid.Empty || province.CountryGlobalId == Guid.Empty)
            throw new InvalidOperationException("Province requiere GlobalId y CountryGlobalId para sincronizacion.");

        var isDeleted = operation == SyncOperation.Deleted;
        var payload = new ProvinceSyncPayload(
            province.GlobalId, province.CountryGlobalId, province.CountryCode, province.Code, province.Name,
            !isDeleted && operation != SyncOperation.Disabled && province.IsActive,
            isDeleted, province.ExternalSystem, province.ExternalCode, province.CreatedAt, province.UpdatedAt);

        return new SyncPublishRequest(
            companyId, SyncMasterBranchEntityCodes.Provinces, province.GlobalId,
            $"{province.CountryCode}|{province.Code}", operation, payload,
            SourceSystem: province.ExternalSystem, SourceReference: province.Id.ToString());
    }
}
