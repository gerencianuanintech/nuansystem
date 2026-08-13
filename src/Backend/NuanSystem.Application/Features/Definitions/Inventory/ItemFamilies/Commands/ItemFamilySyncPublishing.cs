using System.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Commands;

internal static class ItemFamilySyncEventFactory
{
    public static SyncPublishRequest Create(int companyId, ItemFamilyDto item, SyncOperation operation)
    {
        if (item.GlobalId is null || item.GlobalId == Guid.Empty)
            throw new InvalidOperationException("ItemFamily requiere GlobalId para sincronizacion.");
        if (item.ItemGroupGlobalId is null || item.ItemGroupGlobalId == Guid.Empty)
            throw new InvalidOperationException("ItemFamily requiere ItemGroupGlobalId para sincronizacion.");

        var payload = new ItemFamilySyncPayload(
            item.GlobalId.Value, item.ItemGroupGlobalId.Value, item.ItemGroupCode,
            item.Code, item.Name, item.Description,
            operation is not SyncOperation.Disabled and not SyncOperation.Deleted && item.IsActive,
            item.SapFamilyCode, item.SapCode, item.ExternalSystem, item.ExternalCode,
            item.CreatedAt, item.UpdatedAt, item.SortOrder);

        return new SyncPublishRequest(companyId, SyncMasterBranchEntityCodes.ItemFamilies,
            item.GlobalId.Value, item.Code, operation, payload,
            SourceSystem: item.ExternalSystem, SourceReference: item.Id.ToString());
    }
}

public sealed class ItemFamilyLocalOutboxWriter(
    ICompanyContext companyContext,
    ISyncEventPayloadFactory payloadFactory,
    ILocalSyncOutboxRepository localOutboxRepository) : IItemFamilyLocalOutboxWriter
{
    public async Task<Guid?> EnqueueAsync(ItemFamilyDto item, SyncOperation operation,
        IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        var company = companyContext.CurrentCompany;
        if (!companyContext.HasActiveCompany || company is null || !company.IsMaster || !company.SyncEnabled)
            return null;
        var publish = ItemFamilySyncEventFactory.Create(company.CompanyId, item, operation);
        var eventId = Guid.NewGuid();
        var data = new CreateLocalSyncOutboxData(eventId, company.CompanyId, publish.EntityName,
            publish.EntityGlobalId, publish.EntityCode, operation, payloadFactory.CreatePayloadJson(publish));
        await localOutboxRepository.CreateAsync(data, connection, transaction, cancellationToken);
        return eventId;
    }
}
