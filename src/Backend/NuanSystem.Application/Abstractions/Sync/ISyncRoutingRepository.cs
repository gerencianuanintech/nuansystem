using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncRoutingRepository
{
    Task<IReadOnlyCollection<SyncRoutingTargetDto>> ResolveTargetsAsync(
        SyncRoutingContext context,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SyncRoutingConflictDto>> FindActiveConflictsAsync(
        int? profileId,
        int companyId,
        IReadOnlyCollection<SyncRoutingConflictCheckItem> combinations,
        CancellationToken cancellationToken = default);

    Task RecordDecisionAsync(
        long outboxId,
        Guid entityGlobalId,
        SyncDistributionDecisionDto decision,
        CancellationToken cancellationToken = default) => Task.CompletedTask;
}
