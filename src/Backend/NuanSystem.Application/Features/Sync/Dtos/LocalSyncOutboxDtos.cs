using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Sync.Dtos;

public sealed record CreateLocalSyncOutboxData(
    Guid EventId,
    int CompanyId,
    string EntityName,
    Guid EntityGlobalId,
    string? EntityCode,
    SyncOperation Operation,
    string PayloadJson,
    int MaxAttempts = 3);

public sealed record LocalSyncOutboxDto(
    long Id,
    Guid EventId,
    int CompanyId,
    string EntityName,
    Guid EntityGlobalId,
    string? EntityCode,
    SyncOperation Operation,
    string PayloadJson,
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

public sealed record LocalSyncOutboxCompanyDto(int CompanyId, string CompanyCode);

public sealed record SyncOutboxPromotionData(
    LocalSyncOutboxDto Event,
    IReadOnlyCollection<SyncRoutingTargetDto> Targets,
    IReadOnlyCollection<SyncDistributionDecisionDto> Decisions,
    string WorkerInstance);

public enum SyncOutboxPromotionStatus
{
    Created = 1,
    Existing = 2,
    Conflict = 3
}

public sealed record SyncOutboxPromotionResult(
    SyncOutboxPromotionStatus Status,
    long? OutboxId,
    string Reason);
