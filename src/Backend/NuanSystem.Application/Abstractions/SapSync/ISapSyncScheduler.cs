using NuanSystem.Application.Features.SapSync.Scheduling;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncScheduler
{
    Task<SapSyncPollResult> PollAsync(
        SapSyncScheduleCursor cursor,
        int pageSize,
        string workerInstance,
        CancellationToken cancellationToken = default);
}

public interface ISapSyncScheduledExecutionPreparer
{
    Task PrepareAsync(
        SapSyncScheduledExecutionContext context,
        CancellationToken cancellationToken = default);
}

public interface ISapSyncScheduledExecutionProcessor
{
    string EntityCode { get; }

    Task ProcessAsync(
        SapSyncScheduledExecutionContext context,
        CancellationToken cancellationToken = default);
}

public interface ISapSyncExecutionLeaseCoordinator
{
    Task<SapSyncLeaseExecutionResult> PrepareAsync(
        SapSyncScheduledExecutionContext context,
        string workerInstance,
        TimeSpan lockTimeout,
        TimeSpan renewalInterval,
        CancellationToken cancellationToken = default);
}
