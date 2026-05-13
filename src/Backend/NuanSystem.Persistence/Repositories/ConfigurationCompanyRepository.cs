using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.ConfigurationCompanies.Dtos;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Persistence.Repositories;

public sealed class ConfigurationCompanyRepository(IMasterConnectionFactory connectionFactory) : IConfigurationCompanyRepository
{
    private const string ListProcedure = "dbo.SP_NA_GET_COMPANIACONFIGURACIONLISTAR";
    private const string GetByIdProcedure = "dbo.SP_NA_GET_COMPANIACONFIGURACIONBUSCARPORID";
    private const string ExistsByCodeProcedure = "dbo.SP_NA_GET_COMPANIACONFIGURACIONBUSCARPORCODIGO";
    private const string CreateProcedure = "dbo.SP_NA_POST_COMPANIACONFIGURACIONCREAR";
    private const string UpdateProcedure = "dbo.SP_NA_PUT_COMPANIACONFIGURACIONACTUALIZAR";
    private const string DeleteProcedure = "dbo.SP_NA_DELETE_COMPANIACONFIGURACIONELIMINAR";

    public async Task<IReadOnlyCollection<ConfigurationCompanyDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var companies = (await connection.QueryAsync<ConfigurationCompanyRecord>(
            new CommandDefinition(ListProcedure, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure))).AsList();

        return companies.Select(MapCompany).ToArray();
    }

    public async Task<ConfigurationCompanyDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var company = await connection.QuerySingleOrDefaultAsync<ConfigurationCompanyRecord>(
            new CommandDefinition(GetByIdProcedure, new { Id = id }, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return company is null ? null : MapCompany(company);
    }

    public Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return ExistsByCodeCoreAsync(code, null, cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default)
    {
        return ExistsByCodeCoreAsync(code, excludingId, cancellationToken);
    }

    public async Task<int> CreateAsync(CreateConfigurationCompanyData company, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(CreateProcedure, company, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));
    }

    public async Task<bool> UpdateAsync(UpdateConfigurationCompanyData company, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(UpdateProcedure, company, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(
        int id,
        int? deletedByUserId,
        string? deletedByUserName,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                DeleteProcedure,
                new { Id = id, DeletedByUserId = deletedByUserId, DeletedByUserName = deletedByUserName },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return affectedRows > 0;
    }

    private async Task<bool> ExistsByCodeCoreAsync(string code, int? excludingId, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                ExistsByCodeProcedure,
                new { Code = code, ExcluirId = excludingId },
                cancellationToken: cancellationToken,
                commandType: CommandType.StoredProcedure));

        return count > 0;
    }

    private static ConfigurationCompanyDto MapCompany(ConfigurationCompanyRecord company)
    {
        return new ConfigurationCompanyDto(
            company.Id,
            company.Code,
            company.CommercialName,
            company.LegalName,
            company.TaxIdentification,
            company.Address,
            company.Phone,
            company.Email,
            company.LogoImage,
            company.LogoImageContentType,
            company.LogoImageFileName,
            (DatabaseEngine)company.DatabaseEngine,
            company.Server,
            company.Port,
            company.DatabaseName,
            company.DatabaseUser,
            company.IsActive,
            (SapIntegrationMode)company.SapIntegrationMode,
            company.DisplayOrder,
            company.IsDefault,
            company.TimeZoneId,
            company.CultureCode,
            company.CurrencyCode,
            company.CreatedByUserId,
            company.CreatedByUserName,
            company.CreatedAt,
            company.UpdatedByUserId,
            company.UpdatedByUserName,
            company.UpdatedAt,
            company.DeletedByUserId,
            company.DeletedByUserName,
            company.DeletedAt);
    }

    private sealed record ConfigurationCompanyRecord(
        int Id,
        string Code,
        string CommercialName,
        string? LegalName,
        string? TaxIdentification,
        string? Address,
        string? Phone,
        string? Email,
        byte[]? LogoImage,
        string? LogoImageContentType,
        string? LogoImageFileName,
        int DatabaseEngine,
        string Server,
        int? Port,
        string DatabaseName,
        string DatabaseUser,
        bool IsActive,
        int SapIntegrationMode,
        int DisplayOrder,
        bool IsDefault,
        string TimeZoneId,
        string CultureCode,
        string CurrencyCode,
        int? CreatedByUserId,
        string? CreatedByUserName,
        DateTime CreatedAt,
        int? UpdatedByUserId,
        string? UpdatedByUserName,
        DateTime? UpdatedAt,
        int? DeletedByUserId,
        string? DeletedByUserName,
        DateTime? DeletedAt);
}
