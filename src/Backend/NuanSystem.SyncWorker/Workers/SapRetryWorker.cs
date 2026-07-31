using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Abstractions.Operations;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.Operations;
using NuanSystem.SyncWorker.Options;

namespace NuanSystem.SyncWorker.Workers;

public sealed class SapRetryWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<WorkerOptions> workerOptions,
    IOptions<RetryOptions> retryOptions,
    IOptions<SapSyncOptions> sapSyncOptions,
    ILogger<SapRetryWorker> logger) : BackgroundService
{
    private readonly WorkerOptions _workerOptions = workerOptions.Value;
    private readonly RetryOptions _retryOptions = retryOptions.Value;
    private readonly SapSyncOptions _sapSyncOptions = sapSyncOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_workerOptions.Enabled || !_retryOptions.Enabled)
        {
            logger.LogInformation("SAP retry worker deshabilitado por configuracion.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunOnceAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError("Error general del ciclo SAP retry. Tipo={ExceptionType}", exception.GetType().Name);
            }

            await Task.Delay(TimeSpan.FromSeconds(Math.Max(1, _retryOptions.IntervalSeconds)), stoppingToken);
        }
    }

    private async Task RunOnceAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var companies = await scope.ServiceProvider.GetRequiredService<ISapSyncCompanyRepository>()
            .GetActiveSapCompaniesAsync(cancellationToken);

        foreach (var company in companies.Take(Math.Max(1, _workerOptions.MaxParallelCompanies)))
        {
            await ProcessCompanyAsync(company, cancellationToken);
        }
    }

    private async Task ProcessCompanyAsync(SapSyncCompanyDto company, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var companyResolver = scope.ServiceProvider.GetRequiredService<ICompanyResolver>();
        var companyContext = scope.ServiceProvider.GetRequiredService<ICompanyContext>();
        var executionRepository = scope.ServiceProvider.GetRequiredService<ISapSyncExecutionRepository>();
        var retryService = scope.ServiceProvider.GetRequiredService<ISapSyncExecutionRetryService>();
        var heartbeat = scope.ServiceProvider.GetRequiredService<IWorkerHeartbeatService>();

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
            "Running",
            "RetryExecutions",
            typeof(SapRetryWorker).Assembly.GetName().Version?.ToString(),
            DateTime.UtcNow), cancellationToken);

        await executionRepository.RecoverExpiredDetailLocksAsync(DateTime.UtcNow, cancellationToken);
        for (var index = 0; index < Math.Max(1, _sapSyncOptions.DefaultBatchSize); index++)
        {
            var result = await retryService.ProcessNextAsync(
                _workerOptions.InstanceName,
                TimeSpan.FromMinutes(Math.Max(1, _sapSyncOptions.LockTimeoutMinutes)),
                Math.Max(1, _retryOptions.BackoffSeconds),
                cancellationToken);
            if (result.Status == SapSyncRetryCycleResult.Idle)
            {
                break;
            }
        }
    }
}
