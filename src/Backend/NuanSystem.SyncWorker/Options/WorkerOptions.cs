namespace NuanSystem.SyncWorker.Options;

public sealed class WorkerOptions
{
    public const string SectionName = "Worker";

    public bool Enabled { get; set; } = true;
    public string InstanceName { get; set; } = Environment.MachineName;
    public int LoopDelaySeconds { get; set; } = 30;
    public int MaxParallelCompanies { get; set; } = 2;
    public int MaxParallelJobsPerCompany { get; set; } = 2;
    public int ShutdownTimeoutSeconds { get; set; } = 30;
}
