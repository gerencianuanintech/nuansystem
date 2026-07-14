using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.TenantConfiguration.Dtos;
using NuanSystem.Persistence.Connections;

namespace NuanSystem.Persistence.Repositories;

public sealed class TenantIntegrationRepository(IMasterConnectionFactory connectionFactory) : ITenantIntegrationRepository
{
    public async Task<IReadOnlyCollection<TenantIntegrationDto>> GetByCompanyIdAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var integrations = await connection.QueryAsync<TenantIntegrationDto>(
            new CommandDefinition(
                """
SELECT
    IntegrationCode,
    IsEnabled,
    ConfigurationJson,
    CreatedAt,
    UpdatedAt
FROM dbo.TenantIntegrations
WHERE CompanyId = @companyId
ORDER BY IntegrationCode;
""",
                new { companyId },
                cancellationToken: cancellationToken));

        return integrations.AsList();
    }
}

