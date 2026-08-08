using System.Data;
using Dapper;
using NuanSystem.Application.Features.Definitions.General.Common.Dtos;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;

namespace NuanSystem.Persistence.Repositories.Definitions.General;

public sealed partial class GeographyRepository
{
    public async Task<IReadOnlyCollection<CountryDto>> GetCountriesAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<CountryDto>(new CommandDefinition("dbo.SP_NA_GET_COUNTRIES_LISTAR", cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();
    }

    public async Task<CountryPageDto> SearchCountriesAsync(
        CountryListFilter filter,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(new CommandDefinition(
            "dbo.SP_NA_GET_COUNTRIES_BUSCARPAGINADO",
            new
            {
                Search = string.IsNullOrWhiteSpace(filter.Search) ? null : filter.Search.Trim(),
                filter.PageNumber,
                filter.PageSize
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        var items = (await grid.ReadAsync<CountryDto>()).AsList();
        var totalCount = await grid.ReadSingleAsync<int>();
        return new CountryPageDto(items, totalCount, filter.PageNumber, filter.PageSize);
    }

    public async Task<IReadOnlyCollection<GeographyLookupDto>> GetCountryLookupAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<GeographyLookupDto>(new CommandDefinition("dbo.SP_NA_GET_COUNTRIES_LOOKUP", cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();
    }

    public async Task<CountryDto?> GetCountryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await GetCountryByIdAsync(id, connection, null!, cancellationToken);
    }

    public Task<CountryDto?> GetCountryByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) => connection.QuerySingleOrDefaultAsync<CountryDto>(new CommandDefinition("dbo.SP_NA_GET_COUNTRIES_BUSCARPORID", new { Id = id }, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

    public async Task<bool> CountryCodeExistsAsync(string code, int? excludingId = null, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await CountryCodeExistsAsync(code, excludingId, connection, null!, cancellationToken);
    }

    public async Task<bool> CountryCodeExistsAsync(string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) => await connection.ExecuteScalarAsync<int>(new CommandDefinition("dbo.SP_NA_GET_COUNTRIES_BUSCARPORCODIGO", new { Code = code, ExcluirId = excludingId }, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure)) > 0;

    public async Task<int> CreateCountryAsync(SaveCountryData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await CreateCountryAsync(data, connection, null!, cancellationToken);
    }

    public Task<int> CreateCountryAsync(SaveCountryData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) => connection.ExecuteScalarAsync<int>(new CommandDefinition("dbo.SP_NA_POST_COUNTRIES_CREAR", data, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

    public async Task<bool> UpdateCountryAsync(SaveCountryData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await UpdateCountryAsync(data, connection, null!, cancellationToken);
    }

    public async Task<bool> UpdateCountryAsync(SaveCountryData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) => await connection.ExecuteScalarAsync<int>(new CommandDefinition("dbo.SP_NA_PUT_COUNTRIES_ACTUALIZAR", data, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure)) > 0;

    public async Task<bool> DeleteCountryAsync(int id, int? auditUserId, string? auditUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await DeleteCountryAsync(id, auditUserId, auditUserName, connection, null!, cancellationToken);
    }

    public async Task<bool> DeleteCountryAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) => await connection.ExecuteScalarAsync<int>(new CommandDefinition("dbo.SP_NA_DELETE_COUNTRIES_ELIMINAR", new { Id = id, DeletedByUserId = auditUserId, DeletedByUserName = auditUserName }, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure)) > 0;
}
