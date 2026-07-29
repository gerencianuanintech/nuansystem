using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SriDocuments.Dtos;
using NuanSystem.Application.Features.SriDocuments.Services;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SriDocuments.Commands;

public sealed class EnqueueSriDocumentCommandHandler(ISriDocumentQueuePolicy policy, ISriDocumentQueueRepository repository) : ICommandHandler<EnqueueSriDocumentCommand, SriDocumentQueueDetailDto>
{
    public async Task<Result<SriDocumentQueueDetailDto>> Handle(EnqueueSriDocumentCommand request, CancellationToken cancellationToken)
    {
        var environment = SriEnvironmentCodes.Normalize(request.Environment);
        var policyResult = await policy.ValidateEnqueueAsync(environment, cancellationToken);
        if (!policyResult.IsSuccess) return Result<SriDocumentQueueDetailDto>.Failure(policyResult.Message, policyResult.Errors);

        var result = await repository.EnqueueAsync(new EnqueueSriDocumentData(
            environment,
            request.AccessKey.Trim(),
            SriAccessKey.GetDocumentType(request.AccessKey.Trim()),
            SriSourceTypeCodes.Normalize(request.SourceType),
            request.SourceReference.Trim(),
            NormalizeOptional(request.BranchCode),
            request.Priority,
            request.TraceId ?? Guid.NewGuid(),
            request.AuditUserId,
            NormalizeOptional(request.AuditUserName)), cancellationToken);

        return Result<SriDocumentQueueDetailDto>.Success(
            result.Queue,
            result.IsCreated ? "Consulta SRI encolada correctamente." : "La consulta SRI ya estaba encolada; se devolvio el registro existente.");
    }

    internal static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class CancelSriDocumentCommandHandler(ISriDocumentQueueRepository repository) : ICommandHandler<CancelSriDocumentCommand, SriDocumentQueueDetailDto>
{
    public async Task<Result<SriDocumentQueueDetailDto>> Handle(CancelSriDocumentCommand request, CancellationToken cancellationToken)
    {
        var action = await repository.CancelAsync(new SriDocumentQueueActionData(request.Id, request.RowVersion, EnqueueSriDocumentCommandHandler.NormalizeOptional(request.Reason), request.AuditUserId, EnqueueSriDocumentCommandHandler.NormalizeOptional(request.AuditUserName)), cancellationToken);
        return await SriQueueActionResult.ResolveAsync(action, request.Id, repository, "Consulta SRI cancelada correctamente.", cancellationToken);
    }
}

public sealed class ReprocessSriDocumentCommandHandler(ISriDocumentQueuePolicy policy, ISriDocumentQueueRepository repository) : ICommandHandler<ReprocessSriDocumentCommand, SriDocumentQueueDetailDto>
{
    public async Task<Result<SriDocumentQueueDetailDto>> Handle(ReprocessSriDocumentCommand request, CancellationToken cancellationToken)
    {
        var current = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (current is null) return SriQueueActionResult.NotFound(request.Id);
        var policyResult = await policy.ValidateEnqueueAsync(current.Environment, cancellationToken);
        if (!policyResult.IsSuccess) return Result<SriDocumentQueueDetailDto>.Failure(policyResult.Message, policyResult.Errors);

        var action = await repository.ReprocessAsync(new SriDocumentQueueActionData(request.Id, request.RowVersion, request.Reason.Trim(), request.AuditUserId, EnqueueSriDocumentCommandHandler.NormalizeOptional(request.AuditUserName)), cancellationToken);
        return await SriQueueActionResult.ResolveAsync(action, request.Id, repository, "Consulta SRI enviada nuevamente a pendientes.", cancellationToken);
    }
}

file static class SriQueueActionResult
{
    public static async Task<Result<SriDocumentQueueDetailDto>> ResolveAsync(SriDocumentQueueActionCode action, long id, ISriDocumentQueueRepository repository, string message, CancellationToken cancellationToken)
    {
        if (action == SriDocumentQueueActionCode.NotFound) return NotFound(id);
        if (action == SriDocumentQueueActionCode.ConcurrencyConflict)
            return Failure("SRI_QUEUE_CONCURRENCY_CONFLICT", "La consulta SRI fue modificada por otro proceso. Recargue e intente nuevamente.", "RowVersion");
        if (action == SriDocumentQueueActionCode.InvalidState)
            return Failure("SRI_QUEUE_INVALID_STATE", "El estado actual no permite ejecutar esta accion.", "Status");
        var detail = await repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("La cola SRI fue actualizada pero no pudo consultarse.");
        return Result<SriDocumentQueueDetailDto>.Success(detail, message);
    }

    public static Result<SriDocumentQueueDetailDto> NotFound(long id) => Failure("SRI_QUEUE_NOT_FOUND", $"La consulta SRI {id} no existe.", "Id");
    private static Result<SriDocumentQueueDetailDto> Failure(string code, string message, string field) => Result<SriDocumentQueueDetailDto>.Failure(message, [new ApiError(code, message, field)]);
}

public sealed class DownloadAuthorizedSriXmlCommandHandler(ISriDocumentQueueRepository repository) : ICommandHandler<DownloadAuthorizedSriXmlCommand, SriAuthorizedXmlDownloadDto>
{
    public async Task<Result<SriAuthorizedXmlDownloadDto>> Handle(DownloadAuthorizedSriXmlCommand request, CancellationToken cancellationToken)
    {
        var row = await repository.DownloadAuthorizedXmlAsync(new SriAuthorizedXmlDownloadData(request.QueueId, request.AuditUserId, EnqueueSriDocumentCommandHandler.NormalizeOptional(request.AuditUserName), request.TraceId), cancellationToken);
        if (row.Code == SriAuthorizedXmlDownloadCode.NotFound) return Failure("SRI_DOCUMENT_NOT_FOUND", "El documento SRI no existe.");
        if (row.Code == SriAuthorizedXmlDownloadCode.NotAuthorized) return Failure("SRI_DOCUMENT_NOT_AUTHORIZED", "El documento no se encuentra autorizado.");
        if (row.Code == SriAuthorizedXmlDownloadCode.MissingContent || row.DocumentId is null || row.XmlContent.Length == 0) return Failure("SRI_DOCUMENT_XML_MISSING", "El documento autorizado no tiene contenido XML disponible.");

        return Result<SriAuthorizedXmlDownloadDto>.Success(new SriAuthorizedXmlDownloadDto(row.DocumentId.Value, row.QueueId, row.XmlContent, row.ContentType ?? "application/xml", $"sri-{row.QueueId}.xml", row.SizeBytes));
    }

    private static Result<SriAuthorizedXmlDownloadDto> Failure(string code, string message) => Result<SriAuthorizedXmlDownloadDto>.Failure(message, [new ApiError(code, message, "QueueId")]);
}
