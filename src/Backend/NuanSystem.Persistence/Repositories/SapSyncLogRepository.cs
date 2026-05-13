using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class SapSyncLogRepository(ITenantConnectionFactory connectionFactory) : ISapSyncLogRepository
{
    public async Task<IReadOnlyCollection<SapSyncLogDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    Id,
    CompanyId,
    EntityType,
    EntityId,
    SapObjectType,
    Status,
    ErrorMessage,
    SapDocEntry,
    SapDocNum,
    CreatedAt,
    SyncedAt
FROM dbo.SapSyncLog
ORDER BY CreatedAt DESC, Id DESC;
""";

        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        var logs = await connection.QueryAsync<SapSyncLogDto>(command);

        return logs.AsList();
    }

    public async Task<long> CreateAsync(CreateSapSyncLogData log, CancellationToken cancellationToken = default)
    {
        const string sql = """
INSERT INTO dbo.SapSyncLog
(
    CompanyId,
    EntityType,
    EntityId,
    SapObjectType,
    RequestJson,
    ResponseJson,
    Status,
    ErrorMessage,
    SapDocEntry,
    SapDocNum,
    SyncedAt
)
OUTPUT INSERTED.Id
VALUES
(
    @CompanyId,
    @EntityType,
    @EntityId,
    @SapObjectType,
    @RequestJson,
    @ResponseJson,
    @Status,
    @ErrorMessage,
    @SapDocEntry,
    @SapDocNum,
    @SyncedAt
);
""";

        using var connection = connectionFactory.CreateConnection();
        var command = new CommandDefinition(sql, log, cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<long>(command);
    }
}
