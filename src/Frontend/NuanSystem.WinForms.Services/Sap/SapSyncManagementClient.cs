using System.Globalization;
using System.Text;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Sap.Models;

namespace NuanSystem.WinForms.Services.Sap;

public sealed class SapSyncManagementClient(INuanApiClient apiClient) : ISapSyncManagementClient
{
    private const string ProfilePath = "/api/sap/sync-profiles";
    private const string ExecutionPath = "/api/sap/sync-executions";

    public Task<SapPagedResult<SapSyncProfileListItem>> SearchProfilesAsync(SapSyncProfileListFilter filter, CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<SapPagedResult<SapSyncProfileListItem>>($"{ProfilePath}{Query.ForProfiles(filter)}", cancellationToken);

    public Task<SapSyncProfileDetail> GetProfileAsync(long id, CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<SapSyncProfileDetail>($"{ProfilePath}/{id}", cancellationToken);

    public Task<SapSyncProfileCatalog> GetCatalogAsync(CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<SapSyncProfileCatalog>($"{ProfilePath}/catalog", cancellationToken);

    public async Task<SapSyncProfileDetail> CreateProfileAsync(SaveSapSyncProfileRequest request, CancellationToken cancellationToken = default)
    {
        var created = await apiClient.PostAsync<SaveSapSyncProfileRequest, SapSyncProfileWriteResult>(ProfilePath, request, cancellationToken);
        return await GetProfileAsync(created.Id, cancellationToken);
    }

    public async Task<SapSyncProfileDetail> UpdateProfileAsync(long id, UpdateSapSyncProfileRequest request, CancellationToken cancellationToken = default)
    {
        await apiClient.PutAsync<UpdateSapSyncProfileRequest, SapSyncProfileWriteResult>($"{ProfilePath}/{id}", request, cancellationToken);
        return await GetProfileAsync(id, cancellationToken);
    }

    public async Task DeleteProfileAsync(long id, byte[] rowVersion, CancellationToken cancellationToken = default) =>
        _ = await apiClient.DeleteAsync<SapSyncProfileVersionRequest, SapSyncProfileWriteResult>($"{ProfilePath}/{id}", new(rowVersion), cancellationToken);

    public Task<SapSyncProfileValidationResult> ValidateProfileAsync(long id, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<object, SapSyncProfileValidationResult>($"{ProfilePath}/{id}/validate", new { }, cancellationToken);

    public async Task<SapSyncProfileDetail> ActivateProfileAsync(long id, byte[] rowVersion, CancellationToken cancellationToken = default)
    {
        await apiClient.PostAsync<SapSyncProfileVersionRequest, SapSyncProfileWriteResult>($"{ProfilePath}/{id}/activate", new(rowVersion), cancellationToken);
        return await GetProfileAsync(id, cancellationToken);
    }

    public async Task<SapSyncProfileDetail> DeactivateProfileAsync(long id, byte[] rowVersion, CancellationToken cancellationToken = default)
    {
        await apiClient.PostAsync<SapSyncProfileVersionRequest, SapSyncProfileWriteResult>($"{ProfilePath}/{id}/deactivate", new(rowVersion), cancellationToken);
        return await GetProfileAsync(id, cancellationToken);
    }

    public Task<SapPagedResult<SapSyncExecutionListItem>> SearchExecutionsAsync(SapSyncExecutionFilter filter, CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<SapPagedResult<SapSyncExecutionListItem>>($"{ExecutionPath}/{Query.ForExecutions(filter)}", cancellationToken);

    public Task<SapSyncExecutionDetail> GetExecutionAsync(Guid executionUid, CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<SapSyncExecutionDetail>($"{ExecutionPath}/{executionUid:D}", cancellationToken);

    public Task<SapPagedResult<SapSyncExecutionDetailItem>> SearchExecutionDetailsAsync(SapSyncExecutionDetailFilter filter, CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<SapPagedResult<SapSyncExecutionDetailItem>>($"{ExecutionPath}/{filter.ExecutionUid:D}/details{Query.ForExecutionDetails(filter)}", cancellationToken);

    public Task<SapSyncRetryResult> RetryExecutionAsync(Guid executionUid, byte[] rowVersion, string reason, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<SapSyncRetryRequest, SapSyncRetryResult>($"{ExecutionPath}/{executionUid:D}/retry", new(Guid.NewGuid(), reason.Trim(), rowVersion), cancellationToken);

    public async Task CancelExecutionAsync(Guid executionUid, byte[] rowVersion, CancellationToken cancellationToken = default) =>
        _ = await apiClient.PostAsync<SapSyncVersionRequest, bool>($"{ExecutionPath}/{executionUid:D}/cancel", new(rowVersion), cancellationToken);

    public async Task ReleaseExpiredLockAsync(long detailId, byte[] rowVersion, string reason, CancellationToken cancellationToken = default) =>
        _ = await apiClient.PostAsync<SapSyncReleaseLockRequest, bool>($"{ExecutionPath}/details/{detailId}/release-expired-lock", new(reason.Trim(), rowVersion), cancellationToken);

    private static class Query
    {
        public static string ForProfiles(SapSyncProfileListFilter value) => new Builder()
            .Add("companyId", value.CompanyId).Add("search", value.Search).Add("isActive", value.IsActive)
            .Add("entityCode", value.EntityCode).Add("pageNumber", value.PageNumber).Add("pageSize", value.PageSize).ToString();

        public static string ForExecutions(SapSyncExecutionFilter value) => new Builder()
            .Add("profileId", value.ProfileId).Add("entityCode", value.EntityCode).Add("direction", value.Direction)
            .Add("status", value.Status).Add("triggerType", value.TriggerType).Add("dateFromUtc", value.DateFromUtc)
            .Add("dateToUtc", value.DateToUtc).Add("pageNumber", value.PageNumber).Add("pageSize", value.PageSize).ToString();

        public static string ForExecutionDetails(SapSyncExecutionDetailFilter value) => new Builder()
            .Add("status", value.Status).Add("sourceRecordKey", value.SourceRecordKey)
            .Add("pageNumber", value.PageNumber).Add("pageSize", value.PageSize).ToString();

        private sealed class Builder
        {
            private readonly StringBuilder text = new();
            private bool hasValue;
            public Builder Add(string name, object? value)
            {
                if (value is null || value is string stringValue && string.IsNullOrWhiteSpace(stringValue)) return this;
                var serialized = value switch
                {
                    bool boolean => boolean ? "true" : "false",
                    DateTime date => date.ToString("O", CultureInfo.InvariantCulture),
                    IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                    _ => Convert.ToString(value, CultureInfo.InvariantCulture)
                };
                text.Append(hasValue ? '&' : '?').Append(Uri.EscapeDataString(name)).Append('=').Append(Uri.EscapeDataString(serialized ?? string.Empty));
                hasValue = true;
                return this;
            }
            public override string ToString() => text.ToString();
        }
    }
}
