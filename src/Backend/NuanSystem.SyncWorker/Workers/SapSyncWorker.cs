using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.Operations;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Operations;
using NuanSystem.Application.Features.SapSync.Scheduling;
using NuanSystem.SyncWorker.Options;
using NuanSystem.SyncWorker.Services;

namespace NuanSystem.SyncWorker.Workers;

public sealed class SapSyncWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<WorkerOptions> workerOptions,
    IOptions<SapSyncOptions> sapSyncOptions,
    SapSyncWorkerRuntimeState runtime,
    ILogger<SapSyncWorker> logger) : BackgroundService
{
    private readonly WorkerOptions worker = workerOptions.Value;
    private readonly SapSyncOptions sap = sapSyncOptions.Value;
    private readonly string hostName = Environment.MachineName;
    private readonly string workerInstance =
        SapSyncWorkerRuntimeState.NormalizeInstance(workerOptions.Value.InstanceName);
    private SapSyncScheduleCursor cursor = SapSyncScheduleCursor.Start;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ReportHeartbeatSafelyAsync(stoppingToken);
        if (!worker.Enabled)
        {
            runtime.MarkDisabled();
            await ReportHeartbeatSafelyAsync(stoppingToken);
            logger.LogInformation("Scheduler SAP deshabilitado por configuracion.");
            return;
        }

        runtime.MarkStarted(DateTime.UtcNow);
        await ReportHeartbeatSafelyAsync(stoppingToken);
        logger.LogInformation(
            "Scheduler SAP iniciado. WorkerInstance={WorkerInstance}.",
            workerInstance);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var stopwatch = Stopwatch.StartNew();
                runtime.MarkCycleStarted(DateTime.UtcNow);
                try
                {
                    var poll = await PollAsync(stoppingToken);
                    cursor = poll.NextCursor;
                    await ProcessAsync(poll, stoppingToken);
                    stopwatch.Stop();
                    runtime.MarkCycleCompleted(
                        DateTime.UtcNow,
                        ToMilliseconds(stopwatch),
                        successful: true,
                        poll.EnabledCompanyCount);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    stopwatch.Stop();
                    runtime.MarkCycleCompleted(
                        DateTime.UtcNow,
                        ToMilliseconds(stopwatch),
                        successful: false,
                        runtime.Snapshot().EnabledCompanyCount,
                        "SAP_WORKER_CYCLE_FAILED");
                    logger.LogError(
                        "Fallo controlado del ciclo SAP. ErrorType={ErrorType}.",
                        exception.GetType().Name);
                }

                await ReportHeartbeatSafelyAsync(stoppingToken);
                try
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(Math.Max(1, worker.LoopDelaySeconds)),
                        stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
        catch (Exception exception) when (
            exception is not OperationCanceledException
            || !stoppingToken.IsCancellationRequested)
        {
            runtime.MarkFaulted();
            await ReportHeartbeatSafelyAsync(CancellationToken.None);
            logger.LogCritical(
                "Falla fatal del scheduler SAP. ErrorType={ErrorType}.",
                exception.GetType().Name);
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        runtime.MarkStopping();
        await ReportHeartbeatWithTimeoutAsync();
        await base.StopAsync(cancellationToken);
        runtime.MarkStopped();
        await ReportHeartbeatWithTimeoutAsync();
        logger.LogInformation("Scheduler SAP detenido.");
    }

    private async Task<SapSyncPollResult> PollAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        return await scope.ServiceProvider
            .GetRequiredService<ISapSyncScheduler>()
            .PollAsync(
                cursor,
                Math.Clamp(sap.SchedulerPageSize, 1, 500),
                workerInstance,
                cancellationToken);
    }

    private async Task ProcessAsync(
        SapSyncPollResult poll,
        CancellationToken cancellationToken)
    {
        foreach (var rejection in poll.Rejections)
        {
            logger.LogWarning(
                "Candidato SAP rechazado. CompanyCode={CompanyCode} ProfileCode={ProfileCode} EntityCode={EntityCode} RejectionCode={RejectionCode}.",
                Safe(rejection.CompanyCode, 50),
                Safe(rejection.ProfileCode, 80),
                Safe(rejection.EntityCode, 80),
                rejection.Code);
        }

        if (poll.InitializedScheduleCount > 0)
        {
            logger.LogInformation(
                "Agendas SAP inicializadas. Count={InitializedScheduleCount}.",
                poll.InitializedScheduleCount);
        }

        foreach (var execution in poll.Executions)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await PrepareExecutionAsync(execution, cancellationToken);
        }
    }

    private async Task PrepareExecutionAsync(
        SapSyncScheduledExecutionContext execution,
        CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var connectionInfo = await scope.ServiceProvider
            .GetRequiredService<ICompanyResolver>()
            .ResolveByCodeAsync(execution.CompanyCode, cancellationToken);
        if (connectionInfo is null)
        {
            logger.LogWarning(
                "Empresa SAP no resoluble. CompanyCode={CompanyCode} ProfileCode={ProfileCode} EntityCode={EntityCode}.",
                Safe(execution.CompanyCode, 50),
                Safe(execution.ProfileCode, 80),
                Safe(execution.EntityCode, 80));
            return;
        }

        scope.ServiceProvider
            .GetRequiredService<ICompanyContext>()
            .SetCurrentCompany(connectionInfo);
        runtime.SetCurrent(
            execution.CompanyId,
            execution.CompanyCode,
            execution.ProfileCode,
            execution.EntityCode);
        await ReportHeartbeatSafelyAsync(cancellationToken);
        try
        {
            var result = await scope.ServiceProvider
                .GetRequiredService<ISapSyncExecutionLeaseCoordinator>()
                .PrepareAsync(
                    execution,
                    workerInstance,
                    TimeSpan.FromMinutes(Math.Max(1, sap.LockTimeoutMinutes)),
                    TimeSpan.FromSeconds(Math.Max(1, sap.LockRenewalSeconds)),
                    cancellationToken);

            logger.LogInformation(
                "Contexto SAP preparado. CompanyCode={CompanyCode} ProfileCode={ProfileCode} EntityCode={EntityCode} Direction={Direction} Source={Source} Status={Status}.",
                Safe(execution.CompanyCode, 50),
                Safe(execution.ProfileCode, 80),
                Safe(execution.EntityCode, 80),
                execution.Direction,
                execution.CandidateSource,
                result.Status);
        }
        finally
        {
            runtime.ClearCurrent();
        }
    }

    private async Task ReportHeartbeatSafelyAsync(CancellationToken cancellationToken)
    {
        try
        {
            var state = runtime.Snapshot();
            await using var scope = scopeFactory.CreateAsyncScope();
            await scope.ServiceProvider
                .GetRequiredService<IWorkerHeartbeatService>()
                .BeatAsync(
                    new WorkerHeartbeatDto(
                        SapSyncWorkerRuntimeState.CreateStorageKey(hostName, workerInstance),
                        state.CurrentCompanyId,
                        state.CurrentCompanyCode,
                        state.LifecycleState,
                        state.CurrentJob,
                        SapSyncWorkerRuntimeState.ResolveVersion(typeof(SapSyncWorker).Assembly),
                        DateTime.UtcNow,
                        WorkerTypes.SapSync,
                        hostName,
                        workerInstance,
                        state.LifecycleState,
                        worker.Enabled,
                        state.StartedAtUtc,
                        state.LastCycleStartedAtUtc,
                        state.LastCycleCompletedAtUtc,
                        state.LastSuccessfulCycleAtUtc,
                        state.LastCycleDurationMs,
                        state.LastCycleResult,
                        state.LastSafeErrorCode,
                        state.LastSafeErrorMessage,
                        state.EnabledCompanyCount,
                        ActiveLeaseCount: state.ActiveLeaseCount),
                    cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            logger.LogWarning(
                "No se pudo actualizar el heartbeat seguro SAP. ErrorType={ErrorType}.",
                exception.GetType().Name);
        }
    }

    private async Task ReportHeartbeatWithTimeoutAsync()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await ReportHeartbeatSafelyAsync(timeout.Token);
    }

    private static int ToMilliseconds(Stopwatch stopwatch) =>
        (int)Math.Min(int.MaxValue, stopwatch.ElapsedMilliseconds);

    private static string Safe(string? value, int maximumLength) =>
        SapSyncWorkerRuntimeState.SanitizeTelemetry(value, maximumLength) ?? "unknown";
}
