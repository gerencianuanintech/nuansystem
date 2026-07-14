using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.TenantConfiguration.Dtos;
using NuanSystem.Persistence.Connections;

namespace NuanSystem.Persistence.Repositories;

public sealed class TenantFeatureRepository(IMasterConnectionFactory connectionFactory) : ITenantFeatureRepository
{
    public async Task<IReadOnlyCollection<TenantFeatureDto>> GetByCompanyIdAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var features = await connection.QueryAsync<TenantFeatureDto>(
            new CommandDefinition(
                """
SELECT
    FeatureCode,
    IsEnabled,
    CreatedAt,
    UpdatedAt
FROM dbo.TenantFeatures
WHERE CompanyId = @companyId
ORDER BY FeatureCode;
""",
                new { companyId },
                cancellationToken: cancellationToken));

        return features.AsList();
    }
}

