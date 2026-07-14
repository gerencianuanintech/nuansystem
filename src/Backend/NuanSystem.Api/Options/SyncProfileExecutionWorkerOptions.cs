namespace NuanSystem.Api.Options;

public sealed class SyncProfileExecutionWorkerOptions
{
    public const string SectionName = "SyncProfileExecutionWorker";

    public bool Enabled { get; set; } = true;
    public int PollingSeconds { get; set; } = 30;
}
