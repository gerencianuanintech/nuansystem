using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Features.Sync.Services;

public sealed class SyncRoutingService(
    ISyncRoutingRepository repository,
    ISyncDistributionPolicyEvaluator policyEvaluator) : ISyncRoutingService
{
    public Task RecordDecisionAsync(
        long outboxId,
        Guid entityGlobalId,
        SyncDistributionDecisionDto decision,
        CancellationToken cancellationToken = default)
        => repository.RecordDecisionAsync(outboxId, entityGlobalId, decision, cancellationToken);

    public async Task<SyncRoutingEvaluationResult> ResolveTargetsAsync(
        SyncRoutingContext context,
        CancellationToken cancellationToken = default)
    {
        if (context.SourceCompanyId <= 0)
        {
            return new SyncRoutingEvaluationResult(false, [], "La empresa origen no es valida para resolver routing.");
        }

        if (string.IsNullOrWhiteSpace(context.EntityCode))
        {
            return new SyncRoutingEvaluationResult(false, [], "La entidad no es valida para resolver routing.");
        }

        var targets = await repository.ResolveTargetsAsync(
            context with { EntityCode = context.EntityCode.Trim() },
            cancellationToken);

        var decisions = targets
            .Select(target => policyEvaluator.Evaluate(target, context))
            .ToArray();
        var matchedBranches = decisions
            .Where(decision => decision.Matched)
            .Select(decision => decision.BranchCompanyId)
            .ToHashSet();
        var distinctTargets = targets
            .Where(target => matchedBranches.Contains(target.BranchCompanyId))
            .GroupBy(target => target.BranchCompanyId)
            .Select(group => group.OrderBy(target => target.SyncProfileId).First())
            .OrderBy(target => target.BranchCompanyId)
            .ToArray();

        return distinctTargets.Length == 0
            ? new SyncRoutingEvaluationResult(false, distinctTargets, "No existen destinos que cumplan la politica de distribucion.", decisions)
            : new SyncRoutingEvaluationResult(true, distinctTargets, Decisions: decisions);
    }
}
