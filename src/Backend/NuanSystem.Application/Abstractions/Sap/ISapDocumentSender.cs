using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapDocumentSender
{
    Task<SapSendResultDto> SendDocumentAsync(
        long documentId,
        CancellationToken cancellationToken = default);
}
