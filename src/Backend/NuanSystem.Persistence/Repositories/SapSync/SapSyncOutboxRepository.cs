using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Persistence.Repositories.SapSync;

public sealed class SapSyncOutboxRepository(ITenantConnectionFactory connectionFactory) : ISapSyncOutboxRepository
{
    public Task<IReadOnlyCollection<SapSyncOutboxItemDto>> ClaimPendingAsync(int companyId, int batchSize, string workerInstance, TimeSpan lockTimeout, CancellationToken cancellationToken = default)
        => ClaimByStatusAsync(companyId, "Pending", batchSize, workerInstance, lockTimeout, cancellationToken);

    public Task<IReadOnlyCollection<SapSyncOutboxItemDto>> ClaimRetryScheduledAsync(int companyId, int batchSize, string workerInstance, TimeSpan lockTimeout, CancellationToken cancellationToken = default)
        => ClaimByStatusAsync(companyId, "RetryScheduled", batchSize, workerInstance, lockTimeout, cancellationToken);

    public Task MarkProcessingAsync(long id, string workerInstance, string correlationId, CancellationToken cancellationToken = default)
        => ExecuteAsync("UPDATE dbo.SapSyncOutbox SET Status=N'Processing', WorkerInstance=@WorkerInstance, CorrelationId=@CorrelationId, LockedAt=SYSUTCDATETIME(), UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id;", new { Id = id, WorkerInstance = workerInstance, CorrelationId = correlationId }, cancellationToken);

    public Task MarkSucceededAsync(long id, int? sapDocEntry, int? sapDocNum, string? responseJson, CancellationToken cancellationToken = default)
        => ExecuteAsync("UPDATE dbo.SapSyncOutbox SET Status=N'Synced', SapDocEntry=@SapDocEntry, SapDocNum=@SapDocNum, ResponseJson=@ResponseJson, LockedAt=NULL, ExpiresAt=NULL, UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id;", new { Id = id, SapDocEntry = sapDocEntry, SapDocNum = sapDocNum, ResponseJson = responseJson }, cancellationToken);

    public Task MarkFailedAsync(long id, string? errorCode, string? errorMessage, DateTime? nextAttemptAtUtc, CancellationToken cancellationToken = default)
        => ExecuteAsync("UPDATE dbo.SapSyncOutbox SET Status=CASE WHEN @NextAttemptAtUtc IS NULL THEN N'Failed' ELSE N'RetryScheduled' END, ErrorCode=@ErrorCode, ErrorMessage=@ErrorMessage, NextAttemptAtUtc=@NextAttemptAtUtc, LockedAt=NULL, ExpiresAt=NULL, UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id;", new { Id = id, ErrorCode = errorCode, ErrorMessage = errorMessage, NextAttemptAtUtc = nextAttemptAtUtc }, cancellationToken);

    public Task MarkDeadLetterAsync(long id, string? errorCode, string? errorMessage, CancellationToken cancellationToken = default)
        => ExecuteAsync("UPDATE dbo.SapSyncOutbox SET Status=N'DeadLetter', ErrorCode=@ErrorCode, ErrorMessage=@ErrorMessage, LockedAt=NULL, ExpiresAt=NULL, UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id;", new { Id = id, ErrorCode = errorCode, ErrorMessage = errorMessage }, cancellationToken);

    public Task ReleaseExpiredLocksAsync(int companyId, DateTime olderThanUtc, CancellationToken cancellationToken = default)
        => ExecuteAsync("UPDATE dbo.SapSyncOutbox SET Status=CASE WHEN NextAttemptAtUtc IS NULL THEN N'Pending' ELSE N'RetryScheduled' END, WorkerInstance=NULL, LockedAt=NULL, ExpiresAt=NULL, UpdatedAt=SYSUTCDATETIME() WHERE CompanyId=@CompanyId AND Status=N'Processing' AND ExpiresAt <= @OlderThanUtc;", new { CompanyId = companyId, OlderThanUtc = olderThanUtc }, cancellationToken);

    private async Task<IReadOnlyCollection<SapSyncOutboxItemDto>> ClaimByStatusAsync(int companyId, string status, int batchSize, string workerInstance, TimeSpan lockTimeout, CancellationToken cancellationToken)
    {
        const string sql = """
;WITH Pending AS
(
    SELECT TOP (@BatchSize) *
    FROM dbo.SapSyncOutbox WITH (ROWLOCK, READPAST, UPDLOCK)
    WHERE CompanyId=@CompanyId AND Status=@Status AND (NextAttemptAtUtc IS NULL OR NextAttemptAtUtc <= SYSUTCDATETIME()) AND (LockedAt IS NULL OR ExpiresAt <= SYSUTCDATETIME())
    ORDER BY CreatedAt, Id
)
UPDATE Pending
SET Status=N'Processing', WorkerInstance=@WorkerInstance, CorrelationId=CONVERT(nvarchar(80), NEWID()), LockedAt=SYSUTCDATETIME(), ExpiresAt=DATEADD(second, @LockSeconds, SYSUTCDATETIME()), AttemptCount=AttemptCount+1, UpdatedAt=SYSUTCDATETIME()
OUTPUT INSERTED.Id, INSERTED.CompanyId, INSERTED.EntityCode, INSERTED.OperationCode, INSERTED.LocalEntityId, INSERTED.PayloadJson, INSERTED.Status, INSERTED.AttemptCount, INSERTED.NextAttemptAtUtc, INSERTED.WorkerInstance, INSERTED.CorrelationId, INSERTED.CreatedAt, INSERTED.LockedAt, INSERTED.ExpiresAt;
""";
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SapSyncOutboxItemDto>(new CommandDefinition(sql, new { CompanyId = companyId, Status = status, BatchSize = batchSize, WorkerInstance = workerInstance, LockSeconds = Convert.ToInt32(lockTimeout.TotalSeconds) }, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    private async Task ExecuteAsync(string sql, object parameters, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
    }
}
