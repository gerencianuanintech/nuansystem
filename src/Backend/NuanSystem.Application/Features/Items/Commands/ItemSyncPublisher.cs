using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Items.Commands;

internal static class ItemSyncPublisher
{
    private const string EntityName = "Item";

    public static async Task<Result<SyncPublishResult>?> PublishAsync(
        ISyncEventPublisher syncEventPublisher,
        ICompanyContext companyContext,
        ItemDto item,
        SyncOperation operation,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany || companyContext.CurrentCompany is null)
        {
            return null;
        }

        var payload = new ItemSyncPayload(
            item.GlobalId,
            item.Code,
            item.Name,
            item.Description,
            item.ItemType,
            item.ItemGroupId,
            item.ItemGroupCode,
            item.ItemFamilyId,
            item.ItemFamilyCode,
            item.InventoryUnitOfMeasureId,
            item.InventoryUnitOfMeasureCode,
            item.Barcodes.FirstOrDefault()?.Barcode,
            item.IsInventoryItem,
            item.IsSalesItem,
            item.IsPurchaseItem,
            item.IsActive,
            item.ExternalSystem,
            item.ExternalCode,
            item.SapCode);

        return await syncEventPublisher.PublishAsync(
            new SyncPublishRequest(
                companyContext.CurrentCompany.CompanyId,
                EntityName,
                item.GlobalId,
                item.Code,
                operation,
                payload,
                SourceSystem: null,
                SourceReference: item.Id.ToString()),
            cancellationToken);
    }
}
