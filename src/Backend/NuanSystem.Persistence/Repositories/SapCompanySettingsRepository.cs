using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class SapCompanySettingsRepository(IMasterConnectionFactory connectionFactory) : ISapCompanySettingsRepository
{
    public async Task<SapCompanySettingsDto?> GetByCompanyIdAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        const string sql = SelectSql + """
WHERE c.Id = @CompanyId;
""";

        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { CompanyId = companyId }, cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<SapCompanySettingsDto>(command);
    }

    public async Task<SapCompanySettingsDto?> GetByCompanyCodeAsync(
        string companyCode,
        CancellationToken cancellationToken = default)
    {
        const string sql = SelectSql + """
WHERE c.Code = @CompanyCode;
""";

        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, new { CompanyCode = companyCode }, cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<SapCompanySettingsDto>(command);
    }

    // Configuration is read from the master database because SAP settings belong to the company, not to one tenant schema.
    private const string SelectSql = """
SELECT TOP (1)
    s.Id,
    s.CompanyId,
    c.Code AS CompanyCode,
    s.IsEnabled,
    s.IntegrationMode,
    s.ServiceLayerUrl,
    s.SapCompanyDb,
    s.SapUser,
    s.SapPasswordEncrypted,
    s.DiApiServer,
    s.LicenseServer,
    s.Language,
    s.HanaServer,
    s.HanaPort,
    s.HanaSchema,
    s.HanaUser,
    s.HanaPasswordEncrypted,
    s.MaxRetryCount
FROM dbo.SapCompanySettings s
INNER JOIN dbo.Companies c ON c.Id = s.CompanyId
""";
}
