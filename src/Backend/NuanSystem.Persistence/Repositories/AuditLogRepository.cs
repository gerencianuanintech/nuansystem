using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Audit.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class AuditLogRepository(IMasterConnectionFactory connectionFactory) : IAuditLogRepository
{
    public async Task AddAsync(CreateAuditLogData auditLog, CancellationToken cancellationToken = default)
    {
        const string sql = """
INSERT INTO dbo.AuditLogs
(
    UserId,
    UserName,
    CompanyCode,
    HttpMethod,
    [Path],
    QueryString,
    StatusCode,
    IpAddress,
    UserAgent
)
VALUES
(
    @UserId,
    @UserName,
    @CompanyCode,
    @HttpMethod,
    @Path,
    @QueryString,
    @StatusCode,
    @IpAddress,
    @UserAgent
);
""";

        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, auditLog, cancellationToken: cancellationToken));
    }

    public async Task AddErrorAsync(CreateAuditErrorLogData errorLog, CancellationToken cancellationToken = default)
    {
        const string sql = """
INSERT INTO dbo.AuditErrorLogs
(
    [Source],
    UserId,
    UserName,
    CompanyCode,
    ModuleKey,
    FormName,
    ActionName,
    HttpMethod,
    [Path],
    QueryString,
    StatusCode,
    ErrorMessage,
    ExceptionType,
    StackTrace,
    TraceId,
    IpAddress,
    MachineName,
    UserAgent
)
VALUES
(
    @Source,
    @UserId,
    @UserName,
    @CompanyCode,
    @ModuleKey,
    @FormName,
    @ActionName,
    @HttpMethod,
    @Path,
    @QueryString,
    @StatusCode,
    @ErrorMessage,
    @ExceptionType,
    @StackTrace,
    @TraceId,
    @IpAddress,
    @MachineName,
    @UserAgent
);
""";

        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, errorLog, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<AuditLogDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT TOP (@Take)
    Id,
    UserId,
    UserName,
    CompanyCode,
    HttpMethod,
    [Path],
    QueryString,
    StatusCode,
    IpAddress,
    UserAgent,
    CreatedAt
FROM dbo.AuditLogs
ORDER BY Id DESC;
""";

        using var connection = connectionFactory.CreateConnection();
        var logs = await connection.QueryAsync<AuditLogDto>(
            new CommandDefinition(sql, new { Take = take }, cancellationToken: cancellationToken));

        return logs.AsList();
    }

    public async Task<IReadOnlyCollection<AuditErrorLogDto>> GetRecentErrorsAsync(int take, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT TOP (@Take)
    Id,
    [Source],
    UserId,
    UserName,
    CompanyCode,
    ModuleKey,
    FormName,
    ActionName,
    HttpMethod,
    [Path],
    QueryString,
    StatusCode,
    ErrorMessage,
    ExceptionType,
    StackTrace,
    TraceId,
    IpAddress,
    MachineName,
    UserAgent,
    CreatedAt
FROM dbo.AuditErrorLogs
ORDER BY Id DESC;
""";

        using var connection = connectionFactory.CreateConnection();
        var logs = await connection.QueryAsync<AuditErrorLogDto>(
            new CommandDefinition(sql, new { Take = take }, cancellationToken: cancellationToken));

        return logs.AsList();
    }

    public async Task<IReadOnlyCollection<SecurityChangeDto>> GetSecurityChangesAsync(
        string entityName,
        string recordId,
        int take,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT TOP (@Take)
    Id,
    EntityName,
    RecordId,
    [Action],
    FieldName,
    OldValue,
    NewValue,
    UserId,
    UserName,
    [Source],
    CreatedAt
FROM dbo.AuditSecurityChanges
WHERE EntityName = @EntityName
  AND RecordId = @RecordId
ORDER BY CreatedAt DESC, Id DESC;
""";

        using var connection = connectionFactory.CreateConnection();
        var changes = await connection.QueryAsync<SecurityChangeDto>(
            new CommandDefinition(sql, new { EntityName = entityName, RecordId = recordId, Take = take }, cancellationToken: cancellationToken));

        return changes.AsList();
    }
}
