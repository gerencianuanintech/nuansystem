using NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Commands;

internal static class ItemFamilySyncEventFactory
{
    public static SyncPublishRequest Create(
        int companyId,
        ItemFamilyDto itemFamily,
        SyncOperation operation)
    {
        if (itemFamily.GlobalId is null || itemFamily.GlobalId == Guid.Empty)
        {
            throw new InvalidOperationException("ItemFamily requiere GlobalId para sincronizacion.");
        }

        if (itemFamily.ItemGroupGlobalId is null || itemFamily.ItemGroupGlobalId == Guid.Empty)
        {
            throw new InvalidOperationException("ItemFamily requiere ItemGroupGlobalId para sincronizacion.");
        }

        var payload = new ItemFamilySyncPayload(
            itemFamily.GlobalId.Value,
            itemFamily.ItemGroupGlobalId.Value,
            itemFamily.ItemGroupCode,
            itemFamily.Code,
            itemFamily.Name,
            itemFamily.Description,
            operation is not SyncOperation.Disabled and not SyncOperation.Deleted && itemFamily.IsActive,
            itemFamily.SapFamilyCode,
            itemFamily.SapCode,
            itemFamily.ExternalSystem,
            itemFamily.ExternalCode,
            itemFamily.CreatedAt,
            itemFamily.UpdatedAt);

        return new SyncPublishRequest(
            companyId,
            SyncMasterBranchEntityCodes.ItemFamilies,
            itemFamily.GlobalId.Value,
            itemFamily.Code,
            operation,
            payload,
            SourceSystem: itemFamily.ExternalSystem,
            SourceReference: itemFamily.Id.ToString());
    }
}
