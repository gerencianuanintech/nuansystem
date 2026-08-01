using NuanSystem.WinForms.Services.Sap.Models;

namespace NuanSystem.WinForms.Services.Sap;

public interface ISapSyncManagementClient
{
    Task<SapPagedResult<SapSyncProfileListItem>> SearchProfilesAsync(SapSyncProfileListFilter filter, CancellationToken cancellationToken = default);
    Task<SapSyncProfileDetail> GetProfileAsync(long id, CancellationToken cancellationToken = default);
    Task<SapSyncProfileCatalog> GetCatalogAsync(CancellationToken cancellationToken = default);
    Task<SapSyncProfileDetail> CreateProfileAsync(SaveSapSyncProfileRequest request, CancellationToken cancellationToken = default);
    Task<SapSyncProfileDetail> UpdateProfileAsync(long id, UpdateSapSyncProfileRequest request, CancellationToken cancellationToken = default);
    Task DeleteProfileAsync(long id, byte[] rowVersion, CancellationToken cancellationToken = default);
    Task<SapSyncProfileValidationResult> ValidateProfileAsync(long id, CancellationToken cancellationToken = default);
    Task<SapSyncProfileDetail> ActivateProfileAsync(long id, byte[] rowVersion, CancellationToken cancellationToken = default);
    Task<SapSyncProfileDetail> DeactivateProfileAsync(long id, byte[] rowVersion, CancellationToken cancellationToken = default);
    Task<SapPagedResult<SapSyncExecutionListItem>> SearchExecutionsAsync(SapSyncExecutionFilter filter, CancellationToken cancellationToken = default);
    Task<SapSyncExecutionDetail> GetExecutionAsync(Guid executionUid, CancellationToken cancellationToken = default);
    Task<SapPagedResult<SapSyncExecutionDetailItem>> SearchExecutionDetailsAsync(SapSyncExecutionDetailFilter filter, CancellationToken cancellationToken = default);
    Task<SapSyncRetryResult> RetryExecutionAsync(Guid executionUid, byte[] rowVersion, string reason, CancellationToken cancellationToken = default);
    Task CancelExecutionAsync(Guid executionUid, byte[] rowVersion, CancellationToken cancellationToken = default);
    Task ReleaseExpiredLockAsync(long detailId, byte[] rowVersion, string reason, CancellationToken cancellationToken = default);
}
