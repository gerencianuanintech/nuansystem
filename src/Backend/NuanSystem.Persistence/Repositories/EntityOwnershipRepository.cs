using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.TenantConfiguration.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Persistence.Connections;

namespace NuanSystem.Persistence.Repositories;

public sealed class EntityOwnershipRepository(IMasterConnectionFactory connectionFactory) : IEntityOwnershipRepository
{
    public async Task<IReadOnlyCollection<EntityOwnershipConfigurationDto>> GetByCompanyIdAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<EntityOwnershipRecord>(
            new CommandDefinition(
                """
SELECT
    EntityName,
    SourceOfTruth,
    SyncDirection,
    IsEnabled,
    CreatedAt,
    UpdatedAt
FROM dbo.EntityOwnershipConfigurations
WHERE CompanyId = @companyId
ORDER BY EntityName;
""",
                new { companyId },
                cancellationToken: cancellationToken));

        return rows.Select(Map).ToArray();
    }

    public async Task<EntityOwnershipConfigurationDto?> GetByCompanyIdAndEntityAsync(
        int companyId,
        string entityName,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var row = await connection.QuerySingleOrDefaultAsync<EntityOwnershipRecord>(
            new CommandDefinition(
                """
SELECT TOP (1)
    EntityName,
    SourceOfTruth,
    SyncDirection,
    IsEnabled,
    CreatedAt,
    UpdatedAt
FROM dbo.EntityOwnershipConfigurations
WHERE CompanyId = @companyId
  AND EntityName = @entityName;
""",
                new { companyId, entityName },
                cancellationToken: cancellationToken));

        return row is null ? null : Map(row);
    }

    private static EntityOwnershipConfigurationDto Map(EntityOwnershipRecord row)
    {
        return new EntityOwnershipConfigurationDto(
            row.EntityName,
            (EntitySourceOfTruth)row.SourceOfTruth,
            (EntitySyncDirection)row.SyncDirection,
            row.IsEnabled,
            row.CreatedAt,
            row.UpdatedAt);
    }

    private sealed record EntityOwnershipRecord(
        string EntityName,
        int SourceOfTruth,
        int SyncDirection,
        bool IsEnabled,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}

