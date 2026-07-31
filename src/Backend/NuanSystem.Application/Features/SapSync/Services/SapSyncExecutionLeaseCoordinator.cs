using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Scheduling;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class SapSyncScheduledExecutionPreparer : ISapSyncScheduledExecutionPreparer
{
    private readonly IReadOnlyDictionary<string, ISapSyncScheduledExecutionProcessor> processors;

    public SapSyncScheduledExecutionPreparer(
        IEnumerable<ISapSyncScheduledExecutionProcessor> processors)
    {
        this.processors = processors
            .GroupBy(processor => processor.EntityCode, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.Single(),
                StringComparer.OrdinalIgnoreCase);
    }

    public async Task PrepareAsync(
        SapSyncScheduledExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(context.CompanyCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(context.ProfileCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(context.EntityCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(context.WorkerInstance);
        if (context.ExecutionUid == Guid.Empty
            || context.BatchSize <= 0
            || context.MaxAttempts <= 0
            || context.ScheduledForAtUtc.Kind != DateTimeKind.Utc)
        {
            throw new InvalidOperationException(
                "El contexto programado SAP no cumple el contrato de preparacion.");
        }

        if (!processors.TryGetValue(context.EntityCode, out var processor))
        {
            throw new InvalidOperationException(
                "SAP_SYNC_SCHEDULED_PROCESSOR_NOT_IMPLEMENTED");
        }

        using var executionTimeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        executionTimeout.CancelAfter(TimeSpan.FromMinutes(context.ExecutionTimeoutMinutes));
        await processor.ProcessAsync(context, executionTimeout.Token);
    }
}

public sealed class SapSyncExecutionLeaseCoordinator(
    ISapSyncLockService lockService,
    ISapSyncScheduledExecutionPreparer preparer) : ISapSyncExecutionLeaseCoordinator
{
    public async Task<SapSyncLeaseExecutionResult> PrepareAsync(
        SapSyncScheduledExecutionContext context,
        string workerInstance,
        TimeSpan lockTimeout,
        TimeSpan renewalInterval,
        CancellationToken cancellationToken = default)
    {
        if (lockTimeout <= TimeSpan.Zero
            || renewalInterval <= TimeSpan.Zero
            || renewalInterval >= lockTimeout)
        {
            throw new ArgumentOutOfRangeException(
                nameof(renewalInterval),
                "La renovacion debe ser positiva y menor que el lease SAP.");
        }

        var syncLock = await lockService.TryAcquireForExecutionAsync(
            context.CompanyId,
            context.EntityCode,
            context.Direction,
            workerInstance,
            context.CorrelationId,
            context.ExecutionUid,
            lockTimeout,
            cancellationToken);
        if (syncLock is null)
        {
            return new SapSyncLeaseExecutionResult(
                SapSyncLeaseExecutionResult.SkippedConcurrent,
                "SAP_SYNC_LOCK_BUSY");
        }

        using var leaseCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var leaseLost = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var renewal = RenewUntilCancelledAsync(
            syncLock,
            lockTimeout,
            renewalInterval,
            leaseLost,
            leaseCancellation.Token);
        try
        {
            var preparation = preparer.PrepareAsync(context, leaseCancellation.Token);
            var completed = await Task.WhenAny(preparation, leaseLost.Task);
            if (completed == leaseLost.Task)
            {
                leaseCancellation.Cancel();
                await IgnoreCancellationAsync(preparation);
                return new SapSyncLeaseExecutionResult(
                    SapSyncLeaseExecutionResult.LeaseLost,
                    "SAP_SYNC_LOCK_LEASE_LOST");
            }

            await preparation;
            return new SapSyncLeaseExecutionResult(SapSyncLeaseExecutionResult.Prepared);
        }
        finally
        {
            leaseCancellation.Cancel();
            await IgnoreCancellationAsync(renewal);
            using var releaseTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await lockService.ReleaseAsync(syncLock, releaseTimeout.Token);
        }
    }

    private async Task RenewUntilCancelledAsync(
        SapSyncLockDto syncLock,
        TimeSpan lockTimeout,
        TimeSpan renewalInterval,
        TaskCompletionSource leaseLost,
        CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                await Task.Delay(renewalInterval, cancellationToken);
                if (!await lockService.RenewAsync(syncLock, lockTimeout, cancellationToken))
                {
                    leaseLost.TrySetResult();
                    return;
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }

    private static async Task IgnoreCancellationAsync(Task task)
    {
        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
        }
    }
}
