using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Dtos;

namespace NuanSystem.Persistence.Repositories.FinancialCatalogs;

public sealed class PriceListRepository(ITenantConnectionFactory connectionFactory) : IPriceListRepository
{
    public async Task<IReadOnlyCollection<PriceListDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<PriceListDto>(Command(
            "dbo.SP_NA_GET_PRICELISTS_LISTAR", null, null, cancellationToken))).AsList();
    }

    public async Task<IReadOnlyCollection<PriceListLookupDto>> GetLookupAsync(string? appliesTo = null, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<PriceListLookupDto>(Command(
            "dbo.SP_NA_GET_PRICELISTS_LOOKUP", new { AppliesTo = appliesTo }, null, cancellationToken))).AsList();
    }

    public async Task<PriceListDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await GetByIdCoreAsync(id, connection, null, cancellationToken);
    }

    public Task<PriceListDto?> GetByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        GetByIdCoreAsync(id, connection, transaction, cancellationToken);

    private Task<PriceListDto?> GetByIdCoreAsync(int id, IDbConnection connection, IDbTransaction? transaction, CancellationToken cancellationToken) =>
        connection.QuerySingleOrDefaultAsync<PriceListDto>(Command(
            "dbo.SP_NA_GET_PRICELISTS_BUSCARPORID", new { Id = id }, transaction, cancellationToken));

    public Task<PriceListCurrencyDto?> GetCurrencyAsync(string currencyCode, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.QuerySingleOrDefaultAsync<PriceListCurrencyDto>(Command(
            "dbo.SP_NA_GET_PRICELISTS_MONEDAPORCODIGO", new { CurrencyCode = currencyCode }, transaction, cancellationToken));

    public async Task<bool> ExistsByCodeAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await connection.ExecuteScalarAsync<int>(Command(
            "dbo.SP_NA_GET_PRICELISTS_CODIGORESERVADO", new { Code = code, ExcluirId = excludingId }, transaction, cancellationToken)) > 0;

    public async Task<bool> HasDefaultConflictAsync(string appliesTo, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await connection.ExecuteScalarAsync<int>(Command(
            "dbo.SP_NA_GET_PRICELISTS_PREDETERMINADACONFLICTO", new { AppliesTo = appliesTo, ExcluirId = excludingId }, transaction, cancellationToken)) > 0;

    public async Task<bool> HasActiveReferencesAsync(int id, string code, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await connection.ExecuteScalarAsync<int>(Command(
            "dbo.SP_NA_GET_PRICELISTS_REFERENCIASACTIVAS", new { Id = id, Code = code }, transaction, cancellationToken)) > 0;

    public Task<int> CreateAsync(CreatePriceListData priceList, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        connection.ExecuteScalarAsync<int>(Command(
            "dbo.SP_NA_POST_PRICELISTS_CREAR", priceList, transaction, cancellationToken));

    public async Task<bool> UpdateAsync(UpdatePriceListData priceList, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await connection.ExecuteScalarAsync<int>(Command(
            "dbo.SP_NA_PUT_PRICELISTS_ACTUALIZAR", priceList, transaction, cancellationToken)) > 0;

    public async Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) =>
        await connection.ExecuteScalarAsync<int>(Command(
            "dbo.SP_NA_DELETE_PRICELISTS_ELIMINAR",
            new { Id = id, DeletedByUserId = deletedByUserId, DeletedByUserName = deletedByUserName },
            transaction, cancellationToken)) > 0;

    private static CommandDefinition Command(string procedure, object? parameters, IDbTransaction? transaction, CancellationToken cancellationToken) =>
        new(procedure, parameters, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);
}
