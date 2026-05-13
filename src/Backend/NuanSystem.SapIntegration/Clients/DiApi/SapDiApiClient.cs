using System.Text.Json;
using NuanSystem.SapIntegration.Abstractions;
using NuanSystem.SapIntegration.Documents;

namespace NuanSystem.SapIntegration.Clients.DiApi;

public sealed class SapDiApiClient : ISapClient
{
    public Task<SapClientResult> SendDocumentAsync(
        SapDocumentPayload document,
        CancellationToken cancellationToken = default)
    {
        var requestJson = JsonSerializer.Serialize(document);

        return Task.FromResult(new SapClientResult(
            false,
            "Failed",
            "DI API requiere instalacion/configuracion COM de SAP Business One y aun no esta implementada en este entorno.",
            requestJson,
            null,
            null,
            null));
    }
}
