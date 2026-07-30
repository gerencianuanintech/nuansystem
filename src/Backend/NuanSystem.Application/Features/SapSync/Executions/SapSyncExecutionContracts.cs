namespace NuanSystem.Application.Features.SapSync.Executions;

public static class SapSyncTriggerTypes
{
    public const string Manual = "Manual";
    public const string Scheduled = "Scheduled";
    public const string Retry = "Retry";
}

public static class SapSyncExecutionStatuses
{
    public const string Pending = "Pending";
    public const string Running = "Running";
    public const string Cancelling = "Cancelling";
    public const string Cancelled = "Cancelled";
    public const string RetryScheduled = "RetryScheduled";
    public const string SkippedConcurrent = "SkippedConcurrent";
    public const string Completed = "Completed";
    public const string CompletedWithWarnings = "CompletedWithWarnings";
    public const string CompletedWithErrors = "CompletedWithErrors";
    public const string Failed = "Failed";
}

public static class SapSyncExecutionDetailStatuses
{
    public const string Pending = "Pending";
    public const string Processing = "Processing";
    public const string Created = "Created";
    public const string Updated = "Updated";
    public const string Unchanged = "Unchanged";
    public const string ApprovalRequired = "ApprovalRequired";
    public const string Conflict = "Conflict";
    public const string Skipped = "Skipped";
    public const string RetryScheduled = "RetryScheduled";
    public const string Failed = "Failed";
    public const string DeadLetter = "DeadLetter";
}

public static class SapSyncExecutionDetailActions
{
    public const string Create = "Create";
    public const string Update = "Update";
    public const string NoChange = "NoChange";
    public const string Approval = "Approval";
    public const string Conflict = "Conflict";
    public const string Skip = "Skip";
}

public static class SapSyncSafeErrorClasses
{
    public const string Transient = "Transient";
    public const string Terminal = "Terminal";
    public const string Conflict = "Conflict";
    public const string Approval = "Approval";
}

public static class SapSyncApprovedSnapshotTypes
{
    public const string SupplierV1 = "SupplierV1";
    public const string ItemV1 = "ItemV1";
    public const string PaymentTermV1 = "PaymentTermV1";
    public const string WarehouseV1 = "WarehouseV1";
}

public sealed record SapSyncExecutionFilter(
    long? SapSyncProfileId,
    string? EntityCode,
    string? Direction,
    string? Status,
    string? TriggerType,
    DateTime? DateFromUtc,
    DateTime? DateToUtc,
    int PageNumber = 1,
    int PageSize = 50);

public sealed record SapSyncExecutionListItemDto(
    long Id,
    Guid ExecutionUid,
    Guid RunGroupId,
    Guid CorrelationId,
    long? SapSyncProfileId,
    string ProfileCode,
    string ProfileName,
    string EntityCode,
    string Direction,
    string TriggerType,
    string Status,
    DateTime RequestedAtUtc,
    DateTime? StartedAtUtc,
    DateTime? FinishedAtUtc,
    int TotalRecords,
    int SucceededRecords,
    int WarningRecords,
    int FailedRecords);

public sealed record SapSyncExecutionCreateData(
    Guid ExecutionUid,
    Guid RunGroupId,
    Guid CorrelationId,
    long? SapSyncProfileId,
    long? SapSyncProfileEntityId,
    string ProfileCode,
    string ProfileName,
    int CompanyId,
    string CompanyCode,
    string EntityCode,
    string Direction,
    string TriggerType,
    long? ParentExecutionId,
    int BatchSize,
    int MaxAttempts,
    int ExecutionOrder,
    int TimeoutMinutes,
    string? ScheduleType,
    string? TimeZoneId,
    string ProfileSnapshotJson,
    string EffectiveParametersJson,
    int? RequestedByUserId,
    string? RequestedByUserName,
    string? WorkerInstance);

public sealed record SapSyncExecutionDto(
    long Id,
    Guid ExecutionUid,
    Guid RunGroupId,
    Guid CorrelationId,
    long? SapSyncProfileId,
    long? SapSyncProfileEntityId,
    string ProfileCode,
    string ProfileName,
    int CompanyId,
    string CompanyCode,
    string EntityCode,
    string Direction,
    string TriggerType,
    long? ParentExecutionId,
    string Status,
    int BatchSize,
    int MaxAttempts,
    int ExecutionOrder,
    int TimeoutMinutes,
    string? ScheduleType,
    string? TimeZoneId,
    string ProfileSnapshotJson,
    string EffectiveParametersJson,
    int? RequestedByUserId,
    string? RequestedByUserName,
    DateTime RequestedAtUtc,
    string? WorkerInstance,
    DateTime? StartedAtUtc,
    DateTime? LastProgressAtUtc,
    DateTime? FinishedAtUtc,
    DateTime? CancellationRequestedAtUtc,
    int TotalRecords,
    int CreatedRecords,
    int UpdatedRecords,
    int UnchangedRecords,
    int ApprovalRequiredRecords,
    int ConflictRecords,
    int SkippedRecords,
    int RetryScheduledRecords,
    int FailedRecords,
    int DeadLetterRecords,
    string? LastSafeErrorCode,
    string? LastSafeErrorMessage,
    byte[] RowVersion);

public sealed record SapSyncExecutionStateData(
    Guid ExecutionUid,
    string ExpectedStatus,
    string NewStatus,
    int TotalRecords,
    int CreatedRecords,
    int UpdatedRecords,
    int UnchangedRecords,
    int ApprovalRequiredRecords,
    int ConflictRecords,
    int SkippedRecords,
    int RetryScheduledRecords,
    int FailedRecords,
    int DeadLetterRecords,
    string? LastSafeErrorCode,
    string? LastSafeErrorMessage,
    DateTime? NextAttemptAtUtc,
    byte[] ExpectedRowVersion);

public sealed record SapSyncExecutionDetailData(
    long? Id,
    Guid ExecutionUid,
    string SourceRecordKey,
    string? SourceVersion,
    long? LocalEntityId,
    Guid? LocalGlobalId,
    string Action,
    string Status,
    int AttemptCount,
    int MaxAttempts,
    DateTime? NextAttemptAtUtc,
    string? ErrorClass,
    string? ResultCode,
    string? SafeMessage,
    string? ApprovedSnapshotType,
    string? ApprovedSnapshotJson,
    byte[]? SnapshotHash,
    DateTime? StartedAtUtc,
    DateTime? FinishedAtUtc,
    byte[]? RowVersion);

public sealed record SapSyncExecutionDetailFilter(
    Guid ExecutionUid,
    string? Status,
    string? SourceRecordKey,
    int PageNumber = 1,
    int PageSize = 100);

public sealed record SapSyncExecutionDetailClaim(
    long Id,
    Guid ExecutionUid,
    string SourceRecordKey,
    string Status,
    int AttemptCount,
    int MaxAttempts,
    string? ApprovedSnapshotType,
    string? ApprovedSnapshotJson,
    byte[]? SnapshotHash,
    string OwnerToken,
    DateTime LockedAtUtc,
    DateTime LockExpiresAtUtc);

public sealed record SapSyncExecutionWriteResult(
    long? Id,
    string ResultCode,
    byte[]? RowVersion);
