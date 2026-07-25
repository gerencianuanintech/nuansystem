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
    private const string GetMasterDataProcedure = "dbo.SP_NA_GET_ITEMMASTERDATA_BUSCARPORITEMID";
    private const string SaveMasterDataProcedure = "dbo.SP_NA_PUT_ITEMMASTERDATA_GUARDAR";

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
        return await GetByIdCoreAsync(id, connection, transaction: null, cancellationToken);
    }

    public Task<ItemDto?> GetByIdAsync(
        int id,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return GetByIdCoreAsync(id, connection, transaction, cancellationToken);
    }

    private static async Task<ItemDto?> GetByIdCoreAsync(
        int id,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        using var grid = await connection.QueryMultipleAsync(
            new CommandDefinition(
                GetByIdProcedure,
                new { Id = id },
                transaction,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        var item = await grid.ReadSingleOrDefaultAsync<ItemDto>();
        if (item is null)
        {
            return null;
        }

        item.Barcodes = (await grid.ReadAsync<ItemBarcodeDto>()).AsList();
        item.Warehouses = (await grid.ReadAsync<ItemWarehouseDto>()).AsList();

        var masterDataJson = await connection.ExecuteScalarAsync<string?>(
            new CommandDefinition(
                GetMasterDataProcedure,
                new { ItemId = id },
                transaction,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
        item.MasterData = DeserializeMasterData(masterDataJson);

        return item;
    }

    public async Task<ItemLookupsDto> GetLookupsAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(
            new CommandDefinition(LookupsProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return new ItemLookupsDto(
            (await grid.ReadAsync<ItemGroupLookupDto>()).AsList(),
            (await grid.ReadAsync<ItemFamilyLookupDto>()).AsList(),
            (await grid.ReadAsync<UnitOfMeasureLookupDto>()).AsList(),
            (await grid.ReadAsync<TaxLookupDto>()).AsList(),
            (await grid.ReadAsync<WarehouseLookupDto>()).AsList());
    }

    public async Task<int> CreateAsync(CreateItemData item, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await CreateCoreAsync(item, connection, transaction: null, cancellationToken);
    }

    public Task<int> CreateAsync(
        CreateItemData item,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return CreateCoreAsync(item, connection, transaction, cancellationToken);
    }

    private static async Task<int> CreateCoreAsync(
        CreateItemData item,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var id = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                CreateProcedure,
                ToParameters(item),
                transaction,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        await SaveMasterDataAsync(
            connection,
            transaction,
            id,
            item.MasterData,
            item.CreatedByUserId,
            item.CreatedByUserName,
            cancellationToken);
        return id;
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await ExistsByCodeCoreAsync(code, excludingId: null, connection, transaction: null, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await ExistsByCodeCoreAsync(code, excludingId, connection, transaction: null, cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(
        string code,
        int? excludingId,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return ExistsByCodeCoreAsync(code, excludingId, connection, transaction, cancellationToken);
    }

    private static async Task<bool> ExistsByCodeCoreAsync(
        string code,
        int? excludingId,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                ExistsByCodeProcedure,
                new { Code = code, ExcluirId = excludingId },
                transaction,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    public async Task<bool> UpdateAsync(UpdateItemData item, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await UpdateCoreAsync(item, connection, transaction: null, cancellationToken);
    }

    public Task<bool> UpdateAsync(
        UpdateItemData item,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return UpdateCoreAsync(item, connection, transaction, cancellationToken);
    }

    private static async Task<bool> UpdateCoreAsync(
        UpdateItemData item,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                UpdateProcedure,
                ToParameters(item),
                transaction,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        if (affectedRows > 0)
        {
            await SaveMasterDataAsync(
                connection,
                transaction,
                item.Id,
                item.MasterData,
                item.UpdatedByUserId,
                item.UpdatedByUserName,
                cancellationToken);
        }

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await DeleteCoreAsync(
            id,
            deletedByUserId,
            deletedByUserName,
            connection,
            transaction: null,
            cancellationToken);
    }

    public Task<bool> DeleteAsync(
        int id,
        int? deletedByUserId,
        string? deletedByUserName,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return DeleteCoreAsync(
            id,
            deletedByUserId,
            deletedByUserName,
            connection,
            transaction,
            cancellationToken);
    }

    private static async Task<bool> DeleteCoreAsync(
        int id,
        int? deletedByUserId,
        string? deletedByUserName,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                DeleteProcedure,
                new { Id = id, DeletedByUserId = deletedByUserId, DeletedByUserName = deletedByUserName },
                transaction,
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
            item.ItemFamilyId,
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
            item.GlobalId,
            item.ExternalSystem,
            item.ExternalCode,
            item.SapCode,
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
            item.ItemFamilyId,
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
            item.ExternalSystem,
            item.ExternalCode,
            item.SapCode,
            BarcodesJson = JsonSerializer.Serialize(item.Barcodes, JsonOptions),
            WarehousesJson = JsonSerializer.Serialize(item.Warehouses, JsonOptions),
            item.UpdatedByUserId,
            item.UpdatedByUserName
        };
    }

    private static async Task SaveMasterDataAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        int itemId,
        ItemMasterData? masterData,
        int? userId,
        string? userName,
        CancellationToken cancellationToken)
    {
        if (masterData is null)
        {
            return;
        }

        await connection.ExecuteAsync(
            new CommandDefinition(
                SaveMasterDataProcedure,
                new
                {
                    ItemId = itemId,
                    MasterDataJson = SerializeMasterData(masterData),
                    UpdatedByUserId = userId,
                    UpdatedByUserName = userName
                },
                transaction,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }

    private static string? SerializeMasterData(ItemMasterData? masterData)
    {
        return masterData is null ? null : JsonSerializer.Serialize(masterData, JsonOptions);
    }

    private static ItemMasterData? DeserializeMasterData(string? masterDataJson)
    {
        if (string.IsNullOrWhiteSpace(masterDataJson))
        {
            return null;
        }

        return JsonSerializer.Deserialize<ItemMasterData>(masterDataJson, JsonOptions);
    }
}
