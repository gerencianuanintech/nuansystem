namespace NuanSystem.Application.Features.Sync.Dtos;

public sealed record SyncRoutingContext(
    int SourceCompanyId,
    string EntityCode,
    int? SyncProfileId = null,
    string? TargetBranchCode = null,
    bool RequireTargetBranchMatch = false,
    Guid? EntityGlobalId = null,
    string? PayloadJson = null);

public sealed record SyncRoutingTargetDto(
    int SyncProfileId,
    int SyncProfileEntityId,
    string SyncProfileCode,
    int SourceCompanyId,
    int BranchCompanyId,
    string EntityCode,
    int BatchSize,
    int MaxRetries,
    int RetryDelaySeconds,
    int TimeoutMinutes,
    bool AllowInsert,
    bool AllowUpdate,
    bool AllowDeactivate,
    bool ContinueOnError,
    int SyncProfileEntityBranchId = 0,
    string DistributionMode = "All",
    string OnNoMatch = "KeepInMaster",
    string? RuleExpressionJson = null,
    int RuleVersion = 1,
    bool IsSelected = false);

public sealed record SyncDistributionDecisionDto(
    int SyncProfileEntityBranchId,
    int BranchCompanyId,
    string DistributionMode,
    bool Matched,
    string Reason,
    int RuleVersion);

public sealed record SyncRoutingEvaluationResult(
    bool ShouldDistribute,
    IReadOnlyCollection<SyncRoutingTargetDto> Targets,
    string? Reason = null,
    IReadOnlyCollection<SyncDistributionDecisionDto>? Decisions = null);

public sealed record SyncRoutingConflictCheckItem(
    string EntityCode,
    int BranchCompanyId);

public sealed record SyncRoutingConflictDto(
    int SyncProfileId,
    string SyncProfileCode,
    int BranchCompanyId,
    string EntityCode);
