namespace NuanSystem.WinForms.Services.Sync.Models;

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);

public sealed record SyncProfileListItem
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int CompanyId { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public int BranchCount { get; init; }
    public int EntityCount { get; init; }
    public string Direction { get; init; } = string.Empty;
    public string ExecutionMode { get; init; } = string.Empty;
    public string ConflictStrategy { get; init; } = string.Empty;
    public int BatchSize { get; init; }
    public int MaxRetries { get; init; }
    public bool IsActive { get; init; }
    public DateTime? LastExecutionAt { get; init; }
    public DateTime? NextExecutionAt { get; init; }
    public int? CreatedByUserId { get; init; }
    public string? CreatedByUserName { get; init; }
    public DateTime CreatedAt { get; init; }
    public int? UpdatedByUserId { get; init; }
    public string? UpdatedByUserName { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string StatusText => IsActive ? "Activo" : "Inactivo";
}

public sealed record SyncProfileDetail
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int CompanyId { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public string Direction { get; init; } = "MasterToBranch";
    public string ExecutionMode { get; init; } = "Incremental";
    public string ConflictStrategy { get; init; } = "MasterWins";
    public int BatchSize { get; init; } = 500;
    public int MaxRetries { get; init; } = 3;
    public int RetryDelaySeconds { get; init; } = 30;
    public int TimeoutMinutes { get; init; } = 30;
    public bool IsActive { get; init; }
    public IReadOnlyCollection<SyncProfileBranch> Branches { get; init; } = Array.Empty<SyncProfileBranch>();
    public IReadOnlyCollection<SyncProfileEntity> Entities { get; init; } = Array.Empty<SyncProfileEntity>();
    public SyncSchedule? Schedule { get; init; }
}

public sealed record SyncProfileBranch
{
    public int Id { get; init; }
    public int BranchCompanyId { get; init; }
    public string BranchCompanyCode { get; init; } = string.Empty;
    public string BranchCompanyName { get; init; } = string.Empty;
    public int? BatchSize { get; init; }
    public int? MaxRetries { get; init; }
    public bool IsActive { get; init; } = true;
    public DateTime? LastSynchronizationAt { get; init; }
}

public sealed record SyncProfileEntity
{
    public int Id { get; init; }
    public string EntityCode { get; init; } = string.Empty;
    public string EntityName { get; init; } = string.Empty;
    public int ExecutionOrder { get; init; }
    public string SyncMode { get; init; } = "Incremental";
    public string? KeyField { get; init; }
    public string? ModifiedAtField { get; init; }
    public string? VersionField { get; init; }
    public string? ActiveField { get; init; }
    public bool AllowInsert { get; init; } = true;
    public bool AllowUpdate { get; init; } = true;
    public bool AllowDeactivate { get; init; } = true;
    public bool ContinueOnError { get; init; }
    public int? BatchSize { get; init; }
    public bool IsActive { get; init; } = true;
    public IReadOnlyCollection<SyncEntityBranch> Branches { get; init; } = Array.Empty<SyncEntityBranch>();
}

public sealed record SyncEntityBranch
{
    public int Id { get; init; }
    public int SyncProfileBranchId { get; init; }
    public int BranchCompanyId { get; init; }
    public bool IsEnabled { get; init; } = true;
    public int? BatchSize { get; init; }
}

public sealed record SyncDistributionSelection(Guid EntityGlobalId, string? EntityCode);

public sealed record SyncDistributionCandidate(
    Guid EntityGlobalId,
    string EntityCode,
    string EntityName,
    bool IsActive)
{
    public bool IsSelected { get; set; }
    public string DisplayName => $"{EntityCode} - {EntityName}";
}

public sealed record SyncDistributionPolicy(
    int SyncProfileEntityBranchId,
    int SyncProfileId,
    string SyncProfileCode,
    int CompanyId,
    string CompanyCode,
    string EntityCode,
    int BranchCompanyId,
    string BranchCompanyCode,
    string BranchCompanyName,
    string DistributionMode,
    string OnNoMatch,
    string? RuleExpressionJson,
    int RuleVersion,
    IReadOnlyCollection<SyncDistributionSelection> Selections);

public sealed record SyncDistributionPolicyCatalog(
    IReadOnlyCollection<string> Modes,
    IReadOnlyCollection<string> OnNoMatchActions,
    IReadOnlyCollection<string> Operators,
    IReadOnlyCollection<string> Fields);

public sealed record SaveSyncDistributionPolicyRequest
{
    public string DistributionMode { get; init; } = "None";
    public string OnNoMatch { get; init; } = "KeepInMaster";
    public string? RuleExpressionJson { get; init; }
    public IReadOnlyCollection<SyncDistributionSelection> Selections { get; init; } = Array.Empty<SyncDistributionSelection>();
}

public sealed record SyncSchedule
{
    public int Id { get; init; }
    public string ScheduleType { get; init; } = "Manual";
    public int? IntervalMinutes { get; init; }
    public TimeSpan? ExecutionTime { get; init; }
    public string TimeZoneId { get; init; } = "America/Guayaquil";
    public bool PreventConcurrentExecutions { get; init; } = true;
    public bool IsActive { get; init; } = true;
}

public sealed record SaveSyncProfileRequest
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int CompanyId { get; init; }
    public string Direction { get; init; } = "MasterToBranch";
    public string ExecutionMode { get; init; } = "Incremental";
    public string ConflictStrategy { get; init; } = "MasterWins";
    public int BatchSize { get; init; } = 500;
    public int MaxRetries { get; init; } = 3;
    public int RetryDelaySeconds { get; init; } = 30;
    public int TimeoutMinutes { get; init; } = 30;
    public bool IsActive { get; init; }
    public IReadOnlyCollection<SaveSyncProfileBranchRequest> Branches { get; init; } = Array.Empty<SaveSyncProfileBranchRequest>();
    public IReadOnlyCollection<SaveSyncProfileEntityRequest> Entities { get; init; } = Array.Empty<SaveSyncProfileEntityRequest>();
    public SaveSyncScheduleRequest? Schedule { get; init; }
}

