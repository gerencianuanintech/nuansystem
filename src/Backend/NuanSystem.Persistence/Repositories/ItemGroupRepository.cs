using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class ItemGroupRepository(ITenantConnectionFactory connectionFactory) : IItemGroupRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_ITEM_GROUPS_LISTAR";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_ITEM_GROUPS_BUSCARPORID";
    private const string CreateProcedure = "dbo.SP_NA_POST_ITEM_GROUPS_CREAR";
    private const string ExistsByCodeProcedure = "dbo.SP_NA_GET_ITEM_GROUPSBUSCARPORCODIGO";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_ITEM_GROUPS_ACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_ITEM_GROUPS_ELIMINAR";

    public async Task<IReadOnlyCollection<ItemGroupDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var itemGroups = await connection.QueryAsync<ItemGroupDto>(
            new CommandDefinition(ListProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return itemGroups.AsList();
    }

    public async Task<ItemGroupDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await GetByIdCoreAsync(id, connection, transaction: null, cancellationToken);
    }

    public Task<ItemGroupDto?> GetByIdAsync(
        int id,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return GetByIdCoreAsync(id, connection, transaction, cancellationToken);
    }

    public async Task<int> CreateAsync(CreateItemGroupData itemGroup, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await CreateCoreAsync(itemGroup, connection, transaction: null, cancellationToken);
    }

    public Task<int> CreateAsync(
        CreateItemGroupData itemGroup,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return CreateCoreAsync(itemGroup, connection, transaction, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await ExistsByCodeCoreAsync(code, null, connection, transaction: null, cancellationToken);
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

    public async Task<bool> UpdateAsync(UpdateItemGroupData itemGroup, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await UpdateCoreAsync(itemGroup, connection, transaction: null, cancellationToken);
    }

    public Task<bool> UpdateAsync(
        UpdateItemGroupData itemGroup,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        return UpdateCoreAsync(itemGroup, connection, transaction, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await DeleteCoreAsync(
            id, deletedByUserId, deletedByUserName, connection, transaction: null, cancellationToken);
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
            id, deletedByUserId, deletedByUserName, connection, transaction, cancellationToken);
    }

    private static Task<ItemGroupDto?> GetByIdCoreAsync(
        int id,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        return connection.QuerySingleOrDefaultAsync<ItemGroupDto>(
            new CommandDefinition(
                GetByIdProcedure,
                new { Id = id },
                transaction,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
    }

    private static Task<int> CreateCoreAsync(
        CreateItemGroupData itemGroup,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        return connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                CreateProcedure,
                itemGroup,
                transaction,
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));
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

    private static async Task<bool> UpdateCoreAsync(
        UpdateItemGroupData itemGroup,
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken)
    {
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(UpdateProcedure, itemGroup, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
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
