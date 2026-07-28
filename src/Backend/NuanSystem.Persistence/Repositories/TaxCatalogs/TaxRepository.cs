using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.TaxCatalogs.Taxes.Dtos;

namespace NuanSystem.Persistence.Repositories.TaxCatalogs;

public sealed class TaxRepository(ITenantConnectionFactory connectionFactory) : ITaxRepository
{
    public async Task<IReadOnlyCollection<TaxDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<TaxDto>(Command("dbo.SP_NA_GET_TAXES_LISTAR", null, null, cancellationToken))).AsList();
    }

    public async Task<IReadOnlyCollection<TaxLookupDto>> GetLookupAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<TaxLookupDto>(Command("dbo.SP_NA_GET_TAXES_LOOKUP", null, null, cancellationToken))).AsList();
    }

    public async Task<TaxDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await GetByIdAsync(id, connection, null!, cancellationToken);
    }

    public async Task<IReadOnlyCollection<TaxAuditChangeDto>> GetHistoryAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<TaxAuditChangeDto>(Command(
            "dbo.SP_NA_GET_TAXES_HISTORIAL", new { Id = id }, null, cancellationToken))).AsList();
    }

    public Task<TaxDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.QuerySingleOrDefaultAsync<TaxDto>(Command(
            "dbo.SP_NA_GET_TAXES_BUSCARPORID", new { Id = id }, transaction, cancellationToken));

    public async Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_GET_TAXES_CODIGORESERVADO",
            new { Code = code, ExcluirId = excludingId }, transaction, cancellationToken)) > 0;

    public async Task<bool> HasActiveItemReferencesAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_GET_TAXES_REFERENCIASITEMSACTIVOS",
            new { Id = id }, transaction, cancellationToken)) > 0;

    public Task<int> CreateAsync(CreateTaxData tax, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_POST_TAXES_CREAR", tax, transaction, cancellationToken));

    public async Task<bool> UpdateAsync(UpdateTaxData tax, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_PUT_TAXES_ACTUALIZAR", tax, transaction, cancellationToken)) > 0;

    public async Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_DELETE_TAXES_ELIMINAR",
            new { Id = id, DeletedByUserId = deletedByUserId, DeletedByUserName = deletedByUserName },
            transaction, cancellationToken)) > 0;

    private static CommandDefinition Command(string procedure, object? parameters, IDbTransaction? transaction, CancellationToken cancellationToken) =>
        new(procedure, parameters, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);
}
