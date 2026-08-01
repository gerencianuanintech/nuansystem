using System.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Carriers.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Carriers.Commands;

public sealed class CarrierLocalOutboxWriter(
    ICompanyContext companyContext,
    ISyncEventPayloadFactory payloadFactory,
    ILocalSyncOutboxRepository localOutboxRepository) : ICarrierLocalOutboxWriter
{
    public async Task<Guid?> EnqueueAsync(
        CarrierDetailDto carrier,
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

        var request = CarrierSyncEventFactory.Create(company.CompanyId, carrier, operation);
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
