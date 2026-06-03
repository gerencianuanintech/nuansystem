using System.Diagnostics;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class SapSyncOrchestrator(
    IEnumerable<ISapSyncEntityHandler> handlers,
    ISapSyncLockService lockService,
    ISapSyncLogService logService)
    : ISapSyncOrchestrator
{
    public async Task<SapSyncExecutionResult> ExecuteAsync(
        SapSyncCompanyDto company,
        SapSyncEntitySettingsDto settings,
        SapSyncDirection direction,
        string workerInstance,
        TimeSpan lockTimeout,
        CancellationToken cancellationToken = default)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        var startedAt = DateTime.UtcNow;
        var stopwatch = Stopwatch.StartNew();
        var operation = direction == SapSyncDirection.ErpToSap ? SapSyncOperation.Export : SapSyncOperation.Import;
        var context = new SapSyncExecutionContext(company.CompanyId, company.CompanyCode, settings.EntityCode, direction, operation, workerInstance, correlationId, 0, startedAt);
        SapSyncLockDto? syncLock = null;

        try
        {
            syncLock = await lockService.TryAcquireAsync(company.CompanyId, settings.EntityCode, direction, workerInstance, correlationId, lockTimeout, cancellationToken);
            if (syncLock is null)
            {
                var skipped = SapSyncExecutionResult.Skipped("Ya existe un lock vigente para esta empresa, entidad y direccion.");
                await WriteLogAsync(context, skipped, stopwatch.ElapsedMilliseconds, cancellationToken);
                return skipped;
            }

            var handler = handlers.FirstOrDefault(item => string.Equals(item.EntityCode, settings.EntityCode, StringComparison.OrdinalIgnoreCase));
            var result = handler is null
                ? SapSyncExecutionResult.NotImplemented($"No existe handler SAP para la entidad {settings.EntityCode}.")
                : direction switch
                {
                    SapSyncDirection.SapToErp => await handler.ImportFromSapAsync(context, cancellationToken),
                    SapSyncDirection.ErpToSap => await handler.ExportToSapAsync(context, cancellationToken),
                    SapSyncDirection.Both => await handler.ImportFromSapAsync(context with { Direction = SapSyncDirection.SapToErp, Operation = SapSyncOperation.Import }, cancellationToken),
                    _ => SapSyncExecutionResult.Skipped($"Direccion SAP no soportada: {direction}.")
                };

            await WriteLogAsync(context, result, stopwatch.ElapsedMilliseconds, cancellationToken);
            return result;
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            var failed = SapSyncExecutionResult.Failed("La ejecucion SAP fallo de forma controlada.", exception.GetType().Name, exception.Message);
            await WriteLogAsync(context, failed, stopwatch.ElapsedMilliseconds, cancellationToken);
            return failed;
        }
        finally
        {
            if (syncLock is not null)
            {
                await lockService.ReleaseAsync(syncLock, cancellationToken);
            }
        }
    }

    private Task WriteLogAsync(SapSyncExecutionContext context, SapSyncExecutionResult result, long durationMs, CancellationToken cancellationToken)
    {
        return logService.WriteAsync(
            new SapSyncLogWriteDto(
                context.CompanyId,
                context.CompanyCode,
                context.EntityCode,
                context.Direction,
                context.Operation,
                result.Status,
                context.CorrelationId,
                context.WorkerInstance,
                context.AttemptCount,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                result.ErrorCode,
                result.ErrorMessage ?? result.Message,
                durationMs,
                context.StartedAtUtc,
                DateTime.UtcNow),
            cancellationToken);
    }
}
