using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using NuanSystem.Application.Features.Operations;

namespace NuanSystem.SyncWorker.Services;

public sealed class SapSyncWorkerRuntimeState
{
    private readonly object sync = new();
    private string lifecycleState = WorkerLifecycleStates.Starting;
    private bool canSchedule = true;
    private DateTime? startedAtUtc;
    private DateTime? lastCycleStartedAtUtc;
    private DateTime? lastCycleCompletedAtUtc;
    private DateTime? lastSuccessfulCycleAtUtc;
    private int? lastCycleDurationMs;
    private string? lastCycleResult;
    private string? lastSafeErrorCode;
    private string? lastSafeErrorMessage;
    private int enabledCompanyCount;
    private int activeLeaseCount;
    private int? currentCompanyId;
    private string? currentCompanyCode;
    private string? currentJob;

    public void MarkDisabled()
    {
        lock (sync)
        {
            canSchedule = false;
            lifecycleState = WorkerLifecycleStates.Disabled;
        }
    }

    public void MarkStarted(DateTime utcNow)
    {
        lock (sync)
        {
            lifecycleState = WorkerLifecycleStates.Running;
            startedAtUtc ??= utcNow;
            lastSafeErrorCode = null;
            lastSafeErrorMessage = null;
        }
    }

    public void MarkCycleStarted(DateTime utcNow)
    {
        lock (sync)
        {
            lastCycleStartedAtUtc = utcNow;
            lastCycleCompletedAtUtc = null;
            lastCycleResult = "Running";
        }
    }

    public void MarkCycleCompleted(
        DateTime utcNow,
        int durationMs,
        bool successful,
        int companies,
        string? safeErrorCode = null)
    {
        lock (sync)
        {
            lastCycleCompletedAtUtc = utcNow;
            lastCycleDurationMs = durationMs;
            enabledCompanyCount = Math.Max(0, companies);
            lastCycleResult = successful ? "Succeeded" : "Failed";
            if (successful)
            {
                lastSuccessfulCycleAtUtc = utcNow;
                lastSafeErrorCode = null;
                lastSafeErrorMessage = null;
            }
            else
            {
                lastSafeErrorCode = safeErrorCode ?? "SAP_WORKER_CYCLE_FAILED";
                lastSafeErrorMessage = "El ciclo del scheduler SAP no se completo.";
            }
        }
    }

    public void SetCurrent(
        int companyId,
        string companyCode,
        string profileCode,
        string entityCode)
    {
        lock (sync)
        {
            currentCompanyId = companyId;
            currentCompanyCode = SanitizeTelemetry(companyCode, 50);
            currentJob = SanitizeTelemetry($"{profileCode}/{entityCode}", 300);
            activeLeaseCount++;
        }
    }

    public void ClearCurrent()
    {
        lock (sync)
        {
            currentCompanyId = null;
            currentCompanyCode = null;
            currentJob = null;
            activeLeaseCount = Math.Max(0, activeLeaseCount - 1);
        }
    }

    public void MarkStopping()
    {
        lock (sync)
        {
            canSchedule = false;
            lifecycleState = WorkerLifecycleStates.Stopping;
        }
    }

    public void MarkStopped()
    {
        lock (sync)
        {
            canSchedule = false;
            lifecycleState = WorkerLifecycleStates.Stopped;
            currentCompanyId = null;
            currentCompanyCode = null;
            currentJob = null;
            activeLeaseCount = 0;
        }
    }

    public void MarkFaulted()
    {
        lock (sync)
        {
            canSchedule = false;
            lifecycleState = WorkerLifecycleStates.Faulted;
            lastSafeErrorCode = "SAP_WORKER_FATAL";
            lastSafeErrorMessage = "El scheduler SAP finalizo por una falla operativa.";
        }
    }

    public SapSyncWorkerRuntimeSnapshot Snapshot()
    {
        lock (sync)
        {
            return new SapSyncWorkerRuntimeSnapshot(
                lifecycleState,
                canSchedule,
                startedAtUtc,
                lastCycleStartedAtUtc,
                lastCycleCompletedAtUtc,
                lastSuccessfulCycleAtUtc,
                lastCycleDurationMs,
                lastCycleResult,
                lastSafeErrorCode,
                lastSafeErrorMessage,
                enabledCompanyCount,
                activeLeaseCount,
                currentCompanyId,
                currentCompanyCode,
                currentJob);
        }
    }

    public static string CreateStorageKey(string hostName, string workerInstance)
    {
        var hash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes($"{hostName}|{workerInstance}")));
        return $"SAP-{hash[..32]}";
    }

    public static string ResolveVersion(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        var informational = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
        return !string.IsNullOrWhiteSpace(informational)
            ? informational.Trim()
            : assembly.GetName().Version?.ToString() ?? "unknown";
    }

    public static string NormalizeInstance(string? value)
    {
        var normalized = string.IsNullOrWhiteSpace(value)
            ? "NuanSystem-SapSyncWorker"
            : value.Trim();
        return SanitizeTelemetry(normalized, 120) ?? "NuanSystem-SapSyncWorker";
    }

    public static string? SanitizeTelemetry(string? value, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var safe = new string(value
            .Trim()
            .Where(character =>
                char.IsLetterOrDigit(character)
                || character is '-' or '_' or '.' or '/' or ':')
            .Take(maximumLength)
            .ToArray());
        return string.IsNullOrWhiteSpace(safe) ? null : safe;
    }
}

public sealed record SapSyncWorkerRuntimeSnapshot(
    string LifecycleState,
    bool CanSchedule,
    DateTime? StartedAtUtc,
    DateTime? LastCycleStartedAtUtc,
    DateTime? LastCycleCompletedAtUtc,
    DateTime? LastSuccessfulCycleAtUtc,
    int? LastCycleDurationMs,
    string? LastCycleResult,
    string? LastSafeErrorCode,
    string? LastSafeErrorMessage,
    int EnabledCompanyCount,
    int ActiveLeaseCount,
    int? CurrentCompanyId,
    string? CurrentCompanyCode,
    string? CurrentJob);
