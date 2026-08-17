using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemAlertTypes.Dtos;

namespace NuanSystem.Persistence.Repositories.Definitions.Inventory.ItemAlertTypes;

public sealed class ItemAlertTypeRepository(ITenantConnectionFactory connectionFactory) : IItemAlertTypeRepository
{
    private const string Prefix = "dbo.SP_NA_GET_GENERAL_INVENTORY_ItemAlertTypes_";
    public async Task<IReadOnlyCollection<ItemAlertTypeDto>> GetAllAsync(CancellationToken ct = default)
    { using var connection = connectionFactory.CreateConnection(); return (await connection.QueryAsync<ItemAlertTypeDto>(Command(Prefix + "LISTAR", null, null, ct))).AsList(); }
    public async Task<IReadOnlyCollection<ItemAlertTypeLookupDto>> GetLookupAsync(CancellationToken ct = default)
    { using var connection = connectionFactory.CreateConnection(); return (await connection.QueryAsync<ItemAlertTypeLookupDto>(Command(Prefix + "LOOKUP", null, null, ct))).AsList(); }
    public async Task<ItemAlertTypeDto?> GetByIdAsync(int id, CancellationToken ct = default)
    { using var connection = connectionFactory.CreateConnection(); return await GetByIdAsync(id, connection, null!, ct); }
    public Task<ItemAlertTypeDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default) => connection.QuerySingleOrDefaultAsync<ItemAlertTypeDto>(Command(Prefix + "BUSCARPORID", new { Id = id }, transaction, ct));
    public async Task<IReadOnlyCollection<ItemAlertTypeAuditChangeDto>> GetHistoryAsync(int id, CancellationToken ct = default)
    { using var connection = connectionFactory.CreateConnection(); return (await connection.QueryAsync<ItemAlertTypeAuditChangeDto>(Command(Prefix + "HISTORIAL", new { Id = id }, null, ct))).AsList(); }
    public async Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default) => await connection.ExecuteScalarAsync<int>(Command(Prefix + "BUSCARPORCODIGO", new { Code = code, ExcluirId = excludingId }, transaction, ct)) > 0;
    public Task<int> CreateAsync(CreateItemAlertTypeData data, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default) => connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_POST_GENERAL_INVENTORY_ItemAlertTypes_CREAR", data, transaction, ct));
    public Task<int> UpdateAsync(UpdateItemAlertTypeData data, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default) => connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_PUT_GENERAL_INVENTORY_ItemAlertTypes_ACTUALIZAR", data, transaction, ct));
    public Task<int> DeleteAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default) => connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_DELETE_GENERAL_INVENTORY_ItemAlertTypes_ELIMINAR", new { Id = id, DeletedByUserId = auditUserId, DeletedByUserName = auditUserName }, transaction, ct));
    private static CommandDefinition Command(string procedure, object? parameters, IDbTransaction? transaction, CancellationToken ct) => new(procedure, parameters, transaction, cancellationToken: ct, commandType: CommandType.StoredProcedure);
}

