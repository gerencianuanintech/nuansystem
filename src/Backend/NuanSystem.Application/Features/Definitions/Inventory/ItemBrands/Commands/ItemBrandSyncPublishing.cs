using System.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Commands;

internal static class ItemBrandSyncEventFactory
{
    public static SyncPublishRequest Create(int companyId, ItemBrandDto item, SyncOperation operation)
    {
        if (item.GlobalId == Guid.Empty)
            throw new InvalidOperationException("ItemBrand requiere GlobalId para sincronizacion.");

        var isDeleted = operation == SyncOperation.Deleted;
        var payload = new ItemBrandSyncPayload(
            item.GlobalId, item.Code, item.Name, item.Description, item.SortOrder,
            !isDeleted && operation != SyncOperation.Disabled && item.IsActive,
            isDeleted, item.UpdatedAt ?? item.CreatedAt);

        return new SyncPublishRequest(companyId, SyncMasterBranchEntityCodes.ItemBrands,
            item.GlobalId, item.Code, operation, payload,
            SourceSystem: item.ExternalSystem, SourceReference: item.Id.ToString());
    }
}

public sealed class ItemBrandLocalOutboxWriter(
    ICompanyContext companyContext,
    ISyncEventPayloadFactory payloadFactory,
    ILocalSyncOutboxRepository localOutboxRepository) : IItemBrandLocalOutboxWriter
{
    public async Task<Guid?> EnqueueAsync(ItemBrandDto itemBrand, SyncOperation operation,
        IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        var company = companyContext.CurrentCompany;
        if (!companyContext.HasActiveCompany || company is null || !company.IsMaster || !company.SyncEnabled)
            return null;

        var publish = ItemBrandSyncEventFactory.Create(company.CompanyId, itemBrand, operation);
        var eventId = Guid.NewGuid();
        var data = new CreateLocalSyncOutboxData(eventId, company.CompanyId, publish.EntityName,
            publish.EntityGlobalId, publish.EntityCode, operation, payloadFactory.CreatePayloadJson(publish));
        await localOutboxRepository.CreateAsync(data, connection, transaction, cancellationToken);
        return eventId;
    }
}
