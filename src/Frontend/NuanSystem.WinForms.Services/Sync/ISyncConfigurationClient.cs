using NuanSystem.WinForms.Services.Sync.Models;

namespace NuanSystem.WinForms.Services.Sync;

public interface ISyncConfigurationClient
{
    Task<PagedResult<SyncProfileListItem>> SearchProfilesAsync(SyncProfileListFilter filter, CancellationToken cancellationToken = default);

    Task<SyncProfileDetail> GetProfileAsync(int id, CancellationToken cancellationToken = default);

    Task<SyncConfigurationCatalog> GetCatalogAsync(CancellationToken cancellationToken = default);

    Task<SyncProfileDetail> CreateProfileAsync(SaveSyncProfileRequest request, CancellationToken cancellationToken = default);

    Task<SyncProfileDetail> UpdateProfileAsync(int id, SaveSyncProfileRequest request, CancellationToken cancellationToken = default);

    Task DeleteProfileAsync(int id, CancellationToken cancellationToken = default);

    Task<SyncProfileValidationResult> ValidateProfileAsync(SaveSyncProfileRequest request, CancellationToken cancellationToken = default);

    Task<SyncProfileValidationResult> ValidatePersistedProfileAsync(int id, CancellationToken cancellationToken = default);

    Task<SyncProfileDetail> ActivateProfileAsync(int id, CancellationToken cancellationToken = default);

    Task<SyncProfileDetail> DeactivateProfileAsync(int id, CancellationToken cancellationToken = default);

    Task<CreateSyncProfileExecutionResult> ExecuteProfileAsync(int id, ExecuteSyncProfileRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<SyncProfileExecutionListItem>> SearchExecutionsAsync(SyncProfileExecutionFilter filter, CancellationToken cancellationToken = default);

    Task<SyncProfileExecutionDetail> GetExecutionAsync(int id, CancellationToken cancellationToken = default);

    Task<CancelSyncProfileExecutionResult> CancelExecutionAsync(int id, CancellationToken cancellationToken = default);

    Task<RetrySyncProfileExecutionResult> RetryExecutionAsync(int id, CancellationToken cancellationToken = default);

    Task<SyncDistributionPolicy> GetDistributionPolicyAsync(int matrixId, CancellationToken cancellationToken = default);

    Task<SyncDistributionPolicyCatalog> GetDistributionPolicyCatalogAsync(string entityCode, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SyncDistributionCandidate>> SearchDistributionCandidatesAsync(
        int matrixId,
        string? search,
        int take = 100,
        CancellationToken cancellationToken = default);

    Task UpdateDistributionPolicyAsync(
        int matrixId,
        SaveSyncDistributionPolicyRequest request,
        CancellationToken cancellationToken = default);

    Task<BusinessPartnerSapCodePolicy> GetBusinessPartnerSapCodePolicyAsync(
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException("El cliente no implementa la política de códigos SAP.");

    Task<BusinessPartnerSapCodePolicy> UpdateBusinessPartnerSapCodePolicyAsync(
        SaveBusinessPartnerSapCodePolicyRequest request,
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException("El cliente no implementa la política de códigos SAP.");
}

public sealed class SyncProfileListFilter
{
    public string? Search { get; set; }
    public int? CompanyId { get; set; }
    public bool? IsActive { get; set; }
    public string? ExecutionMode { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public sealed class SyncProfileExecutionFilter
{
    public int? ProfileId { get; set; }
    public string? Status { get; set; }
    public string? ExecutionType { get; set; }
    public DateTimeOffset? DateFrom { get; set; }
    public DateTimeOffset? DateTo { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
