using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Items.Commands;

internal static class ItemSyncEventFactory
{
    public static SyncPublishRequest Create(
        int companyId,
        ItemDto item,
        SyncOperation operation)
    {
        var payload = new ItemSyncPayload(
            item.GlobalId,
            item.Code,
            item.Name,
            item.Description,
            item.ItemType,
            item.ItemGroupGlobalId,
            item.ItemGroupCode,
            item.ItemFamilyGlobalId,
            item.ItemFamilyCode,
            item.InventoryUnitOfMeasureGlobalId,
            item.InventoryUnitOfMeasureCode,
            item.PurchaseUnitOfMeasureGlobalId,
            item.PurchaseUnitOfMeasureCode,
            item.SalesUnitOfMeasureGlobalId,
            item.SalesUnitOfMeasureCode,
            item.Barcodes.FirstOrDefault()?.Barcode,
            item.IsInventoryItem,
            item.IsSalesItem,
            item.IsPurchaseItem,
            item.IsActive,
            item.ExternalSystem,
            item.ExternalCode,
            item.SapCode);

        return new SyncPublishRequest(
            companyId,
            SyncMasterBranchEntityCodes.Item,
            item.GlobalId,
            item.Code,
            operation,
            payload,
            SourceSystem: null,
            SourceReference: item.Id.ToString());
    }
}
