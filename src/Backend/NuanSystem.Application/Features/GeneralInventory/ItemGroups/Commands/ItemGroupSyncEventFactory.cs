using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;

internal static class ItemGroupSyncEventFactory
{
    public static SyncPublishRequest Create(
        int companyId,
        ItemGroupDto itemGroup,
        SyncOperation operation)
    {
        if (itemGroup.GlobalId is null || itemGroup.GlobalId == Guid.Empty)
        {
            throw new InvalidOperationException("ItemGroup requiere GlobalId para sincronizacion.");
        }

        var payload = new ItemGroupSyncPayload(
            itemGroup.GlobalId.Value,
            itemGroup.Code,
            itemGroup.Name,
            itemGroup.Description,
            itemGroup.InventoryAccountCode,
            itemGroup.CostOfSalesAccountCode,
            itemGroup.SalesAccountCode,
            itemGroup.PurchaseAccountCode,
            itemGroup.SapGroupCode,
            itemGroup.SapCode,
            operation is not SyncOperation.Disabled and not SyncOperation.Deleted && itemGroup.IsActive,
            itemGroup.ExternalSystem,
            itemGroup.ExternalCode,
            itemGroup.CreatedAt,
            itemGroup.UpdatedAt);

        return new SyncPublishRequest(
            companyId,
            SyncMasterBranchEntityCodes.ItemGroups,
            itemGroup.GlobalId.Value,
            itemGroup.Code,
            operation,
            payload,
            SourceSystem: itemGroup.ExternalSystem,
            SourceReference: itemGroup.Id.ToString());
    }
}
