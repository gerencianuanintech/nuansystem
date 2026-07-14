using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.MasterBranchSyncWorker.Options;
using NuanSystem.Shared.Sync;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class MasterBranchSyncWorkerProcessor(
    IOptionsMonitor<MasterBranchSyncWorkerOptions> options,
    ISyncOutboxRepository outboxRepository,
    ISyncAuditRepository auditRepository,
    ISyncEventApplier eventApplier,
    ILogger<MasterBranchSyncWorkerProcessor> logger) : IMasterBranchSyncWorkerProcessor
{
    public async Task<int> ProcessOnceAsync(CancellationToken cancellationToken = default)
    {
        var currentOptions = options.CurrentValue;
        if (!currentOptions.Enabled)
        {
            logger.LogDebug("Master/Branch sync worker deshabilitado por configuracion.");
            return 0;
        }

        if (currentOptions.SkeletonMode &&
            currentOptions.SkeletonModeBehavior == SkeletonModeBehavior.ObserveOnly)
        {
            logger.LogInformation(
                "Master/Branch sync worker en SkeletonMode ObserveOnly; no se reclamaran eventos SyncOutbox.");
            return 0;
        }

        await outboxRepository.ReleaseExpiredLocksAsync(cancellationToken);

        var events = await outboxRepository.ClaimPendingAsync(
            currentOptions.NormalizedWorkerInstance,
            currentOptions.NormalizedBatchSize,
            currentOptions.LockDuration,
            cancellationToken);

        foreach (var syncEvent in events)
        {
            await ProcessEventAsync(syncEvent, currentOptions, cancellationToken);
        }

        return events.Count;
    }

    private async Task ProcessEventAsync(
        SyncOutboxDto syncEvent,
        MasterBranchSyncWorkerOptions currentOptions,
        CancellationToken cancellationToken)
    {
        await AddAuditAsync(
            syncEvent,
            SyncAuditAction.Claimed,
            previousStatus: SyncEventStatus.Pending,
            newStatus: SyncEventStatus.InProcess,
            message: "Evento SyncOutbox reclamado por worker esqueleto.",
            errorCode: null,
            errorDetail: null,
            currentOptions,
            cancellationToken);

        try
        {
            if (currentOptions.SkeletonMode)
            {
                await HandleSkeletonModeAsync(syncEvent, currentOptions, cancellationToken);
                return;
            }

            var targets = await outboxRepository.GetTargetsAsync(syncEvent.CompanyId, syncEvent.Id, cancellationToken);
            if (targets.Count == 0)
            {
                const string reason = "Evento sin targets de sucursal configurados; ignorado por worker esqueleto.";
                await outboxRepository.MarkIgnoredAsync(syncEvent.Id, reason, cancellationToken);
                await AddAuditAsync(
                    syncEvent,
                    SyncAuditAction.Ignored,
                    previousStatus: SyncEventStatus.InProcess,
                    newStatus: SyncEventStatus.Ignored,
                    message: reason,
                    errorCode: null,
                    errorDetail: null,
                    currentOptions,
                    cancellationToken);
                return;
            }

            var targetStatuses = targets.ToDictionary(target => target.Id, target => target.Status);
            foreach (var target in targets)
            {
                if (target.Status is SyncEventStatus.Applied or SyncEventStatus.Ignored or SyncEventStatus.DeadLetter)
                {
                    continue;
                }

                var claimed = await outboxRepository.TryMarkTargetInProcessAsync(target.Id, cancellationToken);
                if (!claimed)
                {
                    continue;
                }

                try
                {
                    var applyResult = await eventApplier.ApplyAsync(CreateApplyContext(syncEvent, target), cancellationToken);
                    if (applyResult.Applied)
                    {
                        await outboxRepository.MarkTargetAppliedAsync(target.Id, cancellationToken);
                        targetStatuses[target.Id] = SyncEventStatus.Applied;
                        await AddAuditAsync(
                            syncEvent,
                            SyncAuditAction.Applied,
                            previousStatus: SyncEventStatus.InProcess,
                            newStatus: SyncEventStatus.Applied,
                            message: applyResult.Message,
                            errorCode: null,
                            errorDetail: null,
                            currentOptions,
                            cancellationToken,
                            target.BranchCompanyId);
                        continue;
                    }

                    await outboxRepository.MarkTargetIgnoredAsync(target.Id, applyResult.Message, cancellationToken);
                    targetStatuses[target.Id] = SyncEventStatus.Ignored;
                    await AddAuditAsync(
                        syncEvent,
                        SyncAuditAction.Ignored,
                        previousStatus: SyncEventStatus.InProcess,
                        newStatus: SyncEventStatus.Ignored,
                        message: applyResult.Message,
                        errorCode: applyResult.ErrorCode,
                        errorDetail: null,
                        currentOptions,
                        cancellationToken,
                        target.BranchCompanyId);
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    logger.LogError(
                        exception,
                        "Error procesando target SyncOutbox {EventId} hacia sucursal {BranchCompanyId}.",
                        syncEvent.EventId,
                        target.BranchCompanyId);

                    if (target.AttemptCount + 1 >= target.MaxAttempts)
                    {
                        await outboxRepository.MarkTargetDeadLetterAsync(target.Id, exception.Message, cancellationToken);
                        targetStatuses[target.Id] = SyncEventStatus.DeadLetter;
                        await AddAuditAsync(
                            syncEvent,
                            SyncAuditAction.DeadLetter,
                            previousStatus: SyncEventStatus.InProcess,
                            newStatus: SyncEventStatus.DeadLetter,
                            message: "Target SyncOutbox enviado a DeadLetter por agotar intentos.",
                            errorCode: exception.GetType().Name,
                            errorDetail: exception.Message,
                            currentOptions,
                            cancellationToken,
                            target.BranchCompanyId);
                        continue;
                    }

                    await outboxRepository.MarkTargetErrorAsync(target.Id, exception.Message, currentOptions.ErrorDelay, cancellationToken);
                    targetStatuses[target.Id] = SyncEventStatus.Error;
                    await AddAuditAsync(
                        syncEvent,
                        SyncAuditAction.Failed,
                        previousStatus: SyncEventStatus.InProcess,
                        newStatus: SyncEventStatus.Error,
                        message: "Error tecnico procesando target SyncOutbox.",
                        errorCode: exception.GetType().Name,
                        errorDetail: exception.Message,
                        currentOptions,
                        cancellationToken,
                        target.BranchCompanyId);
                }
            }

            await CloseOutboxFromTargetStatusesAsync(syncEvent, targetStatuses.Values, currentOptions, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogError(
                exception,
                "Error procesando evento SyncOutbox {EventId} ({EntityName}).",
                syncEvent.EventId,
                syncEvent.EntityName);

            if (syncEvent.AttemptCount >= syncEvent.MaxAttempts)
            {
                await outboxRepository.MarkDeadLetterAsync(syncEvent.Id, exception.Message, cancellationToken);
                await AddAuditAsync(
                    syncEvent,
                    SyncAuditAction.DeadLetter,
                    previousStatus: SyncEventStatus.InProcess,
                    newStatus: SyncEventStatus.DeadLetter,
                    message: "Evento SyncOutbox enviado a DeadLetter por agotar intentos.",
                    errorCode: exception.GetType().Name,
                    errorDetail: exception.Message,
                    currentOptions,
                    cancellationToken);
                return;
            }

            await outboxRepository.MarkErrorAsync(
                syncEvent.Id,
                exception.Message,
                currentOptions.ErrorDelay,
                cancellationToken);

            await AddAuditAsync(
                syncEvent,
                SyncAuditAction.Failed,
                previousStatus: SyncEventStatus.InProcess,
                newStatus: SyncEventStatus.Error,
                message: "Error tecnico procesando evento SyncOutbox.",
                errorCode: exception.GetType().Name,
                errorDetail: exception.Message,
                currentOptions,
                cancellationToken);
        }
    }

    private async Task HandleSkeletonModeAsync(
        SyncOutboxDto syncEvent,
        MasterBranchSyncWorkerOptions currentOptions,
        CancellationToken cancellationToken)
    {
        switch (currentOptions.SkeletonModeBehavior)
        {
            case SkeletonModeBehavior.ClaimAndRelease:
            {
                const string message = "SkeletonMode ClaimAndRelease: dry-run sin aplicar entidades; evento devuelto a Pending.";
                await outboxRepository.UpdateStatusAsync(syncEvent.Id, SyncEventStatus.Pending, message, cancellationToken);
                await AddAuditAsync(
                    syncEvent,
                    SyncAuditAction.DryRun,
                    previousStatus: SyncEventStatus.InProcess,
                    newStatus: SyncEventStatus.Pending,
                    message: message,
                    errorCode: null,
                    errorDetail: null,
                    currentOptions,
                    cancellationToken);
                return;
            }

            case SkeletonModeBehavior.ClaimAndIgnore:
            {
                const string message = "SkeletonMode ClaimAndIgnore: evento ignorado explicitamente sin aplicar entidades.";
                await outboxRepository.MarkIgnoredAsync(syncEvent.Id, message, cancellationToken);
                await AddAuditAsync(
                    syncEvent,
                    SyncAuditAction.Ignored,
                    previousStatus: SyncEventStatus.InProcess,
                    newStatus: SyncEventStatus.Ignored,
                    message: message,
                    errorCode: null,
                    errorDetail: null,
                    currentOptions,
                    cancellationToken);
                return;
            }

            case SkeletonModeBehavior.ObserveOnly:
            default:
            {
                logger.LogInformation(
                    "Evento SyncOutbox {EventId} fue reclamado con SkeletonMode ObserveOnly; se devuelve a Pending sin aplicar entidades.",
                    syncEvent.EventId);
                const string message = "SkeletonMode ObserveOnly: evento devuelto a Pending sin aplicar entidades.";
                await outboxRepository.UpdateStatusAsync(syncEvent.Id, SyncEventStatus.Pending, message, cancellationToken);
                await AddAuditAsync(
                    syncEvent,
                    SyncAuditAction.DryRun,
                    previousStatus: SyncEventStatus.InProcess,
                    newStatus: SyncEventStatus.Pending,
                    message: message,
                    errorCode: null,
                    errorDetail: null,
                    currentOptions,
                    cancellationToken);
                return;
            }
        }
    }

    private Task<long> AddAuditAsync(
        SyncOutboxDto syncEvent,
        SyncAuditAction action,
        SyncEventStatus? previousStatus,
        SyncEventStatus? newStatus,
        string? message,
        string? errorCode,
        string? errorDetail,
        MasterBranchSyncWorkerOptions currentOptions,
        CancellationToken cancellationToken,
        int? branchCompanyId = null)
    {
        return auditRepository.AddAsync(
            new CreateSyncAuditData(
                syncEvent.CompanyId,
                branchCompanyId,
                syncEvent.EventId,
                syncEvent.EntityName,
                syncEvent.EntityGlobalId,
                action,
                previousStatus,
                newStatus,
                message,
                errorCode,
                errorDetail,
                CreatedBy: currentOptions.NormalizedWorkerInstance),
            cancellationToken);
    }

    private async Task CloseOutboxFromTargetStatusesAsync(
        SyncOutboxDto syncEvent,
        IEnumerable<SyncEventStatus> statuses,
        MasterBranchSyncWorkerOptions currentOptions,
        CancellationToken cancellationToken)
    {
        var statusList = statuses.ToArray();
        if (statusList.All(status => status is SyncEventStatus.Applied or SyncEventStatus.Ignored))
        {
            if (statusList.Any(status => status == SyncEventStatus.Applied))
            {
                await outboxRepository.MarkAppliedAsync(syncEvent.Id, cancellationToken);
                await AddAuditAsync(
                    syncEvent,
                    SyncAuditAction.Applied,
                    previousStatus: SyncEventStatus.InProcess,
                    newStatus: SyncEventStatus.Applied,
                    message: "Evento SyncOutbox aplicado en todos los targets aplicables.",
                    errorCode: null,
                    errorDetail: null,
                    currentOptions,
                    cancellationToken);
                return;
            }

            await outboxRepository.MarkIgnoredAsync(syncEvent.Id, "Todos los targets fueron ignorados por regla de aplicador.", cancellationToken);
            await AddAuditAsync(
                syncEvent,
                SyncAuditAction.Ignored,
                previousStatus: SyncEventStatus.InProcess,
                newStatus: SyncEventStatus.Ignored,
                message: "Todos los targets fueron ignorados por regla de aplicador.",
                errorCode: null,
                errorDetail: null,
                currentOptions,
                cancellationToken);
            return;
        }

        if (statusList.Any(status => status == SyncEventStatus.DeadLetter))
        {
            await outboxRepository.MarkDeadLetterAsync(syncEvent.Id, "Uno o mas targets quedaron en DeadLetter.", cancellationToken);
            await AddAuditAsync(
                syncEvent,
                SyncAuditAction.DeadLetter,
                previousStatus: SyncEventStatus.InProcess,
                newStatus: SyncEventStatus.DeadLetter,
                message: "Uno o mas targets quedaron en DeadLetter.",
                errorCode: null,
                errorDetail: null,
                currentOptions,
                cancellationToken);
            return;
        }

        await outboxRepository.MarkErrorAsync(
            syncEvent.Id,
            "Uno o mas targets quedaron pendientes o con error reprocesable.",
            currentOptions.ErrorDelay,
            cancellationToken);

        await AddAuditAsync(
            syncEvent,
            SyncAuditAction.Failed,
            previousStatus: SyncEventStatus.InProcess,
            newStatus: SyncEventStatus.Error,
            message: "Uno o mas targets quedaron pendientes o con error reprocesable.",
            errorCode: null,
            errorDetail: null,
            currentOptions,
            cancellationToken);
    }

    private static SyncEventApplyContext CreateApplyContext(SyncOutboxDto syncEvent, SyncOutboxTargetDto? target)
    {
        return new SyncEventApplyContext(
            syncEvent.EventId,
            syncEvent.CompanyId,
            syncEvent.EntityName,
            syncEvent.EntityGlobalId,
            syncEvent.Operation.ToString(),
            syncEvent.PayloadJson,
            target?.BranchCompanyId,
            target?.Id);
    }
}
