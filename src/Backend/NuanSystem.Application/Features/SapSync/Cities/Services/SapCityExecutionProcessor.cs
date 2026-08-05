using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Cities.Contracts;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Application.Features.SapSync.Scheduling;

namespace NuanSystem.Application.Features.SapSync.Cities.Services;

public sealed class SapCityExecutionProcessor(
    ISapCityReader reader,
    SapCityRecordProcessor recordProcessor,
    ISapSyncExecutionRepository executionRepository,
    ISapSyncRetryPolicy retryPolicy) : ISapSyncScheduledExecutionProcessor
{
    private const int RetryBackoffSeconds = 30;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    public string EntityCode => SapSyncEntityCode.Cities;

    public async Task ProcessAsync(
        SapSyncScheduledExecutionContext context,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var results = new List<PersistedResult>();
        try
        {
            var current = await EnsureRunningAsync(context, cancellationToken);
            if (await CompleteCancellationAsync(current, results, cancellationToken)) return;
            var rows = await reader.GetCitiesAsync(context.CompanyId, cancellationToken);
            var stop = false;
            foreach (var batch in rows.OrderBy(x => x.CountryCode, StringComparer.OrdinalIgnoreCase)
                         .ThenBy(x => x.ProvinceCode, StringComparer.OrdinalIgnoreCase)
                         .ThenBy(x => x.CityCode, StringComparer.OrdinalIgnoreCase)
                         .Chunk(Math.Max(1, context.BatchSize)))
            {
                foreach (var row in batch)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    current = await GetRequiredAsync(context.ExecutionUid, cancellationToken);
                    if (await CompleteCancellationAsync(current, results, cancellationToken)) return;

                    var snapshot = SapCitySnapshot.FromRecord(row);
                    var json = JsonSerializer.Serialize(snapshot, JsonOptions);
                    var hash = SHA256.HashData(Encoding.UTF8.GetBytes(json));
                    var started = DateTime.UtcNow;
                    DateTime? nextAttempt = null;
                    string? errorClass = null;
                    SapCityRecordProcessResult result;
                    try
                    {
                        result = await recordProcessor.ProcessAsync(
                            snapshot, null, "SAP Sync Worker", cancellationToken);
                        if (result.Status == SapSyncExecutionDetailStatuses.Failed)
                            errorClass = SapSyncSafeErrorClasses.Terminal;
                    }
                    catch (Exception ex) when (ex is not OperationCanceledException)
                    {
                        var decision = retryPolicy.Evaluate(ex.GetType().Name, null, ex, 1,
                            context.MaxAttempts, RetryBackoffSeconds, DateTime.UtcNow);
                        var status = decision.IsRetryable
                            ? decision.MoveToDeadLetter
                                ? SapSyncExecutionDetailStatuses.DeadLetter
                                : SapSyncExecutionDetailStatuses.RetryScheduled
                            : SapSyncExecutionDetailStatuses.Failed;
                        nextAttempt = decision.NextAttemptAtUtc;
                        errorClass = decision.IsRetryable
                            ? SapSyncSafeErrorClasses.Transient : SapSyncSafeErrorClasses.Terminal;
                        result = new(SapSyncExecutionDetailActions.Skip, status, null, null,
                            SapCityResultCodes.SaveFailed,
                            decision.IsRetryable && !decision.MoveToDeadLetter
                                ? "Reintento programado para la ciudad."
                                : "No fue posible procesar la ciudad.");
                    }

                    var write = await executionRepository.UpsertDetailAsync(new(
                        null, context.ExecutionUid, snapshot.ExternalCode,
                        Convert.ToHexString(hash), result.LocalCityId, result.LocalGlobalId,
                        result.Action, result.Status, 1, context.MaxAttempts, nextAttempt,
                        errorClass, result.ResultCode, result.SafeMessage,
                        SapSyncApprovedSnapshotTypes.CityV1, json, hash, started,
                        result.Status == SapSyncExecutionDetailStatuses.RetryScheduled ? null : DateTime.UtcNow,
                        null), cancellationToken);
                    if (write.Id is null)
                        throw new InvalidOperationException($"SAP_SYNC_DETAIL_{write.ResultCode}");
                    results.Add(new(result.Status, result.ResultCode, nextAttempt));
                    if (!context.ContinueOnError && result.Status is SapSyncExecutionDetailStatuses.Failed
                            or SapSyncExecutionDetailStatuses.RetryScheduled
                            or SapSyncExecutionDetailStatuses.DeadLetter)
                    {
                        stop = true;
                        break;
                    }
                }
                if (stop) break;
            }

            current = await GetRequiredAsync(context.ExecutionUid, cancellationToken);
            if (await CompleteCancellationAsync(current, results, cancellationToken)) return;
            await TransitionAsync(current, FinalStatus(results), results,
                LastErrorCode(results), LastErrorCode(results) is null ? null : "Una o mas ciudades no pudieron procesarse.",
                NextAttempt(results), cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            await CloseInterruptedAsync(context.ExecutionUid, results);
            throw;
        }
        catch (Exception ex)
        {
            var current = await executionRepository.GetByExecutionUidAsync(context.ExecutionUid, CancellationToken.None);
            if (current is null) throw;
            if (current.Status == SapSyncExecutionStatuses.Cancelling)
            {
                await TransitionAsync(current, SapSyncExecutionStatuses.Cancelled, results,
                    null, null, null, CancellationToken.None);
                return;
            }
            if (current.Status == SapSyncExecutionStatuses.Running)
                await TransitionAsync(current, SapSyncExecutionStatuses.Failed, results,
                    "SAP_CITY_EXECUTION_FAILED", $"La ejecucion de ciudades fallo: {ex.GetType().Name}.",
                    null, CancellationToken.None);
            throw;
        }
    }

    private async Task<SapSyncExecutionDto> EnsureRunningAsync(
        SapSyncScheduledExecutionContext context, CancellationToken cancellationToken)
    {
        var current = await executionRepository.GetByExecutionUidAsync(context.ExecutionUid, cancellationToken);
        if (current is null)
        {
            var create = await executionRepository.CreateAsync(new(
                context.ExecutionUid, context.ExecutionUid, ToGuid(context.CorrelationId),
                context.ProfileId, context.ProfileEntityId, context.ProfileCode, context.ProfileName,
                context.CompanyId, context.CompanyCode, context.EntityCode, context.Direction.ToString(),
                SapSyncTriggerTypes.Scheduled, null, context.BatchSize, context.MaxAttempts,
                context.ExecutionOrder, context.ExecutionTimeoutMinutes, context.ScheduleType,
                context.TimeZoneId,
                JsonSerializer.Serialize(new { context.ProfileCode, context.ProfileName, context.EntityCode,
                    Direction = context.Direction.ToString(), context.SyncMode }, JsonOptions),
                JsonSerializer.Serialize(new { context.BatchSize, context.MaxAttempts,
                    context.ExecutionOrder, context.ContinueOnError, context.ExecutionTimeoutMinutes }, JsonOptions),
                null, null, context.WorkerInstance), cancellationToken);
            if (create.Id is null || create.RowVersion is null)
                throw new InvalidOperationException($"SAP_SYNC_EXECUTION_{create.ResultCode}");
            var started = await executionRepository.TransitionAsync(
                State(context.ExecutionUid, SapSyncExecutionStatuses.Pending,
                    SapSyncExecutionStatuses.Running, [], create.RowVersion), cancellationToken);
            if (started.Id is null) throw new InvalidOperationException($"SAP_SYNC_EXECUTION_{started.ResultCode}");
            current = await executionRepository.GetByExecutionUidAsync(context.ExecutionUid, cancellationToken);
        }
        if (current is null) throw new InvalidOperationException("SAP_SYNC_EXECUTION_NOT_FOUND");
        if (current.Status == SapSyncExecutionStatuses.Pending)
        {
            var started = await executionRepository.TransitionAsync(
                State(context.ExecutionUid, current.Status, SapSyncExecutionStatuses.Running,
                    [], current.RowVersion), cancellationToken);
            if (started.Id is null) throw new InvalidOperationException($"SAP_SYNC_EXECUTION_{started.ResultCode}");
            current = await GetRequiredAsync(context.ExecutionUid, cancellationToken);
        }
        if (current.Status is not (SapSyncExecutionStatuses.Running or SapSyncExecutionStatuses.Cancelling))
            throw new InvalidOperationException("SAP_SYNC_EXECUTION_NOT_RUNNABLE");
        return current;
    }

    private async Task<SapSyncExecutionDto> GetRequiredAsync(Guid executionUid, CancellationToken cancellationToken) =>
        await executionRepository.GetByExecutionUidAsync(executionUid, cancellationToken)
        ?? throw new InvalidOperationException("SAP_SYNC_EXECUTION_NOT_FOUND");

    private async Task<bool> CompleteCancellationAsync(
        SapSyncExecutionDto current, IReadOnlyCollection<PersistedResult> results,
        CancellationToken cancellationToken)
    {
        if (current.Status != SapSyncExecutionStatuses.Cancelling) return false;
        await TransitionAsync(current, SapSyncExecutionStatuses.Cancelled, results,
            null, null, null, cancellationToken);
        return true;
    }

    private async Task CloseInterruptedAsync(Guid executionUid, IReadOnlyCollection<PersistedResult> results)
    {
        try
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var current = await executionRepository.GetByExecutionUidAsync(executionUid, timeout.Token);
            if (current?.Status == SapSyncExecutionStatuses.Running)
            {
                await TransitionAsync(current, SapSyncExecutionStatuses.Cancelling, results,
                    "SAP_CITY_EXECUTION_INTERRUPTED",
                    "La ejecucion de ciudades fue interrumpida de forma controlada.", null, timeout.Token);
                current = await executionRepository.GetByExecutionUidAsync(executionUid, timeout.Token);
            }
            if (current?.Status == SapSyncExecutionStatuses.Cancelling)
                await TransitionAsync(current, SapSyncExecutionStatuses.Cancelled, results,
                    "SAP_CITY_EXECUTION_INTERRUPTED",
                    "La ejecucion de ciudades fue interrumpida de forma controlada.", null, timeout.Token);
        }
        catch
        {
            // La cancelacion original conserva prioridad.
        }
    }

    private async Task TransitionAsync(
        SapSyncExecutionDto current, string newStatus,
        IReadOnlyCollection<PersistedResult> results, string? errorCode,
        string? errorMessage, DateTime? nextAttemptAtUtc,
        CancellationToken cancellationToken)
    {
        var write = await executionRepository.TransitionAsync(
            State(current.ExecutionUid, current.Status, newStatus, results,
                current.RowVersion, errorCode, errorMessage, nextAttemptAtUtc), cancellationToken);
        if (write.Id is null) throw new InvalidOperationException($"SAP_SYNC_EXECUTION_{write.ResultCode}");
    }

    private static SapSyncExecutionStateData State(
        Guid uid, string expected, string next,
        IReadOnlyCollection<PersistedResult> results, byte[] rowVersion,
        string? errorCode = null, string? errorMessage = null,
        DateTime? nextAttempt = null) =>
        new(uid, expected, next, results.Count,
            Count(results, SapSyncExecutionDetailStatuses.Created),
            Count(results, SapSyncExecutionDetailStatuses.Updated),
            Count(results, SapSyncExecutionDetailStatuses.Unchanged),
            Count(results, SapSyncExecutionDetailStatuses.ApprovalRequired),
            Count(results, SapSyncExecutionDetailStatuses.Conflict),
            Count(results, SapSyncExecutionDetailStatuses.Skipped),
            Count(results, SapSyncExecutionDetailStatuses.RetryScheduled),
            Count(results, SapSyncExecutionDetailStatuses.Failed),
            Count(results, SapSyncExecutionDetailStatuses.DeadLetter),
            errorCode, errorMessage, nextAttempt, rowVersion);

    private static string FinalStatus(IReadOnlyCollection<PersistedResult> results)
    {
        var failed = Count(results, SapSyncExecutionDetailStatuses.Failed)
            + Count(results, SapSyncExecutionDetailStatuses.DeadLetter);
        if (Count(results, SapSyncExecutionDetailStatuses.RetryScheduled) > 0)
            return SapSyncExecutionStatuses.RetryScheduled;
        if (results.Count > 0 && failed == results.Count) return SapSyncExecutionStatuses.Failed;
        if (failed > 0) return SapSyncExecutionStatuses.CompletedWithErrors;
        if (results.Any(x => x.Status is SapSyncExecutionDetailStatuses.ApprovalRequired
                or SapSyncExecutionDetailStatuses.Conflict
                or SapSyncExecutionDetailStatuses.Skipped))
            return SapSyncExecutionStatuses.CompletedWithWarnings;
        return SapSyncExecutionStatuses.Completed;
    }

    private static int Count(IEnumerable<PersistedResult> rows, string status) => rows.Count(x => x.Status == status);
    private static string? LastErrorCode(IEnumerable<PersistedResult> rows) =>
        rows.LastOrDefault(x => x.Status is SapSyncExecutionDetailStatuses.Failed
            or SapSyncExecutionDetailStatuses.DeadLetter)?.ResultCode;
    private static DateTime? NextAttempt(IEnumerable<PersistedResult> rows) =>
        rows.Where(x => x.Status == SapSyncExecutionDetailStatuses.RetryScheduled)
            .Select(x => x.NextAttemptAtUtc).Where(x => x is not null).Min();
    private static Guid ToGuid(string value)
    {
        if (Guid.TryParse(value, out var parsed)) return parsed;
        return new Guid(MD5.HashData(Encoding.UTF8.GetBytes(value)));
    }

    private sealed record PersistedResult(string Status, string ResultCode, DateTime? NextAttemptAtUtc);
}
