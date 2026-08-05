using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Cities.Contracts;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Cities.Queries;

public sealed class PreviewCitiesFromSapQueryHandler(
    ICompanyContext companyContext,
    ISapCityImportService cityImportService)
    : IQueryHandler<PreviewCitiesFromSapQuery, IReadOnlyCollection<SapCityPreviewItemDto>>
{
    public async Task<Result<IReadOnlyCollection<SapCityPreviewItemDto>>> Handle(
        PreviewCitiesFromSapQuery request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<IReadOnlyCollection<SapCityPreviewItemDto>>.Failure(
                "No hay empresa activa para consultar ciudades SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de consultar SAP.", "X-Company-Code")]);
        }

        var preview = await cityImportService.PreviewAsync(
            companyContext.CurrentCompany!.CompanyId,
            cancellationToken);
        return Result<IReadOnlyCollection<SapCityPreviewItemDto>>.Success(preview);
    }
}
