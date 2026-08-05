using System.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.General.Provinces.Commands;

public sealed class ProvinceLocalOutboxWriter(ICompanyContext companyContext, ISyncEventPayloadFactory payloadFactory, ILocalSyncOutboxRepository localOutboxRepository) : IProvinceLocalOutboxWriter
{
    public async Task<Guid?> EnqueueAsync(ProvinceDto province, SyncOperation operation, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        var company = companyContext.CurrentCompany;
        if (!companyContext.HasActiveCompany || company is null || !company.IsMaster || !company.SyncEnabled) return null;
        var request = ProvinceSyncEventFactory.Create(company.CompanyId, province, operation);
        var eventId = Guid.NewGuid();
        await localOutboxRepository.CreateAsync(new CreateLocalSyncOutboxData(
            eventId, company.CompanyId, request.EntityName, request.EntityGlobalId, request.EntityCode,
            operation, payloadFactory.CreatePayloadJson(request)), connection, transaction, cancellationToken);
        return eventId;
    }
}
