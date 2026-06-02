using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Geography.Dtos;

namespace NuanSystem.Persistence.Repositories.Geography;

public sealed class GeographyRepository(ITenantConnectionFactory connectionFactory) : IGeographyRepository
{
    public async Task<IReadOnlyCollection<CountryDto>> GetCountriesAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<CountryDto>(
            new CommandDefinition("dbo.SP_NA_GET_COUNTRIES_LISTAR", cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return items.AsList();
    }

    public async Task<IReadOnlyCollection<ProvinceDto>> GetProvincesAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<ProvinceDto>(
            new CommandDefinition("dbo.SP_NA_GET_PROVINCES_LISTAR", cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return items.AsList();
    }

    public async Task<IReadOnlyCollection<CityDto>> GetCitiesAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<CityDto>(
            new CommandDefinition("dbo.SP_NA_GET_CITIES_LISTAR", cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return items.AsList();
    }

    public async Task<IReadOnlyCollection<GeographyLookupDto>> GetCountryLookupAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<GeographyLookupDto>(
            new CommandDefinition("dbo.SP_NA_GET_COUNTRIES_LOOKUP", cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return items.AsList();
    }

    public async Task<IReadOnlyCollection<GeographyLookupDto>> GetProvinceLookupAsync(string? countryCode = null, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<GeographyLookupDto>(
            new CommandDefinition("dbo.SP_NA_GET_PROVINCES_LOOKUP", new { CountryCode = countryCode }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return items.AsList();
    }

    public async Task<IReadOnlyCollection<GeographyLookupDto>> GetCityLookupAsync(string? countryCode = null, string? provinceCode = null, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var items = await connection.QueryAsync<GeographyLookupDto>(
            new CommandDefinition("dbo.SP_NA_GET_CITIES_LOOKUP", new { CountryCode = countryCode, ProvinceCode = provinceCode }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return items.AsList();
    }

    public async Task<CountryDto?> GetCountryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<CountryDto>(
            new CommandDefinition("dbo.SP_NA_GET_COUNTRIES_BUSCARPORID", new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<ProvinceDto?> GetProvinceByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ProvinceDto>(
            new CommandDefinition("dbo.SP_NA_GET_PROVINCES_BUSCARPORID", new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<CityDto?> GetCityByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<CityDto>(
            new CommandDefinition("dbo.SP_NA_GET_CITIES_BUSCARPORID", new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> CountryCodeExistsAsync(string code, int? excludingId = null, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition("dbo.SP_NA_GET_COUNTRIES_BUSCARPORCODIGO", new { Code = code, ExcluirId = excludingId }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return count > 0;
    }

    public async Task<bool> ProvinceCodeExistsAsync(int countryId, string code, int? excludingId = null, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition("dbo.SP_NA_GET_PROVINCES_BUSCARPORCODIGO", new { CountryId = countryId, Code = code, ExcluirId = excludingId }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return count > 0;
    }

    public async Task<bool> CityCodeExistsAsync(int provinceId, string code, int? excludingId = null, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition("dbo.SP_NA_GET_CITIES_BUSCARPORCODIGO", new { ProvinceId = provinceId, Code = code, ExcluirId = excludingId }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return count > 0;
    }

    public async Task<int> CreateCountryAsync(SaveCountryData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition("dbo.SP_NA_POST_COUNTRIES_CREAR", data, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<int> CreateProvinceAsync(SaveProvinceData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition("dbo.SP_NA_POST_PROVINCES_CREAR", data, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<int> CreateCityAsync(SaveCityData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition("dbo.SP_NA_POST_CITIES_CREAR", data, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> UpdateCountryAsync(SaveCountryData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition("dbo.SP_NA_PUT_COUNTRIES_ACTUALIZAR", data, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return affectedRows > 0;
    }

    public async Task<bool> UpdateProvinceAsync(SaveProvinceData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition("dbo.SP_NA_PUT_PROVINCES_ACTUALIZAR", data, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return affectedRows > 0;
    }

    public async Task<bool> UpdateCityAsync(SaveCityData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition("dbo.SP_NA_PUT_CITIES_ACTUALIZAR", data, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return affectedRows > 0;
    }

    public async Task<bool> DeleteCountryAsync(int id, int? auditUserId, string? auditUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition("dbo.SP_NA_DELETE_COUNTRIES_ELIMINAR", new { Id = id, DeletedByUserId = auditUserId, DeletedByUserName = auditUserName }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return affectedRows > 0;
    }

    public async Task<bool> DeleteProvinceAsync(int id, int? auditUserId, string? auditUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition("dbo.SP_NA_DELETE_PROVINCES_ELIMINAR", new { Id = id, DeletedByUserId = auditUserId, DeletedByUserName = auditUserName }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return affectedRows > 0;
    }

    public async Task<bool> DeleteCityAsync(int id, int? auditUserId, string? auditUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition("dbo.SP_NA_DELETE_CITIES_ELIMINAR", new { Id = id, DeletedByUserId = auditUserId, DeletedByUserName = auditUserName }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return affectedRows > 0;
    }
}
