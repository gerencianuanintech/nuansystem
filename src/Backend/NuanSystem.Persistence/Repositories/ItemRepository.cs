using System.Data;
using System.Text.Json;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Items.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class ItemRepository(ITenantConnectionFactory connectionFactory) : IItemRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_ITEMS_LISTAR";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_ITEMS_BUSCARPORID";
    private const string LookupsProcedure = "dbo.SP_NA_GET_ITEMS_LOOKUPS";
    private const string CreateProcedure = "dbo.SP_NA_POST_ITEMS_CREAR";
    private const string ExistsByCodeProcedure = "dbo.SP_NA_GET_ITEMSBUSCARPORCODIGO";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_ITEMS_ACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_ITEMS_ELIMINAR";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyCollection<ItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<ItemDto>(
            new CommandDefinition(ListProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return items.AsList();
    }

    public async Task<ItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(
            new CommandDefinition(GetByIdProcedure, new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        var item = await grid.ReadSingleOrDefaultAsync<ItemDto>();
        if (item is null)
        {
            return null;
        }

        item.Barcodes = (await grid.ReadAsync<ItemBarcodeDto>()).AsList();
        item.Warehouses = (await grid.ReadAsync<ItemWarehouseDto>()).AsList();
        return item;
    }

    public async Task<ItemLookupsDto> GetLookupsAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(
            new CommandDefinition(LookupsProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return new ItemLookupsDto(
            (await grid.ReadAsync<ItemGroupLookupDto>()).AsList(),
            (await grid.ReadAsync<UnitOfMeasureLookupDto>()).AsList(),
            (await grid.ReadAsync<TaxLookupDto>()).AsList(),
            (await grid.ReadAsync<WarehouseLookupDto>()).AsList());
    }

    public async Task<int> CreateAsync(CreateItemData item, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(CreateProcedure, ToParameters(item), cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(ExistsByCodeProcedure, new { Code = code, ExcluirId = (int?)null }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    public async Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(ExistsByCodeProcedure, new { Code = code, ExcluirId = excludingId }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    public async Task<bool> UpdateAsync(UpdateItemData item, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(UpdateProcedure, ToParameters(item), cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                DeleteProcedure,
                new { Id = id, DeletedByUserId = deletedByUserId, DeletedByUserName = deletedByUserName },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    private static object ToParameters(CreateItemData item)
    {
        return new
        {
            item.Code,
            item.Name,
            item.Description,
            item.ItemGroupId,
            item.ItemType,
            item.InventoryUnitOfMeasureId,
            item.PurchaseUnitOfMeasureId,
            item.SalesUnitOfMeasureId,
            item.IsPurchaseItem,
            item.IsSalesItem,
            item.IsInventoryItem,
            item.PurchaseTaxId,
            item.SalesTaxId,
            item.ValuationMethod,
            item.ManagedBy,
            item.BatchSerialManagementMethod,
            item.PreferredVendorCode,
            item.VendorCatalogCode,
            item.BaseSalesPrice,
            item.ReferenceCost,
            item.PurchaseFactor,
            item.SalesFactor,
            item.AllowDiscount,
            item.AllowSaleWithoutStock,
            item.Remarks,
            item.IsActive,
            BarcodesJson = JsonSerializer.Serialize(item.Barcodes, JsonOptions),
            WarehousesJson = JsonSerializer.Serialize(item.Warehouses, JsonOptions),
            item.CreatedByUserId,
            item.CreatedByUserName
        };
    }

    private static object ToParameters(UpdateItemData item)
    {
        return new
        {
            item.Id,
            item.Code,
            item.Name,
            item.Description,
            item.ItemGroupId,
            item.ItemType,
            item.InventoryUnitOfMeasureId,
            item.PurchaseUnitOfMeasureId,
            item.SalesUnitOfMeasureId,
            item.IsPurchaseItem,
            item.IsSalesItem,
            item.IsInventoryItem,
            item.PurchaseTaxId,
            item.SalesTaxId,
            item.ValuationMethod,
            item.ManagedBy,
            item.BatchSerialManagementMethod,
            item.PreferredVendorCode,
            item.VendorCatalogCode,
            item.BaseSalesPrice,
            item.ReferenceCost,
            item.PurchaseFactor,
            item.SalesFactor,
            item.AllowDiscount,
            item.AllowSaleWithoutStock,
            item.Remarks,
            item.IsActive,
            BarcodesJson = JsonSerializer.Serialize(item.Barcodes, JsonOptions),
            WarehousesJson = JsonSerializer.Serialize(item.Warehouses, JsonOptions),
            item.UpdatedByUserId,
            item.UpdatedByUserName
        };
    }
}
