using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Options;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class LocalSyncOutboxRelay(
    IOptionsMonitor<MasterBranchSyncWorkerOptions> options,
    ILocalSyncOutboxRepository localOutbox,
    ILocalSyncOutboxPromotionService promotionService,
    ILogger<LocalSyncOutboxRelay> logger) : ILocalSyncOutboxRelay
{
    public async Task<int> ProcessOnceAsync(CancellationToken cancellationToken = default)
    {
        var current = options.CurrentValue;
        var relay = current.LocalOutboxRelay;
        if (!current.Enabled || !relay.Enabled)
        {
            return 0;
        }

        var enabledEntityNames = current.EnabledEntityAppliers
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (enabledEntityNames.Length == 0)
        {
            logger.LogWarning(
                "LocalOutbox relay habilitado sin entidades permitidas; no se reclamaran ni liberaran eventos.");
            return 0;
        }

        var processed = 0;
        var companies = await localOutbox.GetRelayCompaniesAsync(cancellationToken);
        foreach (var company in companies)
        {
            await localOutbox.ReleaseExpiredLeasesAsync(
                company.CompanyId,
                current.NormalizedWorkerInstance,
                enabledEntityNames,
                cancellationToken);
            var events = await localOutbox.ClaimAsync(
                company.CompanyId,
                current.NormalizedWorkerInstance,
                relay.NormalizedBatchSize,
                relay.LeaseDuration,
                enabledEntityNames,
                cancellationToken);

            foreach (var syncEvent in events)
            {
                try
                {
                    var result = await promotionService.PromoteAsync(
                        syncEvent,
                        current.NormalizedWorkerInstance,
                        cancellationToken);
                    if (result.Status is SyncOutboxPromotionStatus.Created or SyncOutboxPromotionStatus.Existing)
                    {
                        await localOutbox.MarkPromotedAsync(
                            company.CompanyId, syncEvent.Id, current.NormalizedWorkerInstance, cancellationToken);
                    }
                    else if (result.Status == SyncOutboxPromotionStatus.Conflict)
                    {
                        await localOutbox.MarkConflictAsync(
                            company.CompanyId, syncEvent.Id, current.NormalizedWorkerInstance,
                            result.Reason, cancellationToken);
                    }
                    else
                    {
                        await localOutbox.MarkRetryAsync(
                            company.CompanyId, syncEvent.Id, current.NormalizedWorkerInstance,
                            result.Reason, relay.RetryDelay, cancellationToken);
                    }
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    logger.LogError(exception,
                        "Fallo promoviendo LocalOutbox {EventId} de empresa {CompanyId}.",
                        syncEvent.EventId, company.CompanyId);
                    await localOutbox.MarkRetryAsync(
                        company.CompanyId, syncEvent.Id, current.NormalizedWorkerInstance,
                        exception.Message, relay.RetryDelay, cancellationToken);
                }

                processed++;
            }
        }

        return processed;
    }
}
