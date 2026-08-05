using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Countries.Contracts;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Countries.Commands;

public sealed class ImportCountriesFromSapCommandHandler(
    ICompanyContext companyContext,
    ISapCountryImportService countryImportService)
    : ICommandHandler<ImportCountriesFromSapCommand, SapCountryImportResultDto>
{
    public async Task<Result<SapCountryImportResultDto>> Handle(
        ImportCountriesFromSapCommand request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<SapCountryImportResultDto>.Failure(
                "No hay empresa activa para importar paises SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de sincronizar paises.", "X-Company-Code")]);
        }

        var summary = await countryImportService.ImportAsync(
            companyContext.CurrentCompany!.CompanyId,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);

        var message = summary.Failed > 0
            ? "La importacion de paises SAP finalizo con errores en algunos registros."
            : "La importacion de paises SAP finalizo correctamente.";

        return Result<SapCountryImportResultDto>.Success(summary, message);
    }
}
