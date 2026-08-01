namespace NuanSystem.WinForms.Services.Sap.Models;

public sealed record SapPagedResult<T>(IReadOnlyCollection<T> Items, int TotalCount, int PageNumber, int PageSize);

public sealed record SapSyncProfileListFilter
{
    public int? CompanyId { get; set; }
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public string? EntityCode { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 200;
}

public sealed record SapSyncProfileListItem
{
    public long Id { get; init; }
    public int CompanyId { get; init; }
    public string CompanyCode { get; init; } = string.Empty;
    public string CompanyName { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public int ActiveEntityCount { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public byte[] RowVersion { get; init; } = [];
    public string StatusText => IsActive ? "Activo" : "Inactivo";
}

public sealed record SapSyncProfileCompany(int Id, string Code, string Name)
{
    public string DisplayName => $"{Code} - {Name}";
}

public sealed record SapSyncCatalogItem(string Code, string Name)
{
    public string DisplayName => $"{Code} - {Name}";
}

public sealed record SapSyncEntityCapability
{
    public string EntityCode { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public bool SupportsSapToErp { get; init; }
    public bool SupportsErpToSap { get; init; }
    public bool SupportsFull { get; init; }
    public bool SupportsIncremental { get; init; }
    public bool IsImplemented { get; init; }
    public bool IsActive { get; init; }
}

public sealed record SapSyncProfileCatalog
{
    public IReadOnlyCollection<SapSyncProfileCompany> Companies { get; init; } = [];
    public IReadOnlyCollection<SapSyncEntityCapability> Entities { get; init; } = [];
    public IReadOnlyCollection<SapSyncCatalogItem> Directions { get; init; } = [];
    public IReadOnlyCollection<SapSyncCatalogItem> SyncModes { get; init; } = [];
    public IReadOnlyCollection<SapSyncCatalogItem> ScheduleTypes { get; init; } = [];
    public string DefaultTimeZoneId { get; init; } = "America/Guayaquil";
}

public sealed record SapSyncSchedule
{
    public long? Id { get; init; }
    public string ScheduleType { get; init; } = "Manual";
    public int? IntervalMinutes { get; init; }
    public TimeSpan? ExecutionTime { get; init; }
    public string TimeZoneId { get; init; } = "America/Guayaquil";
    public bool PreventConcurrentExecutions { get; init; } = true;
    public bool IsActive { get; init; }
    public DateTime? NextExecutionAtUtc { get; init; }
    public DateTime? LastScheduledAtUtc { get; init; }
    public DateTime? LastExecutionAtUtc { get; init; }
    public DateTime? LastSuccessfulExecutionAtUtc { get; init; }
    public byte[]? RowVersion { get; init; }
}

public sealed record SapSyncProfileEntity
{
    public long? Id { get; init; }
    public string EntityCode { get; init; } = string.Empty;
    public string Direction { get; init; } = "SapToErp";
    public string SyncMode { get; init; } = "Full";
    public int BatchSize { get; init; } = 100;
    public int MaxAttempts { get; init; } = 3;
    public int ExecutionOrder { get; init; } = 1;
    public bool ContinueOnError { get; init; }
    public int ExecutionTimeoutMinutes { get; init; } = 30;
    public bool IsActive { get; init; }
    public SapSyncSchedule Schedule { get; init; } = new();
    public byte[]? RowVersion { get; init; }
}

public sealed record SapSyncProfileDetail
{
    public long Id { get; init; }
    public int CompanyId { get; init; }
    public string CompanyCode { get; init; } = string.Empty;
    public string CompanyName { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public byte[] RowVersion { get; init; } = [];
    public IReadOnlyCollection<SapSyncProfileEntity> Entities { get; init; } = [];
}

public sealed record SaveSapSyncProfileRequest(int CompanyId, string Code, string Name, string? Description, IReadOnlyCollection<SaveSapSyncProfileEntityRequest> Entities);
public sealed record SaveSapSyncProfileEntityRequest(long? Id, string EntityCode, string Direction, string SyncMode, int BatchSize, int MaxAttempts, int ExecutionOrder, bool ContinueOnError, int ExecutionTimeoutMinutes, bool IsActive, SaveSapSyncScheduleRequest Schedule, byte[]? RowVersion);
public sealed record SaveSapSyncScheduleRequest(long? Id, string ScheduleType, int? IntervalMinutes, TimeSpan? ExecutionTime, string? TimeZoneId, bool PreventConcurrentExecutions, bool IsActive, byte[]? RowVersion);
public sealed record UpdateSapSyncProfileRequest(SaveSapSyncProfileRequest Profile, byte[] RowVersion);
public sealed record SapSyncProfileVersionRequest(byte[] RowVersion);
public sealed record SapSyncProfileWriteResult(long Id, bool IsActive, byte[] RowVersion);
public sealed record SapSyncValidationMessage(string Code, string Message, string? Field);
public sealed record SapSyncProfileValidationResult(bool IsValid, IReadOnlyCollection<SapSyncValidationMessage> Errors);

public sealed record SapSyncExecutionFilter
{
    public long? ProfileId { get; set; }
    public string? EntityCode { get; set; }
    public string? Direction { get; set; }
    public string? Status { get; set; }
    public string? TriggerType { get; set; }
    public DateTime? DateFromUtc { get; set; }
    public DateTime? DateToUtc { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 200;
}

public sealed record SapSyncExecutionListItem
{
    public long Id { get; init; }
    public Guid ExecutionUid { get; init; }
    public long? SapSyncProfileId { get; init; }
    public string ProfileCode { get; init; } = string.Empty;
    public string ProfileName { get; init; } = string.Empty;
    public string EntityCode { get; init; } = string.Empty;
    public string Direction { get; init; } = string.Empty;
    public string TriggerType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime RequestedAtUtc { get; init; }
    public DateTime? StartedAtUtc { get; init; }
    public DateTime? FinishedAtUtc { get; init; }
    public int TotalRecords { get; init; }
    public int SucceededRecords { get; init; }
    public int WarningRecords { get; init; }
    public int FailedRecords { get; init; }
}

public sealed record SapSyncExecutionDetail
{
    public long Id { get; init; }
    public Guid ExecutionUid { get; init; }
    public string ProfileCode { get; init; } = string.Empty;
    public string ProfileName { get; init; } = string.Empty;
    public string CompanyCode { get; init; } = string.Empty;
    public string EntityCode { get; init; } = string.Empty;
    public string Direction { get; init; } = string.Empty;
    public string TriggerType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime RequestedAtUtc { get; init; }
    public DateTime? StartedAtUtc { get; init; }
    public DateTime? LastProgressAtUtc { get; init; }
    public DateTime? FinishedAtUtc { get; init; }
    public int TotalRecords { get; init; }
    public int CreatedRecords { get; init; }
    public int UpdatedRecords { get; init; }
    public int UnchangedRecords { get; init; }
    public int ApprovalRequiredRecords { get; init; }
    public int ConflictRecords { get; init; }
    public int SkippedRecords { get; init; }
    public int RetryScheduledRecords { get; init; }
    public int FailedRecords { get; init; }
    public int DeadLetterRecords { get; init; }
    public string? LastSafeErrorCode { get; init; }
    public string? LastSafeErrorMessage { get; init; }
    public byte[] RowVersion { get; init; } = [];
}

public sealed record SapSyncExecutionDetailFilter
{
    public Guid ExecutionUid { get; init; }
    public string? Status { get; set; }
    public string? SourceRecordKey { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 200;
}

public sealed record SapSyncExecutionDetailItem
{
    public long Id { get; init; }
    public Guid ExecutionUid { get; init; }
    public string SourceRecordKey { get; init; } = string.Empty;
    public string? SourceVersion { get; init; }
    public long? LocalEntityId { get; init; }
    public Guid? LocalGlobalId { get; init; }
    public string Action { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int AttemptCount { get; init; }
    public int MaxAttempts { get; init; }
    public DateTime? NextAttemptAtUtc { get; init; }
    public string? ErrorClass { get; init; }
    public string? ResultCode { get; init; }
    public string? SafeMessage { get; init; }
    public DateTime? StartedAtUtc { get; init; }
    public DateTime? FinishedAtUtc { get; init; }
    public byte[] RowVersion { get; init; } = [];
}

public sealed record SapSyncRetryRequest(Guid ClientRequestId, string Reason, byte[] RowVersion);
public sealed record SapSyncRetryResult(long? Id, Guid? ExecutionUid, string ResultCode, byte[]? RowVersion);
public sealed record SapSyncVersionRequest(byte[] RowVersion);
public sealed record SapSyncReleaseLockRequest(string Reason, byte[] RowVersion);
