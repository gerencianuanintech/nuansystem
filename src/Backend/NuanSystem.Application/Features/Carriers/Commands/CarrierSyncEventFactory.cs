using NuanSystem.Application.Features.Carriers.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Carriers.Commands;

internal static class CarrierSyncEventFactory
{
    public static SyncPublishRequest Create(
        int companyId,
        CarrierDetailDto carrier,
        SyncOperation operation)
    {
        if (carrier.GlobalId is null || carrier.GlobalId == Guid.Empty)
        {
            throw new InvalidOperationException("Carrier requiere GlobalId para sincronizacion.");
        }

        var isDeleted = operation == SyncOperation.Deleted;
        var payload = new CarrierSyncPayloadV1(
            carrier.GlobalId.Value,
            carrier.Code,
            carrier.Name,
            carrier.IdentificationTypeCode,
            carrier.IdentificationNumber,
            carrier.Description,
            operation is not SyncOperation.Disabled and not SyncOperation.Deleted && carrier.IsActive,
            isDeleted,
            carrier.CreatedAt,
            carrier.UpdatedAt);

        return new SyncPublishRequest(
            companyId,
            SyncMasterBranchEntityCodes.Carrier,
            carrier.GlobalId.Value,
            carrier.Code,
            operation,
            payload,
            SourceSystem: null,
            SourceReference: carrier.Id.ToString());
    }
}
