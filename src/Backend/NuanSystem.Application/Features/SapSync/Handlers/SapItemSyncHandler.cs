using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Handlers;

public sealed class SapItemSyncHandler(ISapItemImportService itemImportService) : ISapSyncEntityHandler
{
    public string EntityCode => SapSyncEntityCode.Items;
    public async Task<SapSyncExecutionResult> ImportFromSapAsync(SapSyncExecutionContext context, CancellationToken cancellationToken = default)
    {
        var summary = await itemImportService.ImportAsync(
            context.CompanyId, null, null, "SAP Sync Worker", false, cancellationToken);

        if (summary.Selected == 0 && summary.Failed == 0)
        {
            return SapSyncExecutionResult.Skipped("No hay articulos SAP para procesar.");
        }

        return new SapSyncExecutionResult(
            summary.Failed > 0 ? Enums.SapSyncStatus.Failed : Enums.SapSyncStatus.Synced,
            $"Articulos SAP procesados. Leidos: {summary.Selected}, creados: {summary.Created}, actualizados: {summary.Updated}, sin cambios: {summary.Unchanged}, conflictos/omitidos: {summary.Skipped}, fallidos: {summary.Failed}.",
            summary.Selected,
            summary.Failed,
            summary.Failed > 0 ? "ITEM_IMPORT_FAILED" : null,
            summary.Failed > 0 ? "Uno o mas articulos no pudieron importarse." : null);
    }
    public Task<SapSyncExecutionResult> ExportToSapAsync(SapSyncExecutionContext context, CancellationToken cancellationToken = default)
        => Task.FromResult(SapSyncExecutionResult.NotImplemented("Items ERP a SAP queda pendiente."));
}
