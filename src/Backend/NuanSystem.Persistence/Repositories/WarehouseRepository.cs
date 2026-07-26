using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class WarehouseRepository(ITenantConnectionFactory connectionFactory) : IWarehouseRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_WAREHOUSES_LISTAR";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_WAREHOUSES_BUSCARPORID";
    private const string CreateProcedure = "dbo.SP_NA_POST_WAREHOUSES_CREAR";
    private const string ExistsByCodeProcedure = "dbo.SP_NA_GET_WAREHOUSESBUSCARPORCODIGO";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_WAREHOUSES_ACTUALIZAR";
    private const string SetActiveProcedure = "dbo.SP_NA_PATCH_WAREHOUSES_ESTADO";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_WAREHOUSES_ELIMINAR";

    public async Task<IReadOnlyCollection<WarehouseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<WarehouseDto>(
            new CommandDefinition(ListProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return rows.AsList();
    }

    public async Task<WarehouseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await GetByIdCoreAsync(id, connection, null, cancellationToken);
    }

    public Task<WarehouseDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        GetByIdCoreAsync(id, connection, transaction, cancellationToken);

    public async Task<int> CreateAsync(CreateWarehouseData warehouse, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await CreateCoreAsync(warehouse, connection, null, cancellationToken);
    }

    public Task<int> CreateAsync(CreateWarehouseData warehouse, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        CreateCoreAsync(warehouse, connection, transaction, cancellationToken);

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await ExistsByCodeCoreAsync(code, null, connection, null, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await ExistsByCodeCoreAsync(code, excludingId, connection, null, cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        ExistsByCodeCoreAsync(code, excludingId, connection, transaction, cancellationToken);

    public async Task<bool> UpdateAsync(UpdateWarehouseData warehouse, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await UpdateCoreAsync(warehouse, connection, null, cancellationToken);
    }

    public Task<bool> UpdateAsync(UpdateWarehouseData warehouse, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        UpdateCoreAsync(warehouse, connection, transaction, cancellationToken);

    public async Task<bool> SetActiveStatusAsync(int id, bool isActive, int? updatedByUserId, string? updatedByUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await SetActiveStatusCoreAsync(id, isActive, updatedByUserId, updatedByUserName, connection, null, cancellationToken);
    }

    public Task<bool> SetActiveStatusAsync(int id, bool isActive, int? updatedByUserId, string? updatedByUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        SetActiveStatusCoreAsync(id, isActive, updatedByUserId, updatedByUserName, connection, transaction, cancellationToken);

    public async Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await DeleteCoreAsync(id, deletedByUserId, deletedByUserName, connection, null, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        DeleteCoreAsync(id, deletedByUserId, deletedByUserName, connection, transaction, cancellationToken);

    private static Task<WarehouseDto?> GetByIdCoreAsync(int id, IDbConnection connection, IDbTransaction? transaction, CancellationToken token) =>
        connection.QuerySingleOrDefaultAsync<WarehouseDto>(new CommandDefinition(GetByIdProcedure, new { Id = id }, transaction, cancellationToken: token, commandType: CommandType.StoredProcedure));

    private static Task<int> CreateCoreAsync(CreateWarehouseData data, IDbConnection connection, IDbTransaction? transaction, CancellationToken token) =>
        connection.ExecuteScalarAsync<int>(new CommandDefinition(CreateProcedure, data, transaction, cancellationToken: token, commandType: CommandType.StoredProcedure));

    private static async Task<bool> ExistsByCodeCoreAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction? transaction, CancellationToken token) =>
        await connection.ExecuteScalarAsync<int>(new CommandDefinition(ExistsByCodeProcedure, new { Code = code, ExcluirId = excludingId }, transaction, cancellationToken: token, commandType: CommandType.StoredProcedure)) > 0;

    private static async Task<bool> UpdateCoreAsync(UpdateWarehouseData data, IDbConnection connection, IDbTransaction? transaction, CancellationToken token) =>
        await connection.ExecuteScalarAsync<int>(new CommandDefinition(UpdateProcedure, data, transaction, cancellationToken: token, commandType: CommandType.StoredProcedure)) > 0;

    private static async Task<bool> SetActiveStatusCoreAsync(int id, bool isActive, int? userId, string? userName, IDbConnection connection, IDbTransaction? transaction, CancellationToken token) =>
        await connection.ExecuteScalarAsync<int>(new CommandDefinition(SetActiveProcedure, new { Id = id, IsActive = isActive, UpdatedByUserId = userId, UpdatedByUserName = userName }, transaction, cancellationToken: token, commandType: CommandType.StoredProcedure)) > 0;

    private static async Task<bool> DeleteCoreAsync(int id, int? userId, string? userName, IDbConnection connection, IDbTransaction? transaction, CancellationToken token) =>
        await connection.ExecuteScalarAsync<int>(new CommandDefinition(DeleteProcedure, new { Id = id, DeletedByUserId = userId, DeletedByUserName = userName }, transaction, cancellationToken: token, commandType: CommandType.StoredProcedure)) > 0;
}
