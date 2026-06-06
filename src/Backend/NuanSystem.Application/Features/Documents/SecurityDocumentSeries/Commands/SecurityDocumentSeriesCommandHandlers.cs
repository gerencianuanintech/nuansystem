using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Dtos;
using NuanSystem.Shared.Responses;
using static NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Commands.SecurityDocumentSeriesCommandHelpers;

namespace NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Commands;

public sealed class CreateSecurityDocumentSeriesCommandHandler(
    ISecurityDocumentSeriesRepository seriesRepository)
    : ICommandHandler<CreateSecurityDocumentSeriesCommand, SecurityDocumentSeriesDto>
{
    public async Task<Result<SecurityDocumentSeriesDto>> Handle(
        CreateSecurityDocumentSeriesCommand request,
        CancellationToken cancellationToken)
    {
        var normalized = Normalize(request);

        var validation = await ValidateDuplicatesAsync(
            seriesRepository,
            null,
            normalized.Code,
            normalized.DocumentType,
            normalized.Prefix,
            normalized.Establishment,
            normalized.EmissionPoint,
            cancellationToken);

        if (!validation.IsSuccess)
        {
            return Result<SecurityDocumentSeriesDto>.Failure(validation.Message, validation.Errors);
        }

        var id = await seriesRepository.CreateAsync(
            new CreateSecurityDocumentSeriesData(
                normalized.DocumentType,
                normalized.Code,
                normalized.Name,
                normalized.Description,
                normalized.Prefix,
                normalized.Establishment,
                normalized.EmissionPoint,
                request.InitialNumber,
                request.CurrentNumber,
                request.NextNumber,
                request.NumberLength,
                normalized.SapObjectType,
                request.SapSeriesId,
                normalized.SapSeriesName,
                request.IsDefault,
                request.IsActive,
                request.IsSapIntegrationActive,
                request.AuditUserId,
                NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        var series = await seriesRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("La serie de documento fue creada pero no pudo consultarse.");

        return Result<SecurityDocumentSeriesDto>.Success(series, "Serie de documento creada correctamente.");
    }

    private static NormalizedSeries Normalize(CreateSecurityDocumentSeriesCommand request)
    {
        return new NormalizedSeries(
            NormalizeCode(request.DocumentType),
            NormalizeCode(request.Code),
            request.Name.Trim(),
            NormalizeOptional(request.Description),
            NormalizeCode(request.Prefix),
            NormalizeCode(request.Establishment),
            NormalizeCode(request.EmissionPoint),
            NormalizeOptional(request.SapObjectType),
            NormalizeOptional(request.SapSeriesName));
    }
}

public sealed class UpdateSecurityDocumentSeriesCommandHandler(
    ISecurityDocumentSeriesRepository seriesRepository)
    : ICommandHandler<UpdateSecurityDocumentSeriesCommand, SecurityDocumentSeriesDto>
{
    public async Task<Result<SecurityDocumentSeriesDto>> Handle(
        UpdateSecurityDocumentSeriesCommand request,
        CancellationToken cancellationToken)
    {
        var current = await seriesRepository.GetByIdAsync(request.Id, cancellationToken);
        if (current is null)
        {
            return Result<SecurityDocumentSeriesDto>.Failure("La serie de documento no existe.");
        }

        var normalized = Normalize(request);

        var validation = await ValidateDuplicatesAsync(
            seriesRepository,
            request.Id,
            normalized.Code,
            normalized.DocumentType,
            normalized.Prefix,
            normalized.Establishment,
            normalized.EmissionPoint,
            cancellationToken);

        if (!validation.IsSuccess)
        {
            return Result<SecurityDocumentSeriesDto>.Failure(validation.Message, validation.Errors);
        }

        if (current.CurrentNumber > 0 && NumberingWasChanged(current, request))
        {
            return Result<SecurityDocumentSeriesDto>.Failure(
                "No se puede modificar la numeracion de una serie que ya tiene documentos reservados.",
                new[]
                {
                    new ApiError(
                        "DocumentSeriesNumberingLocked",
                        "La serie ya tiene numeracion en uso.",
                        nameof(request.CurrentNumber))
                });
        }

        var updated = await seriesRepository.UpdateAsync(
            new UpdateSecurityDocumentSeriesData(
                request.Id,
                normalized.DocumentType,
                normalized.Code,
                normalized.Name,
                normalized.Description,
                normalized.Prefix,
                normalized.Establishment,
                normalized.EmissionPoint,
                request.InitialNumber,
                request.CurrentNumber,
                request.NextNumber,
                request.NumberLength,
                normalized.SapObjectType,
                request.SapSeriesId,
                normalized.SapSeriesName,
                request.IsDefault,
                request.IsActive,
                request.IsSapIntegrationActive,
                request.AuditUserId,
                NormalizeOptional(request.AuditUserName)),
            cancellationToken);

        if (!updated)
        {
            return Result<SecurityDocumentSeriesDto>.Failure("La serie de documento no existe o fue eliminada.");
        }

        var series = await seriesRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("La serie de documento fue actualizada pero no pudo consultarse.");

        return Result<SecurityDocumentSeriesDto>.Success(series, "Serie de documento actualizada correctamente.");
    }

    private static bool NumberingWasChanged(SecurityDocumentSeriesDto current, UpdateSecurityDocumentSeriesCommand request)
    {
        return current.InitialNumber != request.InitialNumber
            || current.CurrentNumber != request.CurrentNumber
            || current.NextNumber != request.NextNumber
            || current.NumberLength != request.NumberLength
            || !string.Equals(current.Prefix, NormalizeCode(request.Prefix), StringComparison.OrdinalIgnoreCase)
            || !string.Equals(current.Establishment, NormalizeCode(request.Establishment), StringComparison.OrdinalIgnoreCase)
            || !string.Equals(current.EmissionPoint, NormalizeCode(request.EmissionPoint), StringComparison.OrdinalIgnoreCase);
    }

    private static NormalizedSeries Normalize(UpdateSecurityDocumentSeriesCommand request)
    {
        return new NormalizedSeries(
            NormalizeCode(request.DocumentType),
            NormalizeCode(request.Code),
            request.Name.Trim(),
            NormalizeOptional(request.Description),
            NormalizeCode(request.Prefix),
            NormalizeCode(request.Establishment),
            NormalizeCode(request.EmissionPoint),
            NormalizeOptional(request.SapObjectType),
            NormalizeOptional(request.SapSeriesName));
    }
}

public sealed class DeleteSecurityDocumentSeriesCommandHandler(
    ISecurityDocumentSeriesRepository seriesRepository)
    : ICommandHandler<DeleteSecurityDocumentSeriesCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeleteSecurityDocumentSeriesCommand request,
        CancellationToken cancellationToken)
    {
        var deleted = await seriesRepository.DeleteAsync(
            request.Id,
            request.AuditUserId,
            NormalizeOptional(request.AuditUserName),
            cancellationToken);

        return deleted
            ? Result<bool>.Success(true, "Serie de documento eliminada correctamente.")
            : Result<bool>.Failure("La serie de documento no existe o ya fue eliminada.");
    }
}

