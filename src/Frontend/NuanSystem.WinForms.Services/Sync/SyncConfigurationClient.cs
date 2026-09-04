using System.Globalization;
using System.Text;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Sync.Models;

namespace NuanSystem.WinForms.Services.Sync;

public sealed class SyncConfigurationClient(INuanApiClient apiClient) : ISyncConfigurationClient
{
    private const string BasePath = "/api/sync/configuration";

    public Task<PagedResult<SyncProfileListItem>> SearchProfilesAsync(SyncProfileListFilter filter, CancellationToken cancellationToken = default)
    {
        var query = new QueryBuilder()
            .Add("search", filter.Search)
            .Add("companyId", filter.CompanyId)
            .Add("isActive", filter.IsActive)
            .Add("executionMode", filter.ExecutionMode)
            .Add("pageNumber", filter.PageNumber)
            .Add("pageSize", filter.PageSize)
            .ToString();

        return apiClient.GetAsync<PagedResult<SyncProfileListItem>>($"{BasePath}/profiles{query}", cancellationToken);
    }

    public Task<SyncProfileDetail> GetProfileAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<SyncProfileDetail>($"{BasePath}/profiles/{id}", cancellationToken);
    }

    public Task<SyncConfigurationCatalog> GetCatalogAsync(CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<SyncConfigurationCatalog>($"{BasePath}/catalog", cancellationToken);
    }

    public async Task<SyncProfileDetail> CreateProfileAsync(SaveSyncProfileRequest request, CancellationToken cancellationToken = default)
    {
        var id = await apiClient.PostAsync<SaveSyncProfileRequest, int>($"{BasePath}/profiles", request, cancellationToken);
        return await GetProfileAsync(id, cancellationToken);
    }

    public async Task<SyncProfileDetail> UpdateProfileAsync(int id, SaveSyncProfileRequest request, CancellationToken cancellationToken = default)
    {
        await apiClient.PutAsync<SaveSyncProfileRequest, bool>($"{BasePath}/profiles/{id}", request, cancellationToken);
        return await GetProfileAsync(id, cancellationToken);
    }

    public async Task DeleteProfileAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"{BasePath}/profiles/{id}", cancellationToken);
    }

    public Task<SyncProfileValidationResult> ValidateProfileAsync(SaveSyncProfileRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveSyncProfileRequest, SyncProfileValidationResult>($"{BasePath}/profiles/validate", request, cancellationToken);
    }

    public Task<SyncProfileValidationResult> ValidatePersistedProfileAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<object, SyncProfileValidationResult>($"{BasePath}/profiles/{id}/validate", new { }, cancellationToken);
    }

    public async Task<SyncProfileDetail> ActivateProfileAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.PostAsync<object, bool>($"{BasePath}/profiles/{id}/activate", new { }, cancellationToken);
        return await GetProfileAsync(id, cancellationToken);
    }

    public async Task<SyncProfileDetail> DeactivateProfileAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.PostAsync<object, bool>($"{BasePath}/profiles/{id}/deactivate", new { }, cancellationToken);
        return await GetProfileAsync(id, cancellationToken);
    }

    public Task<CreateSyncProfileExecutionResult> ExecuteProfileAsync(int id, ExecuteSyncProfileRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<ExecuteSyncProfileRequest, CreateSyncProfileExecutionResult>($"{BasePath}/profiles/{id}/execute", request, cancellationToken);
    }

    public Task<PagedResult<SyncProfileExecutionListItem>> SearchExecutionsAsync(SyncProfileExecutionFilter filter, CancellationToken cancellationToken = default)
    {
        var query = new QueryBuilder()
            .Add("profileId", filter.ProfileId)
            .Add("status", filter.Status)
            .Add("executionType", filter.ExecutionType)
            .Add("dateFrom", filter.DateFrom)
            .Add("dateTo", filter.DateTo)
            .Add("pageNumber", filter.PageNumber)
            .Add("pageSize", filter.PageSize)
            .ToString();

        return apiClient.GetAsync<PagedResult<SyncProfileExecutionListItem>>($"{BasePath}/executions{query}", cancellationToken);
    }

    public Task<SyncProfileExecutionDetail> GetExecutionAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<SyncProfileExecutionDetail>($"{BasePath}/executions/{id}", cancellationToken);
    }

    public Task<CancelSyncProfileExecutionResult> CancelExecutionAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<object, CancelSyncProfileExecutionResult>($"{BasePath}/executions/{id}/cancel", new { }, cancellationToken);
    }

    public Task<RetrySyncProfileExecutionResult> RetryExecutionAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<object, RetrySyncProfileExecutionResult>($"{BasePath}/executions/{id}/retry", new { }, cancellationToken);
    }

    public Task<SyncDistributionPolicy> GetDistributionPolicyAsync(int matrixId, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<SyncDistributionPolicy>($"{BasePath}/distribution-policies/{matrixId}", cancellationToken);
    }

    public Task<SyncDistributionPolicyCatalog> GetDistributionPolicyCatalogAsync(
        string entityCode,
        CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<SyncDistributionPolicyCatalog>(
            $"{BasePath}/distribution-policies/catalog/{Uri.EscapeDataString(entityCode)}",
            cancellationToken);
    }

    public Task<IReadOnlyCollection<SyncDistributionCandidate>> SearchDistributionCandidatesAsync(
        int matrixId,
        string? search,
        int take = 100,
        CancellationToken cancellationToken = default)
    {
        var query = new QueryBuilder()
            .Add("search", search)
            .Add("take", take)
            .ToString();
        return apiClient.GetAsync<IReadOnlyCollection<SyncDistributionCandidate>>(
            $"{BasePath}/distribution-policies/{matrixId}/candidates{query}",
            cancellationToken);
    }

    public async Task UpdateDistributionPolicyAsync(
        int matrixId,
        SaveSyncDistributionPolicyRequest request,
        CancellationToken cancellationToken = default)
    {
        await apiClient.PutAsync<SaveSyncDistributionPolicyRequest, bool>(
            $"{BasePath}/distribution-policies/{matrixId}",
            request,
            cancellationToken);
    }

    public Task<BusinessPartnerSapCodePolicy> GetBusinessPartnerSapCodePolicyAsync(
        CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<BusinessPartnerSapCodePolicy>(
            "/api/sap/settings/business-partner-codes",
            cancellationToken);
    }

    public Task<BusinessPartnerSapCodePolicy> UpdateBusinessPartnerSapCodePolicyAsync(
        SaveBusinessPartnerSapCodePolicyRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveBusinessPartnerSapCodePolicyRequest, BusinessPartnerSapCodePolicy>(
            "/api/sap/settings/business-partner-codes",
            request,
            cancellationToken);
    }

    private sealed class QueryBuilder
    {
        private readonly StringBuilder builder = new();
        private bool hasValues;

        public QueryBuilder Add(string name, object? value)
        {
            if (value is null)
            {
                return this;
            }

            var text = value switch
            {
                string stringValue => stringValue,
                DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O", CultureInfo.InvariantCulture),
                DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
                bool boolean => boolean ? "true" : "false",
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                _ => Convert.ToString(value, CultureInfo.InvariantCulture)
            };

            if (string.IsNullOrWhiteSpace(text))
            {
                return this;
            }

            builder.Append(hasValues ? '&' : '?');
            builder.Append(Uri.EscapeDataString(name));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(text));
            hasValues = true;
            return this;
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
}
