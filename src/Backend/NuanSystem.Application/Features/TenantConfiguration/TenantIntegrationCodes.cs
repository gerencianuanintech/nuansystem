namespace NuanSystem.Application.Features.TenantConfiguration;

public static class TenantIntegrationCodes
{
    public const string SapB1 = "SAP_B1";
    public const string Sri = "SRI";
    public const string Qlik = "QLIK";
    public const string ExternalApi = "EXTERNAL_API";

    public static readonly IReadOnlyCollection<string> All =
    [
        SapB1,
        Sri,
        Qlik,
        ExternalApi
    ];
}

