namespace NuanSystem.Application.Features.Operations;

public static class WorkerTypes
{
    public const string SapSync = "SapSync";
    public const string Sri = "Sri";
}

public static class WorkerLifecycleStates
{
    public const string Starting = "Starting";
    public const string Running = "Running";
    public const string Stopping = "Stopping";
    public const string Stopped = "Stopped";
    public const string Faulted = "Faulted";
    public const string Disabled = "Disabled";
}

public sealed record WorkerHeartbeatDto(
    string InstanceName,
    int? CompanyId,
    string? CompanyCode,
    string Status,
    string? CurrentJob,
    string? WorkerVersion,
    DateTime LastBeatAtUtc,
    string WorkerType = WorkerTypes.SapSync,
    string? HostName = null,
    string? WorkerInstance = null,
    string? LifecycleState = null,
    bool IsEnabled = true,
    DateTime? StartedAtUtc = null,
    DateTime? LastCycleStartedAtUtc = null,
    DateTime? LastCycleCompletedAtUtc = null,
    DateTime? LastSuccessfulCycleAtUtc = null,
    int? LastCycleDurationMs = null,
    string? LastCycleResult = null,
    string? LastSafeErrorCode = null,
    string? LastSafeErrorMessage = null,
    int EnabledCompanyCount = 0,
    long PendingCount = 0,
    long RetryScheduledCount = 0,
    long DeadLetterCount = 0,
    long RecentDeadLetterCount = 0,
    long ActiveLeaseCount = 0,
    long ExpiredLeaseCount = 0,
    DateTime? OldestPendingAtUtc = null,
    decimal? StorageFreePercent = null,
    int? CertificateDaysRemaining = null);

public sealed record WorkerHeartbeatSnapshotDto(
    string WorkerType,
    string HostName,
    string WorkerInstance,
    string LifecycleState,
    bool IsEnabled,
    string? WorkerVersion,
    DateTime LastBeatAtUtc,
    DateTime? StartedAtUtc,
    DateTime? LastCycleStartedAtUtc,
    DateTime? LastCycleCompletedAtUtc,
    DateTime? LastSuccessfulCycleAtUtc,
    int? LastCycleDurationMs,
    string? LastCycleResult,
    string? LastSafeErrorCode,
    string? LastSafeErrorMessage,
    int EnabledCompanyCount,
    long PendingCount,
    long RetryScheduledCount,
    long DeadLetterCount,
    long RecentDeadLetterCount,
    long ActiveLeaseCount,
    long ExpiredLeaseCount,
    DateTime? OldestPendingAtUtc,
    decimal? StorageFreePercent,
    int? CertificateDaysRemaining);

public sealed record WorkerHealthThresholds(
    int HeartbeatDegradedSeconds = 90,
    int HeartbeatUnhealthySeconds = 180,
    int OldestPendingWarningMinutes = 10,
    int OldestPendingCriticalMinutes = 30,
    int RetryScheduledWarningCount = 5,
    int RetryScheduledCriticalCount = 20,
    int RecentDeadLetterCriticalCount = 5,
    int LastSuccessDegradedMinutes = 5,
    int LastSuccessUnhealthyMinutes = 15,
    int CertificateWarningDays = 30,
    int CertificateCriticalDays = 14,
    decimal StorageWarningPercent = 20m,
    decimal StorageCriticalPercent = 10m)
{
    public void Validate()
    {
        if (HeartbeatDegradedSeconds < 10 || HeartbeatUnhealthySeconds <= HeartbeatDegradedSeconds ||
            OldestPendingWarningMinutes < 1 || OldestPendingCriticalMinutes <= OldestPendingWarningMinutes ||
            RetryScheduledWarningCount < 1 || RetryScheduledCriticalCount <= RetryScheduledWarningCount ||
            RecentDeadLetterCriticalCount < 1 || LastSuccessDegradedMinutes < 1 || LastSuccessUnhealthyMinutes <= LastSuccessDegradedMinutes ||
            CertificateCriticalDays < 1 || CertificateWarningDays <= CertificateCriticalDays ||
            StorageCriticalPercent <= 0 || StorageWarningPercent <= StorageCriticalPercent || StorageWarningPercent > 100)
            throw new InvalidOperationException("La configuracion de umbrales del SRI Worker es invalida.");
    }
}

public sealed record WorkerHealthInstanceDto(
    string WorkerType,
    string HostName,
    string WorkerInstance,
    string LifecycleState,
    string Health,
    IReadOnlyCollection<string> ReasonCodes,
    DateTime LastBeatAtUtc,
    DateTime? LastSuccessfulCycleAtUtc,
    int EnabledCompanyCount,
    long PendingCount,
    long RetryScheduledCount,
    long DeadLetterCount,
    long ActiveLeaseCount,
    long ExpiredLeaseCount,
    string? LastSafeErrorCode,
    string? LastSafeErrorMessage,
    string? WorkerVersion = null);

public sealed record WorkerHealthReportDto(string OverallHealth, DateTime EvaluatedAtUtc,
    IReadOnlyCollection<WorkerHealthInstanceDto> Instances);
