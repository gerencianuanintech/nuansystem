using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed class SendDocumentToSapCommandHandler(ISapDocumentSender sapDocumentSender)
    : ICommandHandler<SendDocumentToSapCommand, SapSendResultDto>
{
    public async Task<Result<SapSendResultDto>> Handle(
        SendDocumentToSapCommand request,
        CancellationToken cancellationToken)
    {
        var result = await sapDocumentSender.SendDocumentAsync(request.DocumentId, cancellationToken);

        return result.Success
            ? Result<SapSendResultDto>.Success(result, "Documento enviado a SAP correctamente.")
            : Result<SapSendResultDto>.Failure(result.ErrorMessage ?? "No se pudo enviar el documento a SAP.");
    }
}
