using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed class SendDocumentToSapCommandHandler(ISapDocumentSender sapDocumentSender)
    : ICommandHandler<SendDocumentToSapCommand, SapSendResultDto>
{
    public async Task<Result<SapSendResultDto>> Handle(
        SendDocumentToSapCommand request,
        CancellationToken cancellationToken)
    {
        if (request.DocumentId <= 0)
        {
            return Result<SapSendResultDto>.Failure(
                "El documento a sincronizar no es valido.",
                [new ApiError("SAP_DOCUMENT_INVALID_ID", "El identificador del documento debe ser mayor a cero.", nameof(request.DocumentId))]);
        }

        return await sapDocumentSender.SendAsync(
            request.DocumentId,
            Normalize(request.DocumentType),
            request.AuditUserId,
            Normalize(request.AuditUserName),
            cancellationToken);
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
