namespace NuanSystem.Application.Features.Sync.Configuration.Dtos;

public sealed record SyncProfileListFilter(
    string? Search,
    int? CompanyId,
    bool? IsActive,
    string? ExecutionMode,
    int PageNumber = 1,
    int PageSize = 50,
    int? UserId = null);

public sealed record PagedResultDto<T>(
    IReadOnlyCollection<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);

public sealed record SyncProfileListItemDto
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
}

public sealed record SyncProfileApiDetailDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int CompanyId { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public string Direction { get; init; } = string.Empty;
    public string ExecutionMode { get; init; } = string.Empty;
    public string ConflictStrategy { get; init; } = string.Empty;
    public int BatchSize { get; init; }
    public int MaxRetries { get; init; }
    public int RetryDelaySeconds { get; init; }
    public int TimeoutMinutes { get; init; }
    public bool IsActive { get; init; }
    public IReadOnlyCollection<SyncProfileBranchDto> Branches { get; init; } = Array.Empty<SyncProfileBranchDto>();
    public IReadOnlyCollection<SyncProfileEntityDto> Entities { get; init; } = Array.Empty<SyncProfileEntityDto>();
    public SyncScheduleDto? Schedule { get; init; }
}

public sealed record SyncProfileBranchDto
{
    public int Id { get; init; }
    public int BranchCompanyId { get; init; }
    public string BranchCompanyCode { get; init; } = string.Empty;
    public string BranchCompanyName { get; init; } = string.Empty;
    public int? BatchSize { get; init; }
    public int? MaxRetries { get; init; }
    public bool IsActive { get; init; }
    public DateTime? LastSynchronizationAt { get; init; }
}

public sealed record SyncProfileEntityDto
{
    public int Id { get; init; }
    public string EntityCode { get; init; } = string.Empty;
    public string EntityName { get; init; } = string.Empty;
    public int ExecutionOrder { get; init; }
    public string SyncMode { get; init; } = string.Empty;
    public string? KeyField { get; init; }
    public string? ModifiedAtField { get; init; }
    public string? VersionField { get; init; }
    public string? ActiveField { get; init; }
    public bool AllowInsert { get; init; }
    public bool AllowUpdate { get; init; }
    public bool AllowDeactivate { get; init; }
    public bool ContinueOnError { get; init; }
    public int? BatchSize { get; init; }
    public bool IsActive { get; init; }
    public IReadOnlyCollection<SyncEntityBranchDto> Branches { get; init; } = Array.Empty<SyncEntityBranchDto>();
}

public sealed record SyncEntityBranchDto
{
    public int Id { get; init; }
    public int SyncProfileBranchId { get; init; }
    public int BranchCompanyId { get; init; }
    public bool IsEnabled { get; init; }
    public int? BatchSize { get; init; }
}

public sealed record SyncScheduleDto
{
    public int Id { get; init; }
    public string ScheduleType { get; init; } = string.Empty;
    public int? IntervalMinutes { get; init; }
    public TimeSpan? ExecutionTime { get; init; }
    public string TimeZoneId { get; init; } = string.Empty;
    public bool PreventConcurrentExecutions { get; init; }
    public bool IsActive { get; init; }
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

public sealed record SyncConfigurationCatalogDto
{
    public IReadOnlyCollection<CompanyLookupDto> MasterCompanies { get; init; } = Array.Empty<CompanyLookupDto>();
    public IReadOnlyCollection<CompanyLookupDto> BranchCompanies { get; init; } = Array.Empty<CompanyLookupDto>();
    public IReadOnlyCollection<SyncEntityCatalogItemDto> Entities { get; init; } = Array.Empty<SyncEntityCatalogItemDto>();
    public IReadOnlyCollection<LookupItemDto> Directions { get; init; } = Array.Empty<LookupItemDto>();
    public IReadOnlyCollection<LookupItemDto> ExecutionModes { get; init; } = Array.Empty<LookupItemDto>();
    public IReadOnlyCollection<LookupItemDto> ConflictStrategies { get; init; } = Array.Empty<LookupItemDto>();
    public IReadOnlyCollection<LookupItemDto> ScheduleTypes { get; init; } = Array.Empty<LookupItemDto>();
    public string DefaultTimeZoneId { get; init; } = "America/Guayaquil";
}

public sealed record CompanyLookupDto(
    int Id,
    string Code,
    string Name,
    bool IsActive,
    string? BranchCode = null,
    string? DatabaseName = null);

public sealed record SyncCompanyLookupRecord(
    int Id,
    string Code,
    string Name,
    bool IsActive,
    bool IsMaster,
    int? ParentCompanyId,
    bool SyncEnabled,
    string? BranchCode = null,
    string? DatabaseName = null);

public sealed record LookupItemDto(string Code, string Name);

public sealed record SyncEntityCatalogItemDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int DefaultExecutionOrder { get; init; }
    public bool SupportsIncremental { get; init; }
    public bool HasProducer { get; init; }
    public bool HasApplier { get; init; }
    public bool SupportsInsert { get; init; }
    public bool SupportsUpdate { get; init; }
    public bool SupportsDeactivate { get; init; }
    public bool IsSystem { get; init; }
    public bool IsActive { get; init; } = true;
    public string? DefaultKeyField { get; init; }
    public string? DefaultModifiedAtField { get; init; }
    public IReadOnlyCollection<string> Dependencies { get; init; } = Array.Empty<string>();
}

public sealed record SyncProfileValidationResultDto
{
    public bool IsValid { get; init; }
    public IReadOnlyCollection<SyncValidationMessageDto> Errors { get; init; } = Array.Empty<SyncValidationMessageDto>();
    public IReadOnlyCollection<SyncValidationMessageDto> Warnings { get; init; } = Array.Empty<SyncValidationMessageDto>();
}

public sealed record SyncValidationMessageDto(string Code, string? Field, string Message);
