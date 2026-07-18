using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed class ImportItemsFromSapCommandHandler(
    ICompanyContext companyContext,
    ISapItemImportService itemImportService)
    : ICommandHandler<ImportItemsFromSapCommand, SapItemImportResultDto>
{
    public async Task<Result<SapItemImportResultDto>> Handle(
        ImportItemsFromSapCommand request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<SapItemImportResultDto>.Failure(
                "No hay empresa activa para importar articulos SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de sincronizar articulos.", "X-Company-Code")]);
        }

        var summary = await itemImportService.ImportAsync(
            companyContext.CurrentCompany!.CompanyId,
            request.SapItemCodes,
            request.AuditUserId,
            request.AuditUserName,
            true,
            cancellationToken);

        var message = summary.Failed > 0
            ? "La importacion de articulos SAP finalizo con errores en algunos registros."
            : "La importacion de articulos SAP finalizo correctamente.";

        return Result<SapItemImportResultDto>.Success(summary, message);
    }
}
