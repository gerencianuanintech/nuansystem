using NuanSystem.Application.Abstractions.Operations;

namespace NuanSystem.Application.Features.Operations;

public sealed class WorkerHeartbeatService(IWorkerHeartbeatRepository repository) : IWorkerHeartbeatService
{
    public Task BeatAsync(WorkerHeartbeatDto heartbeat, CancellationToken cancellationToken = default) =>
        repository.UpsertAsync(heartbeat, cancellationToken);

    public Task<IReadOnlyCollection<WorkerHeartbeatSnapshotDto>> GetByWorkerTypeAsync(string workerType, CancellationToken cancellationToken = default) =>
        repository.GetByWorkerTypeAsync(workerType, cancellationToken);
}

public static class WorkerHealthEvaluator
{
    public static WorkerHealthReportDto Evaluate(IReadOnlyCollection<WorkerHeartbeatSnapshotDto> snapshots,
        WorkerHealthThresholds thresholds, DateTime utcNow)
    {
        var activeDuplicates = snapshots
            .Where(x => x.IsEnabled && x.LifecycleState is WorkerLifecycleStates.Starting or WorkerLifecycleStates.Running)
            .Where(x => utcNow - x.LastBeatAtUtc <= TimeSpan.FromSeconds(thresholds.HeartbeatUnhealthySeconds))
            .GroupBy(x => x.WorkerType, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var instances = snapshots.Select(snapshot => EvaluateInstance(snapshot, thresholds, utcNow,
            activeDuplicates.Contains(snapshot.WorkerType))).ToArray();
        var overall = instances.Length == 0 ? "Unknown" : instances.Any(x => x.Health == "Unhealthy") ? "Unhealthy"
            : instances.Any(x => x.Health == "Degraded") ? "Degraded"
            : instances.All(x => x.Health == "Disabled") ? "Disabled" : "Healthy";
        return new WorkerHealthReportDto(overall, utcNow, instances);
    }

    private static WorkerHealthInstanceDto EvaluateInstance(WorkerHeartbeatSnapshotDto value,
        WorkerHealthThresholds t, DateTime now, bool duplicate)
    {
        var reasons = new List<string>();
        if (!value.IsEnabled || value.LifecycleState == WorkerLifecycleStates.Disabled)
            return ToResult(value, "Disabled", reasons);

        var unhealthy = false;
        var degraded = false;
        void Critical(string code) { reasons.Add(code); unhealthy = true; }
        void Warning(string code) { reasons.Add(code); degraded = true; }

        if (duplicate) Critical("UNAUTHORIZED_SECOND_INSTANCE");
        if (value.LifecycleState == WorkerLifecycleStates.Faulted) Critical("WORKER_FAULTED");
        else if (value.LifecycleState == WorkerLifecycleStates.Stopped) Critical("WORKER_STOPPED_WHILE_ENABLED");
        else if (value.LifecycleState == WorkerLifecycleStates.Stopping) Warning("WORKER_STOPPING");
        var beatAge = now - value.LastBeatAtUtc;
        if (beatAge > TimeSpan.FromSeconds(t.HeartbeatUnhealthySeconds)) Critical("HEARTBEAT_UNHEALTHY");
        else if (beatAge > TimeSpan.FromSeconds(t.HeartbeatDegradedSeconds)) Warning("HEARTBEAT_DEGRADED");

        if (value.LastSuccessfulCycleAtUtc is { } success)
        {
            var age = now - success;
            if (age > TimeSpan.FromMinutes(t.LastSuccessUnhealthyMinutes)) Critical("LAST_SUCCESS_UNHEALTHY");
            else if (age > TimeSpan.FromMinutes(t.LastSuccessDegradedMinutes)) Warning("LAST_SUCCESS_DEGRADED");
        }
        else if (value.StartedAtUtc is { } started)
        {
            var age = now - started;
            if (age > TimeSpan.FromMinutes(t.LastSuccessUnhealthyMinutes)) Critical("NO_SUCCESSFUL_CYCLE_UNHEALTHY");
            else if (age > TimeSpan.FromMinutes(t.LastSuccessDegradedMinutes)) Warning("NO_SUCCESSFUL_CYCLE_DEGRADED");
        }
        if (value.LastCycleResult == "Failed") Warning("LAST_CYCLE_FAILED");
        if (value.OldestPendingAtUtc is { } pending)
        {
            var age = now - pending;
            if (age > TimeSpan.FromMinutes(t.OldestPendingCriticalMinutes)) Critical("OLDEST_PENDING_CRITICAL");
            else if (age > TimeSpan.FromMinutes(t.OldestPendingWarningMinutes)) Warning("OLDEST_PENDING_WARNING");
        }
        if (value.RetryScheduledCount >= t.RetryScheduledCriticalCount) Critical("RETRY_SCHEDULED_CRITICAL");
        else if (value.RetryScheduledCount >= t.RetryScheduledWarningCount) Warning("RETRY_SCHEDULED_WARNING");
        if (value.RecentDeadLetterCount >= t.RecentDeadLetterCriticalCount) Critical("DEADLETTER_RATE_CRITICAL");
        else if (value.DeadLetterCount > 0) Warning("DEADLETTER_PRESENT");
        if (value.ExpiredLeaseCount > 0) Warning("EXPIRED_LEASE_PRESENT");
        if (value.CertificateDaysRemaining is < 0) Critical("CERTIFICATE_EXPIRED");
        else if (value.CertificateDaysRemaining is { } cert && cert <= t.CertificateCriticalDays) Critical("CERTIFICATE_CRITICAL");
        else if (value.CertificateDaysRemaining is { } warningCert && warningCert <= t.CertificateWarningDays) Warning("CERTIFICATE_WARNING");
        if (value.StorageFreePercent is { } storage && storage < t.StorageCriticalPercent) Critical("STORAGE_CRITICAL");
        else if (value.StorageFreePercent is { } warningStorage && warningStorage < t.StorageWarningPercent) Warning("STORAGE_WARNING");

        return ToResult(value, unhealthy ? "Unhealthy" : degraded ? "Degraded" : "Healthy", reasons);
    }

    private static WorkerHealthInstanceDto ToResult(WorkerHeartbeatSnapshotDto x, string health, IReadOnlyCollection<string> reasons) =>
        new(x.WorkerType, x.HostName, x.WorkerInstance, x.LifecycleState, health, reasons, x.LastBeatAtUtc,
            x.LastSuccessfulCycleAtUtc, x.EnabledCompanyCount, x.PendingCount, x.RetryScheduledCount,
            x.DeadLetterCount, x.ActiveLeaseCount, x.ExpiredLeaseCount, x.LastSafeErrorCode, x.LastSafeErrorMessage);
}
