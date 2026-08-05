using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Provinces.Contracts;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Provinces.Commands;

public sealed class ImportProvincesFromSapCommandHandler(
    ICompanyContext companyContext,
    ISapProvinceImportService provinceImportService)
    : ICommandHandler<ImportProvincesFromSapCommand, SapProvinceImportResultDto>
{
    public async Task<Result<SapProvinceImportResultDto>> Handle(
        ImportProvincesFromSapCommand request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<SapProvinceImportResultDto>.Failure(
                "No hay empresa activa para importar provincias SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de sincronizar provincias.", "X-Company-Code")]);
        }

        var summary = await provinceImportService.ImportAsync(
            companyContext.CurrentCompany!.CompanyId,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);

        var message = summary.Failed > 0
            ? "La importacion de provincias SAP finalizo con errores en algunos registros."
            : "La importacion de provincias SAP finalizo correctamente.";

        return Result<SapProvinceImportResultDto>.Success(summary, message);
    }
}
