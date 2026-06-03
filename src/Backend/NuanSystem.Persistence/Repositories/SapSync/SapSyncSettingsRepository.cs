using Dapper;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Persistence.Connections;

namespace NuanSystem.Persistence.Repositories.SapSync;

public sealed class SapSyncSettingsRepository(MasterConnectionFactory connectionFactory) : ISapSyncSettingsRepository
{
    public async Task<IReadOnlyCollection<SapSyncEntitySettingsDto>> GetEnabledEntitiesAsync(int companyId, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT s.Id, s.CompanyId, c.Code AS CompanyCode, s.EntityCode, s.Direction, s.IsEnabled, s.BatchSize, s.MaxRetryCount, s.ExecutionOrder, s.CreatedAt, s.UpdatedAt
FROM dbo.SapSyncEntitySettings s
INNER JOIN dbo.Companies c ON c.Id = s.CompanyId
WHERE s.CompanyId = @CompanyId AND s.IsEnabled = 1
ORDER BY s.ExecutionOrder, s.EntityCode;
""";
        await using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SapSyncEntitySettingsDto>(new CommandDefinition(sql, new { CompanyId = companyId }, cancellationToken: cancellationToken));
        return rows.AsList();
    }
}
