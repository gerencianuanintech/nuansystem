using System.Data;
using Dapper;
using NuanSystem.Application.Features.Definitions.General.Common.Dtos;
using NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;

namespace NuanSystem.Persistence.Repositories.Definitions.General;

public sealed partial class GeographyRepository
{
    public async Task<IReadOnlyCollection<ProvinceDto>> GetProvincesAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<ProvinceDto>(new CommandDefinition("dbo.SP_NA_GET_PROVINCES_LISTAR", cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();
    }

    public async Task<IReadOnlyCollection<GeographyLookupDto>> GetProvinceLookupAsync(string? countryCode = null, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<GeographyLookupDto>(new CommandDefinition("dbo.SP_NA_GET_PROVINCES_LOOKUP", new { CountryCode = countryCode }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();
    }

    public async Task<ProvinceDto?> GetProvinceByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await GetProvinceByIdAsync(id, connection, null!, cancellationToken);
    }

    public Task<ProvinceDto?> GetProvinceByIdAsync(int id, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) => connection.QuerySingleOrDefaultAsync<ProvinceDto>(new CommandDefinition("dbo.SP_NA_GET_PROVINCES_BUSCARPORID", new { Id = id }, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

    public async Task<bool> ProvinceCodeExistsAsync(int countryId, string code, int? excludingId = null, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await ProvinceCodeExistsAsync(countryId, code, excludingId, connection, null!, cancellationToken);
    }

    public async Task<bool> ProvinceCodeExistsAsync(int countryId, string code, int? excludingId, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) => await connection.ExecuteScalarAsync<int>(new CommandDefinition("dbo.SP_NA_GET_PROVINCES_BUSCARPORCODIGO", new { CountryId = countryId, Code = code, ExcluirId = excludingId }, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure)) > 0;

    public async Task<int> CreateProvinceAsync(SaveProvinceData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await CreateProvinceAsync(data, connection, null!, cancellationToken);
    }

    public Task<int> CreateProvinceAsync(SaveProvinceData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) => connection.ExecuteScalarAsync<int>(new CommandDefinition("dbo.SP_NA_POST_PROVINCES_CREAR", data, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

    public async Task<bool> UpdateProvinceAsync(SaveProvinceData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await UpdateProvinceAsync(data, connection, null!, cancellationToken);
    }

    public async Task<bool> UpdateProvinceAsync(SaveProvinceData data, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) => await connection.ExecuteScalarAsync<int>(new CommandDefinition("dbo.SP_NA_PUT_PROVINCES_ACTUALIZAR", data, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure)) > 0;

    public async Task<bool> DeleteProvinceAsync(int id, int? auditUserId, string? auditUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await DeleteProvinceAsync(id, auditUserId, auditUserName, connection, null!, cancellationToken);
    }

    public async Task<bool> DeleteProvinceAsync(int id, int? auditUserId, string? auditUserName, IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default) => await connection.ExecuteScalarAsync<int>(new CommandDefinition("dbo.SP_NA_DELETE_PROVINCES_ELIMINAR", new { Id = id, DeletedByUserId = auditUserId, DeletedByUserName = auditUserName }, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure)) > 0;
}
