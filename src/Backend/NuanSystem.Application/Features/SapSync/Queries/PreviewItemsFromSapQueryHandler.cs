using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Queries;

public sealed class PreviewItemsFromSapQueryHandler(
    ICompanyContext companyContext,
    ISapItemImportService itemImportService)
    : IQueryHandler<PreviewItemsFromSapQuery, IReadOnlyCollection<SapItemPreviewItemDto>>
{
    public async Task<Result<IReadOnlyCollection<SapItemPreviewItemDto>>> Handle(
        PreviewItemsFromSapQuery request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<IReadOnlyCollection<SapItemPreviewItemDto>>.Failure(
                "No hay empresa activa para consultar articulos SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de consultar SAP.", "X-Company-Code")]);
        }

        var preview = await itemImportService.PreviewAsync(
            companyContext.CurrentCompany!.CompanyId,
            Math.Clamp(request.Take, 1, 1000),
            request.Search,
            cancellationToken);

        return Result<IReadOnlyCollection<SapItemPreviewItemDto>>.Success(preview);
    }
}
