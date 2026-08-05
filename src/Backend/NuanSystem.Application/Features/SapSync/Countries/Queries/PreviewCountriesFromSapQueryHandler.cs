using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Countries.Contracts;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Countries.Queries;

public sealed class PreviewCountriesFromSapQueryHandler(
    ICompanyContext companyContext,
    ISapCountryImportService countryImportService)
    : IQueryHandler<PreviewCountriesFromSapQuery, IReadOnlyCollection<SapCountryPreviewItemDto>>
{
    public async Task<Result<IReadOnlyCollection<SapCountryPreviewItemDto>>> Handle(
        PreviewCountriesFromSapQuery request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<IReadOnlyCollection<SapCountryPreviewItemDto>>.Failure(
                "No hay empresa activa para consultar paises SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de consultar SAP.", "X-Company-Code")]);
        }

        var preview = await countryImportService.PreviewAsync(
            companyContext.CurrentCompany!.CompanyId,
            cancellationToken);

        return Result<IReadOnlyCollection<SapCountryPreviewItemDto>>.Success(preview);
    }
}
