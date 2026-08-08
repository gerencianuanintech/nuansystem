using System.Data;
using Dapper;
using NuanSystem.Application.Features.Definitions.General.Cities.Dtos;
using NuanSystem.Application.Features.Definitions.General.Common.Dtos;

namespace NuanSystem.Persistence.Repositories.Definitions.General;

public sealed partial class GeographyRepository
{
    public async Task<IReadOnlyCollection<CityDto>> GetCitiesAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<CityDto>(new CommandDefinition("dbo.SP_NA_GET_CITIES_LISTAR", cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();
    }

    public async Task<CityPageDto> SearchCitiesAsync(
        CityListFilter filter,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(new CommandDefinition(
            "dbo.SP_NA_GET_CITIES_BUSCARPAGINADO",
            new
            {
                Search = string.IsNullOrWhiteSpace(filter.Search) ? null : filter.Search.Trim(),
                filter.PageNumber,
                filter.PageSize
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        var items = (await grid.ReadAsync<CityDto>()).AsList();
        var totalCount = await grid.ReadSingleAsync<int>();
        return new CityPageDto(items, totalCount, filter.PageNumber, filter.PageSize);
    }

    public async Task<IReadOnlyCollection<GeographyLookupDto>> GetCityLookupAsync(string? countryCode = null, string? provinceCode = null, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<GeographyLookupDto>(new CommandDefinition("dbo.SP_NA_GET_CITIES_LOOKUP", new { CountryCode = countryCode, ProvinceCode = provinceCode }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();
    }

    public async Task<CityDto?> GetCityByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await GetCityByIdAsync(id, connection, null!, cancellationToken);
    }

    public Task<CityDto?> GetCityByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) => connection.QuerySingleOrDefaultAsync<CityDto>(new CommandDefinition("dbo.SP_NA_GET_CITIES_BUSCARPORID", new { Id = id }, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

    public async Task<bool> CityCodeExistsAsync(int provinceId, string code, int? excludingId = null, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await CityCodeExistsAsync(provinceId, code, excludingId, connection, null!, cancellationToken);
    }

    public async Task<bool> CityCodeExistsAsync(int provinceId, string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) => await connection.ExecuteScalarAsync<int>(new CommandDefinition("dbo.SP_NA_GET_CITIES_BUSCARPORCODIGO", new { ProvinceId = provinceId, Code = code, ExcluirId = excludingId }, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure)) > 0;

    public async Task<int> CreateCityAsync(SaveCityData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await CreateCityAsync(data, connection, null!, cancellationToken);
    }

    public Task<int> CreateCityAsync(SaveCityData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) => connection.ExecuteScalarAsync<int>(new CommandDefinition("dbo.SP_NA_POST_CITIES_CREAR", data, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

    public async Task<bool> UpdateCityAsync(SaveCityData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await UpdateCityAsync(data, connection, null!, cancellationToken);
    }

    public async Task<bool> UpdateCityAsync(SaveCityData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) => await connection.ExecuteScalarAsync<int>(new CommandDefinition("dbo.SP_NA_PUT_CITIES_ACTUALIZAR", data, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure)) > 0;

    public async Task<bool> DeleteCityAsync(int id, int? auditUserId, string? auditUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await DeleteCityAsync(id, auditUserId, auditUserName, connection, null!, cancellationToken);
    }

    public async Task<bool> DeleteCityAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) => await connection.ExecuteScalarAsync<int>(new CommandDefinition("dbo.SP_NA_DELETE_CITIES_ELIMINAR", new { Id = id, DeletedByUserId = auditUserId, DeletedByUserName = auditUserName }, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure)) > 0;
}
