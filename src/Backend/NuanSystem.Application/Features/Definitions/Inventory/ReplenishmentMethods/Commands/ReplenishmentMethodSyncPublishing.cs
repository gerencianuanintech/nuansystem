using System.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Commands;

internal static class ReplenishmentMethodSyncEventFactory
{
    public static SyncPublishRequest Create(int companyId, ReplenishmentMethodDto method,
        SyncOperation operation)
    {
        if (method.GlobalId == Guid.Empty)
            throw new InvalidOperationException("ReplenishmentMethod requiere GlobalId para sincronizacion.");

        var isDeleted = operation == SyncOperation.Deleted;
        var payload = new ReplenishmentMethodSyncPayload(method.GlobalId, method.Code, method.Name,
            method.Description, method.SortOrder,
            !isDeleted && operation != SyncOperation.Disabled && method.IsActive,
            isDeleted, method.UpdatedAt ?? method.CreatedAt);
        return new SyncPublishRequest(companyId, SyncMasterBranchEntityCodes.ReplenishmentMethods,
            method.GlobalId, method.Code, operation, payload, SourceSystem: null,
            SourceReference: method.Id.ToString());
    }
}

public sealed class ReplenishmentMethodLocalOutboxWriter(
    ICompanyContext companyContext, ISyncEventPayloadFactory payloadFactory,
    ILocalSyncOutboxRepository localOutboxRepository) : IReplenishmentMethodLocalOutboxWriter
{
    public async Task<Guid?> EnqueueAsync(ReplenishmentMethodDto method, SyncOperation operation,
        IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        var company = companyContext.CurrentCompany;
        if (!companyContext.HasActiveCompany || company is null || !company.IsMaster || !company.SyncEnabled)
            return null;

        var publish = ReplenishmentMethodSyncEventFactory.Create(company.CompanyId, method, operation);
        var eventId = Guid.NewGuid();
        await localOutboxRepository.CreateAsync(new CreateLocalSyncOutboxData(eventId, company.CompanyId,
            publish.EntityName, publish.EntityGlobalId, publish.EntityCode, operation,
            payloadFactory.CreatePayloadJson(publish)), connection, transaction, cancellationToken);
        return eventId;
    }
}
