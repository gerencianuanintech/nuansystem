namespace NuanSystem.SyncWorker.Options;

public sealed class RetryOptions
{
    public const string SectionName = "Retry";

    public bool Enabled { get; set; } = true;
    public int IntervalSeconds { get; set; } = 60;
    public int MaxRetryCount { get; set; } = 3;
    public int BackoffSeconds { get; set; } = 30;
}
