namespace NuanSystem.SyncWorker.Options;

public sealed class ServiceLayerWorkerOptions
{
    public const string SectionName = "ServiceLayer";

    public int HttpTimeoutSeconds { get; set; } = 100;
    public int SessionRenewBeforeMinutes { get; set; } = 5;
    public bool IgnoreSslErrors { get; set; }
}
