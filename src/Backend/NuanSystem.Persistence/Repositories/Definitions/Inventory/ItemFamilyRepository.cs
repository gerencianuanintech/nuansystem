using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Dtos;

namespace NuanSystem.Persistence.Repositories.Definitions.Inventory;

public sealed class ItemFamilyRepository(ITenantConnectionFactory connectionFactory) : IItemFamilyRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_ITEM_FAMILIES_LISTAR";
    private const string LookupProcedure = "dbo.SP_NA_GET_ITEM_FAMILIES_LOOKUP";
    private const string DetailProcedure = "dbo.SP_NA_GET_ITEM_FAMILIES_BUSCARPORID";
    private const string HistoryProcedure = "dbo.SP_NA_GET_ITEM_FAMILIES_HISTORIAL";
    private const string CreateProcedure = "dbo.SP_NA_POST_ITEM_FAMILIES_CREAR";
    private const string ExistsProcedure = "dbo.SP_NA_GET_ITEM_FAMILIESBUSCARPORCODIGO";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_ITEM_FAMILIES_ACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_ITEM_FAMILIES_ELIMINAR";

    public async Task<IReadOnlyCollection<ItemFamilyDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<ItemFamilyDto>(Command(ListProcedure, null, null, cancellationToken))).AsList();
    }

    public async Task<IReadOnlyCollection<ItemFamilyLookupDto>> GetLookupAsync(int? itemGroupId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<ItemFamilyLookupDto>(Command(
            LookupProcedure, new { ItemGroupId = itemGroupId }, null, cancellationToken))).AsList();
    }

    public async Task<IReadOnlyCollection<ItemFamilyAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<ItemFamilyAuditChangeDto>(Command(
            HistoryProcedure, new { Id = id }, null, cancellationToken))).AsList();
    }

    public async Task<ItemFamilyDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await GetByIdAsync(id, connection, null!, cancellationToken);
    }

    public Task<ItemFamilyDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.QuerySingleOrDefaultAsync<ItemFamilyDto>(Command(DetailProcedure, new { Id = id }, transaction, cancellationToken));

    public async Task<int> CreateAsync(CreateItemFamilyData itemFamily, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await CreateAsync(itemFamily, connection, null!, cancellationToken);
    }

    public Task<int> CreateAsync(CreateItemFamilyData itemFamily, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command(CreateProcedure, itemFamily, transaction, cancellationToken));

    public async Task<bool> ExistsByCodeAsync(int itemGroupId, string code, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await ExistsCore(itemGroupId, code, null, connection, null, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(int itemGroupId, string code, int excludingId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await ExistsCore(itemGroupId, code, excludingId, connection, null, cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(int itemGroupId, string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        ExistsCore(itemGroupId, code, excludingId, connection, transaction, cancellationToken);

    public async Task<bool> UpdateAsync(UpdateItemFamilyData itemFamily, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(Command(UpdateProcedure, itemFamily, null, cancellationToken)) > 0;
    }

    public async Task<bool> UpdateAsync(UpdateItemFamilyData itemFamily, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await UpdateWithResultAsync(itemFamily, connection, transaction, cancellationToken) > 0;

    public Task<int> UpdateWithResultAsync(UpdateItemFamilyData itemFamily, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command(UpdateProcedure, itemFamily, transaction, cancellationToken));

    public async Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(Command(DeleteProcedure,
            new { Id = id, DeletedByUserId = deletedByUserId, DeletedByUserName = deletedByUserName }, null, cancellationToken)) > 0;
    }

    public async Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await DeleteWithResultAsync(id, deletedByUserId, deletedByUserName, connection, transaction, cancellationToken) > 0;

    public Task<int> DeleteWithResultAsync(int id, int? deletedByUserId, string? deletedByUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command(DeleteProcedure,
            new { Id = id, DeletedByUserId = deletedByUserId, DeletedByUserName = deletedByUserName }, transaction, cancellationToken));

    private static async Task<bool> ExistsCore(int itemGroupId, string code, int? excludingId,
        IDbConnection connection, IDbTransaction? transaction, CancellationToken cancellationToken) =>
        await connection.ExecuteScalarAsync<int>(Command(ExistsProcedure,
            new { ItemGroupId = itemGroupId, Code = code, ExcluirId = excludingId }, transaction, cancellationToken)) > 0;

    private static CommandDefinition Command(string procedure, object? parameters,
        IDbTransaction? transaction, CancellationToken cancellationToken) =>
        new(procedure, parameters, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);
}
