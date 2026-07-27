using System.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.TaxCatalogs.Taxes.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.TaxCatalogs.Taxes.Commands;

public sealed class TaxLocalOutboxWriter(
    ICompanyContext companyContext,
    ISyncEventPayloadFactory payloadFactory,
    ILocalSyncOutboxRepository localOutboxRepository) : ITaxLocalOutboxWriter
{
    public async Task<Guid?> EnqueueAsync(TaxDto tax, SyncOperation operation,
        IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        var company = companyContext.CurrentCompany;
        if (!companyContext.HasActiveCompany || company is null || !company.IsMaster || !company.SyncEnabled)
            return null;

        var payload = new TaxSyncPayloadV1(
            tax.GlobalId, tax.Code, tax.Name, tax.Description, tax.Rate,
            operation is not SyncOperation.Disabled and not SyncOperation.Deleted && tax.IsActive,
            tax.ExternalSystem, tax.ExternalCode, tax.CreatedAt, tax.UpdatedAt);
        var request = new SyncPublishRequest(
            company.CompanyId, SyncMasterBranchEntityCodes.Taxes, tax.GlobalId, tax.Code,
            operation, payload, tax.ExternalSystem, tax.Id.ToString());
        var eventId = Guid.NewGuid();
        await localOutboxRepository.CreateAsync(new CreateLocalSyncOutboxData(
            eventId, company.CompanyId, request.EntityName, request.EntityGlobalId,
            request.EntityCode, operation, payloadFactory.CreatePayloadJson(request)),
            connection, transaction, cancellationToken);
        return eventId;
    }
}
