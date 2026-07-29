using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SriDocuments.Services;
using NuanSystem.Application.Features.SriTxtImports.Dtos;
using NuanSystem.Application.Features.SriTxtImports.Services;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SriTxtImports.Commands;

public sealed class UploadSriTxtImportCommandHandler(
    ISriTxtFileParser parser,
    ISriTxtImportRepository repository)
    : ICommandHandler<UploadSriTxtImportCommand, SriTxtImportDetailDto>
{
    public async Task<Result<SriTxtImportDetailDto>> Handle(
        UploadSriTxtImportCommand request,
        CancellationToken cancellationToken)
    {
        SriTxtParsedFile parsed;
        try
        {
            parsed = await parser.ParseAsync(request.Content, cancellationToken);
        }
        catch (SriTxtParseException exception)
        {
            return Result<SriTxtImportDetailDto>.Failure(
                exception.Message,
                [new ApiError(exception.Code, exception.Message, exception.Field)]);
        }

        var result = await repository.RegisterValidatedAsync(
            new RegisterValidatedSriTxtImportData(
                Guid.NewGuid(),
                Path.GetFileName(request.OriginalFileName.Replace('\\', '/')),
                parsed.FileSha256,
                request.FileSizeBytes,
                parsed.EncodingCode,
                parsed.HeaderLine,
                parsed.Rows,
                request.TraceId,
                request.AuditUserId,
                NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        return Result<SriTxtImportDetailDto>.Success(
            result.Import,
            result.IsCreated
                ? "El archivo TXT fue registrado y validado correctamente."
                : "El contenido del archivo ya estaba registrado; se devolvio la carga existente.");
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed class EnqueueSriTxtImportCommandHandler(
    ISriDocumentQueuePolicy queuePolicy,
    ISriTxtImportRepository repository)
    : ICommandHandler<EnqueueSriTxtImportCommand, SriTxtImportDetailDto>
{
    public async Task<Result<SriTxtImportDetailDto>> Handle(
        EnqueueSriTxtImportCommand request,
        CancellationToken cancellationToken)
    {
        var environments = await repository.GetStagedEnvironmentsAsync(request.ImportId, cancellationToken);
        foreach (var environment in environments.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var policy = await queuePolicy.ValidateEnqueueAsync(environment, cancellationToken);
            if (!policy.IsSuccess)
            {
                return Result<SriTxtImportDetailDto>.Failure(policy.Message, policy.Errors);
            }
        }

        var result = await repository.EnqueueAsync(
            new EnqueueSriTxtImportData(
                request.ImportId,
                request.RowVersion,
                request.TraceId,
                request.AuditUserId,
                NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        return result.Code switch
        {
            SriTxtImportEnqueueCode.NotFound => Failure(
                "SRI_TXT_IMPORT_NOT_FOUND",
                "La carga TXT no existe.",
                "ImportId"),
            SriTxtImportEnqueueCode.ConcurrencyConflict => Failure(
                "SRI_TXT_IMPORT_CONCURRENCY_CONFLICT",
                "La carga TXT fue modificada por otro proceso. Recargue e intente nuevamente.",
                "RowVersion"),
            SriTxtImportEnqueueCode.InvalidState => Failure(
                "SRI_TXT_IMPORT_INVALID_STATE",
                "El estado de la carga no permite encolar sus documentos.",
                "Status"),
            _ => Result<SriTxtImportDetailDto>.Success(
                result.Import!,
                "La accion de encolado termino de forma idempotente.")
        };
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static Result<SriTxtImportDetailDto> Failure(string code, string message, string field) =>
        Result<SriTxtImportDetailDto>.Failure(message, [new ApiError(code, message, field)]);
}
