using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Abstractions.Operations;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.Operations;
using NuanSystem.Application.Features.SapSync.Enums;
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
                logger.LogError(exception, "Error general del ciclo SAP retry.");
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
        var inboxRepository = scope.ServiceProvider.GetRequiredService<ISapSyncInboxRepository>();
        var retryPolicy = scope.ServiceProvider.GetRequiredService<ISapSyncRetryPolicy>();
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
            "RetryInbox",
            typeof(SapRetryWorker).Assembly.GetName().Version?.ToString(),
            DateTime.UtcNow), cancellationToken);

        await inboxRepository.ReleaseExpiredLocksAsync(company.CompanyId, DateTime.UtcNow, cancellationToken);

        var retryItems = await inboxRepository.ClaimRetryScheduledAsync(
            company.CompanyId,
            SapSyncEntityCode.Suppliers,
            Math.Max(1, _sapSyncOptions.DefaultBatchSize),
            _workerOptions.InstanceName,
            TimeSpan.FromMinutes(Math.Max(1, _sapSyncOptions.LockTimeoutMinutes)),
            cancellationToken);

        foreach (var item in retryItems)
        {
            var decision = retryPolicy.Evaluate(
                null,
                item.Status.ToString(),
                null,
                item.AttemptCount,
                Math.Max(_retryOptions.MaxRetryCount, _sapSyncOptions.DefaultMaxRetryCount),
                Math.Max(1, _retryOptions.BackoffSeconds),
                DateTime.UtcNow);

            if (!decision.IsRetryable || decision.MoveToDeadLetter)
            {
                await inboxRepository.MarkDeadLetterAsync(item.Id, "SAP_RETRY_EXHAUSTED", decision.Reason, cancellationToken);
                continue;
            }

            await inboxRepository.MarkFailedAsync(item.Id, "SAP_RETRY_PENDING", decision.Reason, decision.NextAttemptAtUtc, cancellationToken);
        }
    }
}
