namespace NuanSystem.Application.Features.Sync.Execution.Dtos;

public sealed record SyncProfileExecutionRequest
{
    public string ExecutionType { get; init; } = "Manual";
    public string RequestedBy { get; init; } = string.Empty;
    public IReadOnlyCollection<string>? EntityCodes { get; init; }
    public string? FromKey { get; init; }
    public int? MaxRecords { get; init; }
}

public sealed record ExecuteSyncProfileRequest
{
    public IReadOnlyCollection<string>? EntityCodes { get; init; }
    public string? FromKey { get; init; }
    public int? MaxRecords { get; init; }
}

public sealed record CreateSyncProfileExecutionResultDto(
    int ExecutionId,
    int SyncProfileId,
    string Status,
    string ExecutionType,
    string CorrelationId,
    DateTimeOffset RequestedAt);

public sealed record CancelSyncProfileExecutionResultDto(
    int ExecutionId,
    string Status,
    DateTimeOffset CancelledAt);

public sealed record RetrySyncProfileExecutionResultDto(
    int OriginalExecutionId,
    int NewExecutionId,
    string Status,
    string CorrelationId);

public sealed record SyncProfileExecutionListItemDto(
    int Id,
    int SyncProfileId,
    string ProfileCode,
    string ProfileName,
    int CompanyId,
    string CompanyName,
    string ExecutionType,
    string Status,
    string CorrelationId,
    string? RequestedBy,
    DateTimeOffset RequestedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? FinishedAt,
    int TotalEntities,
    int TotalRecordsRead,
    int TotalEventsPublished,
    int TotalSkipped,
    int TotalErrors,
    string? Message);

public sealed record SyncProfileExecutionDetailDto(
    int Id,
    int SyncProfileId,
    string ProfileCode,
    string ProfileName,
    int CompanyId,
    string CompanyName,
    string ExecutionType,
    string Status,
    string CorrelationId,
    string? RequestedBy,
    DateTimeOffset RequestedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? FinishedAt,
    DateTimeOffset? CancelledAt,
    string? CancelledBy,
    string? EntityCodesJson,
    string? FromKey,
    int? MaxRecords,
    int TotalEntities,
    int TotalRecordsRead,
    int TotalEventsPublished,
    int TotalSkipped,
    int TotalErrors,
    string? Message,
    IReadOnlyCollection<SyncProfileExecutionEntityDetailDto> Details);

public sealed record SyncProfileExecutionEntityDetailDto(
    int Id,
    int SyncProfileExecutionId,
    int SyncProfileEntityId,
    string EntityCode,
    string Status,
    DateTimeOffset? StartedAt,
    DateTimeOffset? FinishedAt,
    int TotalRecordsRead,
    int TotalEventsPublished,
    int TotalSkipped,
    int TotalErrors,
    string? LastProcessedKey,
    string? Message);

public sealed record SyncProfileExecutionFilter(
    int? ProfileId,
    string? Status,
    string? ExecutionType,
    DateTimeOffset? DateFrom,
    DateTimeOffset? DateTo,
    int PageNumber = 1,
    int PageSize = 50);

public sealed record CreateSyncProfileExecutionData(
    int SyncProfileId,
    string ExecutionType,
    string RequestedBy,
    string CorrelationId,
    string? EntityCodesJson,
    string? FromKey,
    int? MaxRecords,
    int TotalEntities,
    int? CreatedByUserId,
    string? CreatedByUserName);

public sealed record CompleteSyncProfileExecutionData(
    int ExecutionId,
    string Status,
    int TotalRecordsRead,
    int TotalEventsPublished,
    int TotalSkipped,
    int TotalErrors,
    string? Message);

public sealed record SyncProfileExecutionDetailUpdate(
    int SyncProfileExecutionId,
    int SyncProfileEntityId,
    string EntityCode,
    string Status,
    int TotalRecordsRead,
    int TotalEventsPublished,
    int TotalSkipped,
    int TotalErrors,
    string? LastProcessedKey,
    string? Message);

public sealed record SyncScheduleDefinition(
    int SyncProfileId,
    string ScheduleType,
    int? IntervalMinutes,
    TimeSpan? ExecutionTime,
    string TimeZoneId,
    DateTimeOffset? LastSuccessfulScheduledExecutionAt,
    DateTimeOffset ConfiguredAt);

public sealed record DueSyncProfileDto(
    int SyncProfileId,
    string ProfileCode,
    string ProfileName,
    int CompanyId,
    string ScheduleType,
    int? IntervalMinutes,
    TimeSpan? ExecutionTime,
    string TimeZoneId,
    DateTimeOffset? LastSuccessfulScheduledExecutionAt,
    DateTimeOffset ConfiguredAt,
    DateTimeOffset? NextExecutionAt);

public sealed record SyncSourceReadContext(
    int CompanyId,
    string? LastKey,
    int PageSize,
    int? RemainingLimit);

public sealed record SyncSourcePage(
    IReadOnlyCollection<SyncSourceRecord> Records,
    string? LastKey,
    bool HasMore);

public sealed record SyncSourceRecord(
    Guid GlobalId,
    string EntityKey,
    bool IsActive,
    object Payload);
