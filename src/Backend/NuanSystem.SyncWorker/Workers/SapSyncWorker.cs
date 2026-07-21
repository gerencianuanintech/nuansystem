using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Abstractions.Operations;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.Operations;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.SyncWorker.Options;

namespace NuanSystem.SyncWorker.Workers;

public sealed class SapSyncWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<WorkerOptions> workerOptions,
    IOptions<SapSyncOptions> sapSyncOptions,
    ILogger<SapSyncWorker> logger) : BackgroundService
{
    private readonly WorkerOptions _workerOptions = workerOptions.Value;
    private readonly SapSyncOptions _sapSyncOptions = sapSyncOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_workerOptions.Enabled)
        {
            logger.LogInformation("SAP sync worker deshabilitado por configuracion.");
            return;
        }

        logger.LogInformation("SAP sync worker iniciado como {WorkerInstance}.", _workerOptions.InstanceName);

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
                logger.LogError(exception, "Error general del ciclo SAP sync.");
            }

            await Task.Delay(TimeSpan.FromSeconds(Math.Max(1, _workerOptions.LoopDelaySeconds)), stoppingToken);
        }
    }

    private async Task RunOnceAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var companyRepository = scope.ServiceProvider.GetRequiredService<ISapSyncCompanyRepository>();
        var heartbeat = scope.ServiceProvider.GetRequiredService<IWorkerHeartbeatService>();

        await heartbeat.BeatAsync(CreateHeartbeat(null, "Running", "DiscoverCompanies"), cancellationToken);
        var companies = await companyRepository.GetActiveSapCompaniesAsync(cancellationToken);

        foreach (var company in companies.Take(Math.Max(1, _workerOptions.MaxParallelCompanies)))
        {
            await ProcessCompanyAsync(company, cancellationToken);
        }

        await heartbeat.BeatAsync(CreateHeartbeat(null, "Idle", null), cancellationToken);
    }

    private async Task ProcessCompanyAsync(SapSyncCompanyDto company, CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var companyResolver = scope.ServiceProvider.GetRequiredService<ICompanyResolver>();
        var companyContext = scope.ServiceProvider.GetRequiredService<ICompanyContext>();
        var settingsRepository = scope.ServiceProvider.GetRequiredService<ISapSyncSettingsRepository>();
        var orchestrator = scope.ServiceProvider.GetRequiredService<ISapSyncOrchestrator>();
        var heartbeat = scope.ServiceProvider.GetRequiredService<IWorkerHeartbeatService>();

        var connectionInfo = await companyResolver.ResolveByCodeAsync(company.CompanyCode, cancellationToken);
        if (connectionInfo is null)
        {
            logger.LogWarning("No se pudo resolver la empresa SAP {CompanyCode}.", company.CompanyCode);
            return;
        }

        companyContext.SetCurrentCompany(connectionInfo);
        await heartbeat.BeatAsync(CreateHeartbeat(company, "Running", "LoadSettings"), cancellationToken);

        var settings = await settingsRepository.GetEnabledEntitiesAsync(company.CompanyId, cancellationToken);
        var syncSettings = settings
            .Where(setting => setting.Direction is SapSyncDirection.SapToErp or SapSyncDirection.Both)
            .OrderBy(setting => setting.ExecutionOrder)
            .Take(Math.Max(1, _workerOptions.MaxParallelJobsPerCompany))
            .ToArray();

        foreach (var setting in syncSettings)
        {
            await heartbeat.BeatAsync(CreateHeartbeat(company, "Running", setting.EntityCode), cancellationToken);
            var result = await orchestrator.ExecuteAsync(
                company,
                setting,
                SapSyncDirection.SapToErp,
                _workerOptions.InstanceName,
                TimeSpan.FromMinutes(Math.Max(1, _sapSyncOptions.LockTimeoutMinutes)),
                cancellationToken);

            logger.LogInformation(
                "SAP sync {CompanyCode}/{EntityCode}: {Status}. Procesados={ProcessedCount}, Fallidos={FailedCount}.",
                company.CompanyCode,
                setting.EntityCode,
                result.Status,
                result.ProcessedCount,
                result.FailedCount);
        }
    }

    private WorkerHeartbeatDto CreateHeartbeat(SapSyncCompanyDto? company, string status, string? job)
        => new(
            _workerOptions.InstanceName,
            company?.CompanyId,
            company?.CompanyCode,
            status,
            job,
            typeof(SapSyncWorker).Assembly.GetName().Version?.ToString(),
            DateTime.UtcNow);
}
