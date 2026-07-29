using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.Operations;
using NuanSystem.Application.Abstractions.Sri;
using NuanSystem.Application.Features.Operations;
using NuanSystem.SriWorker.Options;
using NuanSystem.SriWorker.Services;

namespace NuanSystem.SriWorker.Workers;

public sealed class SriHeartbeatWorker(IServiceScopeFactory scopeFactory, SriWorkerRuntimeState runtime,
    IOptionsMonitor<SriWorkerOptions> options, IConfiguration configuration, ILogger<SriHeartbeatWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ReportAsync(stoppingToken);
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(options.CurrentValue.HeartbeatSeconds));
        while (await timer.WaitForNextTickAsync(stoppingToken)) await ReportAsync(stoppingToken);
    }

    public async Task ReportAsync(CancellationToken cancellationToken)
    {
        try
        {
            var current = options.CurrentValue;
            var host = Environment.MachineName;
            var state = runtime.Snapshot();
            await using var scope = scopeFactory.CreateAsyncScope();
            var companies = await scope.ServiceProvider.GetRequiredService<ISriWorkerCompanyRepository>().GetEnabledCompaniesAsync(cancellationToken);
            var queue = scope.ServiceProvider.GetRequiredService<ISriWorkerQueueRepository>();
            var summaries = new List<SriWorkerOperationalSummary>();
            foreach (var company in companies) summaries.Add(await queue.GetOperationalSummaryAsync(company.CompanyId, cancellationToken));
            var oldest = summaries.Where(x => x.OldestPendingAtUtc.HasValue).Select(x => x.OldestPendingAtUtc).Min();
            var heartbeat = new WorkerHeartbeatDto(
                SriWorkerRuntimeState.StorageKey(host, current.NormalizedWorkerInstance), null, null,
                state.LifecycleState, state.LastCycleResult, WorkerVersionResolver.Resolve(typeof(SriHeartbeatWorker).Assembly), DateTime.UtcNow,
                WorkerTypes.Sri, host, current.NormalizedWorkerInstance, state.LifecycleState, current.Enabled,
                state.StartedAtUtc, state.LastCycleStartedAtUtc, state.LastCycleCompletedAtUtc, state.LastSuccessfulCycleAtUtc,
                state.LastCycleDurationMs, state.LastCycleResult, state.LastSafeErrorCode, state.LastSafeErrorMessage,
                companies.Count, summaries.Sum(x => x.PendingCount), summaries.Sum(x => x.RetryScheduledCount),
                summaries.Sum(x => x.DeadLetterCount), summaries.Sum(x => x.RecentDeadLetterCount),
                summaries.Sum(x => x.ActiveLeaseCount), summaries.Sum(x => x.ExpiredLeaseCount), oldest,
                GetStorageFreePercent());
            await scope.ServiceProvider.GetRequiredService<IWorkerHeartbeatService>().BeatAsync(heartbeat, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { }
        catch (Exception exception) { logger.LogWarning("No se pudo actualizar el heartbeat seguro del SRI Worker. ErrorType={ErrorType}",exception.GetType().Name); }
    }

    private decimal? GetStorageFreePercent()
    {
        try
        {
            var configured=configuration["Operations:LogDirectory"];
            var path=string.IsNullOrWhiteSpace(configured)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),"NuanSystem","SriWorker","logs")
                : Path.GetFullPath(configured);
            var root=Path.GetPathRoot(path);
            if(string.IsNullOrWhiteSpace(root)) return null;
            var drive=new DriveInfo(root);
            return drive.TotalSize<=0 ? null : decimal.Round(100m*drive.AvailableFreeSpace/drive.TotalSize,2);
        }
        catch { return null; }
    }
}
