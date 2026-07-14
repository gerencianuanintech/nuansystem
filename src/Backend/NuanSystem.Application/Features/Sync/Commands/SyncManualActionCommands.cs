using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Features.Sync.Commands;

public sealed record RetrySyncOutboxCommand(
    long Id,
    string? Reason,
    string? AuditUserName) : ICommand<SyncOutboxActionResultDto>;

public sealed record RetryDeadLetterSyncOutboxCommand(
    long Id,
    string Reason,
    bool ResetAttemptCount,
    string? AuditUserName) : ICommand<SyncOutboxActionResultDto>;

public sealed record ReleaseExpiredSyncLockCommand(
    long Id,
    string? Reason,
    string? AuditUserName) : ICommand<SyncOutboxActionResultDto>;
