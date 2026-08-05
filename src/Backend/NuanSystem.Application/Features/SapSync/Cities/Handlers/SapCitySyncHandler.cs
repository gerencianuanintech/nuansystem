using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Cities.Contracts;
using NuanSystem.Application.Features.SapSync.Cities.Services;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Executions;

namespace NuanSystem.Application.Features.SapSync.Cities.Handlers;

public sealed class SapCitySyncHandler(ISapCityReader reader, SapCityRecordProcessor recordProcessor)
    : ISapSyncEntityHandler
{
    public string EntityCode => SapSyncEntityCode.Cities;

    public async Task<SapSyncExecutionResult> ImportFromSapAsync(
        SapSyncExecutionContext context, CancellationToken cancellationToken = default)
    {
        var rows = await reader.GetCitiesAsync(context.CompanyId, cancellationToken);
        var results = new List<SapCityRecordProcessResult>(rows.Count);
        foreach (var row in rows.OrderBy(x => x.CountryCode, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(x => x.ProvinceCode, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(x => x.CityCode, StringComparer.OrdinalIgnoreCase))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                results.Add(await recordProcessor.ProcessAsync(
                    SapCitySnapshot.FromRecord(row), null, "SAP Sync Worker", cancellationToken));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                results.Add(new(SapSyncExecutionDetailActions.Skip,
                    SapSyncExecutionDetailStatuses.Failed, null, null,
                    SapCityResultCodes.SaveFailed,
                    $"No fue posible importar la ciudad: {ex.GetType().Name}."));
            }
        }
        if (rows.Count == 0)
            return SapSyncExecutionResult.Skipped("No hay ciudades SAP para procesar.");
        var failed = Count(results, SapSyncExecutionDetailStatuses.Failed);
        var warnings = Count(results, SapSyncExecutionDetailStatuses.ApprovalRequired)
            + Count(results, SapSyncExecutionDetailStatuses.Conflict)
            + Count(results, SapSyncExecutionDetailStatuses.Skipped);
        return new(failed > 0 ? SapSyncStatus.Failed : SapSyncStatus.Synced,
            $"Ciudades SAP procesadas. Leidas: {rows.Count}, creadas: {Count(results, SapSyncExecutionDetailStatuses.Created)}, actualizadas: {Count(results, SapSyncExecutionDetailStatuses.Updated)}, sin cambios: {Count(results, SapSyncExecutionDetailStatuses.Unchanged)}, aprobacion/conflictos/omitidas: {warnings}, fallidas: {failed}.",
            rows.Count, failed, failed > 0 ? "CITY_IMPORT_FAILED" : null,
            failed > 0 ? "Una o mas ciudades no pudieron importarse." : null);
    }

    public Task<SapSyncExecutionResult> ExportToSapAsync(
        SapSyncExecutionContext context, CancellationToken cancellationToken = default) =>
        Task.FromResult(SapSyncExecutionResult.NotImplemented(
            "Ciudades ERP a SAP no forman parte del alcance aprobado."));

    private static int Count(IEnumerable<SapCityRecordProcessResult> rows, string status) => rows.Count(x => x.Status == status);
}
