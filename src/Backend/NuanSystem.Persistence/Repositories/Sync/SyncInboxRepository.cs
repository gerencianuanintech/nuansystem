using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class SyncInboxRepository(ITenantConnectionFactory connectionFactory) : ISyncInboxRepository
{
    public async Task<long> RegisterAsync(CreateSyncInboxEventData data, CancellationToken cancellationToken = default)
    {
        const string sql = """
IF EXISTS (SELECT 1 FROM dbo.SyncInbox WHERE EventId = @EventId)
BEGIN
    SELECT Id FROM dbo.SyncInbox WHERE EventId = @EventId;
    RETURN;
END;

BEGIN TRY
    INSERT INTO dbo.SyncInbox
    (
        EventId,
        SourceCompanyId,
        EntityName,
        EntityGlobalId,
        Operation,
        PayloadJson,
        Status
    )
    VALUES
    (
        @EventId,
        @SourceCompanyId,
        @EntityName,
        @EntityGlobalId,
        @Operation,
        @PayloadJson,
        @Status
    );

    SELECT CAST(SCOPE_IDENTITY() AS bigint);
END TRY
BEGIN CATCH
    IF ERROR_NUMBER() IN (2601, 2627)
    BEGIN
        SELECT Id FROM dbo.SyncInbox WHERE EventId = @EventId;
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
                data.SourceCompanyId,
                data.EntityName,
                data.EntityGlobalId,
                Operation = data.Operation.ToString(),
                data.PayloadJson,
                Status = SyncEventStatus.Pending.ToString()
            },
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<SyncInboxDto>> GetPendingAsync(int take, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT TOP (@Take)
    Id,
    EventId,
    SourceCompanyId,
    EntityName,
    EntityGlobalId,
    Operation,
    PayloadJson,
    Status,
    AttemptCount,
    MaxAttempts,
    NextRetryAt,
    ReceivedAt,
    AppliedAt,
    ErrorMessage,
    LastErrorMessage
FROM dbo.SyncInbox
WHERE Status = N'Pending'
  AND AttemptCount < MaxAttempts
  AND (NextRetryAt IS NULL OR NextRetryAt <= SYSUTCDATETIME())
ORDER BY ReceivedAt, Id;
""";

        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SyncInboxDto>(new CommandDefinition(
            sql,
            new { Take = Math.Clamp(take, 1, 500) },
            cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<SyncInboxDto?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT
    Id,
    EventId,
    SourceCompanyId,
    EntityName,
    EntityGlobalId,
    Operation,
    PayloadJson,
    Status,
    AttemptCount,
    MaxAttempts,
    NextRetryAt,
    ReceivedAt,
    AppliedAt,
    ErrorMessage,
    LastErrorMessage
FROM dbo.SyncInbox
WHERE EventId = @EventId;
""";

        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SyncInboxDto>(new CommandDefinition(
            sql,
            new { EventId = eventId },
            cancellationToken: cancellationToken));
    }

    public async Task MarkDeadLetterAsync(long id, string errorMessage, CancellationToken cancellationToken = default)
    {
        const string sql = """
UPDATE dbo.SyncInbox
SET Status = N'DeadLetter',
    ErrorMessage = @ErrorMessage,
    LastErrorMessage = @ErrorMessage,
    NextRetryAt = NULL,
    AppliedAt = SYSUTCDATETIME()
WHERE Id = @Id;
""";

        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { Id = id, ErrorMessage = errorMessage },
            cancellationToken: cancellationToken));
    }
}
