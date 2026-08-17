using System.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Commands;

internal static class StorageConditionSyncEventFactory
{
    public static SyncPublishRequest Create(
        int companyId,
        StorageConditionDto condition,
        SyncOperation operation)
    {
        if (condition.GlobalId == Guid.Empty)
        {
            throw new InvalidOperationException("StorageCondition requiere GlobalId para sincronizacion.");
        }

        var isDeleted = operation == SyncOperation.Deleted;
        var payload = new StorageConditionSyncPayload(
            condition.GlobalId,
            condition.Code,
            condition.Name,
            condition.Description,
            condition.SortOrder,
            !isDeleted && operation != SyncOperation.Disabled && condition.IsActive,
            isDeleted,
            condition.UpdatedAt ?? condition.CreatedAt);

        return new SyncPublishRequest(
            companyId,
            SyncMasterBranchEntityCodes.StorageConditions,
            condition.GlobalId,
            condition.Code,
            operation,
            payload,
            SourceSystem: null,
            SourceReference: condition.Id.ToString());
    }
}

public sealed class StorageConditionLocalOutboxWriter(
    ICompanyContext companyContext,
    ISyncEventPayloadFactory payloadFactory,
    ILocalSyncOutboxRepository localOutboxRepository) : IStorageConditionLocalOutboxWriter
{
    public async Task<Guid?> EnqueueAsync(
        StorageConditionDto condition,
        SyncOperation operation,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        var company = companyContext.CurrentCompany;
        if (!companyContext.HasActiveCompany || company is null || !company.IsMaster || !company.SyncEnabled)
        {
            return null;
        }

        var publish = StorageConditionSyncEventFactory.Create(company.CompanyId, condition, operation);
        var eventId = Guid.NewGuid();
        await localOutboxRepository.CreateAsync(
            new CreateLocalSyncOutboxData(
                eventId,
                company.CompanyId,
                publish.EntityName,
                publish.EntityGlobalId,
                publish.EntityCode,
                operation,
                payloadFactory.CreatePayloadJson(publish)),
            connection,
            transaction,
            cancellationToken);
        return eventId;
    }
}
