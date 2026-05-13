using NuanSystem.Domain.Tenancy;
using NuanSystem.SapIntegration.Abstractions;
using NuanSystem.SapIntegration.Clients.DiApi;
using NuanSystem.SapIntegration.Clients.ServiceLayer;

namespace NuanSystem.SapIntegration.Clients;

public sealed class SapClientFactory(
    SapServiceLayerClient serviceLayerClient,
    SapDiApiClient diApiClient) : ISapClientFactory
{
    public ISapClient Create(SapIntegrationMode mode)
    {
        return mode switch
        {
            SapIntegrationMode.ServiceLayer => serviceLayerClient,
            SapIntegrationMode.DiApi => diApiClient,
            SapIntegrationMode.None => throw new InvalidOperationException("La empresa no tiene integracion SAP activa."),
            _ => throw new NotSupportedException($"Modo SAP no soportado: {mode}.")
        };
    }
}
