using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Sync.Dtos;

public sealed record SyncDashboardDto(
    int TotalPending,
    int TotalInProcess,
    int TotalApplied,
    int TotalErrors,
    int TotalDeadLetter,
    int TotalIgnored,
    IReadOnlyCollection<SyncOutboxListItemDto> LatestErrors,
    IReadOnlyCollection<SyncOutboxListItemDto> LatestEvents,
    IReadOnlyCollection<SyncEntityStatusCountDto> ByEntity,
    IReadOnlyCollection<SyncBranchStatusCountDto> ByBranch);

public sealed record SyncSummaryDto(
    int TotalPending,
    int TotalInProcess,
    int TotalApplied,
    int TotalErrors,
    int TotalDeadLetter,
    int TotalIgnored,
    IReadOnlyCollection<SyncStatusCountDto> StatusCounts,
    IReadOnlyCollection<SyncEntityStatusCountDto> EntityStatusCounts,
    IReadOnlyCollection<SyncBranchStatusCountDto> BranchStatusCounts);

public sealed record SyncOutboxListItemDto(
    long Id,
    Guid EventId,
    int CompanyId,
    string EntityName,
    Guid EntityGlobalId,
    string? EntityCode,
    SyncOperation Operation,
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

public sealed record SyncOutboxDetailDto(
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

public sealed record SyncStatusCountDto(
    SyncEventStatus Status,
    int Count);

public sealed record SyncEntityStatusCountDto(
    string EntityName,
    SyncEventStatus Status,
    int Count);

public sealed record SyncBranchStatusCountDto(
    int BranchCompanyId,
    SyncEventStatus Status,
    int Count);

public sealed record SyncOutboxQueryFilter(
    SyncEventStatus? Status = null,
    string? EntityName = null,
    Guid? EntityGlobalId = null,
    Guid? EventId = null,
    int? BranchCompanyId = null,
    DateTime? CreatedFrom = null,
    DateTime? CreatedTo = null,
    bool? HasErrors = null,
    bool? DeadLetterOnly = null,
    int Page = 1,
    int PageSize = 100);

public sealed record SyncAuditQueryFilter(
    SyncEventStatus? Status = null,
    string? EntityName = null,
    Guid? EntityGlobalId = null,
    Guid? EventId = null,
    int? BranchCompanyId = null,
    DateTime? CreatedFrom = null,
    DateTime? CreatedTo = null,
    bool? HasErrors = null,
    bool? DeadLetterOnly = null,
    int Page = 1,
    int PageSize = 100);
