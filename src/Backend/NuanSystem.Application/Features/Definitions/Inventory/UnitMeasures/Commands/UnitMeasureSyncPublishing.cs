using System.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Commands;

internal static class UnitMeasureSyncEventFactory
{
    public static SyncPublishRequest Create(int companyId, UnitMeasureDto item, SyncOperation operation)
    {
        if (item.GlobalId == Guid.Empty)
            throw new InvalidOperationException("UnitOfMeasure requiere GlobalId para sincronizacion.");

        var isDeleted = operation == SyncOperation.Deleted;
        var payload = new UnitMeasureSyncPayload(item.GlobalId, item.Code, item.Name, item.Description,
            item.Symbol, item.MagnitudeCode, item.SortOrder,
            !isDeleted && operation != SyncOperation.Disabled && item.IsActive,
            isDeleted, item.UpdatedAt ?? item.CreatedAt);
        return new SyncPublishRequest(companyId, SyncMasterBranchEntityCodes.UnitOfMeasures,
            item.GlobalId, item.Code, operation, payload,
            SourceSystem: item.ExternalSystem, SourceReference: item.Id.ToString());
    }
}

public sealed class UnitMeasureLocalOutboxWriter(
    ICompanyContext companyContext, ISyncEventPayloadFactory payloadFactory,
    ILocalSyncOutboxRepository localOutboxRepository) : IUnitMeasureLocalOutboxWriter
{
    public async Task<Guid?> EnqueueAsync(UnitMeasureDto unitMeasure, SyncOperation operation,
        IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        var company = companyContext.CurrentCompany;
        if (!companyContext.HasActiveCompany || company is null || !company.IsMaster || !company.SyncEnabled)
            return null;

        var publish = UnitMeasureSyncEventFactory.Create(company.CompanyId, unitMeasure, operation);
        var eventId = Guid.NewGuid();
        var data = new CreateLocalSyncOutboxData(eventId, company.CompanyId, publish.EntityName,
            publish.EntityGlobalId, publish.EntityCode, operation, payloadFactory.CreatePayloadJson(publish));
        await localOutboxRepository.CreateAsync(data, connection, transaction, cancellationToken);
        return eventId;
    }
}
