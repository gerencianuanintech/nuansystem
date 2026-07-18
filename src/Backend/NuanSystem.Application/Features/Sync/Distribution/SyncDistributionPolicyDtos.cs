using System.Text.Json;

namespace NuanSystem.Application.Features.Sync.Distribution;

public sealed record SyncDistributionSelectionDto(Guid EntityGlobalId, string? EntityCode);

public sealed record SyncDistributionCandidateDto(
    Guid EntityGlobalId,
    string EntityCode,
    string EntityName,
    bool IsActive);

public sealed record SyncDistributionPolicyDto(
    int SyncProfileEntityBranchId,
    int SyncProfileId,
    string SyncProfileCode,
    int CompanyId,
    string CompanyCode,
    string EntityCode,
    int BranchCompanyId,
    string BranchCompanyCode,
    string BranchCompanyName,
    string DistributionMode,
    string OnNoMatch,
    string? RuleExpressionJson,
    int RuleVersion,
    IReadOnlyCollection<SyncDistributionSelectionDto> Selections);

public sealed record SaveSyncDistributionPolicyRequest
{
    public string DistributionMode { get; init; } = "None";
    public string OnNoMatch { get; init; } = "KeepInMaster";
    public string? RuleExpressionJson { get; init; }
    public IReadOnlyCollection<SyncDistributionSelectionDto> Selections { get; init; } = [];
}

public sealed record PreviewSyncDistributionPolicyRequest(
    Guid EntityGlobalId,
    string? EntityCode,
    JsonElement Facts);

public sealed record SyncDistributionPolicyPreviewDto(
    bool Matched,
    int BranchCompanyId,
    string DistributionMode,
    string Reason,
    int RuleVersion);

public sealed record SyncDistributionPolicyCatalogDto(
    IReadOnlyCollection<string> Modes,
    IReadOnlyCollection<string> OnNoMatchActions,
    IReadOnlyCollection<string> Operators,
    IReadOnlyCollection<string> Fields);

public sealed record UpdateSyncDistributionPolicyData(
    int SyncProfileEntityBranchId,
    string DistributionMode,
    string OnNoMatch,
    string? RuleExpressionJson,
    IReadOnlyCollection<SyncDistributionSelectionDto> Selections,
    int? AuditUserId,
    string? AuditUserName);
