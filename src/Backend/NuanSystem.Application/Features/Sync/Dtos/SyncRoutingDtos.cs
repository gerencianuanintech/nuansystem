namespace NuanSystem.Application.Features.Sync.Dtos;

public sealed record SyncRoutingContext(
    int SourceCompanyId,
    string EntityCode,
    int? SyncProfileId = null);

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
    bool ContinueOnError);

public sealed record SyncRoutingEvaluationResult(
    bool ShouldDistribute,
    IReadOnlyCollection<SyncRoutingTargetDto> Targets,
    string? Reason = null);

public sealed record SyncRoutingConflictCheckItem(
    string EntityCode,
    int BranchCompanyId);

public sealed record SyncRoutingConflictDto(
    int SyncProfileId,
    string SyncProfileCode,
    int BranchCompanyId,
    string EntityCode);
