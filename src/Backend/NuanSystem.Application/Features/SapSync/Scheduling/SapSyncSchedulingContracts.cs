using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Features.SapSync.Scheduling;

public static class SapSyncScheduleCandidateSources
{
    public const string Profile = "Profile";
    public const string LegacyFallback = "LegacyFallback";
}

public static class SapSyncScheduleRejectionCodes
{
    public const string Inactive = "SAP_SCHEDULE_INACTIVE";
    public const string Manual = "SAP_SCHEDULE_MANUAL";
    public const string BothUnsupported = "SAP_SYNC_BOTH_UNSUPPORTED";
    public const string PurchaseOrdersUnsupported = "SAP_SYNC_PURCHASE_ORDERS_UNSUPPORTED";
    public const string LegacyFallbackUnsupported = "SAP_SYNC_LEGACY_FALLBACK_UNSUPPORTED";
    public const string HandlerNotImplemented = "SAP_SYNC_HANDLER_NOT_IMPLEMENTED";
    public const string DirectionUnsupported = "SAP_SYNC_DIRECTION_UNSUPPORTED";
    public const string ModeUnsupported = "SAP_SYNC_MODE_UNSUPPORTED";
    public const string ScheduleInvalid = "SAP_SYNC_SCHEDULE_INVALID";
    public const string ConcurrentReservation = "SAP_SYNC_SCHEDULE_CONCURRENT_RESERVATION";
}

public readonly record struct SapSyncScheduleCursor(
    int CompanyId,
    long ProfileId,
    int ExecutionOrder,
    long EntityId)
{
    public static SapSyncScheduleCursor Start => new(0, 0, -1, 0);
    public bool IsStart => this == Start;
}

public sealed record SapSyncScheduleCandidate(
    string CandidateSource,
    int CompanyId,
    string CompanyCode,
    long? ProfileId,
    string ProfileCode,
    string ProfileName,
    bool ProfileIsActive,
    long? ProfileEntityId,
    string EntityCode,
    SapSyncDirection Direction,
    string SyncMode,
    int BatchSize,
    int MaxAttempts,
    int ExecutionOrder,
    bool ContinueOnError,
    int ExecutionTimeoutMinutes,
    bool EntityIsActive,
    long? ScheduleId,
    string ScheduleType,
    int? IntervalMinutes,
    TimeSpan? ExecutionTime,
    string TimeZoneId,
    bool PreventConcurrentExecutions,
    DateTime? NextExecutionAtUtc,
    DateTime? LastScheduledAtUtc,
    DateTime? LastExecutionAtUtc,
    bool ScheduleIsActive,
    byte[]? ScheduleRowVersion,
    bool SupportsSapToErp,
    bool SupportsErpToSap,
    bool SupportsFull,
    bool SupportsIncremental,
    bool CapabilityIsImplemented,
    bool CapabilityIsActive,
    bool LegacyFallbackEnabled,
    string? CompatibilityVersion,
    int RequiredSuccessfulCycles,
    long SortProfileId,
    long SortEntityId)
{
    public SapSyncScheduleCursor Cursor =>
        new(CompanyId, SortProfileId, ExecutionOrder, SortEntityId);

    public bool IsLegacyFallback =>
        CandidateSource.Equals(
            SapSyncScheduleCandidateSources.LegacyFallback,
            StringComparison.OrdinalIgnoreCase);
}

public sealed record SapSyncScheduleCandidatePage(
    IReadOnlyCollection<SapSyncScheduleCandidate> Items,
    int EnabledCompanyCount);

public sealed record SapSyncScheduleReservation(
    long ScheduleId,
    byte[] ExpectedRowVersion,
    DateTime UtcNow,
    DateTime? ObservedNextExecutionAtUtc,
    DateTime? ScheduledAtUtc,
    DateTime NextExecutionAtUtc);

public sealed record SapSyncScheduleCalculation(
    bool IsValid,
    DateTime? NextExecutionAtUtc,
    string? ErrorCode = null);

public sealed record SapSyncScheduledExecutionContext(
    Guid ExecutionUid,
    string CorrelationId,
    string CandidateSource,
    int CompanyId,
    string CompanyCode,
    long? ProfileId,
    string ProfileCode,
    string ProfileName,
    long? ProfileEntityId,
    string EntityCode,
    SapSyncDirection Direction,
    string SyncMode,
    int BatchSize,
    int MaxAttempts,
    int ExecutionOrder,
    bool ContinueOnError,
    int ExecutionTimeoutMinutes,
    long? ScheduleId,
    string ScheduleType,
    string TimeZoneId,
    DateTime ScheduledForAtUtc,
    string WorkerInstance,
    string? CompatibilityVersion,
    int RequiredSuccessfulCycles);

public sealed record SapSyncScheduleRejection(
    int CompanyId,
    string CompanyCode,
    string ProfileCode,
    string EntityCode,
    string Code);

public sealed record SapSyncPollResult(
    IReadOnlyCollection<SapSyncScheduledExecutionContext> Executions,
    IReadOnlyCollection<SapSyncScheduleRejection> Rejections,
    SapSyncScheduleCursor NextCursor,
    int EnabledCompanyCount,
    int InitializedScheduleCount);

public sealed record SapSyncLeaseExecutionResult(
    string Status,
    string? SafeCode = null)
{
    public const string Prepared = "Prepared";
    public const string SkippedConcurrent = "SkippedConcurrent";
    public const string LeaseLost = "LeaseLost";
}
