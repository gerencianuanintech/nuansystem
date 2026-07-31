using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Warehouses.Contracts;
using NuanSystem.Application.Features.SapSync.Warehouses.Services;

namespace NuanSystem.Application.Features.SapSync.Warehouses.Handlers;

public sealed class SapWarehouseSyncHandler(
    ISapWarehouseReader reader,
    SapWarehouseRecordProcessor recordProcessor) : ISapSyncEntityHandler
{
    public string EntityCode => SapSyncEntityCode.Warehouses;

    public async Task<SapSyncExecutionResult> ImportFromSapAsync(
        SapSyncExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var rows = await reader.GetWarehousesAsync(context.CompanyId, cancellationToken);
        var results = new List<SapWarehouseRecordProcessResult>(rows.Count);
        foreach (var row in rows.OrderBy(item => item.WarehouseCode, StringComparer.OrdinalIgnoreCase))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                results.Add(await recordProcessor.ProcessAsync(
                    SapWarehouseSnapshot.FromRecord(row), null, "SAP Sync Worker", cancellationToken));
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                results.Add(new(
                    SapSyncExecutionDetailActions.Skip,
                    SapSyncExecutionDetailStatuses.Failed,
                    null,
                    null,
                    SapWarehouseResultCodes.SaveFailed,
                    $"No fue posible importar la bodega: {exception.GetType().Name}."));
            }
        }

        var failed = results.Count(item => item.Status == SapSyncExecutionDetailStatuses.Failed);
        var warnings = results.Count(item => item.Status is
            SapSyncExecutionDetailStatuses.ApprovalRequired or
            SapSyncExecutionDetailStatuses.Conflict or
            SapSyncExecutionDetailStatuses.Skipped);
        var message = $"Bodegas SAP procesadas. Leidas: {rows.Count}, creadas: {Count(results, SapSyncExecutionDetailStatuses.Created)}, actualizadas: {Count(results, SapSyncExecutionDetailStatuses.Updated)}, sin cambios: {Count(results, SapSyncExecutionDetailStatuses.Unchanged)}, aprobacion/conflictos/omitidas: {warnings}, fallidas: {failed}.";

        if (rows.Count == 0)
        {
            return SapSyncExecutionResult.Skipped("No hay bodegas SAP para procesar.");
        }

        return new SapSyncExecutionResult(
            failed > 0 ? SapSyncStatus.Failed : SapSyncStatus.Synced,
            message,
            rows.Count,
            failed,
            failed > 0 ? "WAREHOUSE_IMPORT_FAILED" : null,
            failed > 0 ? "Una o mas bodegas no pudieron importarse." : null);
    }

    public Task<SapSyncExecutionResult> ExportToSapAsync(
        SapSyncExecutionContext context,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(SapSyncExecutionResult.NotImplemented(
            "Bodegas ERP a SAP no forman parte del alcance aprobado."));

    private static int Count(
        IEnumerable<SapWarehouseRecordProcessResult> results,
        string status) => results.Count(item => item.Status == status);
}
