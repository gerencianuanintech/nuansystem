using NuanSystem.Shared.Sync;

namespace NuanSystem.WinForms.Services.Sync.Models;

public sealed record SyncDashboard(
    int TotalPending,
    int TotalInProcess,
    int TotalApplied,
    int TotalErrors,
    int TotalDeadLetter,
    int TotalIgnored,
    IReadOnlyCollection<SyncOutboxListItem> LatestErrors,
    IReadOnlyCollection<SyncOutboxListItem> LatestEvents,
    IReadOnlyCollection<SyncEntityStatusCount> ByEntity,
    IReadOnlyCollection<SyncBranchStatusCount> ByBranch);

public sealed record SyncSummary(
    int TotalPending,
    int TotalInProcess,
    int TotalApplied,
    int TotalErrors,
    int TotalDeadLetter,
    int TotalIgnored,
    IReadOnlyCollection<SyncStatusCount> StatusCounts,
    IReadOnlyCollection<SyncEntityStatusCount> EntityStatusCounts,
    IReadOnlyCollection<SyncBranchStatusCount> BranchStatusCounts);

public sealed record SyncOutboxListItem(
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
    string? LastErrorMessage)
{
    public string? LastErrorSummary => string.IsNullOrWhiteSpace(LastErrorMessage)
        ? null
        : LastErrorMessage.Length <= 160 ? LastErrorMessage : LastErrorMessage[..160] + "...";
}

public sealed record SyncOutboxDetail(
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

public sealed record SyncOutboxTarget(
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
    DateTime? UpdatedAt)
{
    public string BranchDisplay => BranchCompanyId.ToString();

    public string? LastErrorSummary => string.IsNullOrWhiteSpace(LastErrorMessage)
        ? null
        : LastErrorMessage.Length <= 160 ? LastErrorMessage : LastErrorMessage[..160] + "...";
}

public sealed record SyncAuditItem(
    long Id,
    int CompanyId,
    int? BranchCompanyId,
    Guid? EventId,
    string EntityName,
    Guid? EntityGlobalId,
    SyncAuditAction Action,
    SyncEventStatus? PreviousStatus,
    SyncEventStatus? NewStatus,
    string? Message,
    string? ErrorCode,
    string? ErrorDetail,
    DateTime CreatedAt,
    string? CreatedBy)
{
    public string? ErrorDetailSummary => string.IsNullOrWhiteSpace(ErrorDetail)
        ? null
        : ErrorDetail.Length <= 180 ? ErrorDetail : ErrorDetail[..180] + "...";
}

public sealed record SyncStatusCount(SyncEventStatus Status, int Count);

public sealed record SyncEntityStatusCount(string EntityName, SyncEventStatus Status, int Count);

public sealed record SyncBranchStatusCount(int BranchCompanyId, SyncEventStatus Status, int Count)
{
    public string BranchDisplay => BranchCompanyId.ToString();
}

public sealed record RetrySyncOutboxRequest(string? Reason = null);

public sealed record RetryDeadLetterRequest(string Reason, bool ResetAttemptCount = true);

public sealed record ReleaseExpiredLockRequest(string? Reason = null);

public sealed record SyncManualActionResult(
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

public sealed class SyncOutboxFilter
{
    public SyncEventStatus? Status { get; set; }
    public string? EntityName { get; set; }
    public Guid? EntityGlobalId { get; set; }
    public Guid? EventId { get; set; }
    public int? BranchCompanyId { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public bool? HasErrors { get; set; }
    public bool? DeadLetterOnly { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 100;
}

public sealed class SyncAuditFilter
{
    public SyncEventStatus? Status { get; set; }
    public string? EntityName { get; set; }
    public Guid? EntityGlobalId { get; set; }
    public Guid? EventId { get; set; }
    public int? BranchCompanyId { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public bool? HasErrors { get; set; }
    public bool? DeadLetterOnly { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 100;
}
public sealed record RetrySyncOutboxBatchRequest(IReadOnlyCollection<long> Ids,string Reason,bool ResetDeadLetterAttempts=true);
public sealed record RetrySyncOutboxBatchItem(long Id,string Status,string Message);
public sealed record RetrySyncOutboxBatchResult(int Requested,int Retried,int Skipped,IReadOnlyCollection<RetrySyncOutboxBatchItem> Items);
