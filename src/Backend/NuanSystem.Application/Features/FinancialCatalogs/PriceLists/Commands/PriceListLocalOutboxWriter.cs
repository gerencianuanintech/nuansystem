using System.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Commands;

public sealed class PriceListLocalOutboxWriter(
    ICompanyContext companyContext,
    ISyncEventPayloadFactory payloadFactory,
    ILocalSyncOutboxRepository localOutboxRepository) : IPriceListLocalOutboxWriter
{
    public async Task<Guid?> EnqueueAsync(PriceListDto priceList, SyncOperation operation,
        IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        var company = companyContext.CurrentCompany;
        if (!companyContext.HasActiveCompany || company is null || !company.IsMaster || !company.SyncEnabled)
        {
            return null;
        }

        var payload = new PriceListSyncPayloadV2(
            priceList.GlobalId, priceList.Code, priceList.Name, priceList.Description,
            priceList.CurrencyGlobalId, priceList.CurrencyCode, priceList.AppliesTo,
            priceList.IsDefault, operation is not SyncOperation.Disabled and not SyncOperation.Deleted && priceList.IsActive,
            priceList.ExternalSystem, priceList.ExternalCode, priceList.SapCode,
            priceList.CreatedAt, priceList.UpdatedAt);
        var request = new SyncPublishRequest(
            company.CompanyId, SyncMasterBranchEntityCodes.PriceLists, priceList.GlobalId,
            priceList.Code, operation, payload, priceList.ExternalSystem, priceList.Id.ToString());
        var eventId = Guid.NewGuid();
        await localOutboxRepository.CreateAsync(new CreateLocalSyncOutboxData(
            eventId, company.CompanyId, request.EntityName, request.EntityGlobalId,
            request.EntityCode, operation, payloadFactory.CreatePayloadJson(request)),
            connection, transaction, cancellationToken);
        return eventId;
    }
}
