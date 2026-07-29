using Microsoft.Extensions.Options;
using System.Diagnostics;
using NuanSystem.SriWorker.Options;
using NuanSystem.SriWorker.Services;

namespace NuanSystem.SriWorker.Workers;

public sealed class SriBackgroundWorker(IServiceScopeFactory scopeFactory, IOptionsMonitor<SriWorkerOptions> options,
    SriWorkerRuntimeState runtime, SriHeartbeatWorker heartbeat, SriSingleInstanceGuard instanceGuard,
    IWorkerOperationalEventPublisher events, ILogger<SriBackgroundWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var current = options.CurrentValue;
        try { instanceGuard.Acquire($"{Environment.MachineName}|{current.NormalizedWorkerInstance}"); }
        catch (Exception exception)
        {
            runtime.MarkFaulted();
            events.Publish(WorkerOperationalEvent.WorkerFaulted,"No se pudo adquirir la identidad unica del SRI Worker.",true,true);
            logger.LogCritical("Falla de identidad unica SRI Worker. ErrorType={ErrorType}",exception.GetType().Name);
            throw;
        }
        if (!current.Enabled)
        {
            runtime.MarkDisabled();
            events.Publish(WorkerOperationalEvent.WorkerDisabled, "SRI Worker deshabilitado por configuracion.", true);
            return;
        }

        runtime.MarkStarted(DateTime.UtcNow);
        events.Publish(WorkerOperationalEvent.WorkerStarted, "SRI Worker iniciado.", true);
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                current = options.CurrentValue;
                var stopwatch = Stopwatch.StartNew();
                runtime.MarkCycleStarted(DateTime.UtcNow);
                try
                {
                    await using var scope = scopeFactory.CreateAsyncScope();
                    var processed = await scope.ServiceProvider.GetRequiredService<ISriWorkerProcessor>().ProcessOnceAsync(stoppingToken);
                    stopwatch.Stop();
                    runtime.MarkCycleCompleted(DateTime.UtcNow, ToMilliseconds(stopwatch), true);
                    if (processed == 0) await Task.Delay(current.EmptyQueueDelay, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
                catch (Exception exception)
                {
                    stopwatch.Stop();
                    runtime.MarkCycleCompleted(DateTime.UtcNow, ToMilliseconds(stopwatch), false);
                    logger.LogError("Fallo de ciclo SRI Worker. EventCode={EventCode} ErrorType={ErrorType}", WorkerOperationalEvent.CycleFailed,exception.GetType().Name);
                    events.Publish(WorkerOperationalEvent.CycleFailed, "Un ciclo del SRI Worker fallo.", false);
                    await Task.Delay(current.ErrorDelay, stoppingToken);
                }
            }
        }
        catch (Exception exception)
        {
            runtime.MarkFaulted();
            events.Publish(WorkerOperationalEvent.WorkerFaulted, "El SRI Worker finalizo inesperadamente.", true, true);
            logger.LogCritical("Falla fatal SRI Worker. EventCode={EventCode} ErrorType={ErrorType}", WorkerOperationalEvent.WorkerFaulted,exception.GetType().Name);
            throw;
        }
        finally { instanceGuard.Dispose(); }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        runtime.StopClaims();
        events.Publish(WorkerOperationalEvent.WorkerStopping, "SRI Worker deteniendo nuevos claims.", false);
        await heartbeat.ReportAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
        runtime.MarkStopped();
        using var finalReport = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await heartbeat.ReportAsync(finalReport.Token);
        events.Publish(WorkerOperationalEvent.WorkerStopped, "SRI Worker detenido.", true);
    }

    private static int ToMilliseconds(Stopwatch stopwatch) => (int)Math.Min(int.MaxValue, stopwatch.ElapsedMilliseconds);
}
