namespace NuanSystem.SapIntegration.ServiceLayer;

public sealed class SapServiceLayerTransportOptions
{
    public const string SectionName = "ServiceLayer";

    public int HttpTimeoutSeconds { get; init; } = 100;
    public bool IgnoreSslErrors { get; init; }
}
