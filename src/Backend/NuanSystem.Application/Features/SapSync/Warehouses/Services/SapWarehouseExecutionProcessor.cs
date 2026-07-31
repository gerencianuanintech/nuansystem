using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Scheduling;
using NuanSystem.Application.Features.SapSync.Warehouses.Contracts;

namespace NuanSystem.Application.Features.SapSync.Warehouses.Services;

public sealed class SapWarehouseExecutionProcessor(
    ISapWarehouseReader reader,
    SapWarehouseRecordProcessor recordProcessor,
    ISapSyncExecutionRepository executionRepository,
    ISapSyncRetryPolicy retryPolicy) : ISapSyncScheduledExecutionProcessor
{
    private const int RetryBackoffSeconds = 30;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public string EntityCode => SapSyncEntityCode.Warehouses;

    public async Task ProcessAsync(
        SapSyncScheduledExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var current = await EnsureRunningExecutionAsync(context, cancellationToken);
        var results = new List<PersistedResult>();

        try
        {
            if (await TryCompleteCancellationAsync(current, results, cancellationToken))
            {
                return;
            }

            var rows = await reader.GetWarehousesAsync(context.CompanyId, cancellationToken);
            var stopProcessing = false;
            foreach (var batch in rows
                         .OrderBy(item => item.WarehouseCode, StringComparer.OrdinalIgnoreCase)
                         .Chunk(Math.Max(1, context.BatchSize)))
            {
                foreach (var row in batch)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    current = await executionRepository.GetByExecutionUidAsync(context.ExecutionUid, cancellationToken)
                        ?? throw new InvalidOperationException("SAP_SYNC_EXECUTION_NOT_FOUND");
                    if (await TryCompleteCancellationAsync(current, results, cancellationToken))
                    {
                        return;
                    }

                var snapshot = SapWarehouseSnapshot.FromRecord(row);
                var snapshotJson = JsonSerializer.Serialize(snapshot, JsonOptions);
                var snapshotHash = SHA256.HashData(Encoding.UTF8.GetBytes(snapshotJson));
                var sourceKey = Normalize(snapshot.WarehouseCode);
                var startedAtUtc = DateTime.UtcNow;
                SapWarehouseRecordProcessResult result;
                DateTime? nextAttemptAtUtc = null;
                string? errorClass = null;

                try
                {
                    result = await recordProcessor.ProcessAsync(
                        snapshot, null, "SAP Sync Worker", cancellationToken);
                    if (result.Status == SapSyncExecutionDetailStatuses.Failed)
                    {
                        errorClass = SapSyncSafeErrorClasses.Terminal;
                    }
                }
                catch (Exception exception) when (exception is not OperationCanceledException)
                {
                    var decision = retryPolicy.Evaluate(
                        exception.GetType().Name,
                        null,
                        exception,
                        1,
                        context.MaxAttempts,
                        RetryBackoffSeconds,
                        DateTime.UtcNow);
                    var status = decision.IsRetryable
                        ? decision.MoveToDeadLetter
                            ? SapSyncExecutionDetailStatuses.DeadLetter
                            : SapSyncExecutionDetailStatuses.RetryScheduled
                        : SapSyncExecutionDetailStatuses.Failed;
                    errorClass = decision.IsRetryable
                        ? SapSyncSafeErrorClasses.Transient
                        : SapSyncSafeErrorClasses.Terminal;
                    nextAttemptAtUtc = decision.NextAttemptAtUtc;
                    result = new(
                        SapSyncExecutionDetailActions.Skip,
                        status,
                        null,
                        null,
                        SapWarehouseResultCodes.SaveFailed,
                        decision.IsRetryable && !decision.MoveToDeadLetter
                            ? "Reintento programado para la bodega."
                            : "No fue posible procesar la bodega.");
                }

                var write = await executionRepository.UpsertDetailAsync(new(
                    Id: null,
                    ExecutionUid: context.ExecutionUid,
                    SourceRecordKey: sourceKey,
                    SourceVersion: Convert.ToHexString(snapshotHash),
                    LocalEntityId: result.LocalWarehouseId,
                    LocalGlobalId: result.LocalGlobalId,
                    Action: result.Action,
                    Status: result.Status,
                    AttemptCount: 1,
                    MaxAttempts: context.MaxAttempts,
                    NextAttemptAtUtc: nextAttemptAtUtc,
                    ErrorClass: errorClass,
                    ResultCode: result.ResultCode,
                    SafeMessage: result.SafeMessage,
                    ApprovedSnapshotType: SapSyncApprovedSnapshotTypes.WarehouseV1,
                    ApprovedSnapshotJson: snapshotJson,
                    SnapshotHash: snapshotHash,
                    StartedAtUtc: startedAtUtc,
                    FinishedAtUtc: result.Status == SapSyncExecutionDetailStatuses.RetryScheduled
                        ? null
                        : DateTime.UtcNow,
                    RowVersion: null), cancellationToken);
                if (write.Id is null)
                {
                    throw new InvalidOperationException($"SAP_SYNC_DETAIL_{write.ResultCode}");
                }

                    results.Add(new PersistedResult(result.Status, result.ResultCode, nextAttemptAtUtc));
                    if (!context.ContinueOnError
                        && result.Status is SapSyncExecutionDetailStatuses.Failed
                            or SapSyncExecutionDetailStatuses.RetryScheduled
                            or SapSyncExecutionDetailStatuses.DeadLetter)
                    {
                        stopProcessing = true;
                        break;
                    }
                }

                if (stopProcessing)
                {
                    break;
                }
            }

            current = await executionRepository.GetByExecutionUidAsync(context.ExecutionUid, cancellationToken)
                ?? throw new InvalidOperationException("SAP_SYNC_EXECUTION_NOT_FOUND");
            if (await TryCompleteCancellationAsync(current, results, cancellationToken))
            {
                return;
            }

            await TransitionAsync(
                current,
                ResolveFinalStatus(results),
                results,
                LastErrorCode(results),
                LastErrorMessage(results),
                NextAttempt(results),
                cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            await TryCloseInterruptedExecutionAsync(context.ExecutionUid, results);
            throw;
        }
        catch (Exception exception)
        {
            var recovered = await executionRepository.GetByExecutionUidAsync(
                context.ExecutionUid, CancellationToken.None);
            if (recovered is null)
            {
                throw;
            }
            current = recovered;
            if (current.Status == SapSyncExecutionStatuses.Cancelling)
            {
                await TransitionAsync(
                    current, SapSyncExecutionStatuses.Cancelled, results,
                    null, null, null, CancellationToken.None);
                return;
            }

            if (current.Status == SapSyncExecutionStatuses.Running)
            {
                await TransitionAsync(
                    current, SapSyncExecutionStatuses.Failed, results,
                    "SAP_WAREHOUSE_EXECUTION_FAILED",
                    $"La ejecucion de bodegas fallo: {exception.GetType().Name}.",
                    null,
                    CancellationToken.None);
            }

            throw;
        }
    }

    private async Task TryCloseInterruptedExecutionAsync(
        Guid executionUid,
        IReadOnlyCollection<PersistedResult> results)
    {
        try
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var current = await executionRepository.GetByExecutionUidAsync(executionUid, timeout.Token);
            if (current?.Status is SapSyncExecutionStatuses.Running or SapSyncExecutionStatuses.Cancelling)
            {
                await TransitionAsync(
                    current,
                    SapSyncExecutionStatuses.Cancelled,
                    results,
                    "SAP_WAREHOUSE_EXECUTION_INTERRUPTED",
                    "La ejecucion de bodegas fue interrumpida de forma controlada.",
                    null,
                    timeout.Token);
            }
        }
        catch
        {
            // La cancelacion original conserva prioridad; una futura recuperacion
            // operativa podra cerrar cualquier cabecera que no haya respondido.
        }
    }

    private async Task<SapSyncExecutionDto> EnsureRunningExecutionAsync(
        SapSyncScheduledExecutionContext context,
        CancellationToken cancellationToken)
    {
        var current = await executionRepository.GetByExecutionUidAsync(context.ExecutionUid, cancellationToken);
        if (current is null)
        {
            var create = await executionRepository.CreateAsync(new(
                context.ExecutionUid,
                context.ExecutionUid,
                ToGuid(context.CorrelationId),
                context.ProfileId,
                context.ProfileEntityId,
                context.ProfileCode,
                context.ProfileName,
                context.CompanyId,
                context.CompanyCode,
                context.EntityCode,
                context.Direction.ToString(),
                SapSyncTriggerTypes.Scheduled,
                null,
                context.BatchSize,
                context.MaxAttempts,
                context.ExecutionOrder,
                context.ExecutionTimeoutMinutes,
                context.ScheduleType,
                context.TimeZoneId,
                JsonSerializer.Serialize(new
                {
                    context.ProfileCode,
                    context.ProfileName,
                    context.EntityCode,
                    Direction = context.Direction.ToString(),
                    context.SyncMode
                }, JsonOptions),
                JsonSerializer.Serialize(new
                {
                    context.BatchSize,
                    context.MaxAttempts,
                    context.ExecutionOrder,
                    context.ContinueOnError,
                    context.ExecutionTimeoutMinutes
                }, JsonOptions),
                null,
                null,
                context.WorkerInstance), cancellationToken);
            if (create.Id is null || create.RowVersion is null)
            {
                throw new InvalidOperationException($"SAP_SYNC_EXECUTION_{create.ResultCode}");
            }

            var started = await executionRepository.TransitionAsync(
                State(context.ExecutionUid, SapSyncExecutionStatuses.Pending,
                    SapSyncExecutionStatuses.Running, [], create.RowVersion), cancellationToken);
            if (started.Id is null)
            {
                throw new InvalidOperationException($"SAP_SYNC_EXECUTION_{started.ResultCode}");
            }

            current = await executionRepository.GetByExecutionUidAsync(context.ExecutionUid, cancellationToken);
        }

        if (current is null)
        {
            throw new InvalidOperationException("SAP_SYNC_EXECUTION_NOT_FOUND");
        }
        if (current.Status == SapSyncExecutionStatuses.Pending)
        {
            var started = await executionRepository.TransitionAsync(
                State(context.ExecutionUid, current.Status, SapSyncExecutionStatuses.Running,
                    [], current.RowVersion), cancellationToken);
            if (started.Id is null)
            {
                throw new InvalidOperationException($"SAP_SYNC_EXECUTION_{started.ResultCode}");
            }
            current = await executionRepository.GetByExecutionUidAsync(context.ExecutionUid, cancellationToken)
                ?? throw new InvalidOperationException("SAP_SYNC_EXECUTION_NOT_FOUND");
        }
        if (current.Status is not (SapSyncExecutionStatuses.Running or SapSyncExecutionStatuses.Cancelling))
        {
            throw new InvalidOperationException("SAP_SYNC_EXECUTION_NOT_RUNNABLE");
        }
        return current;
    }

    private async Task<bool> TryCompleteCancellationAsync(
        SapSyncExecutionDto current,
        IReadOnlyCollection<PersistedResult> results,
        CancellationToken cancellationToken)
    {
        if (current.Status != SapSyncExecutionStatuses.Cancelling)
        {
            return false;
        }

        await TransitionAsync(current, SapSyncExecutionStatuses.Cancelled, results,
            null, null, null, cancellationToken);
        return true;
    }

    private async Task TransitionAsync(
        SapSyncExecutionDto current,
        string newStatus,
        IReadOnlyCollection<PersistedResult> results,
        string? errorCode,
        string? errorMessage,
        DateTime? nextAttemptAtUtc,
        CancellationToken cancellationToken)
    {
        var write = await executionRepository.TransitionAsync(
            State(current.ExecutionUid, current.Status, newStatus, results, current.RowVersion,
                errorCode, errorMessage, nextAttemptAtUtc), cancellationToken);
        if (write.Id is null)
        {
            throw new InvalidOperationException($"SAP_SYNC_EXECUTION_{write.ResultCode}");
        }
    }

    private static SapSyncExecutionStateData State(
        Guid executionUid,
        string expectedStatus,
        string newStatus,
        IReadOnlyCollection<PersistedResult> results,
        byte[] rowVersion,
        string? errorCode = null,
        string? errorMessage = null,
        DateTime? nextAttemptAtUtc = null) =>
        new(
            executionUid,
            expectedStatus,
            newStatus,
            results.Count,
            Count(results, SapSyncExecutionDetailStatuses.Created),
            Count(results, SapSyncExecutionDetailStatuses.Updated),
            Count(results, SapSyncExecutionDetailStatuses.Unchanged),
            Count(results, SapSyncExecutionDetailStatuses.ApprovalRequired),
            Count(results, SapSyncExecutionDetailStatuses.Conflict),
            Count(results, SapSyncExecutionDetailStatuses.Skipped),
            Count(results, SapSyncExecutionDetailStatuses.RetryScheduled),
            Count(results, SapSyncExecutionDetailStatuses.Failed),
            Count(results, SapSyncExecutionDetailStatuses.DeadLetter),
            errorCode,
            errorMessage,
            nextAttemptAtUtc,
            rowVersion);

    private static string ResolveFinalStatus(IReadOnlyCollection<PersistedResult> results)
    {
        var failed = Count(results, SapSyncExecutionDetailStatuses.Failed)
            + Count(results, SapSyncExecutionDetailStatuses.DeadLetter);
        if (Count(results, SapSyncExecutionDetailStatuses.RetryScheduled) > 0)
            return SapSyncExecutionStatuses.RetryScheduled;
        if (results.Count > 0 && failed == results.Count)
            return SapSyncExecutionStatuses.Failed;
        if (failed > 0)
            return SapSyncExecutionStatuses.CompletedWithErrors;
        if (results.Any(item => item.Status is SapSyncExecutionDetailStatuses.ApprovalRequired
            or SapSyncExecutionDetailStatuses.Conflict
            or SapSyncExecutionDetailStatuses.Skipped))
            return SapSyncExecutionStatuses.CompletedWithWarnings;
        return SapSyncExecutionStatuses.Completed;
    }

    private static int Count(IEnumerable<PersistedResult> results, string status) =>
        results.Count(item => item.Status == status);
    private static string? LastErrorCode(IEnumerable<PersistedResult> results) =>
        results.LastOrDefault(item => item.Status is SapSyncExecutionDetailStatuses.Failed
            or SapSyncExecutionDetailStatuses.DeadLetter)?.ResultCode;
    private static string? LastErrorMessage(IEnumerable<PersistedResult> results) =>
        LastErrorCode(results) is null ? null : "Una o mas bodegas no pudieron procesarse.";
    private static DateTime? NextAttempt(IEnumerable<PersistedResult> results) =>
        results.Where(item => item.Status == SapSyncExecutionDetailStatuses.RetryScheduled)
            .Select(item => item.NextAttemptAtUtc)
            .Where(value => value is not null)
            .Min();
    private static string Normalize(string? value) => value?.Trim() ?? string.Empty;

    private static Guid ToGuid(string value)
    {
        if (Guid.TryParse(value, out var parsed))
            return parsed;
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(value));
        return new Guid(hash);
    }

    private sealed record PersistedResult(
        string Status,
        string ResultCode,
        DateTime? NextAttemptAtUtc);
}
