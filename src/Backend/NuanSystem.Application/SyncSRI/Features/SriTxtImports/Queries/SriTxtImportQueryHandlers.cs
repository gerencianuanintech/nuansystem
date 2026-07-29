using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SriDocuments;
using NuanSystem.Application.Features.SriTxtImports.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SriTxtImports.Queries;

public sealed class GetSriTxtImportsQueryHandler(ISriTxtImportRepository repository)
    : IQueryHandler<GetSriTxtImportsQuery, SriTxtImportPageDto>
{
    public async Task<Result<SriTxtImportPageDto>> Handle(
        GetSriTxtImportsQuery request,
        CancellationToken cancellationToken)
    {
        var filter = request.Filter with
        {
            Status = SriTxtImportStatusCodes.NormalizeOptional(request.Filter.Status),
            FileName = string.IsNullOrWhiteSpace(request.Filter.FileName) ? null : request.Filter.FileName.Trim(),
            Environment = string.IsNullOrWhiteSpace(request.Filter.Environment)
                ? null
                : SriEnvironmentCodes.Normalize(request.Filter.Environment)
        };
        return Result<SriTxtImportPageDto>.Success(await repository.SearchAsync(filter, cancellationToken));
    }
}

public sealed class GetSriTxtImportByIdQueryHandler(ISriTxtImportRepository repository)
    : IQueryHandler<GetSriTxtImportByIdQuery, SriTxtImportDetailDto>
{
    public async Task<Result<SriTxtImportDetailDto>> Handle(
        GetSriTxtImportByIdQuery request,
        CancellationToken cancellationToken)
    {
        var value = await repository.GetByIdAsync(request.ImportId, cancellationToken);
        return value is null
            ? Result<SriTxtImportDetailDto>.Failure(
                "Importación TXT SRI no encontrada.",
                [new ApiError("SRI_TXT_IMPORT_NOT_FOUND", "La importación no existe.", "ImportId")])
            : Result<SriTxtImportDetailDto>.Success(value);
    }
}

public sealed class GetSriTxtImportRowsQueryHandler(ISriTxtImportRepository repository)
    : IQueryHandler<GetSriTxtImportRowsQuery, SriTxtImportRowPageDto>
{
    public async Task<Result<SriTxtImportRowPageDto>> Handle(
        GetSriTxtImportRowsQuery request,
        CancellationToken cancellationToken)
    {
        var filter = request.Filter with { Validity = SriTxtRowValidityCodes.Normalize(request.Filter.Validity) };
        var value = await repository.GetRowsAsync(request.ImportId, filter, cancellationToken);
        return value is null
            ? Result<SriTxtImportRowPageDto>.Failure(
                "Importación TXT SRI no encontrada.",
                [new ApiError("SRI_TXT_IMPORT_NOT_FOUND", "La importación no existe.", "ImportId")])
            : Result<SriTxtImportRowPageDto>.Success(value);
    }
}
