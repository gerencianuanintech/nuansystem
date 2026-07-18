using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;

internal static class ItemGroupSyncPublisher
{
    private const string EntityName = SyncMasterBranchEntityCodes.ItemGroups;

    public static async Task<Result<SyncPublishResult>?> PublishAsync(
        ISyncEventPublisher syncEventPublisher,
        ICompanyContext companyContext,
        ItemGroupDto itemGroup,
        SyncOperation operation,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany || companyContext.CurrentCompany is null)
        {
            return null;
        }

        if (itemGroup.GlobalId is null || itemGroup.GlobalId == Guid.Empty)
        {
            return Result<SyncPublishResult>.Failure(
                "El grupo de articulos no tiene GlobalId y no puede publicarse para sincronizacion.",
                [new ApiError("SYNC_ITEM_GROUP_GLOBAL_ID_REQUIRED", "ItemGroups requiere GlobalId.", nameof(itemGroup.GlobalId))]);
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

        return await syncEventPublisher.PublishAsync(
            new SyncPublishRequest(
                companyContext.CurrentCompany.CompanyId,
                EntityName,
                itemGroup.GlobalId.Value,
                itemGroup.Code,
                operation,
                payload,
                SourceSystem: itemGroup.ExternalSystem,
                SourceReference: itemGroup.Id.ToString()),
            cancellationToken);
    }
}
