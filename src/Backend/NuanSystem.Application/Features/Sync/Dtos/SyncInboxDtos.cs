using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Sync.Dtos;

public sealed record CreateSyncInboxEventData(
    Guid EventId,
    int SourceCompanyId,
    string EntityName,
    Guid EntityGlobalId,
    SyncOperation Operation,
    string PayloadJson);

public sealed record SyncInboxDto(
    long Id,
    Guid EventId,
    int SourceCompanyId,
    string EntityName,
    Guid EntityGlobalId,
    SyncOperation Operation,
    string PayloadJson,
    SyncEventStatus Status,
    int AttemptCount,
    int MaxAttempts,
    DateTime? NextRetryAt,
    DateTime ReceivedAt,
    DateTime? AppliedAt,
    string? ErrorMessage,
    string? LastErrorMessage);
