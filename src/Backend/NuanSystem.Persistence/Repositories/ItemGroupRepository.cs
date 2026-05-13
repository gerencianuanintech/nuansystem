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
        return await connection.QuerySingleOrDefaultAsync<ItemGroupDto>(
            new CommandDefinition(GetByIdProcedure, new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<int> CreateAsync(CreateItemGroupData itemGroup, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(CreateProcedure, itemGroup, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
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

    public async Task<bool> UpdateAsync(UpdateItemGroupData itemGroup, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(UpdateProcedure, itemGroup, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

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
}
