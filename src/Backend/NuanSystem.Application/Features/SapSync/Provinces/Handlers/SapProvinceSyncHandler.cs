using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Provinces.Contracts;
using NuanSystem.Application.Features.SapSync.Provinces.Services;

namespace NuanSystem.Application.Features.SapSync.Provinces.Handlers;

public sealed class SapProvinceSyncHandler(
    ISapProvinceReader reader,
    SapProvinceRecordProcessor recordProcessor) : ISapSyncEntityHandler
{
    public string EntityCode => SapSyncEntityCode.Provinces;

    public async Task<SapSyncExecutionResult> ImportFromSapAsync(
        SapSyncExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        var rows = await reader.GetProvincesAsync(context.CompanyId, cancellationToken);
        var results = new List<SapProvinceRecordProcessResult>(rows.Count);

        foreach (var row in rows
                     .OrderBy(item => item.CountryCode, StringComparer.OrdinalIgnoreCase)
                     .ThenBy(item => item.ProvinceCode, StringComparer.OrdinalIgnoreCase))
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                results.Add(await recordProcessor.ProcessAsync(
                    SapProvinceSnapshot.FromRecord(row), null, "SAP Sync Worker", cancellationToken));
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                results.Add(new(
                    SapSyncExecutionDetailActions.Skip,
                    SapSyncExecutionDetailStatuses.Failed,
                    null,
                    null,
                    SapProvinceResultCodes.SaveFailed,
                    $"No fue posible importar la provincia: {exception.GetType().Name}."));
            }
        }

        if (rows.Count == 0)
        {
            return SapSyncExecutionResult.Skipped("No hay provincias SAP para procesar.");
        }

        var failed = Count(results, SapSyncExecutionDetailStatuses.Failed);
        var warnings = Count(results, SapSyncExecutionDetailStatuses.ApprovalRequired)
            + Count(results, SapSyncExecutionDetailStatuses.Conflict)
            + Count(results, SapSyncExecutionDetailStatuses.Skipped);
        var message = $"Provincias SAP procesadas. Leidas: {rows.Count}, creadas: {Count(results, SapSyncExecutionDetailStatuses.Created)}, actualizadas: {Count(results, SapSyncExecutionDetailStatuses.Updated)}, sin cambios: {Count(results, SapSyncExecutionDetailStatuses.Unchanged)}, aprobacion/conflictos/omitidas: {warnings}, fallidas: {failed}.";

        return new SapSyncExecutionResult(
            failed > 0 ? SapSyncStatus.Failed : SapSyncStatus.Synced,
            message,
            rows.Count,
            failed,
            failed > 0 ? "PROVINCE_IMPORT_FAILED" : null,
            failed > 0 ? "Una o mas provincias no pudieron importarse." : null);
    }

    public Task<SapSyncExecutionResult> ExportToSapAsync(
        SapSyncExecutionContext context,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(SapSyncExecutionResult.NotImplemented(
            "Provincias ERP a SAP no forman parte del alcance aprobado."));

    private static int Count(IEnumerable<SapProvinceRecordProcessResult> results, string status) =>
        results.Count(item => item.Status == status);
}
