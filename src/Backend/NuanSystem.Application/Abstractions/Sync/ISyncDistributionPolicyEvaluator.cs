using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncDistributionPolicyEvaluator
{
    SyncDistributionDecisionDto Evaluate(SyncRoutingTargetDto target, SyncRoutingContext context);
}
