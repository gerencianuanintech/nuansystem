using NuanSystem.Application.Features.SapSync.Executions;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncExecutionRetryProcessor
{
    string ApprovedSnapshotType { get; }

    Task<SapSyncExecutionRetryProcessResult> ProcessAsync(
        SapSyncExecutionDetailClaim claim,
        CancellationToken cancellationToken = default);
}

public interface ISapSyncExecutionRetryService
{
    Task<SapSyncRetryCycleResult> ProcessNextAsync(
        string workerInstance,
        TimeSpan lockTimeout,
        int backoffSeconds,
        CancellationToken cancellationToken = default);
}

public sealed record SapSyncRetryCycleResult(string Status, long? DetailId = null)
{
    public const string Idle = "Idle";
    public const string Completed = "Completed";
    public const string RetryScheduled = "RetryScheduled";
    public const string DeadLetter = "DeadLetter";
}
