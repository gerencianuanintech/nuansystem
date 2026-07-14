using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncRuleEvaluator
{
    Task<SyncRuleEvaluationResult> EvaluateAsync(SyncRuleEvaluationContext context, CancellationToken cancellationToken = default);
}
