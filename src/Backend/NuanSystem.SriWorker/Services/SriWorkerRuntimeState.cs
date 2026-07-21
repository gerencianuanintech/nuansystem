using System.Security.Cryptography;
using System.Text;
using NuanSystem.Application.Features.Operations;

namespace NuanSystem.SriWorker.Services;

public interface ISriWorkerExecutionGate
{
    bool CanClaim { get; }
    void StopClaims();
}

public sealed class SriWorkerRuntimeState : ISriWorkerExecutionGate
{
    private readonly object sync = new();
    private string lifecycle = WorkerLifecycleStates.Starting;
    private DateTime? startedAt;
    private DateTime? cycleStartedAt;
    private DateTime? cycleCompletedAt;
    private DateTime? lastSuccessAt;
    private int? durationMs;
    private string? result;
    private string? errorCode;
    private string? errorMessage;
    private bool canClaim = true;

    public bool CanClaim { get { lock (sync) return canClaim; } }
    public void StopClaims() { lock (sync) { canClaim = false; lifecycle = WorkerLifecycleStates.Stopping; } }
    public void MarkDisabled() { lock (sync) { canClaim = false; lifecycle = WorkerLifecycleStates.Disabled; } }
    public void MarkStarted(DateTime now) { lock (sync) { lifecycle = WorkerLifecycleStates.Running; startedAt ??= now; errorCode = errorMessage = null; } }
    public void MarkCycleStarted(DateTime now) { lock (sync) { cycleStartedAt = now; cycleCompletedAt = null; result = "Running"; } }
    public void MarkCycleCompleted(DateTime now, int elapsedMs, bool successful, string? safeCode = null)
    {
        lock (sync)
        {
            cycleCompletedAt = now; durationMs = elapsedMs; result = successful ? "Succeeded" : "Failed";
            if (successful) { lastSuccessAt = now; errorCode = errorMessage = null; }
            else { errorCode = safeCode ?? "SRI_WORKER_CYCLE_FAILED"; errorMessage = "El ciclo del SRI Worker no se completo."; }
        }
    }
    public void MarkFaulted() { lock (sync) { lifecycle = WorkerLifecycleStates.Faulted; canClaim = false; errorCode = "SRI_WORKER_FATAL"; errorMessage = "El SRI Worker finalizo por una falla operativa."; } }
    public void MarkStopped() { lock (sync) { lifecycle = WorkerLifecycleStates.Stopped; canClaim = false; } }
    public SriRuntimeSnapshot Snapshot() { lock (sync) return new(lifecycle, canClaim, startedAt, cycleStartedAt, cycleCompletedAt, lastSuccessAt, durationMs, result, errorCode, errorMessage); }

    public static string StorageKey(string host, string instance)
    {
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{host}|{instance}")));
        return $"SRI-{hash[..32]}";
    }
}

public sealed record SriRuntimeSnapshot(string LifecycleState, bool CanClaim, DateTime? StartedAtUtc,
    DateTime? LastCycleStartedAtUtc, DateTime? LastCycleCompletedAtUtc, DateTime? LastSuccessfulCycleAtUtc,
    int? LastCycleDurationMs, string? LastCycleResult, string? LastSafeErrorCode, string? LastSafeErrorMessage);

public sealed class SriSingleInstanceGuard : IDisposable
{
    private Mutex? mutex;
    public void Acquire(string identity)
    {
        var suffix = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identity)))[..24];
        mutex = new Mutex(true, $"Global\\NuanSystem.SriWorker.{suffix}", out var createdNew);
        if (!createdNew) { mutex.Dispose(); mutex=null; throw new InvalidOperationException("Ya existe una instancia local del SRI Worker para esta identidad."); }
    }
    public void Dispose() { mutex?.ReleaseMutex(); mutex?.Dispose(); mutex = null; }
}
