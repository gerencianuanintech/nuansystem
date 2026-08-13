using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Dtos;

namespace NuanSystem.Persistence.Repositories.Definitions.Inventory;

public sealed class ItemBrandRepository(ITenantConnectionFactory connectionFactory) : IItemBrandRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_ITEM_BRANDS_LISTAR";
    private const string DetailProcedure = "dbo.SP_NA_GET_ITEM_BRANDS_BUSCARPORID";
    private const string LookupProcedure = "dbo.SP_NA_GET_ITEM_BRANDS_LOOKUP";
    private const string ExistsProcedure = "dbo.SP_NA_GET_ITEM_BRANDSBUSCARPORCODIGO";
    private const string HistoryProcedure = "dbo.SP_NA_GET_ITEM_BRANDS_HISTORIAL";
    private const string CreateProcedure = "dbo.SP_NA_POST_ITEM_BRANDS_CREAR";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_ITEM_BRANDS_ACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_ITEM_BRANDS_ELIMINAR";

    public async Task<IReadOnlyCollection<ItemBrandDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<ItemBrandDto>(Command(ListProcedure, null, null, cancellationToken))).AsList();
    }

    public async Task<IReadOnlyCollection<ItemBrandLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<ItemBrandLookupDto>(Command(LookupProcedure, null, null, cancellationToken))).AsList();
    }

    public async Task<ItemBrandDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await GetByIdAsync(id, connection, null!, cancellationToken);
    }

    public Task<ItemBrandDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.QuerySingleOrDefaultAsync<ItemBrandDto>(Command(DetailProcedure, new { Id = id }, transaction, cancellationToken));

    public async Task<IReadOnlyCollection<ItemBrandAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<ItemBrandAuditChangeDto>(Command(HistoryProcedure, new { Id = id }, null, cancellationToken))).AsList();
    }

    public async Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await connection.ExecuteScalarAsync<int>(Command(ExistsProcedure, new { Code = code, ExcluirId = excludingId }, transaction, cancellationToken)) > 0;

    public Task<int> CreateAsync(CreateItemBrandData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command(CreateProcedure, data, transaction, cancellationToken));

    public Task<int> UpdateAsync(UpdateItemBrandData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command(UpdateProcedure, data, transaction, cancellationToken));

    public Task<int> DeleteAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command(DeleteProcedure,
            new { Id = id, DeletedByUserId = auditUserId, DeletedByUserName = auditUserName }, transaction, cancellationToken));

    private static CommandDefinition Command(string procedure, object? parameters,
        IDbTransaction? transaction, CancellationToken cancellationToken) =>
        new(procedure, parameters, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);
}
