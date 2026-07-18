using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.Sync.Distribution;

public sealed record GetSyncDistributionPolicyQuery(int MatrixId, int? UserId) : IQuery<SyncDistributionPolicyDto>;
public sealed record GetSyncDistributionPolicyCatalogQuery(string EntityCode) : IQuery<SyncDistributionPolicyCatalogDto>;
public sealed record GetSyncDistributionCandidatesQuery(
    int MatrixId,
    string? Search,
    int Take,
    int? UserId) : IQuery<IReadOnlyCollection<SyncDistributionCandidateDto>>;
public sealed record UpdateSyncDistributionPolicyCommand(
    int MatrixId,
    SaveSyncDistributionPolicyRequest Request,
    int? AuditUserId,
    string? AuditUserName) : ICommand<bool>;
public sealed record PreviewSyncDistributionPolicyQuery(
    int MatrixId,
    PreviewSyncDistributionPolicyRequest Request,
    int? UserId) : IQuery<SyncDistributionPolicyPreviewDto>;
