using System.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Commands;

internal static class ItemLineSyncEventFactory
{
    public static SyncPublishRequest Create(int companyId, ItemLineDto item, SyncOperation operation)
    {
        if (item.GlobalId == Guid.Empty)
            throw new InvalidOperationException("ItemLine requiere GlobalId para sincronizacion.");

        var isDeleted = operation == SyncOperation.Deleted;
        var payload = new ItemLineSyncPayload(item.GlobalId, item.Code, item.Name, item.Description,
            item.SortOrder, !isDeleted && operation != SyncOperation.Disabled && item.IsActive,
            isDeleted, item.UpdatedAt ?? item.CreatedAt);

        return new SyncPublishRequest(companyId, SyncMasterBranchEntityCodes.ItemLines,
            item.GlobalId, item.Code, operation, payload, SourceSystem: null, SourceReference: item.Id.ToString());
    }
}

public sealed class ItemLineLocalOutboxWriter(
    ICompanyContext companyContext, ISyncEventPayloadFactory payloadFactory,
    ILocalSyncOutboxRepository localOutboxRepository) : IItemLineLocalOutboxWriter
{
    public async Task<Guid?> EnqueueAsync(ItemLineDto itemLine, SyncOperation operation,
        IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        var company = companyContext.CurrentCompany;
        if (!companyContext.HasActiveCompany || company is null || !company.IsMaster || !company.SyncEnabled)
            return null;

        var publish = ItemLineSyncEventFactory.Create(company.CompanyId, itemLine, operation);
        var eventId = Guid.NewGuid();
        await localOutboxRepository.CreateAsync(new CreateLocalSyncOutboxData(eventId, company.CompanyId,
            publish.EntityName, publish.EntityGlobalId, publish.EntityCode, operation,
            payloadFactory.CreatePayloadJson(publish)), connection, transaction, cancellationToken);
        return eventId;
    }
}
