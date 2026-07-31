using System.Security.Cryptography;
using System.Text;
using NuanSystem.Application.Abstractions.SapSync;

namespace NuanSystem.Application.Features.SapSync.Executions;

public sealed class SapSyncExecutionRetryService(
    ISapSyncExecutionRepository repository,
    IEnumerable<ISapSyncExecutionRetryProcessor> processors,
    ISapSyncRetryPolicy retryPolicy) : ISapSyncExecutionRetryService
{
    private readonly IReadOnlyDictionary<string, ISapSyncExecutionRetryProcessor> _processors = processors
        .GroupBy(x => x.ApprovedSnapshotType, StringComparer.Ordinal)
        .ToDictionary(x => x.Key, x => x.Single(), StringComparer.Ordinal);

    public async Task<SapSyncRetryCycleResult> ProcessNextAsync(
        string workerInstance,
        TimeSpan lockTimeout,
        int backoffSeconds,
        CancellationToken cancellationToken = default)
    {
        if (_processors.Count == 0)
        {
            return new(SapSyncRetryCycleResult.Idle);
        }

        var ownerToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        var claim = await repository.TryClaimDueDetailAsync(
            workerInstance,
            ownerToken,
            DateTime.UtcNow.Add(lockTimeout),
            _processors.Keys.ToArray(),
            cancellationToken);
        if (claim is null)
        {
            return new(SapSyncRetryCycleResult.Idle);
        }

        try
        {
            if (!TryValidateSnapshot(claim) ||
                !_processors.TryGetValue(claim.ApprovedSnapshotType!, out var processor))
            {
                await CompleteAsync(claim, "DeadLetter", "SAP_SNAPSHOT_INVALID", "Snapshot no aprobado.", null, cancellationToken);
                return new(SapSyncRetryCycleResult.DeadLetter, claim.Id);
            }

            try
            {
                var result = await processor.ProcessAsync(claim, cancellationToken);
                await repository.CompleteClaimedDetailAsync(new(
                    claim.Id, claim.OwnerToken, result.Action, result.Status,
                    result.LocalEntityId, result.LocalGlobalId, null,
                    result.ResultCode, result.SafeMessage, null), cancellationToken);
                return new(SapSyncRetryCycleResult.Completed, claim.Id);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                var decision = retryPolicy.Evaluate(
                    exception.GetType().Name, null, exception,
                    claim.AttemptCount, claim.MaxAttempts,
                    Math.Max(1, backoffSeconds), DateTime.UtcNow);
                var status = decision.MoveToDeadLetter ? "DeadLetter" : "RetryScheduled";
                await CompleteAsync(
                    claim, status, "SAP_RETRY_PROCESSING_FAILED",
                    decision.MoveToDeadLetter ? "Reintentos agotados." : "Reintento programado.",
                    decision.NextAttemptAtUtc, cancellationToken);
                return new(status, claim.Id);
            }
        }
        catch
        {
            await repository.ReleaseDetailLockAsync(claim.Id, claim.OwnerToken, CancellationToken.None);
            throw;
        }
    }

    private Task<SapSyncExecutionWriteResult> CompleteAsync(
        SapSyncExecutionDetailClaim claim,
        string status,
        string resultCode,
        string safeMessage,
        DateTime? nextAttemptAtUtc,
        CancellationToken cancellationToken) =>
        repository.CompleteClaimedDetailAsync(new(
            claim.Id, claim.OwnerToken, "Skip", status, null, null,
            status == "RetryScheduled" ? "Transient" : "Terminal",
            resultCode, safeMessage, nextAttemptAtUtc), cancellationToken);

    private static bool TryValidateSnapshot(SapSyncExecutionDetailClaim claim)
    {
        if (string.IsNullOrWhiteSpace(claim.ApprovedSnapshotType) ||
            string.IsNullOrWhiteSpace(claim.ApprovedSnapshotJson) ||
            claim.SnapshotHash is not { Length: 32 })
        {
            return false;
        }

        var calculated = SHA256.HashData(Encoding.UTF8.GetBytes(claim.ApprovedSnapshotJson));
        return CryptographicOperations.FixedTimeEquals(calculated, claim.SnapshotHash);
    }
}
