using System.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.General.Countries.Commands;

public sealed class CountryLocalOutboxWriter(
    ICompanyContext companyContext,
    ISyncEventPayloadFactory payloadFactory,
    ILocalSyncOutboxRepository localOutboxRepository) : ICountryLocalOutboxWriter
{
    public async Task<Guid?> EnqueueAsync(
        CountryDto country,
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

        var request = CountrySyncEventFactory.Create(company.CompanyId, country, operation);
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
