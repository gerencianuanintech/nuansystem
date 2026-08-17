using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Dtos;

namespace NuanSystem.Persistence.Repositories.Definitions.Inventory;

public sealed class ItemSubgroupRepository(ITenantConnectionFactory connectionFactory) : IItemSubgroupRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_ITEM_SUBGROUPS_LISTAR";
    private const string LookupProcedure = "dbo.SP_NA_GET_ITEM_SUBGROUPS_LOOKUP";
    private const string DetailProcedure = "dbo.SP_NA_GET_ITEM_SUBGROUPS_BUSCARPORID";
    private const string ExistsProcedure = "dbo.SP_NA_GET_ITEM_SUBGROUPSBUSCARPORCODIGO";
    private const string HistoryProcedure = "dbo.SP_NA_GET_ITEM_SUBGROUPS_HISTORIAL";
    private const string CreateProcedure = "dbo.SP_NA_POST_ITEM_SUBGROUPS_CREAR";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_ITEM_SUBGROUPS_ACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_ITEM_SUBGROUPS_ELIMINAR";

    public async Task<IReadOnlyCollection<ItemSubgroupDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<ItemSubgroupDto>(Command(ListProcedure, null, null, cancellationToken))).AsList();
    }

    public async Task<IReadOnlyCollection<ItemSubgroupLookupDto>> GetLookupAsync(int? itemFamilyId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<ItemSubgroupLookupDto>(Command(
            LookupProcedure, new { ItemFamilyId = itemFamilyId }, null, cancellationToken))).AsList();
    }

    public async Task<ItemSubgroupDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await GetByIdAsync(id, connection, null!, cancellationToken);
    }

    public Task<ItemSubgroupDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.QuerySingleOrDefaultAsync<ItemSubgroupDto>(Command(DetailProcedure, new { Id = id }, transaction, cancellationToken));

    public async Task<IReadOnlyCollection<ItemSubgroupAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<ItemSubgroupAuditChangeDto>(Command(
            HistoryProcedure, new { Id = id }, null, cancellationToken))).AsList();
    }

    public async Task<bool> ExistsByCodeAsync(int itemFamilyId, string code, int? excludingId,
        IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await connection.ExecuteScalarAsync<int>(Command(ExistsProcedure,
            new { ItemFamilyId = itemFamilyId, Code = code, ExcluirId = excludingId }, transaction, cancellationToken)) > 0;

    public async Task<bool> ExistsActiveByFamilyAndCodeAsync(int itemFamilyId, string code,
        IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            "SELECT COUNT(1) FROM dbo.ItemSubgroups WHERE ItemFamilyId=@ItemFamilyId AND Code=@Code AND IsDeleted=0 AND IsActive=1;",
            new { ItemFamilyId = itemFamilyId, Code = code }, transaction,
            cancellationToken: cancellationToken)) > 0;

    public Task<int> CreateAsync(CreateItemSubgroupData data, IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command(CreateProcedure, data, transaction, cancellationToken));

    public Task<int> UpdateWithResultAsync(UpdateItemSubgroupData data, IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command(UpdateProcedure, data, transaction, cancellationToken));

    public Task<int> DeleteWithResultAsync(int id, int? auditUserId, string? auditUserName,
        IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command(DeleteProcedure,
            new { Id = id, DeletedByUserId = auditUserId, DeletedByUserName = auditUserName }, transaction, cancellationToken));

    private static CommandDefinition Command(string procedure, object? parameters,
        IDbTransaction? transaction, CancellationToken cancellationToken) =>
        new(procedure, parameters, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);
}