public sealed class ReserveSecurityDocumentNumberCommandHandler(
    ISecurityDocumentNumberingService numberingService)
    : ICommandHandler<ReserveSecurityDocumentNumberCommand, ReserveSecurityDocumentNumberResult>
{
    public async Task<Result<ReserveSecurityDocumentNumberResult>> Handle(
        ReserveSecurityDocumentNumberCommand request,
        CancellationToken cancellationToken)
    {
        var result = await numberingService.ReserveNumberAsync(
            request.Id,
            request.AuditUserId,
            NormalizeOptional(request.AuditUserName),
            cancellationToken);

        return result.Success
            ? Result<ReserveSecurityDocumentNumberResult>.Success(result, result.Message)
            : Result<ReserveSecurityDocumentNumberResult>.Failure(result.Message);
    }
}

internal sealed record NormalizedSeries(
    string DocumentType,
    string Code,
    string Name,
    string? Description,
    string Prefix,
    string Establishment,
    string EmissionPoint,
    string? SapObjectType,
    string? SapSeriesName);

internal sealed record DocumentSeriesValidationResult(
    bool IsSuccess,
    string Message,
    IReadOnlyCollection<ApiError> Errors)
{
    public static DocumentSeriesValidationResult Success()
    {
        return new DocumentSeriesValidationResult(true, string.Empty, Array.Empty<ApiError>());
    }

    public static DocumentSeriesValidationResult Failure(string message, IReadOnlyCollection<ApiError> errors)
    {
        return new DocumentSeriesValidationResult(false, message, errors);
    }
}

internal static class SecurityDocumentSeriesCommandHelpers
{
    internal static string NormalizeCode(string value)
    {
        return value.Trim().ToUpperInvariant();
    }

    internal static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    internal static async Task<DocumentSeriesValidationResult> ValidateDuplicatesAsync(
        ISecurityDocumentSeriesRepository seriesRepository,
        int? excludedId,
        string code,
        string documentType,
        string prefix,
        string establishment,
        string emissionPoint,
        CancellationToken cancellationToken)
    {
        if (await seriesRepository.ExistsByCodeAsync(code, excludedId, cancellationToken))
        {
            return DocumentSeriesValidationResult.Failure(
                "Ya existe una serie de documento con el codigo indicado.",
                new[]
                {
                    new ApiError(
                        "DocumentSeriesCodeAlreadyExists",
                        "El codigo de la serie ya existe.",
                        "Code")
                });
        }

        if (await seriesRepository.ExistsBySeriesKeyAsync(
            documentType,
            prefix,
            establishment,
            emissionPoint,
            excludedId,
            cancellationToken))
        {
            return DocumentSeriesValidationResult.Failure(
                "Ya existe una serie para el tipo de documento, prefijo, establecimiento y punto de emision.",
                new[]
                {
                    new ApiError(
                        "DocumentSeriesKeyAlreadyExists",
                        "La clave operativa de la serie ya existe.",
                        "DocumentType")
                });
        }

        return DocumentSeriesValidationResult.Success();
    }
}
