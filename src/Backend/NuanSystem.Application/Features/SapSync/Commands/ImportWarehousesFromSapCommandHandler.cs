using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed class ImportWarehousesFromSapCommandHandler(
    ICompanyContext companyContext,
    ISapWarehouseImportService warehouseImportService)
    : ICommandHandler<ImportWarehousesFromSapCommand, SapWarehouseImportResultDto>
{
    public async Task<Result<SapWarehouseImportResultDto>> Handle(
        ImportWarehousesFromSapCommand request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<SapWarehouseImportResultDto>.Failure(
                "No hay empresa activa para importar bodegas SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de sincronizar bodegas.", "X-Company-Code")]);
        }

        var summary = await warehouseImportService.ImportAsync(
            companyContext.CurrentCompany!.CompanyId,
            request.Mappings,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);

        var message = summary.Failed > 0
            ? "La importacion de bodegas SAP finalizo con errores en algunos registros."
            : "La importacion de bodegas SAP finalizo correctamente.";

        return Result<SapWarehouseImportResultDto>.Success(summary, message);
    }
}
