using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.SapIntegration.Documents;

public sealed class SapDocumentSender : ISapDocumentSender
{
    public Task<Result<SapSendResultDto>> SendAsync(
        long documentId,
        string? documentType,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default)
    {
        var result = Result<SapSendResultDto>.Failure(
            "El envio directo de documentos a SAP aun no esta implementado.",
            [
                new ApiError(
                    "SAP_DOCUMENT_SENDER_NOT_IMPLEMENTED",
                    "El documento debe procesarse mediante la cola de sincronizacion SAP configurada.",
                    nameof(documentId))
            ]);

        return Task.FromResult(result);
    }
}
