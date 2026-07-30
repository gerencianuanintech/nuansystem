namespace NuanSystem.Application.Features.SapSync.Profiles;

public static class SapSyncProfileErrorCodes
{
    public const string NotFound = "SAP_SYNC_PROFILE_NOT_FOUND";
    public const string DuplicateCode = "SAP_SYNC_PROFILE_DUPLICATE_CODE";
    public const string ConcurrencyConflict = "SAP_SYNC_PROFILE_CONCURRENCY_CONFLICT";
    public const string CompanyNotFound = "SAP_SYNC_PROFILE_COMPANY_NOT_FOUND";
    public const string CompanyInactive = "SAP_SYNC_PROFILE_COMPANY_INACTIVE";
    public const string CompanySapDisabled = "SAP_SYNC_PROFILE_COMPANY_SAP_DISABLED";
    public const string CompanyAccessDenied = "SAP_SYNC_PROFILE_COMPANY_ACCESS_DENIED";
    public const string CompanyImmutable = "SAP_SYNC_PROFILE_COMPANY_IMMUTABLE";
    public const string EntityRequired = "SAP_SYNC_PROFILE_ENTITY_REQUIRED";
    public const string EntityUnknown = "SAP_SYNC_PROFILE_ENTITY_UNKNOWN";
    public const string EntityNotImplemented = "SAP_SYNC_PROFILE_ENTITY_NOT_IMPLEMENTED";
    public const string PurchaseOrdersUnsupported = "SAP_SYNC_PROFILE_PURCHASE_ORDERS_UNSUPPORTED";
    public const string DirectionInvalid = "SAP_SYNC_PROFILE_DIRECTION_INVALID";
    public const string DirectionBothUnsupported = "SAP_SYNC_PROFILE_DIRECTION_BOTH_UNSUPPORTED";
    public const string DirectionUnsupported = "SAP_SYNC_PROFILE_DIRECTION_UNSUPPORTED";
    public const string SyncModeInvalid = "SAP_SYNC_PROFILE_SYNC_MODE_INVALID";
    public const string SyncModeUnsupported = "SAP_SYNC_PROFILE_SYNC_MODE_UNSUPPORTED";
    public const string DuplicateEntityDirection = "SAP_SYNC_PROFILE_ENTITY_DIRECTION_DUPLICATE";
    public const string ScheduleInvalid = "SAP_SYNC_PROFILE_SCHEDULE_INVALID";
    public const string TimeZoneInvalid = "SAP_SYNC_PROFILE_TIME_ZONE_INVALID";
    public const string ConcurrentExecutionRequired = "SAP_SYNC_PROFILE_CONCURRENCY_REQUIRED";
    public const string NoActiveSupportedEntities = "SAP_SYNC_PROFILE_NO_ACTIVE_SUPPORTED_ENTITIES";
    public const string UnsupportedCapability = "SAP_SYNC_PROFILE_UNSUPPORTED_CAPABILITY";
    public const string PersistenceRejected = "SAP_SYNC_PROFILE_PERSISTENCE_REJECTED";
}

public sealed record SapSyncProfileListRequest(
    int? CompanyId,
    string? Search,
    bool? IsActive,
    string? EntityCode,
    int PageNumber = 1,
    int PageSize = 50);

public sealed record SaveSapSyncProfileRequest(
    int CompanyId,
    string Code,
    string Name,
    string? Description,
    IReadOnlyCollection<SaveSapSyncProfileEntityRequest> Entities);

public sealed record SaveSapSyncProfileEntityRequest(
    long? Id,
    string EntityCode,
    string Direction,
    string SyncMode,
    int BatchSize,
    int MaxAttempts,
    int ExecutionOrder,
    bool ContinueOnError,
    int ExecutionTimeoutMinutes,
    bool IsActive,
    SaveSapSyncScheduleRequest Schedule,
    byte[]? RowVersion = null);

public sealed record SaveSapSyncScheduleRequest(
    long? Id,
    string ScheduleType,
    int? IntervalMinutes,
    TimeSpan? ExecutionTime,
    string? TimeZoneId,
    bool PreventConcurrentExecutions,
    bool IsActive,
    byte[]? RowVersion = null);

public sealed record UpdateSapSyncProfileRequest(
    SaveSapSyncProfileRequest Profile,
    byte[] RowVersion);

public sealed record SapSyncProfileVersionRequest(byte[] RowVersion);

public sealed record SapSyncProfileWriteDto(
    long Id,
    bool IsActive,
    byte[] RowVersion);

public sealed record SapSyncProfileEntityDto(
    long? Id,
    string EntityCode,
    string Direction,
    string SyncMode,
    int BatchSize,
    int MaxAttempts,
    int ExecutionOrder,
    bool ContinueOnError,
    int ExecutionTimeoutMinutes,
    bool IsActive,
    SapSyncScheduleDto Schedule,
    byte[]? RowVersion);

public sealed record SapSyncScheduleDto(
    long? Id,
    string ScheduleType,
    int? IntervalMinutes,
    TimeSpan? ExecutionTime,
    string TimeZoneId,
    bool PreventConcurrentExecutions,
    bool IsActive,
    DateTime? NextExecutionAtUtc,
    DateTime? LastScheduledAtUtc,
    DateTime? LastExecutionAtUtc,
    DateTime? LastSuccessfulExecutionAtUtc,
    byte[]? RowVersion);

public sealed record SapSyncProfileDto(
    long Id,
    int CompanyId,
    string CompanyCode,
    string CompanyName,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    int? CreatedByUserId,
    string? CreatedByUserName,
    DateTime CreatedAtUtc,
    int? UpdatedByUserId,
    string? UpdatedByUserName,
    DateTime? UpdatedAtUtc,
    byte[] RowVersion,
    IReadOnlyCollection<SapSyncProfileEntityDto> Entities);

public sealed record SapSyncProfileCompanyDto(
    int Id,
    string Code,
    string Name);

public sealed record SapSyncProfileCatalogDto(
    IReadOnlyCollection<SapSyncProfileCompanyDto> Companies,
    IReadOnlyCollection<SapSyncHandlerCapabilityDto> Entities,
    IReadOnlyCollection<SapSyncProfileCatalogItemDto> Directions,
    IReadOnlyCollection<SapSyncProfileCatalogItemDto> SyncModes,
    IReadOnlyCollection<SapSyncProfileCatalogItemDto> ScheduleTypes,
    string DefaultTimeZoneId);

public sealed record SapSyncProfileCatalogItemDto(string Code, string Name);

public sealed record SapSyncProfileValidationMessageDto(
    string Code,
    string Message,
    string? Field = null);

public sealed record SapSyncProfileValidationResultDto(
    bool IsValid,
    IReadOnlyCollection<SapSyncProfileValidationMessageDto> Errors);
