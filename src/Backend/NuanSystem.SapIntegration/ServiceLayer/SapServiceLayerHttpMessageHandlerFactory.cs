namespace NuanSystem.SapIntegration.ServiceLayer;

internal static class SapServiceLayerHttpMessageHandlerFactory
{
    public static HttpClientHandler Create(bool ignoreSslErrors)
    {
        var handler = new HttpClientHandler
        {
            // SAP sessions are scoped explicitly per request to avoid cross-company cookie reuse.
            UseCookies = false
        };

        if (ignoreSslErrors)
        {
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        }

        return handler;
    }
}
