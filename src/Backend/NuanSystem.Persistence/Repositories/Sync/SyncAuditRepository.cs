using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using System.Text;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class SyncAuditRepository(IMasterConnectionFactory connectionFactory) : ISyncAuditRepository
{
    public async Task<long> AddAsync(CreateSyncAuditData data, CancellationToken cancellationToken = default)
    {
        const string sql = """
INSERT INTO dbo.SyncAudit
(
    CompanyId,
    BranchCompanyId,
    EventId,
    EntityName,
    EntityGlobalId,
    Action,
    PreviousStatus,
    NewStatus,
    Message,
    ErrorCode,
    ErrorDetail,
    CreatedBy
)
VALUES
(
    @CompanyId,
    @BranchCompanyId,
    @EventId,
    @EntityName,
    @EntityGlobalId,
    @Action,
    @PreviousStatus,
    @NewStatus,
    @Message,
    @ErrorCode,
    @ErrorDetail,
    @CreatedBy
);

SELECT CAST(SCOPE_IDENTITY() AS bigint);
""";

        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<long>(new CommandDefinition(
            sql,
            new
            {
                data.CompanyId,
                data.BranchCompanyId,
                data.EventId,
                data.EntityName,
                data.EntityGlobalId,
                Action = data.Action.ToString(),
                PreviousStatus = data.PreviousStatus?.ToString(),
                NewStatus = data.NewStatus?.ToString(),
                data.Message,
                data.ErrorCode,
                data.ErrorDetail,
                data.CreatedBy
            },
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<SyncAuditDto>> GetRecentAsync(int companyId, int take, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT TOP (@Take)
    Id,
    CompanyId,
    BranchCompanyId,
    EventId,
    EntityName,
    EntityGlobalId,
    Action,
    PreviousStatus,
    NewStatus,
    Message,
    ErrorCode,
    ErrorDetail,
    CreatedAt,
    CreatedBy
FROM dbo.SyncAudit
WHERE CompanyId = @CompanyId
ORDER BY CreatedAt DESC, Id DESC;
""";

        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SyncAuditDto>(new CommandDefinition(
            sql,
            new { CompanyId = companyId, Take = Math.Clamp(take, 1, 500) },
            cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<IReadOnlyCollection<SyncAuditDto>> SearchAuditAsync(
        int companyId,
        SyncAuditQueryFilter filter,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        var where = BuildAuditWhere(companyId, filter, parameters);
        parameters.Add("Offset", NormalizeOffset(filter.Page, filter.PageSize));
        parameters.Add("PageSize", NormalizePageSize(filter.PageSize));

        var sql = $"""
SELECT
    Id,
    CompanyId,
    BranchCompanyId,
    EventId,
    EntityName,
    EntityGlobalId,
    Action,
    PreviousStatus,
    NewStatus,
    Message,
    ErrorCode,
    ErrorDetail,
    CreatedAt,
    CreatedBy
FROM dbo.SyncAudit AS audit
{where}
ORDER BY audit.CreatedAt DESC, audit.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
""";

        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SyncAuditDto>(new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken));
        return rows.AsList();
    }

    private static int NormalizePageSize(int pageSize) => Math.Clamp(pageSize, 1, 500);

    private static int NormalizeOffset(int page, int pageSize)
    {
        var normalizedPage = Math.Max(page, 1);
        return (normalizedPage - 1) * NormalizePageSize(pageSize);
    }

    private static string BuildAuditWhere(int companyId, SyncAuditQueryFilter filter, DynamicParameters parameters)
    {
        var where = new StringBuilder("WHERE audit.CompanyId = @CompanyId");
        parameters.Add("CompanyId", companyId);

        if (filter.Status is not null)
        {
            where.AppendLine().Append("  AND audit.NewStatus = @Status");
            parameters.Add("Status", filter.Status.ToString());
        }

        if (!string.IsNullOrWhiteSpace(filter.EntityName))
        {
            where.AppendLine().Append("  AND audit.EntityName = @EntityName");
            parameters.Add("EntityName", filter.EntityName.Trim());
        }

        if (filter.EntityGlobalId is not null)
        {
            where.AppendLine().Append("  AND audit.EntityGlobalId = @EntityGlobalId");
            parameters.Add("EntityGlobalId", filter.EntityGlobalId);
        }

        if (filter.EventId is not null)
        {
            where.AppendLine().Append("  AND audit.EventId = @EventId");
            parameters.Add("EventId", filter.EventId);
        }

        if (filter.BranchCompanyId is not null)
        {
            where.AppendLine().Append("  AND audit.BranchCompanyId = @BranchCompanyId");
            parameters.Add("BranchCompanyId", filter.BranchCompanyId);
        }

        if (filter.CreatedFrom is not null)
        {
            where.AppendLine().Append("  AND audit.CreatedAt >= @CreatedFrom");
            parameters.Add("CreatedFrom", filter.CreatedFrom);
        }

        if (filter.CreatedTo is not null)
        {
            where.AppendLine().Append("  AND audit.CreatedAt <= @CreatedTo");
            parameters.Add("CreatedTo", filter.CreatedTo);
        }

        if (filter.HasErrors == true)
        {
            where.AppendLine().Append("  AND (audit.ErrorCode IS NOT NULL OR audit.ErrorDetail IS NOT NULL OR audit.Action IN (N'Failed', N'DeadLetter'))");
        }
        else if (filter.HasErrors == false)
        {
            where.AppendLine().Append("  AND audit.ErrorCode IS NULL AND audit.ErrorDetail IS NULL AND audit.Action NOT IN (N'Failed', N'DeadLetter')");
        }

        if (filter.DeadLetterOnly == true)
        {
            where.AppendLine().Append("  AND (audit.Action = N'DeadLetter' OR audit.NewStatus = N'DeadLetter')");
        }

        return where.ToString();
    }
}
