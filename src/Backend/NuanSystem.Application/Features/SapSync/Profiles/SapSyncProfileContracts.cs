using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Features.SapSync.Profiles;

public static class SapSyncScheduleTypes
{
    public const string Manual = "Manual";
    public const string Interval = "Interval";
    public const string Daily = "Daily";

    public static readonly IReadOnlySet<string> All =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase) { Manual, Interval, Daily };
}

public static class SapSyncModes
{
    public const string Full = "Full";
    public const string Incremental = "Incremental";
}

public static class SapSyncProfilePersistenceCodes
{
    public const string Created = "Created";
    public const string Updated = "Updated";
    public const string Activated = "Activated";
    public const string Deactivated = "Deactivated";
    public const string Deleted = "Deleted";
    public const string NotFound = "NotFound";
    public const string DuplicateCode = "DuplicateCode";
    public const string ConcurrencyConflict = "ConcurrencyConflict";
    public const string UnsupportedDirection = "UnsupportedDirection";
    public const string InvalidSchedule = "InvalidSchedule";
}

public sealed record SapSyncProfileFilter(
    int? CompanyId,
    string? Search,
    bool? IsActive,
    string? EntityCode,
    int PageNumber = 1,
    int PageSize = 50);

public sealed record SapSyncPagedResult<T>(
    IReadOnlyCollection<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);

public sealed record SapSyncProfileListItemDto(
    long Id,
    int CompanyId,
    string CompanyCode,
    string CompanyName,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    int ActiveEntityCount,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    byte[] RowVersion);

public sealed record SapSyncHandlerCapabilityDto(
    string EntityCode,
    string DisplayName,
    bool SupportsSapToErp,
    bool SupportsErpToSap,
    bool SupportsFull,
    bool SupportsIncremental,
    bool IsImplemented,
    bool IsActive)
{
    public bool Supports(SapSyncDirection direction)
    {
        return direction switch
        {
            SapSyncDirection.SapToErp => SupportsSapToErp,
            SapSyncDirection.ErpToSap => SupportsErpToSap,
            SapSyncDirection.Both => SupportsSapToErp && SupportsErpToSap,
            _ => false
        };
    }
}

public sealed record SapSyncScheduleData(
    long? Id,
    string ScheduleType,
    int? IntervalMinutes,
    TimeSpan? ExecutionTime,
    string TimeZoneId,
    bool PreventConcurrentExecutions,
    DateTime? NextExecutionAtUtc,
    DateTime? LastScheduledAtUtc,
    DateTime? LastExecutionAtUtc,
    DateTime? LastSuccessfulExecutionAtUtc,
    bool IsActive,
    byte[]? RowVersion);

public sealed record SapSyncProfileEntityData(
    long? Id,
    string EntityCode,
    SapSyncDirection Direction,
    string SyncMode,
    int BatchSize,
    int MaxAttempts,
    int ExecutionOrder,
    bool ContinueOnError,
    int ExecutionTimeoutMinutes,
    bool IsActive,
    SapSyncScheduleData Schedule,
    byte[]? RowVersion);

public sealed record SapSyncProfileAggregate(
    long? Id,
    int CompanyId,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    IReadOnlyCollection<SapSyncProfileEntityData> Entities,
    int? AuditUserId,
    string? AuditUserName,
    byte[]? RowVersion);

public sealed record SapSyncProfileDetailDto(
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
    IReadOnlyCollection<SapSyncProfileEntityData> Entities);

public sealed record SapSyncProfileWriteResult(
    long? Id,
    string ResultCode,
    byte[]? RowVersion);
