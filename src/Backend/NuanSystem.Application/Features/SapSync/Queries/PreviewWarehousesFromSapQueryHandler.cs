using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Queries;

public sealed class PreviewWarehousesFromSapQueryHandler(
    ICompanyContext companyContext,
    ISapWarehouseImportService warehouseImportService)
    : IQueryHandler<PreviewWarehousesFromSapQuery, IReadOnlyCollection<SapWarehousePreviewItemDto>>
{
    public async Task<Result<IReadOnlyCollection<SapWarehousePreviewItemDto>>> Handle(
        PreviewWarehousesFromSapQuery request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<IReadOnlyCollection<SapWarehousePreviewItemDto>>.Failure(
                "No hay empresa activa para consultar bodegas SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de consultar SAP.", "X-Company-Code")]);
        }

        var preview = await warehouseImportService.PreviewAsync(
            companyContext.CurrentCompany!.CompanyId,
            cancellationToken);

        return Result<IReadOnlyCollection<SapWarehousePreviewItemDto>>.Success(preview);
    }
}
