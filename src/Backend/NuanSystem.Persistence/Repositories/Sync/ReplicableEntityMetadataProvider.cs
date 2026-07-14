using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class ReplicableEntityMetadataProvider(IMasterConnectionFactory connectionFactory) : IReplicableEntityMetadataProvider
{
    public async Task<ReplicableEntityMetadata> GetAsync(
        int companyId,
        string entityName,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT TOP (1)
    company.Id AS CompanyId,
    company.IsMaster,
    company.SyncEnabled,
    config.EntityName,
    CAST(CASE WHEN config.Id IS NULL AND activeProfile.Direction IS NULL THEN 0 ELSE 1 END AS bit) AS IsConfigured,
    CAST(CASE WHEN activeProfile.Direction IS NOT NULL OR COALESCE(config.IsEnabled, 0) = 1 THEN 1 ELSE 0 END AS bit) AS IsEnabled,
    COALESCE(activeProfile.Direction, config.Direction) AS Direction
FROM dbo.Companies AS company
LEFT JOIN dbo.SyncEntityConfigurations AS config
    ON config.CompanyId = company.Id
   AND config.EntityName = @EntityName
OUTER APPLY
(
    SELECT TOP (1)
        profile.Direction
    FROM dbo.SyncProfiles AS profile
    INNER JOIN dbo.SyncProfileEntities AS entity
        ON entity.SyncProfileId = profile.Id
       AND entity.IsDeleted = 0
       AND entity.IsActive = 1
       AND entity.EntityCode = @EntityName
       AND entity.SyncMode = N'Incremental'
    INNER JOIN dbo.SyncProfileEntityBranches AS matrix
        ON matrix.SyncProfileEntityId = entity.Id
       AND matrix.SyncProfileId = profile.Id
       AND matrix.IsDeleted = 0
       AND matrix.IsEnabled = 1
    INNER JOIN dbo.SyncProfileBranches AS branchConfig
        ON branchConfig.Id = matrix.SyncProfileBranchId
       AND branchConfig.SyncProfileId = profile.Id
       AND branchConfig.IsDeleted = 0
       AND branchConfig.IsActive = 1
    INNER JOIN dbo.Companies AS branch
        ON branch.Id = branchConfig.BranchCompanyId
       AND branch.IsActive = 1
       AND branch.IsMaster = 0
       AND branch.SyncEnabled = 1
       AND branch.ParentCompanyId = company.Id
       AND branch.IsDeleted = 0
    WHERE profile.CompanyId = company.Id
      AND profile.IsDeleted = 0
      AND profile.IsActive = 1
      AND profile.Direction = N'MasterToBranch'
      AND profile.ExecutionMode = N'Incremental'
      AND profile.ConflictStrategy = N'MasterWins'
) AS activeProfile
WHERE company.Id = @CompanyId;
""";

        using var connection = connectionFactory.CreateConnection();
        var row = await connection.QuerySingleOrDefaultAsync<ReplicableEntityMetadataRecord>(
            new CommandDefinition(
                sql,
                new { CompanyId = companyId, EntityName = entityName },
                cancellationToken: cancellationToken));

        if (row is null)
        {
            return new ReplicableEntityMetadata(
                companyId,
                IsMaster: false,
                SyncEnabled: false,
                entityName,
                IsConfigured: false,
                IsEnabled: false,
                Direction: null);
        }

        return new ReplicableEntityMetadata(
            row.CompanyId,
            row.IsMaster,
            row.SyncEnabled,
            row.EntityName ?? entityName,
            row.IsConfigured,
            row.IsEnabled,
            ParseDirection(row.Direction));
    }

    private static SyncDirection? ParseDirection(string? direction)
    {
        return Enum.TryParse<SyncDirection>(direction, ignoreCase: true, out var parsed)
            ? parsed
            : null;
    }

    private sealed record ReplicableEntityMetadataRecord(
        int CompanyId,
        bool IsMaster,
        bool SyncEnabled,
        string? EntityName,
        bool IsConfigured,
        bool IsEnabled,
        string? Direction);
}
