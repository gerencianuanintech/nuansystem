namespace NuanSystem.Application.Features.Sync.Dtos;

public sealed record SyncRuleEvaluationContext(
    int CompanyId,
    string EntityName,
    Guid EntityGlobalId,
    string? EntityCode,
    string? PayloadJson);

public sealed record SyncRuleEvaluationResult(
    bool ShouldDistribute,
    IReadOnlyCollection<int> BranchCompanyIds,
    string? Reason = null);

public sealed record SyncEventApplyContext(
    Guid EventId,
    int SourceCompanyId,
    string EntityName,
    Guid EntityGlobalId,
    string Operation,
    string PayloadJson,
    int? TargetCompanyId = null,
    long? TargetId = null);

public sealed record SyncEventApplyResult(
    bool Applied,
    string? Message = null,
    string? ErrorCode = null,
    bool Retryable = false);
