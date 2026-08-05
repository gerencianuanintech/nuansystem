using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Provinces.Contracts;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Provinces.Queries;

public sealed class PreviewProvincesFromSapQueryHandler(
    ICompanyContext companyContext,
    ISapProvinceImportService provinceImportService)
    : IQueryHandler<PreviewProvincesFromSapQuery, IReadOnlyCollection<SapProvincePreviewItemDto>>
{
    public async Task<Result<IReadOnlyCollection<SapProvincePreviewItemDto>>> Handle(
        PreviewProvincesFromSapQuery request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<IReadOnlyCollection<SapProvincePreviewItemDto>>.Failure(
                "No hay empresa activa para consultar provincias SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de consultar SAP.", "X-Company-Code")]);
        }

        var preview = await provinceImportService.PreviewAsync(
            companyContext.CurrentCompany!.CompanyId,
            cancellationToken);

        return Result<IReadOnlyCollection<SapProvincePreviewItemDto>>.Success(preview);
    }
}
