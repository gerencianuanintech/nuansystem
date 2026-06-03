using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Persistence.Repositories.SapSync;

public sealed class SapSyncWatermarkRepository(ITenantConnectionFactory connectionFactory) : ISapSyncWatermarkRepository
{
    public async Task<SapSyncWatermarkDto?> GetAsync(int companyId, string entityCode, SapSyncDirection direction, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT TOP (1) Id, CompanyId, EntityCode, Direction, LastSuccessfulSyncAtUtc, LastSapKey, LastLocalKey, MetadataJson, CreatedAt, UpdatedAt
FROM dbo.SapSyncWatermark
WHERE CompanyId = @CompanyId AND EntityCode = @EntityCode AND Direction = @Direction;
""";
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SapSyncWatermarkDto>(new CommandDefinition(sql, new { CompanyId = companyId, EntityCode = entityCode, Direction = direction.ToString() }, cancellationToken: cancellationToken));
    }

    public async Task UpsertSuccessAsync(int companyId, string entityCode, SapSyncDirection direction, DateTime syncedAtUtc, string? lastSapKey, string? metadataJson, CancellationToken cancellationToken = default)
    {
        const string sql = """
MERGE dbo.SapSyncWatermark AS target
USING (SELECT @CompanyId AS CompanyId, @EntityCode AS EntityCode, @Direction AS Direction) AS source
ON target.CompanyId = source.CompanyId AND target.EntityCode = source.EntityCode AND target.Direction = source.Direction
WHEN MATCHED THEN
    UPDATE SET LastSuccessfulSyncAtUtc = @SyncedAtUtc, LastSapKey = @LastSapKey, MetadataJson = @MetadataJson, UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT (CompanyId, EntityCode, Direction, LastSuccessfulSyncAtUtc, LastSapKey, MetadataJson)
    VALUES (@CompanyId, @EntityCode, @Direction, @SyncedAtUtc, @LastSapKey, @MetadataJson);
""";
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, new { CompanyId = companyId, EntityCode = entityCode, Direction = direction.ToString(), SyncedAtUtc = syncedAtUtc, LastSapKey = lastSapKey, MetadataJson = metadataJson }, cancellationToken: cancellationToken));
    }
}
