using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed class ImportSuppliersFromSapCommandHandler(
    ICompanyContext companyContext,
    ISapSupplierImportService supplierImportService)
    : ICommandHandler<ImportSuppliersFromSapCommand, SapSupplierImportResultDto>
{
    public async Task<Result<SapSupplierImportResultDto>> Handle(
        ImportSuppliersFromSapCommand request,
        CancellationToken cancellationToken)
    {
        if (!companyContext.HasActiveCompany)
        {
            return Result<SapSupplierImportResultDto>.Failure(
                "No hay empresa activa para importar proveedores SAP.",
                [new ApiError("COMPANY_REQUIRED", "Seleccione una empresa antes de sincronizar proveedores.", "X-Company-Code")]);
        }

        var summary = await supplierImportService.ImportAsync(
            companyContext.CurrentCompany!.CompanyId,
            new SapSupplierImportOptions(
                request.AuditUserId,
                request.AuditUserName,
                WritePublicSapLog: true,
                WriteInbox: false,
                UseIncrementalWatermark: false,
                WorkerInstance: "ManualEndpoint",
                CorrelationId: Guid.NewGuid().ToString("N")),
            cancellationToken);

        var message = summary.Failed > 0
            ? "La sincronizacion de proveedores SAP finalizo con errores en algunos registros."
            : "La sincronizacion de proveedores SAP finalizo correctamente.";

        return Result<SapSupplierImportResultDto>.Success(summary, message);
    }
}
