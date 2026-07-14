using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Sync.Dtos;

public sealed record RetrySyncOutboxRequest(string? Reason = null);

public sealed record RetryDeadLetterSyncOutboxRequest(string Reason, bool ResetAttemptCount = true);

public sealed record ReleaseExpiredLockRequest(string? Reason = null);

public sealed record SyncOutboxActionResultDto(
    long Id,
    Guid EventId,
    int CompanyId,
    string EntityName,
    Guid EntityGlobalId,
    SyncEventStatus PreviousStatus,
    SyncEventStatus NewStatus,
    int AttemptCount,
    int MaxAttempts,
    DateTime? PreviousLockExpiresAt,
    string Message);
