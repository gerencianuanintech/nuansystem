using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Persistence.Repositories.SapSync;

public sealed class SapSyncInboxRepository(ITenantConnectionFactory connectionFactory) : ISapSyncInboxRepository
{
    public async Task<long> UpsertSupplierAsync(int companyId, string sapCardCode, string payloadJson, SapSyncStatus status, string workerInstance, string correlationId, CancellationToken cancellationToken = default)
    {
        const string sql = """
MERGE dbo.SapSyncInbox AS target
USING (SELECT @CompanyId AS CompanyId, @EntityCode AS EntityCode, @SapEntityId AS SapEntityId) AS source
ON target.CompanyId = source.CompanyId AND target.EntityCode = source.EntityCode AND target.SapEntityId = source.SapEntityId
WHEN MATCHED THEN
    UPDATE SET PayloadJson = @PayloadJson, Status = @Status, WorkerInstance = @WorkerInstance, CorrelationId = @CorrelationId, LockedAt = SYSUTCDATETIME(), UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT (CompanyId, EntityCode, SapEntityId, PayloadJson, Status, WorkerInstance, CorrelationId, LockedAt)
    VALUES (@CompanyId, @EntityCode, @SapEntityId, @PayloadJson, @Status, @WorkerInstance, @CorrelationId, SYSUTCDATETIME())
OUTPUT INSERTED.Id;
""";
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<long>(new CommandDefinition(sql, new { CompanyId = companyId, EntityCode = SapSyncEntityCode.Suppliers, SapEntityId = sapCardCode, PayloadJson = payloadJson, Status = status.ToString(), WorkerInstance = workerInstance, CorrelationId = correlationId }, cancellationToken: cancellationToken));
    }

    public Task<IReadOnlyCollection<SapSyncInboxItemDto>> ClaimPendingAsync(int companyId, string entityCode, int batchSize, string workerInstance, TimeSpan lockTimeout, CancellationToken cancellationToken = default)
        => ClaimByStatusAsync(companyId, entityCode, "Pending", batchSize, workerInstance, lockTimeout, cancellationToken);

    public Task<IReadOnlyCollection<SapSyncInboxItemDto>> ClaimRetryScheduledAsync(int companyId, string entityCode, int batchSize, string workerInstance, TimeSpan lockTimeout, CancellationToken cancellationToken = default)
        => ClaimByStatusAsync(companyId, entityCode, "RetryScheduled", batchSize, workerInstance, lockTimeout, cancellationToken);

    public Task MarkProcessingAsync(long id, string workerInstance, string correlationId, CancellationToken cancellationToken = default)
        => ExecuteAsync("UPDATE dbo.SapSyncInbox SET Status=N'Processing', WorkerInstance=@WorkerInstance, CorrelationId=@CorrelationId, LockedAt=SYSUTCDATETIME(), UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id;", new { Id = id, WorkerInstance = workerInstance, CorrelationId = correlationId }, cancellationToken);

    public Task MarkImportedAsync(long id, string? localEntityId, CancellationToken cancellationToken = default)
        => ExecuteAsync("UPDATE dbo.SapSyncInbox SET Status=N'Synced', LocalEntityId=@LocalEntityId, LockedAt=NULL, ExpiresAt=NULL, UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id;", new { Id = id, LocalEntityId = localEntityId }, cancellationToken);

    public Task MarkConflictAsync(long id, string? message, CancellationToken cancellationToken = default)
        => ExecuteAsync("UPDATE dbo.SapSyncInbox SET Status=N'Failed', ErrorCode=N'CONFLICT', ErrorMessage=@Message, LockedAt=NULL, ExpiresAt=NULL, UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id;", new { Id = id, Message = message }, cancellationToken);

    public Task MarkFailedAsync(long id, string? errorCode, string? errorMessage, DateTime? nextAttemptAtUtc, CancellationToken cancellationToken = default)
        => ExecuteAsync("UPDATE dbo.SapSyncInbox SET Status=CASE WHEN @NextAttemptAtUtc IS NULL THEN N'Failed' ELSE N'RetryScheduled' END, ErrorCode=@ErrorCode, ErrorMessage=@ErrorMessage, NextAttemptAtUtc=@NextAttemptAtUtc, LockedAt=NULL, ExpiresAt=NULL, UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id;", new { Id = id, ErrorCode = errorCode, ErrorMessage = errorMessage, NextAttemptAtUtc = nextAttemptAtUtc }, cancellationToken);

    public Task MarkDeadLetterAsync(long id, string? errorCode, string? errorMessage, CancellationToken cancellationToken = default)
        => ExecuteAsync("UPDATE dbo.SapSyncInbox SET Status=N'DeadLetter', ErrorCode=@ErrorCode, ErrorMessage=@ErrorMessage, LockedAt=NULL, ExpiresAt=NULL, UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id;", new { Id = id, ErrorCode = errorCode, ErrorMessage = errorMessage }, cancellationToken);

    public Task ReleaseExpiredLocksAsync(int companyId, DateTime olderThanUtc, CancellationToken cancellationToken = default)
        => ExecuteAsync("UPDATE dbo.SapSyncInbox SET Status=CASE WHEN NextAttemptAtUtc IS NULL THEN N'Pending' ELSE N'RetryScheduled' END, WorkerInstance=NULL, LockedAt=NULL, ExpiresAt=NULL, UpdatedAt=SYSUTCDATETIME() WHERE CompanyId=@CompanyId AND Status=N'Processing' AND ExpiresAt <= @OlderThanUtc;", new { CompanyId = companyId, OlderThanUtc = olderThanUtc }, cancellationToken);

    private async Task<IReadOnlyCollection<SapSyncInboxItemDto>> ClaimByStatusAsync(int companyId, string entityCode, string status, int batchSize, string workerInstance, TimeSpan lockTimeout, CancellationToken cancellationToken)
    {
        const string sql = """
;WITH Pending AS
(
    SELECT TOP (@BatchSize) *
    FROM dbo.SapSyncInbox WITH (ROWLOCK, READPAST, UPDLOCK)
    WHERE CompanyId=@CompanyId AND EntityCode=@EntityCode AND Status=@Status AND (NextAttemptAtUtc IS NULL OR NextAttemptAtUtc <= SYSUTCDATETIME()) AND (LockedAt IS NULL OR ExpiresAt <= SYSUTCDATETIME())
    ORDER BY CreatedAt, Id
)
UPDATE Pending
SET Status=N'Processing', WorkerInstance=@WorkerInstance, CorrelationId=CONVERT(nvarchar(80), NEWID()), LockedAt=SYSUTCDATETIME(), ExpiresAt=DATEADD(second, @LockSeconds, SYSUTCDATETIME()), AttemptCount=AttemptCount+1, UpdatedAt=SYSUTCDATETIME()
OUTPUT INSERTED.Id, INSERTED.CompanyId, INSERTED.EntityCode, INSERTED.SapEntityId, INSERTED.PayloadJson, INSERTED.Status, INSERTED.AttemptCount, INSERTED.NextAttemptAtUtc, INSERTED.WorkerInstance, INSERTED.CorrelationId, INSERTED.CreatedAt, INSERTED.LockedAt, INSERTED.ExpiresAt;
""";
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SapSyncInboxItemDto>(new CommandDefinition(sql, new { CompanyId = companyId, EntityCode = entityCode, Status = status, BatchSize = batchSize, WorkerInstance = workerInstance, LockSeconds = Convert.ToInt32(lockTimeout.TotalSeconds) }, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    private async Task ExecuteAsync(string sql, object parameters, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
    }
}