public sealed record SaveSyncProfileBranchRequest
{
    public int BranchCompanyId { get; init; }
    public int? BatchSize { get; init; }
    public int? MaxRetries { get; init; }
    public bool IsActive { get; init; } = true;
}

public sealed record SaveSyncProfileEntityRequest
{
    public string EntityCode { get; init; } = string.Empty;
    public string EntityName { get; init; } = string.Empty;
    public int ExecutionOrder { get; init; }
    public string SyncMode { get; init; } = "Incremental";
    public string? KeyField { get; init; }
    public string? ModifiedAtField { get; init; }
    public string? VersionField { get; init; }
    public string? ActiveField { get; init; }
    public bool AllowInsert { get; init; } = true;
    public bool AllowUpdate { get; init; } = true;
    public bool AllowDeactivate { get; init; } = true;
    public bool ContinueOnError { get; init; }
    public int? BatchSize { get; init; }
    public bool IsActive { get; init; } = true;
    public IReadOnlyCollection<SaveSyncEntityBranchRequest> Branches { get; init; } = Array.Empty<SaveSyncEntityBranchRequest>();
}

public sealed record SaveSyncEntityBranchRequest
{
    public int BranchCompanyId { get; init; }
    public bool IsEnabled { get; init; } = true;
    public int? BatchSize { get; init; }
}

public sealed record SaveSyncScheduleRequest
{
    public string ScheduleType { get; init; } = "Manual";
    public int? IntervalMinutes { get; init; }
    public TimeSpan? ExecutionTime { get; init; }
    public string TimeZoneId { get; init; } = "America/Guayaquil";
    public bool PreventConcurrentExecutions { get; init; } = true;
    public bool IsActive { get; init; } = true;
}

public sealed record SyncConfigurationCatalog
{
    public IReadOnlyCollection<CompanyLookupItem> MasterCompanies { get; init; } = Array.Empty<CompanyLookupItem>();
    public IReadOnlyCollection<CompanyLookupItem> BranchCompanies { get; init; } = Array.Empty<CompanyLookupItem>();
    public IReadOnlyCollection<SyncEntityCatalogItem> Entities { get; init; } = Array.Empty<SyncEntityCatalogItem>();
    public IReadOnlyCollection<LookupItem> Directions { get; init; } = Array.Empty<LookupItem>();
    public IReadOnlyCollection<LookupItem> ExecutionModes { get; init; } = Array.Empty<LookupItem>();
    public IReadOnlyCollection<LookupItem> ConflictStrategies { get; init; } = Array.Empty<LookupItem>();
    public IReadOnlyCollection<LookupItem> ScheduleTypes { get; init; } = Array.Empty<LookupItem>();
    public string DefaultTimeZoneId { get; init; } = "America/Guayaquil";
}

public sealed record CompanyLookupItem(
    int Id,
    string Code,
    string Name,
    bool IsActive,
    string? BranchCode = null,
    string? DatabaseName = null)
{
    public string DisplayName => $"{Code} - {Name}";
}

public sealed record LookupItem(string Code, string Name)
{
    public string DisplayName => $"{Code} - {Name}";
}

public sealed record SyncEntityCatalogItem
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int DefaultExecutionOrder { get; init; }
    public bool SupportsIncremental { get; init; }
    public bool HasProducer { get; init; }
    public bool HasApplier { get; init; }
    public bool IsOperative => HasProducer && HasApplier;
    public bool SupportsInsert { get; init; }
    public bool SupportsUpdate { get; init; }
    public bool SupportsDeactivate { get; init; }
    public string? DefaultKeyField { get; init; }
    public string? DefaultModifiedAtField { get; init; }
    public IReadOnlyCollection<string> Dependencies { get; init; } = Array.Empty<string>();
}

public sealed record SyncProfileValidationResult
{
    public bool IsValid { get; init; }
    public IReadOnlyCollection<SyncValidationMessage> Errors { get; init; } = Array.Empty<SyncValidationMessage>();
    public IReadOnlyCollection<SyncValidationMessage> Warnings { get; init; } = Array.Empty<SyncValidationMessage>();
}

public sealed record SyncValidationMessage(string Code, string? Field, string Message);

public sealed record ExecuteSyncProfileRequest
{
    public IReadOnlyCollection<string>? EntityCodes { get; init; }
    public string? FromKey { get; init; }
    public int? MaxRecords { get; init; }
}

public sealed record CreateSyncProfileExecutionResult(
    int ExecutionId,
    int SyncProfileId,
    string Status,
    string ExecutionType,
    string CorrelationId,
    DateTimeOffset RequestedAt);

public sealed record CancelSyncProfileExecutionResult(int ExecutionId, string Status, DateTimeOffset CancelledAt);

public sealed record RetrySyncProfileExecutionResult(int OriginalExecutionId, int NewExecutionId, string Status, string CorrelationId);

public sealed record SyncProfileExecutionListItem(
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

public sealed record SyncProfileExecutionDetail(
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
    IReadOnlyCollection<SyncProfileExecutionEntityDetail> Details);

public sealed record SyncProfileExecutionEntityDetail(
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
