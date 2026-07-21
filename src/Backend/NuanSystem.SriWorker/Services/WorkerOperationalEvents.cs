using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace NuanSystem.SriWorker.Services;

public static class WorkerOperationalEvent
{
    public const string WorkerStarted = "SRI_WORKER_STARTED";
    public const string WorkerDisabled = "SRI_WORKER_DISABLED";
    public const string WorkerStopping = "SRI_WORKER_STOPPING";
    public const string WorkerStopped = "SRI_WORKER_STOPPED";
    public const string CycleFailed = "SRI_WORKER_CYCLE_FAILED";
    public const string WorkerFaulted = "SRI_WORKER_FAULTED";
}

public interface IWorkerOperationalEventPublisher
{
    void Publish(string eventCode, string safeMessage, bool writeToEventLog, bool critical = false);
}

public sealed class WorkerOperationalEventPublisher(IOptions<WorkerEventLogOptions> options,
    ILogger<WorkerOperationalEventPublisher> logger) : IWorkerOperationalEventPublisher
{
    public void Publish(string eventCode, string safeMessage, bool writeToEventLog, bool critical = false)
    {
        logger.Log(critical ? LogLevel.Critical : LogLevel.Information,
            "WorkerEvent EventCode={EventCode} Message={SafeMessage}", eventCode, safeMessage);
        var current = options.Value;
        if (!writeToEventLog || !current.Enabled || !OperatingSystem.IsWindows()) return;
        try
        {
            if (!EventLog.SourceExists(current.SourceName)) return;
            EventLog.WriteEntry(current.SourceName, $"{eventCode}: {safeMessage}", critical ? EventLogEntryType.Error : EventLogEntryType.Information, current.CriticalEventId);
        }
        catch (Exception exception) { logger.LogWarning("No se pudo escribir en Windows Event Log. ErrorType={ErrorType}", exception.GetType().Name); }
    }
}

public sealed class WorkerEventLogOptions
{
    public const string SectionName = "WorkerEventLog";
    public bool Enabled { get; init; }
    public string SourceName { get; init; } = "NuanSystem.SriWorker";
    public int CriticalEventId { get; init; } = 5501;
}
