using NuanSystem.SapIntegration.Documents;

namespace NuanSystem.SapIntegration.Abstractions;

public interface ISapClient
{
    Task<SapClientResult> SendDocumentAsync(
        SapDocumentPayload document,
        CancellationToken cancellationToken = default);
}
