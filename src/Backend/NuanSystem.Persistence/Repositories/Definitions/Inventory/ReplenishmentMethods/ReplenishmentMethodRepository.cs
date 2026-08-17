using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Dtos;

namespace NuanSystem.Persistence.Repositories.Definitions.Inventory.ReplenishmentMethods;

public sealed class ReplenishmentMethodRepository(ITenantConnectionFactory connectionFactory) : IReplenishmentMethodRepository
{
    private const string Prefix = "dbo.SP_NA_GET_GENERAL_INVENTORY_ReplenishmentMethods_";

    public async Task<IReadOnlyCollection<ReplenishmentMethodDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<ReplenishmentMethodDto>(Command(Prefix + "LISTAR", null, null, cancellationToken))).AsList();
    }

    public async Task<IReadOnlyCollection<ReplenishmentMethodLookupDto>> GetLookupAsync(string? includeCode = null, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<ReplenishmentMethodLookupDto>(Command(Prefix + "LOOKUP", new { IncludeCode = Normalize(includeCode) }, null, cancellationToken))).AsList();
    }

    public async Task<ReplenishmentMethodDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await GetByIdAsync(id, connection, null!, cancellationToken);
    }

    public Task<ReplenishmentMethodDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.QuerySingleOrDefaultAsync<ReplenishmentMethodDto>(Command(Prefix + "BUSCARPORID", new { Id = id }, transaction, cancellationToken));

    public Task<ReplenishmentMethodDto?> GetByCodeAsync(string code, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.QuerySingleOrDefaultAsync<ReplenishmentMethodDto>(Command(Prefix + "BUSCARPORCODIGO_DETALLE", new { Code = code }, transaction, cancellationToken));

    public async Task<IReadOnlyCollection<ReplenishmentMethodAuditChangeDto>> GetHistoryAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<ReplenishmentMethodAuditChangeDto>(Command(Prefix + "HISTORIAL", new { Id = id }, null, cancellationToken))).AsList();
    }

    public async Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await connection.ExecuteScalarAsync<int>(Command(Prefix + "BUSCARPORCODIGO", new { Code = code, ExcluirId = excludingId }, transaction, cancellationToken)) > 0;

    public Task<int> CreateAsync(CreateReplenishmentMethodData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_POST_GENERAL_INVENTORY_ReplenishmentMethods_CREAR", data, transaction, cancellationToken));

    public Task<int> UpdateAsync(UpdateReplenishmentMethodData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_PUT_GENERAL_INVENTORY_ReplenishmentMethods_ACTUALIZAR", data, transaction, cancellationToken));

    public Task<int> DeleteAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_DELETE_GENERAL_INVENTORY_ReplenishmentMethods_ELIMINAR", new { Id = id, DeletedByUserId = auditUserId, DeletedByUserName = auditUserName }, transaction, cancellationToken));

    private static string? Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static CommandDefinition Command(string procedure, object? parameters, IDbTransaction? transaction, CancellationToken cancellationToken) =>
        new(procedure, parameters, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);
}
