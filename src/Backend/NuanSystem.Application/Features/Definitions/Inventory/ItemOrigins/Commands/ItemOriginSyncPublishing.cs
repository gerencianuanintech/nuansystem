using System.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Commands;

internal static class ItemOriginSyncEventFactory
{
    public static SyncPublishRequest Create(int companyId, ItemOriginDto item, SyncOperation operation)
    {
        if (item.GlobalId == Guid.Empty)
            throw new InvalidOperationException("ItemOrigin requiere GlobalId para sincronizacion.");

        var isDeleted = operation == SyncOperation.Deleted;
        var payload = new ItemOriginSyncPayload(item.GlobalId, item.Code, item.Name, item.Description,
            item.SortOrder, !isDeleted && operation != SyncOperation.Disabled && item.IsActive,
            isDeleted, item.UpdatedAt ?? item.CreatedAt);
        return new SyncPublishRequest(companyId, SyncMasterBranchEntityCodes.ItemOrigins,
            item.GlobalId, item.Code, operation, payload, SourceSystem: null,
            SourceReference: item.Id.ToString());
    }
}

public sealed class ItemOriginLocalOutboxWriter(
    ICompanyContext companyContext, ISyncEventPayloadFactory payloadFactory,
    ILocalSyncOutboxRepository localOutboxRepository) : IItemOriginLocalOutboxWriter
{
    public async Task<Guid?> EnqueueAsync(ItemOriginDto itemOrigin, SyncOperation operation,
        IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        var company = companyContext.CurrentCompany;
        if (!companyContext.HasActiveCompany || company is null || !company.IsMaster || !company.SyncEnabled)
            return null;

        var publish = ItemOriginSyncEventFactory.Create(company.CompanyId, itemOrigin, operation);
        var eventId = Guid.NewGuid();
        await localOutboxRepository.CreateAsync(new CreateLocalSyncOutboxData(eventId, company.CompanyId,
            publish.EntityName, publish.EntityGlobalId, publish.EntityCode, operation,
            payloadFactory.CreatePayloadJson(publish)), connection, transaction, cancellationToken);
        return eventId;
    }
}
