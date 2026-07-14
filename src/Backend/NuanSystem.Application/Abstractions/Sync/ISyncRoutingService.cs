using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncRoutingService
{
    Task<SyncRoutingEvaluationResult> ResolveTargetsAsync(
        SyncRoutingContext context,
        CancellationToken cancellationToken = default);
}
