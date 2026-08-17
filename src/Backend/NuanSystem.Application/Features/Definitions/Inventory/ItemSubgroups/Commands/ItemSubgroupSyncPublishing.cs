using System.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Commands;

internal static class ItemSubgroupSyncEventFactory
{
    public static SyncPublishRequest Create(int companyId, ItemSubgroupDto item, SyncOperation operation)
    {
        if (item.GlobalId is null || item.GlobalId == Guid.Empty)
            throw new InvalidOperationException("ItemSubgroup requiere GlobalId para sincronización.");
        if (item.ItemFamilyGlobalId is null || item.ItemFamilyGlobalId == Guid.Empty)
            throw new InvalidOperationException("ItemSubgroup requiere ItemFamilyGlobalId para sincronización.");

        var payload = new ItemSubgroupSyncPayload(
            item.GlobalId.Value, item.ItemFamilyGlobalId.Value, item.ItemFamilyCode,
            item.Code, item.Name, item.Description, item.SortOrder,
            operation is not SyncOperation.Disabled and not SyncOperation.Deleted && item.IsActive,
            operation is SyncOperation.Deleted,
            item.CreatedAt, item.UpdatedAt);

        return new SyncPublishRequest(companyId, SyncMasterBranchEntityCodes.ItemSubgroups,
            item.GlobalId.Value, $"{item.ItemFamilyCode}|{item.Code}", operation, payload,
            SourceSystem: null, SourceReference: item.Id.ToString());
    }
}

public sealed class ItemSubgroupLocalOutboxWriter(
    ICompanyContext companyContext,
    ISyncEventPayloadFactory payloadFactory,
    ILocalSyncOutboxRepository localOutboxRepository) : IItemSubgroupLocalOutboxWriter
{
    public async Task<Guid?> EnqueueAsync(ItemSubgroupDto item, SyncOperation operation,
        IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        var company = companyContext.CurrentCompany;
        if (!companyContext.HasActiveCompany || company is null || !company.IsMaster || !company.SyncEnabled)
            return null;

        var publish = ItemSubgroupSyncEventFactory.Create(company.CompanyId, item, operation);
        var eventId = Guid.NewGuid();
        var data = new CreateLocalSyncOutboxData(eventId, company.CompanyId, publish.EntityName,
            publish.EntityGlobalId, publish.EntityCode, operation, payloadFactory.CreatePayloadJson(publish));
        await localOutboxRepository.CreateAsync(data, connection, transaction, cancellationToken);
        return eventId;
    }
}
