using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Countries.Contracts;
using NuanSystem.Application.Features.SapSync.Countries.Services;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Executions;

namespace NuanSystem.Application.Features.SapSync.Countries.Handlers;

public sealed class SapCountrySyncHandler(
    ISapCountryReader reader,
    SapCountryRecordProcessor recordProcessor) : ISapSyncEntityHandler
{
    public string EntityCode => SapSyncEntityCode.Countries;

    public async Task<SapSyncExecutionResult> ImportFromSapAsync(
        SapSyncExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var rows = await reader.GetCountriesAsync(context.CompanyId, cancellationToken);
        var results = new List<SapCountryRecordProcessResult>(rows.Count);

        foreach (var row in rows.OrderBy(item => item.CountryCode, StringComparer.OrdinalIgnoreCase))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                results.Add(await recordProcessor.ProcessAsync(
                    SapCountrySnapshot.FromRecord(row), null, "SAP Sync Worker", cancellationToken));
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                results.Add(new(
                    SapSyncExecutionDetailActions.Skip,
                    SapSyncExecutionDetailStatuses.Failed,
                    null,
                    null,
                    SapCountryResultCodes.SaveFailed,
                    $"No fue posible importar el pais: {exception.GetType().Name}."));
            }
        }

        if (rows.Count == 0)
        {
            return SapSyncExecutionResult.Skipped("No hay paises SAP para procesar.");
        }

        var failed = Count(results, SapSyncExecutionDetailStatuses.Failed);
        var warnings = Count(results, SapSyncExecutionDetailStatuses.ApprovalRequired)
            + Count(results, SapSyncExecutionDetailStatuses.Conflict)
            + Count(results, SapSyncExecutionDetailStatuses.Skipped);
        var message = $"Paises SAP procesados. Leidos: {rows.Count}, creados: {Count(results, SapSyncExecutionDetailStatuses.Created)}, actualizados: {Count(results, SapSyncExecutionDetailStatuses.Updated)}, sin cambios: {Count(results, SapSyncExecutionDetailStatuses.Unchanged)}, aprobacion/conflictos/omitidos: {warnings}, fallidos: {failed}.";

        return new SapSyncExecutionResult(
            failed > 0 ? SapSyncStatus.Failed : SapSyncStatus.Synced,
            message,
            rows.Count,
            failed,
            failed > 0 ? "COUNTRY_IMPORT_FAILED" : null,
            failed > 0 ? "Uno o mas paises no pudieron importarse." : null);
    }

    public Task<SapSyncExecutionResult> ExportToSapAsync(
        SapSyncExecutionContext context,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(SapSyncExecutionResult.NotImplemented(
            "Paises ERP a SAP no forman parte del alcance aprobado."));

    private static int Count(IEnumerable<SapCountryRecordProcessResult> results, string status) =>
        results.Count(item => item.Status == status);
}
