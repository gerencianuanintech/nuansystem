using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.SyncWorker.Options;

namespace NuanSystem.SyncWorker.Workers;

public sealed class SapOutboxWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<WorkerOptions> workerOptions,
    ILogger<SapOutboxWorker> logger) : BackgroundService
{
    private readonly WorkerOptions _workerOptions = workerOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_workerOptions.Enabled)
        {
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunHeartbeatAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error general del ciclo SAP outbox.");
            }

            await Task.Delay(TimeSpan.FromSeconds(Math.Max(1, _workerOptions.LoopDelaySeconds)), stoppingToken);
        }
    }

    private async Task RunHeartbeatAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var companies = await scope.ServiceProvider.GetRequiredService<ISapSyncCompanyRepository>()
            .GetActiveSapCompaniesAsync(cancellationToken);

        foreach (var company in companies.Take(Math.Max(1, _workerOptions.MaxParallelCompanies)))
        {
            await BeatCompanyAsync(scope.ServiceProvider, company, cancellationToken);
        }
    }

    private async Task BeatCompanyAsync(IServiceProvider services, SapSyncCompanyDto company, CancellationToken cancellationToken)
    {
        var companyResolver = services.GetRequiredService<ICompanyResolver>();
        var companyContext = services.GetRequiredService<ICompanyContext>();
        var heartbeat = services.GetRequiredService<IWorkerHeartbeatService>();

        var connectionInfo = await companyResolver.ResolveByCodeAsync(company.CompanyCode, cancellationToken);
        if (connectionInfo is null)
        {
            return;
        }

        companyContext.SetCurrentCompany(connectionInfo);
        await heartbeat.BeatAsync(new WorkerHeartbeatDto(
            _workerOptions.InstanceName,
            company.CompanyId,
            company.CompanyCode,
            "NotImplemented",
            "OutboxErpToSap",
            typeof(SapOutboxWorker).Assembly.GetName().Version?.ToString(),
            DateTime.UtcNow), cancellationToken);
    }
}
