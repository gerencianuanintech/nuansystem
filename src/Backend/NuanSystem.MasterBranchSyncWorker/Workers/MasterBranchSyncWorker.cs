using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuanSystem.MasterBranchSyncWorker.Options;
using NuanSystem.MasterBranchSyncWorker.Services;

namespace NuanSystem.MasterBranchSyncWorker.Workers;

public sealed class MasterBranchSyncWorker(
    IServiceScopeFactory scopeFactory,
    IOptionsMonitor<MasterBranchSyncWorkerOptions> options,
    ILogger<MasterBranchSyncWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("NuanSystem Master/Branch Sync Worker iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var currentOptions = options.CurrentValue;

            try
            {
                if (!currentOptions.Enabled)
                {
                    await Task.Delay(currentOptions.EmptyQueueDelay, stoppingToken);
                    continue;
                }

                using var scope = scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IMasterBranchSyncWorkerProcessor>();
                var processed = await processor.ProcessOnceAsync(stoppingToken);
                var delay = processed == 0 ? currentOptions.EmptyQueueDelay : TimeSpan.Zero;

                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error no controlado en ciclo Master/Branch Sync Worker.");
                await Task.Delay(currentOptions.ErrorDelay, stoppingToken);
            }
        }

        logger.LogInformation("NuanSystem Master/Branch Sync Worker detenido.");
    }
}
