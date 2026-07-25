namespace NuanSystem.MasterBranchSyncWorker.Options;

public sealed class MasterBranchSyncWorkerOptions
{
    public const string SectionName = "MasterBranchSyncWorker";

    public bool Enabled { get; init; } = false;
    public string WorkerInstance { get; init; } = Environment.MachineName;
    public int BatchSize { get; init; } = 50;
    public int LockMinutes { get; init; } = 5;
    public int EmptyQueueDelaySeconds { get; init; } = 10;
    public int ErrorDelaySeconds { get; init; } = 30;
    public bool SkeletonMode { get; init; } = true;
    public SkeletonModeBehavior SkeletonModeBehavior { get; init; } = SkeletonModeBehavior.ObserveOnly;
    public string[] EnabledEntityAppliers { get; init; } = [];
    public LocalOutboxRelayOptions LocalOutboxRelay { get; init; } = new();
    public MasterBranchSyncWorkerDiagnosticsOptions Diagnostics { get; init; } = new();

    public int NormalizedBatchSize => Math.Clamp(BatchSize, 1, 500);
    public TimeSpan LockDuration => TimeSpan.FromMinutes(Math.Clamp(LockMinutes, 1, 240));
    public TimeSpan EmptyQueueDelay => TimeSpan.FromSeconds(Math.Clamp(EmptyQueueDelaySeconds, 1, 3600));
    public TimeSpan ErrorDelay => TimeSpan.FromSeconds(Math.Clamp(ErrorDelaySeconds, 1, 3600));
    public string NormalizedWorkerInstance => string.IsNullOrWhiteSpace(WorkerInstance)
        ? Environment.MachineName
        : WorkerInstance.Trim();

    public bool IsEntityApplierEnabled(string entityName)
    {
        return EnabledEntityAppliers.Any(enabled =>
            string.Equals(enabled?.Trim(), entityName, StringComparison.OrdinalIgnoreCase));
    }
}

public sealed class LocalOutboxRelayOptions
{
    public bool Enabled { get; init; } = false;
    public int BatchSize { get; init; } = 25;
    public int LeaseMinutes { get; init; } = 5;
    public int RetryDelaySeconds { get; init; } = 30;

    public int NormalizedBatchSize => Math.Clamp(BatchSize, 1, 500);
    public TimeSpan LeaseDuration => TimeSpan.FromMinutes(Math.Clamp(LeaseMinutes, 1, 240));
    public TimeSpan RetryDelay => TimeSpan.FromSeconds(Math.Clamp(RetryDelaySeconds, 1, 86400));
}

public sealed class MasterBranchSyncWorkerDiagnosticsOptions
{
    public bool SqlConnectionDiagnostics { get; init; }
    public bool OpenMasterConnectionAndExit { get; init; }
    public bool ReleaseExpiredLocksAndExit { get; init; }
}
