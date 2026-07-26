using System.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;

public sealed class WarehouseLocalOutboxWriter(
    ICompanyContext companyContext,
    ISyncEventPayloadFactory payloadFactory,
    ILocalSyncOutboxRepository localOutboxRepository) : IWarehouseLocalOutboxWriter
{
    public async Task<Guid?> EnqueueAsync(
        WarehouseDto warehouse,
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

        var request = WarehouseSyncEventFactory.Create(company.CompanyId, warehouse, operation);
        var eventId = Guid.NewGuid();
        await localOutboxRepository.CreateAsync(
            new CreateLocalSyncOutboxData(
                eventId,
                company.CompanyId,
                request.EntityName,
                request.EntityGlobalId,
                request.EntityCode,
                operation,
                payloadFactory.CreatePayloadJson(request)),
            connection,
            transaction,
            cancellationToken);
        return eventId;
    }
}
