using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Cities.Contracts;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Cities.Commands;

public sealed class ImportCitiesFromSapCommandHandler(
    ICompanyContext companyContext,
    ISapCityImportService cityImportService)
    : ICommandHandler<ImportCitiesFromSapCommand, SapCityImportResultDto>
{
    public async Task<Result<SapCityImportResultDto>> Handle(
        ImportCitiesFromSapCommand request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<SapCityImportResultDto>.Failure(
                "No hay empresa activa para importar ciudades SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de sincronizar ciudades.", "X-Company-Code")]);
        }

        var summary = await cityImportService.ImportAsync(
            companyContext.CurrentCompany!.CompanyId,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);

        var message = summary.Failed > 0
            ? "La importacion de ciudades SAP finalizo con errores en algunos registros."
            : "La importacion de ciudades SAP finalizo correctamente.";

        return Result<SapCityImportResultDto>.Success(summary, message);
    }
}
