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
    int MaxAttempts = 3,
    int? TargetCompanyId = null,
    Guid? CausationEventId = null);

public sealed class LocalSyncOutboxDto
{
    public LocalSyncOutboxDto()
    {
    }

    public LocalSyncOutboxDto(
        long id,
        Guid eventId,
        int companyId,
        string entityName,
        Guid entityGlobalId,
        string? entityCode,
        SyncOperation operation,
        string payloadJson,
        SyncEventStatus status,
        int attemptCount,
        int maxAttempts,
        DateTime? nextRetryAt,
        string? lockedBy,
        DateTime? lockedAt,
        DateTime? lockExpiresAt,
        DateTime createdAt,
        DateTime? processedAt,
        string? lastErrorMessage,
        int? targetCompanyId = null,
        Guid? causationEventId = null)
    {
        Id = id;
        EventId = eventId;
        CompanyId = companyId;
        EntityName = entityName;
        EntityGlobalId = entityGlobalId;
        EntityCode = entityCode;
        Operation = operation;
        PayloadJson = payloadJson;
        Status = status;
        AttemptCount = attemptCount;
        MaxAttempts = maxAttempts;
        NextRetryAt = nextRetryAt;
        LockedBy = lockedBy;
        LockedAt = lockedAt;
        LockExpiresAt = lockExpiresAt;
        CreatedAt = createdAt;
        ProcessedAt = processedAt;
        LastErrorMessage = lastErrorMessage;
        TargetCompanyId = targetCompanyId;
        CausationEventId = causationEventId;
    }

    public long Id { get; set; }
    public Guid EventId { get; set; }
    public int CompanyId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public Guid EntityGlobalId { get; set; }
    public string? EntityCode { get; set; }
    public SyncOperation Operation { get; set; }
    public string PayloadJson { get; set; } = string.Empty;
    public SyncEventStatus Status { get; set; }
    public int AttemptCount { get; set; }
    public int MaxAttempts { get; set; }
    public DateTime? NextRetryAt { get; set; }
    public string? LockedBy { get; set; }
    public DateTime? LockedAt { get; set; }
    public DateTime? LockExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? LastErrorMessage { get; set; }
    public int? TargetCompanyId { get; set; }
    public Guid? CausationEventId { get; set; }
}

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
    Conflict = 3,
    Deferred = 4
}

public sealed record SyncOutboxPromotionResult(
    SyncOutboxPromotionStatus Status,
    long? OutboxId,
    string Reason);
