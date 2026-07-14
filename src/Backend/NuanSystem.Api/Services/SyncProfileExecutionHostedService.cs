using Microsoft.Extensions.Options;
using NuanSystem.Api.Options;
using NuanSystem.Application.Abstractions.Common;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Execution.Dtos;

namespace NuanSystem.Api.Services;

public sealed class SyncProfileExecutionHostedService(
    IServiceScopeFactory scopeFactory,
    IOptions<SyncProfileExecutionWorkerOptions> options,
    ILogger<SyncProfileExecutionHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var workerOptions = options.Value;
        if (!workerOptions.Enabled)
        {
            logger.LogInformation("Sync profile execution hosted service is disabled.");
            return;
        }

        var delay = TimeSpan.FromSeconds(Math.Clamp(workerOptions.PollingSeconds, 5, 3600));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunIterationAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing sync profile executions.");
            }

            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task RunIterationAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var executionService = scope.ServiceProvider.GetRequiredService<ISyncProfileExecutionService>();
        var executionRepository = scope.ServiceProvider.GetRequiredService<ISyncProfileExecutionRepository>();
        var scheduleCalculator = scope.ServiceProvider.GetRequiredService<ISyncScheduleCalculator>();
        var clock = scope.ServiceProvider.GetRequiredService<ISystemClock>();

        var dueProfiles = await executionRepository.GetDueProfilesAsync(clock.UtcNow, cancellationToken);
        foreach (var dueProfile in dueProfiles)
        {
            var utcNow = clock.UtcNow;
            var next = scheduleCalculator.CalculateNextExecution(
                new SyncScheduleDefinition(
                    dueProfile.SyncProfileId,
                    dueProfile.ScheduleType,
                    dueProfile.IntervalMinutes,
                    dueProfile.ExecutionTime,
                    dueProfile.TimeZoneId,
                    dueProfile.LastSuccessfulScheduledExecutionAt,
                    dueProfile.ConfiguredAt),
                utcNow);

            if (!dueProfile.NextExecutionAt.HasValue && next.HasValue && next.Value > utcNow)
            {
                await executionRepository.MarkScheduledAsync(dueProfile.SyncProfileId, next.Value, cancellationToken);
                continue;
            }

            var requestResult = await executionService.RequestExecutionAsync(
                dueProfile.SyncProfileId,
                new SyncProfileExecutionRequest
                {
                    ExecutionType = "Scheduled",
                    RequestedBy = "System"
                },
                null,
                "System",
                cancellationToken);

            if (requestResult.IsSuccess && next.HasValue)
            {
                await executionRepository.MarkScheduledAsync(dueProfile.SyncProfileId, next.Value, cancellationToken);
            }
        }

        await executionService.ProcessPendingAsync(cancellationToken);
    }
}
