using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Handlers;

public sealed class SapSupplierSyncHandler(ISapSupplierImportService supplierImportService) : ISapSyncEntityHandler
{
    public string EntityCode => SapSyncEntityCode.Suppliers;

    public async Task<SapSyncExecutionResult> ImportFromSapAsync(SapSyncExecutionContext context, CancellationToken cancellationToken = default)
    {
        var summary = await supplierImportService.ImportAsync(
            context.CompanyId,
            new SapSupplierImportOptions(
                AuditUserId: null,
                AuditUserName: "SAP Sync Worker",
                WritePublicSapLog: false,
                WriteInbox: true,
                UseIncrementalWatermark: true,
                context.WorkerInstance,
                context.CorrelationId),
            cancellationToken);

        return new SapSyncExecutionResult(
            summary.Failed > 0 ? Enums.SapSyncStatus.Failed : Enums.SapSyncStatus.Synced,
            $"Proveedores SAP procesados. Leidos: {summary.TotalRead}, creados: {summary.Created}, actualizados: {summary.Updated}, sin cambios: {summary.Unchanged}, conflictos/omitidos: {summary.Skipped}, fallidos: {summary.Failed}.",
            summary.TotalRead,
            summary.Failed,
            summary.Failed > 0 ? "SUPPLIER_IMPORT_FAILED" : null,
            summary.Failed > 0 ? "Uno o mas proveedores no pudieron importarse." : null);
    }

    public Task<SapSyncExecutionResult> ExportToSapAsync(SapSyncExecutionContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(SapSyncExecutionResult.NotImplemented("Exportacion de proveedores a SAP queda pendiente para fase 3."));
}
