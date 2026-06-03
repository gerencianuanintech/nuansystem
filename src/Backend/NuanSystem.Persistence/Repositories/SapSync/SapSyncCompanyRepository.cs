using Dapper;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Persistence.Connections;

namespace NuanSystem.Persistence.Repositories.SapSync;

public sealed class SapSyncCompanyRepository(MasterConnectionFactory connectionFactory) : ISapSyncCompanyRepository
{
    public async Task<IReadOnlyCollection<SapSyncCompanyDto>> GetActiveSapCompaniesAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT c.Id AS CompanyId, c.Code AS CompanyCode, c.CommercialName AS CompanyName, s.IntegrationMode, s.IsEnabled AS IsSapEnabled
FROM dbo.Companies c
INNER JOIN dbo.SapCompanySettings s ON s.CompanyId = c.Id
WHERE c.IsActive = 1 AND c.SapIntegrationMode <> 0 AND s.IsEnabled = 1 AND s.IntegrationMode <> 0
ORDER BY c.Code;
""";
        await using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SapSyncCompanyDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return rows.AsList();
    }
}
