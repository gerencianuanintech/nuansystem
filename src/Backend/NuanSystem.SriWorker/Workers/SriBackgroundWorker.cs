using Microsoft.Extensions.Options;
using NuanSystem.SriWorker.Options;
using NuanSystem.SriWorker.Services;

namespace NuanSystem.SriWorker.Workers;

public sealed class SriBackgroundWorker(IServiceScopeFactory scopeFactory, IOptionsMonitor<SriWorkerOptions> options,
    ILogger<SriBackgroundWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("NuanSystem SRI Worker iniciado. Enabled={Enabled}", options.CurrentValue.Enabled);
        while (!stoppingToken.IsCancellationRequested)
        {
            var current = options.CurrentValue;
            try
            {
                if (!current.Enabled)
                {
                    await Task.Delay(current.EmptyQueueDelay, stoppingToken);
                    continue;
                }

                await using var scope = scopeFactory.CreateAsyncScope();
                var processed = await scope.ServiceProvider.GetRequiredService<ISriWorkerProcessor>().ProcessOnceAsync(stoppingToken);
                if (processed == 0) await Task.Delay(current.EmptyQueueDelay, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error no controlado en ciclo SRI Worker.");
                await Task.Delay(current.ErrorDelay, stoppingToken);
            }
        }
        logger.LogInformation("NuanSystem SRI Worker detenido.");
    }
}
