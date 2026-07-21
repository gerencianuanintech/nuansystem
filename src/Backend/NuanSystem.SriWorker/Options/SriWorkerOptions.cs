namespace NuanSystem.SriWorker.Options;

public sealed class SriWorkerOptions
{
    public const string SectionName = "SriWorker";
    public bool Enabled { get; init; }
    public string WorkerInstance { get; init; } = Environment.MachineName;
    public int BatchSize { get; init; } = 10;
    public int MaxConcurrency { get; init; } = 2;
    public int LeaseSeconds { get; init; } = 120;
    public int EmptyQueueDelaySeconds { get; init; } = 10;
    public int ErrorDelaySeconds { get; init; } = 30;
    public int MaxAttempts { get; init; } = 5;
    public int NotFoundMaxAttempts { get; init; } = 3;
    public int NotFoundWindowMinutes { get; init; } = 30;
    public int BaseRetrySeconds { get; init; } = 30;
    public int MaxRetrySeconds { get; init; } = 900;
    public double RetryJitterRatio { get; init; } = 0.20;

    public string NormalizedWorkerInstance => string.IsNullOrWhiteSpace(WorkerInstance) ? Environment.MachineName : WorkerInstance.Trim();
    public TimeSpan EmptyQueueDelay => TimeSpan.FromSeconds(Math.Clamp(EmptyQueueDelaySeconds, 1, 3600));
    public TimeSpan ErrorDelay => TimeSpan.FromSeconds(Math.Clamp(ErrorDelaySeconds, 1, 3600));
}

public sealed class SriProviderOptions
{
    public const string SectionName = "SriProvider";
    public string TestAuthorizationUrl { get; init; } = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline";
    public string ProductionAuthorizationUrl { get; init; } = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline";
    public int TimeoutSeconds { get; init; } = 30;
    public int MaxXmlBytes { get; init; } = 5 * 1024 * 1024;

    public Uri GetEndpoint(string environment) => environment switch
    {
        var value when value.Equals("Test", StringComparison.OrdinalIgnoreCase) => new Uri(TestAuthorizationUrl, UriKind.Absolute),
        var value when value.Equals("Production", StringComparison.OrdinalIgnoreCase) => new Uri(ProductionAuthorizationUrl, UriKind.Absolute),
        _ => throw new ArgumentOutOfRangeException(nameof(environment), environment, "Ambiente SRI no soportado.")
    };

    public static bool IsOfficialEndpoint(string? value, string expectedHost) =>
        Uri.TryCreate(value, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps &&
        uri.Host.Equals(expectedHost, StringComparison.OrdinalIgnoreCase) &&
        uri.AbsolutePath.Equals("/comprobantes-electronicos-ws/AutorizacionComprobantesOffline", StringComparison.Ordinal);
}
