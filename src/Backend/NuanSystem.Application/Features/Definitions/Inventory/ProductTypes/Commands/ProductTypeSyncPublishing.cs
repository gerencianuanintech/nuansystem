using System.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Commands;

internal static class ProductTypeSyncEventFactory
{
    public static SyncPublishRequest Create(int companyId, ProductTypeDto item, SyncOperation operation)
    {
        if (item.GlobalId == Guid.Empty) throw new InvalidOperationException("ProductType requiere GlobalId para sincronizacion.");
        var isDeleted = operation == SyncOperation.Deleted;
        var payload = new ProductTypeSyncPayload(item.GlobalId, item.Code, item.Name, item.Description,
            item.NatureCode, item.SortOrder, item.IsSystem,
            !isDeleted && operation != SyncOperation.Disabled && item.IsActive,
            isDeleted, item.UpdatedAt ?? item.CreatedAt);
        return new SyncPublishRequest(companyId, SyncMasterBranchEntityCodes.ProductTypes,
            item.GlobalId, item.Code, operation, payload,
            SourceSystem: null, SourceReference: item.Id.ToString());
    }
}

public sealed class ProductTypeLocalOutboxWriter(
    ICompanyContext companyContext, ISyncEventPayloadFactory payloadFactory,
    ILocalSyncOutboxRepository localOutboxRepository) : IProductTypeLocalOutboxWriter
{
    public async Task<Guid?> EnqueueAsync(ProductTypeDto productType, SyncOperation operation,
        IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default)
    {
        var company = companyContext.CurrentCompany;
        if (!companyContext.HasActiveCompany || company is null || !company.IsMaster || !company.SyncEnabled) return null;

        var publish = ProductTypeSyncEventFactory.Create(company.CompanyId, productType, operation);
        var eventId = Guid.NewGuid();
        await localOutboxRepository.CreateAsync(new CreateLocalSyncOutboxData(eventId, company.CompanyId,
            publish.EntityName, publish.EntityGlobalId, publish.EntityCode, operation,
            payloadFactory.CreatePayloadJson(publish)), connection, transaction, cancellationToken);
        return eventId;
    }
}
