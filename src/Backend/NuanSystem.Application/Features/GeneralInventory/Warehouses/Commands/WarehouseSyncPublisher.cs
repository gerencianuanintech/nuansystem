using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;

internal static class WarehouseSyncPublisher
{
    private const string EntityName = SyncMasterBranchEntityCodes.Warehouse;

    public static async Task<Result<SyncPublishResult>?> PublishAsync(
        ISyncEventPublisher syncEventPublisher,
        ICompanyContext companyContext,
        WarehouseDto warehouse,
        SyncOperation operation,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany || companyContext.CurrentCompany is null)
        {
            return null;
        }

        var payload = new WarehouseSyncPayload(
            warehouse.GlobalId,
            warehouse.Code,
            warehouse.Name,
            warehouse.Description,
            warehouse.BranchCode,
            warehouse.Address,
            warehouse.City,
            warehouse.Province,
            warehouse.Country,
            warehouse.Phone,
            warehouse.Email,
            warehouse.ManagerName,
            warehouse.AllowsSales,
            warehouse.AllowsPurchases,
            warehouse.AllowsTransfers,
            warehouse.AllowsProduction,
            warehouse.IsDefault,
            warehouse.IsActive,
            warehouse.ExternalSystem,
            warehouse.ExternalCode,
            warehouse.SapCode,
            warehouse.CreatedAt,
            warehouse.UpdatedAt);

        return await syncEventPublisher.PublishAsync(
            new SyncPublishRequest(
                companyContext.CurrentCompany.CompanyId,
                EntityName,
                warehouse.GlobalId,
                warehouse.Code,
                operation,
                payload,
                SourceSystem: null,
                SourceReference: warehouse.Id.ToString()),
            cancellationToken);
    }
}
