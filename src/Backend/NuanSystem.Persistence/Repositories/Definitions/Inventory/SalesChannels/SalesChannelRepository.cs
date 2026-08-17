using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.Inventory.SalesChannels.Dtos;

namespace NuanSystem.Persistence.Repositories.Definitions.Inventory.SalesChannels;

public sealed class SalesChannelRepository(ITenantConnectionFactory connectionFactory) : ISalesChannelRepository
{
    private const string Prefix = "dbo.SP_NA_GET_GENERAL_INVENTORY_SalesChannels_";
    public async Task<IReadOnlyCollection<SalesChannelDto>> GetAllAsync(CancellationToken ct = default)
    { using var connection = connectionFactory.CreateConnection(); return (await connection.QueryAsync<SalesChannelDto>(Command(Prefix + "LISTAR", null, null, ct))).AsList(); }
    public async Task<IReadOnlyCollection<SalesChannelLookupDto>> GetLookupAsync(CancellationToken ct = default)
    { using var connection = connectionFactory.CreateConnection(); return (await connection.QueryAsync<SalesChannelLookupDto>(Command(Prefix + "LOOKUP", null, null, ct))).AsList(); }
    public async Task<SalesChannelDto?> GetByIdAsync(int id, CancellationToken ct = default)
    { using var connection = connectionFactory.CreateConnection(); return await GetByIdAsync(id, connection, null!, ct); }
    public Task<SalesChannelDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default) => connection.QuerySingleOrDefaultAsync<SalesChannelDto>(Command(Prefix + "BUSCARPORID", new { Id = id }, transaction, ct));
    public async Task<IReadOnlyCollection<SalesChannelAuditChangeDto>> GetHistoryAsync(int id, CancellationToken ct = default)
    { using var connection = connectionFactory.CreateConnection(); return (await connection.QueryAsync<SalesChannelAuditChangeDto>(Command(Prefix + "HISTORIAL", new { Id = id }, null, ct))).AsList(); }
    public async Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default) => await connection.ExecuteScalarAsync<int>(Command(Prefix + "BUSCARPORCODIGO", new { Code = code, ExcluirId = excludingId }, transaction, ct)) > 0;
    public Task<int> CreateAsync(CreateSalesChannelData data, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default) => connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_POST_GENERAL_INVENTORY_SalesChannels_CREAR", data, transaction, ct));
    public Task<int> UpdateAsync(UpdateSalesChannelData data, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default) => connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_PUT_GENERAL_INVENTORY_SalesChannels_ACTUALIZAR", data, transaction, ct));
    public Task<int> DeleteAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken ct = default) => connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_DELETE_GENERAL_INVENTORY_SalesChannels_ELIMINAR", new { Id = id, DeletedByUserId = auditUserId, DeletedByUserName = auditUserName }, transaction, ct));
    private static CommandDefinition Command(string procedure, object? parameters, IDbTransaction? transaction, CancellationToken ct) => new(procedure, parameters, transaction, cancellationToken: ct, commandType: CommandType.StoredProcedure);
}


