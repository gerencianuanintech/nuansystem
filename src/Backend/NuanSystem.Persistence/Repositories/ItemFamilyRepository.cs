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
        return await connection.QuerySingleOrDefaultAsync<ItemFamilyDto>(
            new CommandDefinition(GetByIdProcedure, new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<int> CreateAsync(CreateItemFamilyData itemFamily, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(CreateProcedure, itemFamily, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> ExistsByCodeAsync(int itemGroupId, string code, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(ExistsByCodeProcedure, new { ItemGroupId = itemGroupId, Code = code, ExcluirId = (int?)null }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    public async Task<bool> ExistsByCodeAsync(int itemGroupId, string code, int excludingId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(ExistsByCodeProcedure, new { ItemGroupId = itemGroupId, Code = code, ExcluirId = excludingId }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    public async Task<bool> UpdateAsync(UpdateItemFamilyData itemFamily, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(UpdateProcedure, itemFamily, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

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
