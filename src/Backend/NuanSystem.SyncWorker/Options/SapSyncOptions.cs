namespace NuanSystem.SyncWorker.Options;

public sealed class SapSyncOptions
{
    public const string SectionName = "SapSync";

    public int DefaultBatchSize { get; set; } = 100;
    public int DefaultMaxRetryCount { get; set; } = 3;
    public int LockTimeoutMinutes { get; set; } = 10;
    public int LockRenewalSeconds { get; set; } = 60;
    public int ExecutionTimeoutMinutes { get; set; } = 15;
    public int SchedulerPageSize { get; set; } = 50;
}
