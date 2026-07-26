using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class ItemFamilyRepository(ITenantConnectionFactory connectionFactory) : IItemFamilyRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_ITEM_FAMILIES_LISTAR";
    private const string ListByGroupProcedure = "dbo.SP_NA_GET_ITEM_FAMILIES_BUSCARPORGRUPO";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_ITEM_FAMILIES_BUSCARPORID";
    private const string CreateProcedure = "dbo.SP_NA_POST_ITEM_FAMILIES_CREAR";
    private const string ExistsByCodeProcedure = "dbo.SP_NA_GET_ITEM_FAMILIESBUSCARPORCODIGO";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_ITEM_FAMILIES_ACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_ITEM_FAMILIES_ELIMINAR";

    public async Task<IReadOnlyCollection<ItemFamilyDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var itemFamilies = await connection.QueryAsync<ItemFamilyDto>(
            new CommandDefinition(ListProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return itemFamilies.AsList();
    }

    public async Task<IReadOnlyCollection<ItemFamilyDto>> GetByGroupAsync(int itemGroupId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var itemFamilies = await connection.QueryAsync<ItemFamilyDto>(
            new CommandDefinition(ListByGroupProcedure, new { ItemGroupId = itemGroupId }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return itemFamilies.AsList();
    }

    public async Task<ItemFamilyDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await GetByIdCoreAsync(id, connection, transaction: null, cancellationToken);
    }

    public Task<ItemFamilyDto?> GetByIdAsync(
        int id,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return GetByIdCoreAsync(id, connection, transaction, cancellationToken);
    }

    public async Task<int> CreateAsync(CreateItemFamilyData itemFamily, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await CreateCoreAsync(itemFamily, connection, transaction: null, cancellationToken);
    }

    public Task<int> CreateAsync(
        CreateItemFamilyData itemFamily,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return CreateCoreAsync(itemFamily, connection, transaction, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(int itemGroupId, string code, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await ExistsByCodeCoreAsync(itemGroupId, code, null, connection, transaction: null, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(int itemGroupId, string code, int excludingId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await ExistsByCodeCoreAsync(itemGroupId, code, excludingId, connection, transaction: null, cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(
        int itemGroupId,
        string code,
        int? excludingId,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return ExistsByCodeCoreAsync(itemGroupId, code, excludingId, connection, transaction, cancellationToken);
    }

    public async Task<bool> UpdateAsync(UpdateItemFamilyData itemFamily, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await UpdateCoreAsync(itemFamily, connection, transaction: null, cancellationToken);
    }

    public Task<bool> UpdateAsync(
        UpdateItemFamilyData itemFamily,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return UpdateCoreAsync(itemFamily, connection, transaction, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await DeleteCoreAsync(id, deletedByUserId, deletedByUserName, connection, transaction: null, cancellationToken);
    }

    public Task<bool> DeleteAsync(
        int id,
        int? deletedByUserId,
        string? deletedByUserName,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return DeleteCoreAsync(id, deletedByUserId, deletedByUserName, connection, transaction, cancellationToken);
    }

    private static Task<ItemFamilyDto?> GetByIdCoreAsync(
        int id,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        return connection.QuerySingleOrDefaultAsync<ItemFamilyDto>(
            new CommandDefinition(
                GetByIdProcedure,
                new { Id = id },
                transaction,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }

    private static Task<int> CreateCoreAsync(
        CreateItemFamilyData itemFamily,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        return connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                CreateProcedure,
                itemFamily,
                transaction,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }

    private static async Task<bool> ExistsByCodeCoreAsync(
        int itemGroupId,
        string code,
        int? excludingId,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                ExistsByCodeProcedure,
                new { ItemGroupId = itemGroupId, Code = code, ExcluirId = excludingId },
                transaction,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
        return count > 0;
    }

    private static async Task<bool> UpdateCoreAsync(
        UpdateItemFamilyData itemFamily,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                UpdateProcedure,
                itemFamily,
                transaction,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
        return affectedRows > 0;
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
}
