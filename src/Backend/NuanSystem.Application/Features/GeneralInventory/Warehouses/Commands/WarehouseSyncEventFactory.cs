using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;

public static class WarehouseSyncEventFactory
{
    public static SyncPublishRequest Create(int companyId, WarehouseDto warehouse, SyncOperation operation)
    {
        if (warehouse.GlobalId == Guid.Empty)
        {
            throw new InvalidOperationException("Warehouse requiere GlobalId para sincronizacion.");
        }

        var payload = new WarehouseSyncPayload(
            warehouse.GlobalId,
            warehouse.Code,
            warehouse.Name,
            operation is not SyncOperation.Disabled and not SyncOperation.Deleted && warehouse.IsActive,
            warehouse.ExternalSystem,
            warehouse.ExternalCode,
            warehouse.SapCode,
            warehouse.CreatedAt,
            warehouse.UpdatedAt);

        return new SyncPublishRequest(
            companyId,
            SyncMasterBranchEntityCodes.Warehouse,
            warehouse.GlobalId,
            warehouse.Code,
            operation,
            payload,
            SourceSystem: warehouse.ExternalSystem,
            SourceReference: warehouse.Id.ToString());
    }
}
