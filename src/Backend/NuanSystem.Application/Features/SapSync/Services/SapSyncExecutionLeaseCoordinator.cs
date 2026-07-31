using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Scheduling;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class SapSyncScheduledExecutionPreparer : ISapSyncScheduledExecutionPreparer
{
    public Task PrepareAsync(
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

        // Limite intencional de la Fase 10.4: valida y materializa el contexto,
        // pero no despacha ningun handler ni transporte SAP.
        return Task.CompletedTask;
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
