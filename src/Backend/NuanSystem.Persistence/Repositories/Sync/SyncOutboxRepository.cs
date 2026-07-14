using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;
using System.Data;
using System.Text;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class SyncOutboxRepository(IMasterConnectionFactory connectionFactory) : ISyncOutboxRepository
{
    private const string ClaimPendingSql = """
;WITH Claimable AS
(
    SELECT TOP (@Take) *
    FROM dbo.SyncOutbox WITH (ROWLOCK, READPAST, UPDLOCK)
    WHERE Status IN (N'Pending', N'Error')
      AND AttemptCount < MaxAttempts
      AND (NextRetryAt IS NULL OR NextRetryAt <= SYSUTCDATETIME())
      AND (LockedBy IS NULL OR LockExpiresAt IS NULL OR LockExpiresAt <= SYSUTCDATETIME())
    ORDER BY CreatedAt, Id
)
UPDATE Claimable
SET Status = N'InProcess',
    AttemptCount = AttemptCount + 1,
    LockedBy = @LockedBy,
    LockedAt = SYSUTCDATETIME(),
    LockExpiresAt = DATEADD(minute, @LockMinutes, SYSUTCDATETIME()),
    LastErrorMessage = NULL
OUTPUT
    INSERTED.Id,
    INSERTED.EventId,
    INSERTED.CompanyId,
    INSERTED.EntityName,
    INSERTED.EntityGlobalId,
    INSERTED.EntityCode,
    INSERTED.Operation,
    INSERTED.PayloadJson,
    INSERTED.SourceSystem,
    INSERTED.SourceReference,
    INSERTED.Status,
    INSERTED.AttemptCount,
    INSERTED.MaxAttempts,
    INSERTED.NextRetryAt,
    INSERTED.LockedBy,
    INSERTED.LockedAt,
    INSERTED.LockExpiresAt,
    INSERTED.CreatedAt,
    INSERTED.ProcessedAt,
    INSERTED.LastErrorMessage;
""";

    private const string ReleaseExpiredLocksSql = """
UPDATE dbo.SyncOutbox
SET Status = CASE
        WHEN AttemptCount >= MaxAttempts THEN N'DeadLetter'
        ELSE N'Pending'
    END,
    LockedBy = NULL,
    LockedAt = NULL,
    LockExpiresAt = NULL,
    ProcessedAt = CASE
        WHEN AttemptCount >= MaxAttempts THEN SYSUTCDATETIME()
        ELSE ProcessedAt
    END,
    LastErrorMessage = CASE
        WHEN AttemptCount >= MaxAttempts THEN COALESCE(LastErrorMessage, N'Lock expirado y maximo de intentos alcanzado.')
        ELSE LastErrorMessage
    END
WHERE Status = N'InProcess'
  AND LockExpiresAt IS NOT NULL
  AND LockExpiresAt <= SYSUTCDATETIME();
""";

    public async Task<long> CreateAsync(CreateSyncOutboxEventData data, CancellationToken cancellationToken = default)
    {
        const string sql = """
IF EXISTS (SELECT 1 FROM dbo.SyncOutbox WHERE EventId = @EventId)
BEGIN
    SELECT Id FROM dbo.SyncOutbox WHERE EventId = @EventId;
    RETURN;
END;

BEGIN TRY
    INSERT INTO dbo.SyncOutbox
    (
        EventId,
        CompanyId,
        EntityName,
        EntityGlobalId,
        EntityCode,
        Operation,
        PayloadJson,
        SourceSystem,
        SourceReference,
        Status,
        MaxAttempts
    )
    VALUES
    (
        @EventId,
        @CompanyId,
        @EntityName,
        @EntityGlobalId,
        @EntityCode,
        @Operation,
        @PayloadJson,
        @SourceSystem,
        @SourceReference,
        @Status,
        @MaxAttempts
    );

    SELECT CAST(SCOPE_IDENTITY() AS bigint);
END TRY
BEGIN CATCH
    IF ERROR_NUMBER() IN (2601, 2627)
    BEGIN
        SELECT Id FROM dbo.SyncOutbox WHERE EventId = @EventId;
        RETURN;
    END;

    THROW;
END CATCH;
""";

        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<long>(new CommandDefinition(
            sql,
            new
            {
                data.EventId,
                data.CompanyId,
                data.EntityName,
                data.EntityGlobalId,
                data.EntityCode,
                Operation = data.Operation.ToString(),
                data.PayloadJson,
                data.SourceSystem,
                data.SourceReference,
                Status = SyncEventStatus.Pending.ToString(),
                data.MaxAttempts
            },
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<SyncOutboxDto>> GetPendingAsync(int companyId, int take, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT TOP (@Take)
    Id,
    EventId,
    CompanyId,
    EntityName,
    EntityGlobalId,
    EntityCode,
    Operation,
    PayloadJson,
    SourceSystem,
    SourceReference,
    Status,
    AttemptCount,
    MaxAttempts,
    NextRetryAt,
    LockedBy,
    LockedAt,
    LockExpiresAt,
    CreatedAt,
    ProcessedAt,
    LastErrorMessage
FROM dbo.SyncOutbox
WHERE CompanyId = @CompanyId
  AND Status = N'Pending'
  AND (NextRetryAt IS NULL OR NextRetryAt <= SYSUTCDATETIME())
ORDER BY CreatedAt, Id;
""";

        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SyncOutboxDto>(new CommandDefinition(
            sql,
            new { CompanyId = companyId, Take = NormalizeTake(take) },
            cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<IReadOnlyCollection<SyncOutboxDto>> ClaimPendingAsync(
        string lockedBy,
        int take,
        TimeSpan lockDuration,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await ClaimPendingAsync(connection, transaction: null, lockedBy, take, lockDuration, cancellationToken);
    }

    public async Task<int> ReleaseExpiredLocksAsync(CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await ReleaseExpiredLocksAsync(connection, transaction: null, cancellationToken);
    }

    internal async Task<IReadOnlyCollection<SyncOutboxDto>> ClaimPendingAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        string lockedBy,
        int take,
        TimeSpan lockDuration,
        CancellationToken cancellationToken = default)
    {
        var rows = await connection.QueryAsync<SyncOutboxDto>(new CommandDefinition(
            ClaimPendingSql,
            new
            {
                LockedBy = NormalizeLockedBy(lockedBy),
                Take = NormalizeTake(take),
                LockMinutes = NormalizeLockMinutes(lockDuration)
            },
            transaction,
            cancellationToken: cancellationToken));
        return rows.AsList();
    }

    internal Task<int> ReleaseExpiredLocksAsync(
        IDbConnection connection,
        IDbTransaction? transaction,
        CancellationToken cancellationToken = default)
    {
        return connection.ExecuteAsync(new CommandDefinition(
            ReleaseExpiredLocksSql,
            transaction: transaction,
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<SyncOutboxDto>> GetRecentAsync(int companyId, int take, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT TOP (@Take)
    Id,
    EventId,
    CompanyId,
    EntityName,
    EntityGlobalId,
    EntityCode,
    Operation,
    PayloadJson,
    SourceSystem,
    SourceReference,
    Status,
    AttemptCount,
    MaxAttempts,
    NextRetryAt,
    LockedBy,
    LockedAt,
    LockExpiresAt,
    CreatedAt,
    ProcessedAt,
    LastErrorMessage
FROM dbo.SyncOutbox
WHERE CompanyId = @CompanyId
ORDER BY CreatedAt DESC, Id DESC;
""";

        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SyncOutboxDto>(new CommandDefinition(
            sql,
            new { CompanyId = companyId, Take = NormalizeTake(take) },
            cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<SyncDashboardDto> GetDashboardAsync(int companyId, int take, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    Status,
    COUNT(1) AS Count
FROM dbo.SyncOutbox
WHERE CompanyId = @CompanyId
GROUP BY Status;

SELECT
    EntityName,
    Status,
    COUNT(1) AS Count
FROM dbo.SyncOutbox
WHERE CompanyId = @CompanyId
GROUP BY EntityName, Status
ORDER BY EntityName, Status;

SELECT
    target.BranchCompanyId,
    target.Status,
    COUNT(1) AS Count
FROM dbo.SyncOutboxTargets AS target
INNER JOIN dbo.SyncOutbox AS outbox
    ON outbox.Id = target.OutboxId
WHERE outbox.CompanyId = @CompanyId
GROUP BY target.BranchCompanyId, target.Status
ORDER BY target.BranchCompanyId, target.Status;

SELECT TOP (@Take)
    Id,
    EventId,
    CompanyId,
    EntityName,
    EntityGlobalId,
    EntityCode,
    Operation,
    SourceSystem,
    SourceReference,
    Status,
    AttemptCount,
    MaxAttempts,
    NextRetryAt,
    LockedBy,
    LockedAt,
    LockExpiresAt,
    CreatedAt,
    ProcessedAt,
    LastErrorMessage
FROM dbo.SyncOutbox
WHERE CompanyId = @CompanyId
  AND (LastErrorMessage IS NOT NULL OR Status IN (N'Error', N'DeadLetter'))
ORDER BY COALESCE(ProcessedAt, CreatedAt) DESC, Id DESC;

SELECT TOP (@Take)
    Id,
    EventId,
    CompanyId,
    EntityName,
    EntityGlobalId,
    EntityCode,
    Operation,
    SourceSystem,
    SourceReference,
    Status,
    AttemptCount,
    MaxAttempts,
    NextRetryAt,
    LockedBy,
    LockedAt,
    LockExpiresAt,
    CreatedAt,
    ProcessedAt,
    LastErrorMessage
FROM dbo.SyncOutbox
WHERE CompanyId = @CompanyId
ORDER BY CreatedAt DESC, Id DESC;
""";

        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(new CommandDefinition(
            sql,
            new { CompanyId = companyId, Take = NormalizeTake(take) },
            cancellationToken: cancellationToken));

        var statusCounts = (await grid.ReadAsync<SyncStatusCountDto>()).AsList();
        var entityCounts = (await grid.ReadAsync<SyncEntityStatusCountDto>()).AsList();
        var branchCounts = (await grid.ReadAsync<SyncBranchStatusCountDto>()).AsList();
        var latestErrors = (await grid.ReadAsync<SyncOutboxListItemDto>()).AsList();
        var latestEvents = (await grid.ReadAsync<SyncOutboxListItemDto>()).AsList();

        return new SyncDashboardDto(
            GetStatusCount(statusCounts, SyncEventStatus.Pending),
            GetStatusCount(statusCounts, SyncEventStatus.InProcess),
            GetStatusCount(statusCounts, SyncEventStatus.Applied),
            GetStatusCount(statusCounts, SyncEventStatus.Error),
            GetStatusCount(statusCounts, SyncEventStatus.DeadLetter),
            GetStatusCount(statusCounts, SyncEventStatus.Ignored),
            latestErrors,
            latestEvents,
            entityCounts,
            branchCounts);
    }

    public async Task<SyncSummaryDto> GetSummaryAsync(int companyId, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    Status,
    COUNT(1) AS Count
FROM dbo.SyncOutbox
WHERE CompanyId = @CompanyId
GROUP BY Status;

SELECT
    EntityName,
    Status,
    COUNT(1) AS Count
FROM dbo.SyncOutbox
WHERE CompanyId = @CompanyId
GROUP BY EntityName, Status
ORDER BY EntityName, Status;

SELECT
    target.BranchCompanyId,
    target.Status,
    COUNT(1) AS Count
FROM dbo.SyncOutboxTargets AS target
INNER JOIN dbo.SyncOutbox AS outbox
    ON outbox.Id = target.OutboxId
WHERE outbox.CompanyId = @CompanyId
GROUP BY target.BranchCompanyId, target.Status
ORDER BY target.BranchCompanyId, target.Status;
""";

        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(new CommandDefinition(
            sql,
            new { CompanyId = companyId },
            cancellationToken: cancellationToken));

        var statusCounts = (await grid.ReadAsync<SyncStatusCountDto>()).AsList();
        var entityCounts = (await grid.ReadAsync<SyncEntityStatusCountDto>()).AsList();
        var branchCounts = (await grid.ReadAsync<SyncBranchStatusCountDto>()).AsList();

        return new SyncSummaryDto(
            GetStatusCount(statusCounts, SyncEventStatus.Pending),
            GetStatusCount(statusCounts, SyncEventStatus.InProcess),
            GetStatusCount(statusCounts, SyncEventStatus.Applied),
            GetStatusCount(statusCounts, SyncEventStatus.Error),
            GetStatusCount(statusCounts, SyncEventStatus.DeadLetter),
            GetStatusCount(statusCounts, SyncEventStatus.Ignored),
            statusCounts,
            entityCounts,
            branchCounts);
    }

    public async Task<IReadOnlyCollection<SyncOutboxListItemDto>> SearchOutboxAsync(
        int companyId,
        SyncOutboxQueryFilter filter,
        CancellationToken cancellationToken = default)
    {
        var parameters = new DynamicParameters();
        var where = BuildOutboxWhere(companyId, filter, parameters);
        parameters.Add("Offset", NormalizeOffset(filter.Page, filter.PageSize));
        parameters.Add("PageSize", NormalizePageSize(filter.PageSize));

        var sql = $"""
SELECT
    outbox.Id,
    outbox.EventId,
    outbox.CompanyId,
    outbox.EntityName,
    outbox.EntityGlobalId,
    outbox.EntityCode,
    outbox.Operation,
    outbox.SourceSystem,
    outbox.SourceReference,
    outbox.Status,
    outbox.AttemptCount,
    outbox.MaxAttempts,
    outbox.NextRetryAt,
    outbox.LockedBy,
    outbox.LockedAt,
    outbox.LockExpiresAt,
    outbox.CreatedAt,
    outbox.ProcessedAt,
    outbox.LastErrorMessage
FROM dbo.SyncOutbox AS outbox
{where}
ORDER BY outbox.CreatedAt DESC, outbox.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
""";

        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SyncOutboxListItemDto>(new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<SyncOutboxDetailDto?> GetOutboxDetailAsync(int companyId, long id, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    Id,
    EventId,
    CompanyId,
    EntityName,
    EntityGlobalId,
    EntityCode,
    Operation,
    PayloadJson,
    SourceSystem,
    SourceReference,
    Status,
    AttemptCount,
    MaxAttempts,
    NextRetryAt,
    LockedBy,
    LockedAt,
    LockExpiresAt,
    CreatedAt,
    ProcessedAt,
    LastErrorMessage
FROM dbo.SyncOutbox
WHERE CompanyId = @CompanyId
  AND Id = @Id;
""";

        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SyncOutboxDetailDto>(new CommandDefinition(
            sql,
            new { CompanyId = companyId, Id = id },
            cancellationToken: cancellationToken));
    }

    public async Task<SyncOutboxDto?> GetByIdAsync(int companyId, long id, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    Id,
    EventId,
    CompanyId,
    EntityName,
    EntityGlobalId,
    EntityCode,
    Operation,
    PayloadJson,
    SourceSystem,
    SourceReference,
    Status,
    AttemptCount,
    MaxAttempts,
    NextRetryAt,
    LockedBy,
    LockedAt,
    LockExpiresAt,
    CreatedAt,
    ProcessedAt,
    LastErrorMessage
FROM dbo.SyncOutbox
WHERE CompanyId = @CompanyId
  AND Id = @Id;
""";

        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SyncOutboxDto>(new CommandDefinition(
            sql,
            new { CompanyId = companyId, Id = id },
            cancellationToken: cancellationToken));
    }

    public async Task<SyncOutboxActionResultDto?> RetryErrorAsync(
        int companyId,
        long id,
        string? reason,
        string? createdBy,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        var current = await GetActionRowAsync(connection, transaction, companyId, id, cancellationToken);
        if (current is null || current.Status != SyncEventStatus.Error)
        {
            transaction.Rollback();
            return null;
        }

        const string sql = """
UPDATE dbo.SyncOutbox
SET Status = N'Pending',
    NextRetryAt = NULL,
    LockedBy = NULL,
    LockedAt = NULL,
    LockExpiresAt = NULL,
    ProcessedAt = NULL
WHERE CompanyId = @CompanyId
  AND Id = @Id
  AND Status = N'Error';
""";

        var affectedRows = await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { CompanyId = companyId, Id = id },
            transaction,
            cancellationToken: cancellationToken));

        if (affectedRows != 1)
        {
            transaction.Rollback();
            return null;
        }

        await AddAuditAsync(
            connection,
            transaction,
            current,
            SyncAuditAction.Retried,
            SyncEventStatus.Pending,
            BuildMessage("Reintento manual de evento Error.", reason),
            createdBy,
            cancellationToken);

        transaction.Commit();
        return CreateActionResult(current, SyncEventStatus.Pending, current.AttemptCount, "Evento Error devuelto a Pending.");
    }

    public async Task<SyncOutboxActionResultDto?> RetryDeadLetterAsync(
        int companyId,
        long id,
        string reason,
        bool resetAttemptCount,
        string? createdBy,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        var current = await GetActionRowAsync(connection, transaction, companyId, id, cancellationToken);
        if (current is null || current.Status != SyncEventStatus.DeadLetter)
        {
            transaction.Rollback();
            return null;
        }

        var newAttemptCount = resetAttemptCount ? 0 : current.AttemptCount;

        const string sql = """
UPDATE dbo.SyncOutbox
SET Status = N'Pending',
    AttemptCount = CASE WHEN @ResetAttemptCount = 1 THEN 0 ELSE AttemptCount END,
    NextRetryAt = NULL,
    LockedBy = NULL,
    LockedAt = NULL,
    LockExpiresAt = NULL,
    ProcessedAt = NULL
WHERE CompanyId = @CompanyId
  AND Id = @Id
  AND Status = N'DeadLetter';
""";

        var affectedRows = await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { CompanyId = companyId, Id = id, ResetAttemptCount = resetAttemptCount },
            transaction,
            cancellationToken: cancellationToken));

        if (affectedRows != 1)
        {
            transaction.Rollback();
            return null;
        }

        await AddAuditAsync(
            connection,
            transaction,
            current,
            SyncAuditAction.RetriedFromDeadLetter,
            SyncEventStatus.Pending,
            BuildMessage(resetAttemptCount
                ? "Reintento manual de DeadLetter con reset de intentos."
                : "Reintento manual de DeadLetter sin reset de intentos.",
                reason),
            createdBy,
            cancellationToken);

        transaction.Commit();
        return CreateActionResult(current, SyncEventStatus.Pending, newAttemptCount, "Evento DeadLetter devuelto a Pending.");
    }

    public async Task<SyncOutboxActionResultDto?> ReleaseExpiredLockAsync(
        int companyId,
        long id,
        string? reason,
        string? createdBy,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        var current = await GetActionRowAsync(connection, transaction, companyId, id, cancellationToken);
        if (current is null ||
            current.Status is not (SyncEventStatus.InProcess or SyncEventStatus.Error) ||
            current.LockExpiresAt is null ||
            current.LockExpiresAt.Value >= DateTime.UtcNow)
        {
            transaction.Rollback();
            return null;
        }

        var newStatus = current.Status == SyncEventStatus.InProcess
            ? SyncEventStatus.Pending
            : SyncEventStatus.Error;

        const string sql = """
UPDATE dbo.SyncOutbox
SET Status = CASE WHEN Status = N'InProcess' THEN N'Pending' ELSE Status END,
    LockedBy = NULL,
    LockedAt = NULL,
    LockExpiresAt = NULL
WHERE CompanyId = @CompanyId
  AND Id = @Id
  AND Status IN (N'InProcess', N'Error')
  AND LockExpiresAt IS NOT NULL
  AND LockExpiresAt < SYSUTCDATETIME();
""";

        var affectedRows = await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { CompanyId = companyId, Id = id },
            transaction,
            cancellationToken: cancellationToken));

        if (affectedRows != 1)
        {
            transaction.Rollback();
            return null;
        }

        await AddAuditAsync(
            connection,
            transaction,
            current,
            SyncAuditAction.LockReleased,
            newStatus,
            BuildMessage("Lock vencido liberado manualmente.", reason),
            createdBy,
            cancellationToken);

        transaction.Commit();
        return CreateActionResult(current, newStatus, current.AttemptCount, "Lock vencido liberado.");
    }

    public async Task<IReadOnlyCollection<SyncOutboxTargetDto>> GetTargetsAsync(int companyId, long outboxId, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    target.Id,
    target.OutboxId,
    target.BranchCompanyId,
    target.Status,
    target.AttemptCount,
    target.MaxAttempts,
    target.NextRetryAt,
    target.AppliedAt,
    target.LastErrorMessage,
    target.CreatedAt,
    target.UpdatedAt
FROM dbo.SyncOutboxTargets AS target
INNER JOIN dbo.SyncOutbox AS outbox
    ON outbox.Id = target.OutboxId
WHERE outbox.CompanyId = @CompanyId
  AND target.OutboxId = @OutboxId
ORDER BY target.CreatedAt, target.Id;
""";

        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SyncOutboxTargetDto>(new CommandDefinition(
            sql,
            new { CompanyId = companyId, OutboxId = outboxId },
            cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<long> CreateTargetAsync(CreateSyncOutboxTargetData data, CancellationToken cancellationToken = default)
    {
        const string sql = """
IF EXISTS (SELECT 1 FROM dbo.SyncOutboxTargets WHERE OutboxId = @OutboxId AND BranchCompanyId = @BranchCompanyId)
BEGIN
    SELECT Id FROM dbo.SyncOutboxTargets WHERE OutboxId = @OutboxId AND BranchCompanyId = @BranchCompanyId;
    RETURN;
END;

BEGIN TRY
    INSERT INTO dbo.SyncOutboxTargets
    (
        OutboxId,
        BranchCompanyId,
        Status,
        MaxAttempts
    )
    VALUES
    (
        @OutboxId,
        @BranchCompanyId,
        @Status,
        @MaxAttempts
    );

    SELECT CAST(SCOPE_IDENTITY() AS bigint);
END TRY
BEGIN CATCH
    IF ERROR_NUMBER() IN (2601, 2627)
    BEGIN
        SELECT Id FROM dbo.SyncOutboxTargets WHERE OutboxId = @OutboxId AND BranchCompanyId = @BranchCompanyId;
        RETURN;
    END;

    THROW;
END CATCH;
""";

        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<long>(new CommandDefinition(
            sql,
            new
            {
                data.OutboxId,
                data.BranchCompanyId,
                Status = SyncEventStatus.Pending.ToString(),
                data.MaxAttempts
            },
            cancellationToken: cancellationToken));
    }

    public async Task UpdateStatusAsync(long id, SyncEventStatus status, string? lastErrorMessage = null, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE dbo.SyncOutbox
SET Status = @Status,
    LastErrorMessage = @LastErrorMessage,
    ProcessedAt = CASE WHEN @Status IN (N'Applied', N'Ignored', N'DeadLetter') THEN SYSUTCDATETIME() ELSE ProcessedAt END,
    LockedBy = CASE WHEN @Status IN (N'Applied', N'Ignored', N'Error', N'Pending', N'DeadLetter') THEN NULL ELSE LockedBy END,
    LockedAt = CASE WHEN @Status IN (N'Applied', N'Ignored', N'Error', N'Pending', N'DeadLetter') THEN NULL ELSE LockedAt END,
    LockExpiresAt = CASE WHEN @Status IN (N'Applied', N'Ignored', N'Error', N'Pending', N'DeadLetter') THEN NULL ELSE LockExpiresAt END
WHERE Id = @Id;
""";

        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { Id = id, Status = status.ToString(), LastErrorMessage = lastErrorMessage },
            cancellationToken: cancellationToken));
    }

    public Task MarkAppliedAsync(long id, CancellationToken cancellationToken = default)
    {
        return UpdateStatusAsync(id, SyncEventStatus.Applied, cancellationToken: cancellationToken);
    }

    public Task MarkIgnoredAsync(long id, string? reason, CancellationToken cancellationToken = default)
    {
        return UpdateStatusAsync(id, SyncEventStatus.Ignored, reason, cancellationToken);
    }

    public async Task MarkErrorAsync(long id, string errorMessage, TimeSpan retryDelay, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE dbo.SyncOutbox
SET Status = CASE
        WHEN AttemptCount >= MaxAttempts THEN N'DeadLetter'
        ELSE N'Error'
    END,
    LastErrorMessage = @ErrorMessage,
    NextRetryAt = CASE
        WHEN AttemptCount < MaxAttempts THEN DATEADD(second, @RetryDelaySeconds, SYSUTCDATETIME())
        ELSE NULL
    END,
    ProcessedAt = CASE
        WHEN AttemptCount >= MaxAttempts THEN SYSUTCDATETIME()
        ELSE ProcessedAt
    END,
    LockedBy = NULL,
    LockedAt = NULL,
    LockExpiresAt = NULL
WHERE Id = @Id;
""";

        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new
            {
                Id = id,
                ErrorMessage = errorMessage,
                RetryDelaySeconds = NormalizeRetryDelaySeconds(retryDelay)
            },
            cancellationToken: cancellationToken));
    }

    public async Task MarkDeadLetterAsync(long id, string errorMessage, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE dbo.SyncOutbox
SET Status = N'DeadLetter',
    LastErrorMessage = @ErrorMessage,
    NextRetryAt = NULL,
    ProcessedAt = SYSUTCDATETIME(),
    LockedBy = NULL,
    LockedAt = NULL,
    LockExpiresAt = NULL
WHERE Id = @Id;
""";

        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { Id = id, ErrorMessage = errorMessage },
            cancellationToken: cancellationToken));
    }

    public async Task<bool> TryMarkTargetInProcessAsync(long targetId, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE dbo.SyncOutboxTargets
SET Status = N'InProcess',
    AttemptCount = AttemptCount + 1,
    NextRetryAt = NULL,
    LastErrorMessage = NULL,
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @TargetId
  AND Status IN (N'Pending', N'Error')
  AND AttemptCount < MaxAttempts
  AND (NextRetryAt IS NULL OR NextRetryAt <= SYSUTCDATETIME());
""";

        using var connection = connectionFactory.CreateConnection();
        var affectedRows = await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { TargetId = targetId },
            cancellationToken: cancellationToken));
        return affectedRows > 0;
    }

    public async Task MarkTargetAppliedAsync(long targetId, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE dbo.SyncOutboxTargets
SET Status = N'Applied',
    AppliedAt = SYSUTCDATETIME(),
    NextRetryAt = NULL,
    LastErrorMessage = NULL,
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @TargetId;
""";

        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, new { TargetId = targetId }, cancellationToken: cancellationToken));
    }

    public async Task MarkTargetIgnoredAsync(long targetId, string? reason, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE dbo.SyncOutboxTargets
SET Status = N'Ignored',
    AppliedAt = SYSUTCDATETIME(),
    NextRetryAt = NULL,
    LastErrorMessage = @Reason,
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @TargetId;
""";

        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { TargetId = targetId, Reason = reason },
            cancellationToken: cancellationToken));
    }

    public async Task MarkTargetErrorAsync(long targetId, string errorMessage, TimeSpan retryDelay, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE dbo.SyncOutboxTargets
SET Status = CASE
        WHEN AttemptCount >= MaxAttempts THEN N'DeadLetter'
        ELSE N'Error'
    END,
    NextRetryAt = CASE
        WHEN AttemptCount < MaxAttempts THEN DATEADD(second, @RetryDelaySeconds, SYSUTCDATETIME())
        ELSE NULL
    END,
    LastErrorMessage = @ErrorMessage,
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @TargetId;
""";

        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new
            {
                TargetId = targetId,
                ErrorMessage = errorMessage,
                RetryDelaySeconds = NormalizeRetryDelaySeconds(retryDelay)
            },
            cancellationToken: cancellationToken));
    }

    public async Task MarkTargetDeadLetterAsync(long targetId, string errorMessage, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE dbo.SyncOutboxTargets
SET Status = N'DeadLetter',
    NextRetryAt = NULL,
    LastErrorMessage = @ErrorMessage,
    UpdatedAt = SYSUTCDATETIME()
WHERE Id = @TargetId;
""";

        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { TargetId = targetId, ErrorMessage = errorMessage },
            cancellationToken: cancellationToken));
    }

    private static int NormalizeTake(int take) => Math.Clamp(take, 1, 500);

    private static async Task<SyncOutboxDetailDto?> GetActionRowAsync(
        System.Data.IDbConnection connection,
        System.Data.IDbTransaction transaction,
        int companyId,
        long id,
        CancellationToken cancellationToken)
    {
        const string sql = """
SELECT
    Id,
    EventId,
    CompanyId,
    EntityName,
    EntityGlobalId,
    EntityCode,
    Operation,
    PayloadJson,
    SourceSystem,
    SourceReference,
    Status,
    AttemptCount,
    MaxAttempts,
    NextRetryAt,
    LockedBy,
    LockedAt,
    LockExpiresAt,
    CreatedAt,
    ProcessedAt,
    LastErrorMessage
FROM dbo.SyncOutbox WITH (UPDLOCK, HOLDLOCK)
WHERE CompanyId = @CompanyId
  AND Id = @Id;
""";

        return await connection.QuerySingleOrDefaultAsync<SyncOutboxDetailDto>(new CommandDefinition(
            sql,
            new { CompanyId = companyId, Id = id },
            transaction,
            cancellationToken: cancellationToken));
    }

    private static async Task AddAuditAsync(
        System.Data.IDbConnection connection,
        System.Data.IDbTransaction transaction,
        SyncOutboxDetailDto current,
        SyncAuditAction action,
        SyncEventStatus newStatus,
        string message,
        string? createdBy,
        CancellationToken cancellationToken)
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
    NULL,
    @EventId,
    @EntityName,
    @EntityGlobalId,
    @Action,
    @PreviousStatus,
    @NewStatus,
    @Message,
    NULL,
    NULL,
    @CreatedBy
);
""";

        await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new
            {
                current.CompanyId,
                current.EventId,
                current.EntityName,
                current.EntityGlobalId,
                Action = action.ToString(),
                PreviousStatus = current.Status.ToString(),
                NewStatus = newStatus.ToString(),
                Message = message,
                CreatedBy = NormalizeCreatedBy(createdBy)
            },
            transaction,
            cancellationToken: cancellationToken));
    }

    private static SyncOutboxActionResultDto CreateActionResult(
        SyncOutboxDetailDto current,
        SyncEventStatus newStatus,
        int attemptCount,
        string message)
    {
        return new SyncOutboxActionResultDto(
            current.Id,
            current.EventId,
            current.CompanyId,
            current.EntityName,
            current.EntityGlobalId,
            current.Status,
            newStatus,
            attemptCount,
            current.MaxAttempts,
            current.LockExpiresAt,
            message);
    }

    private static string BuildMessage(string action, string? reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return action;
        }

        var message = $"{action} Motivo: {reason.Trim()}";
        return message.Length <= 500 ? message : message[..500];
    }

    private static string? NormalizeCreatedBy(string? createdBy)
    {
        if (string.IsNullOrWhiteSpace(createdBy))
        {
            return null;
        }

        createdBy = createdBy.Trim();
        return createdBy.Length <= 120 ? createdBy : createdBy[..120];
    }

    private static int NormalizePageSize(int pageSize) => Math.Clamp(pageSize, 1, 500);

    private static int NormalizeOffset(int page, int pageSize)
    {
        var normalizedPage = Math.Max(page, 1);
        return (normalizedPage - 1) * NormalizePageSize(pageSize);
    }

    private static int GetStatusCount(IEnumerable<SyncStatusCountDto> statusCounts, SyncEventStatus status)
    {
        return statusCounts.FirstOrDefault(item => item.Status == status)?.Count ?? 0;
    }

    private static string BuildOutboxWhere(int companyId, SyncOutboxQueryFilter filter, DynamicParameters parameters)
    {
        var where = new StringBuilder("WHERE outbox.CompanyId = @CompanyId");
        parameters.Add("CompanyId", companyId);

        if (filter.Status is not null)
        {
            where.AppendLine().Append("  AND outbox.Status = @Status");
            parameters.Add("Status", filter.Status.ToString());
        }

        if (!string.IsNullOrWhiteSpace(filter.EntityName))
        {
            where.AppendLine().Append("  AND outbox.EntityName = @EntityName");
            parameters.Add("EntityName", filter.EntityName.Trim());
        }

        if (filter.EntityGlobalId is not null)
        {
            where.AppendLine().Append("  AND outbox.EntityGlobalId = @EntityGlobalId");
            parameters.Add("EntityGlobalId", filter.EntityGlobalId);
        }

        if (filter.EventId is not null)
        {
            where.AppendLine().Append("  AND outbox.EventId = @EventId");
            parameters.Add("EventId", filter.EventId);
        }

        if (filter.BranchCompanyId is not null)
        {
            where.AppendLine().Append("""
  AND EXISTS
  (
      SELECT 1
      FROM dbo.SyncOutboxTargets AS target
      WHERE target.OutboxId = outbox.Id
        AND target.BranchCompanyId = @BranchCompanyId
  )
""");
            parameters.Add("BranchCompanyId", filter.BranchCompanyId);
        }

        if (filter.CreatedFrom is not null)
        {
            where.AppendLine().Append("  AND outbox.CreatedAt >= @CreatedFrom");
            parameters.Add("CreatedFrom", filter.CreatedFrom);
        }

        if (filter.CreatedTo is not null)
        {
            where.AppendLine().Append("  AND outbox.CreatedAt <= @CreatedTo");
            parameters.Add("CreatedTo", filter.CreatedTo);
        }

        if (filter.HasErrors == true)
        {
            where.AppendLine().Append("  AND (outbox.LastErrorMessage IS NOT NULL OR outbox.Status IN (N'Error', N'DeadLetter'))");
        }
        else if (filter.HasErrors == false)
        {
            where.AppendLine().Append("  AND outbox.LastErrorMessage IS NULL AND outbox.Status NOT IN (N'Error', N'DeadLetter')");
        }

        if (filter.DeadLetterOnly == true)
        {
            where.AppendLine().Append("  AND outbox.Status = N'DeadLetter'");
        }

        return where.ToString();
    }

    private static string NormalizeLockedBy(string lockedBy)
    {
        return string.IsNullOrWhiteSpace(lockedBy)
            ? Environment.MachineName
            : lockedBy.Trim()[..Math.Min(lockedBy.Trim().Length, 120)];
    }

    private static int NormalizeLockMinutes(TimeSpan lockDuration)
    {
        return Math.Clamp(Convert.ToInt32(Math.Ceiling(lockDuration.TotalMinutes)), 1, 240);
    }

    private static int NormalizeRetryDelaySeconds(TimeSpan retryDelay)
    {
        return Math.Clamp(Convert.ToInt32(Math.Ceiling(retryDelay.TotalSeconds)), 1, 86400);
    }
}
