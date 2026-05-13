using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.ConfigurationSettings.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class ConfigurationSettingRepository(
    IMasterConnectionFactory connectionFactory,
    ICompanyContext companyContext) : IConfigurationSettingRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_PARAMETROCONFIGURACIONLISTAR";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_PARAMETROCONFIGURACIONBUSCARPORID";
    private const string ExistsByKeyProcedure = "dbo.SP_NA_GET_PARAMETROCONFIGURACIONBUSCARPORCLAVE";
    private const string CreateProcedure = "dbo.SP_NA_POST_PARAMETROCONFIGURACIONCREAR";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_PARAMETROCONFIGURACIONACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_PARAMETROCONFIGURACIONELIMINAR";

    public async Task<IReadOnlyCollection<ConfigurationSettingDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var settings = await connection.QueryAsync<ConfigurationSettingDto>(
            new CommandDefinition(ListProcedure, new { CompanyId = GetCurrentCompanyId() }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return settings.AsList();
    }

    public async Task<ConfigurationSettingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ConfigurationSettingDto>(
            new CommandDefinition(GetByIdProcedure, new { CompanyId = GetCurrentCompanyId(), Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public Task<bool> ExistsByKeyAsync(string key, CancellationToken cancellationToken = default) => ExistsByKeyCoreAsync(key, null, cancellationToken);

    public Task<bool> ExistsByKeyAsync(string key, int excludingId, CancellationToken cancellationToken = default) => ExistsByKeyCoreAsync(key, excludingId, cancellationToken);

    public async Task<int> CreateAsync(CreateConfigurationSettingData setting, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(CreateProcedure, setting with { CompanyId = GetCurrentCompanyId() }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> UpdateAsync(UpdateConfigurationSettingData setting, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(UpdateProcedure, setting with { CompanyId = GetCurrentCompanyId() }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(DeleteProcedure, new { CompanyId = GetCurrentCompanyId(), Id = id, DeletedByUserId = deletedByUserId, DeletedByUserName = deletedByUserName }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return affectedRows > 0;
    }

    private async Task<bool> ExistsByKeyCoreAsync(string key, int? excludingId, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(ExistsByKeyProcedure, new { CompanyId = GetCurrentCompanyId(), Key = key, ExcluirId = excludingId }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
        return count > 0;
    }

    private int GetCurrentCompanyId()
    {
        return companyContext.CurrentCompany?.CompanyId
            ?? throw new InvalidOperationException("No hay empresa activa para consultar parametros.");
    }
}
