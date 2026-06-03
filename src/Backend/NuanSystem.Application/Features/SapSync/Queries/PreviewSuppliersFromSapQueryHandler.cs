using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Queries;

public sealed class PreviewSuppliersFromSapQueryHandler(
    ICompanyContext companyContext,
    ISapSupplierImportService supplierImportService)
    : IQueryHandler<PreviewSuppliersFromSapQuery, IReadOnlyCollection<SapSupplierPreviewItemDto>>
{
    public async Task<Result<IReadOnlyCollection<SapSupplierPreviewItemDto>>> Handle(
        PreviewSuppliersFromSapQuery request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<IReadOnlyCollection<SapSupplierPreviewItemDto>>.Failure(
                "No hay empresa activa para consultar proveedores SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de consultar SAP.", "X-Company-Code")]);
        }

        var preview = await supplierImportService.PreviewAsync(companyContext.CurrentCompany!.CompanyId, cancellationToken);
        return Result<IReadOnlyCollection<SapSupplierPreviewItemDto>>.Success(preview);
    }
}
