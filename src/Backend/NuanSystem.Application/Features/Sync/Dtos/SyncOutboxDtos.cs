using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Sync.Dtos;

public sealed record CreateSyncOutboxEventData(
    Guid EventId,
    int CompanyId,
    string EntityName,
    Guid EntityGlobalId,
    string? EntityCode,
    SyncOperation Operation,
    string PayloadJson,
    string? SourceSystem,
    string? SourceReference,
    int MaxAttempts = 3);

public sealed record SyncOutboxDto(
    long Id,
    Guid EventId,
    int CompanyId,
    string EntityName,
    Guid EntityGlobalId,
    string? EntityCode,
    SyncOperation Operation,
    string PayloadJson,
    string? SourceSystem,
    string? SourceReference,
    SyncEventStatus Status,
    int AttemptCount,
    int MaxAttempts,
    DateTime? NextRetryAt,
    string? LockedBy,
    DateTime? LockedAt,
    DateTime? LockExpiresAt,
    DateTime CreatedAt,
    DateTime? ProcessedAt,
    string? LastErrorMessage);

public sealed record CreateSyncOutboxTargetData(
    long OutboxId,
    int BranchCompanyId,
    int MaxAttempts = 3);

public sealed record SyncOutboxTargetDto(
    long Id,
    long OutboxId,
    int BranchCompanyId,
    SyncEventStatus Status,
    int AttemptCount,
    int MaxAttempts,
    DateTime? NextRetryAt,
    DateTime? AppliedAt,
    string? LastErrorMessage,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
