using NuanSystem.WinForms.Services.Documents.SecurityDocumentSeries.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Documents.SecurityDocumentSeries;

public sealed class SecurityDocumentSeriesClient : ISecurityDocumentSeriesClient
{
    private const string BasePath = "/api/security-document-series";
    private readonly INuanApiClient apiClient;

    public SecurityDocumentSeriesClient(INuanApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<IReadOnlyCollection<SecurityDocumentSeriesItem>> GetAsync(
        string? search = null,
        string? documentType = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<SecurityDocumentSeriesItem>>(
            BuildPath(BasePath, search, documentType, isActive),
            cancellationToken);
    }

    public Task<SecurityDocumentSeriesItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<SecurityDocumentSeriesItem>($"{BasePath}/{id}", cancellationToken);
    }

    public async Task<IReadOnlyCollection<SecurityDocumentSeriesLookupItem>> GetLookupAsync(
        string? documentType = null,
        CancellationToken cancellationToken = default)
    {
        var path = string.IsNullOrWhiteSpace(documentType)
            ? $"{BasePath}/lookups"
            : $"{BasePath}/lookups?documentType={Uri.EscapeDataString(documentType.Trim())}";

        return await apiClient.GetAsync<List<SecurityDocumentSeriesLookupItem>>(path, cancellationToken);
    }

    public Task<SecurityDocumentSeriesItem> CreateAsync(
        SaveSecurityDocumentSeriesRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveSecurityDocumentSeriesRequest, SecurityDocumentSeriesItem>(
            BasePath,
            request,
            cancellationToken);
    }

    public Task<SecurityDocumentSeriesItem> UpdateAsync(
        int id,
        SaveSecurityDocumentSeriesRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveSecurityDocumentSeriesRequest, SecurityDocumentSeriesItem>(
            $"{BasePath}/{id}",
            request,
            cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"{BasePath}/{id}", cancellationToken);
    }

    public Task<ReserveSecurityDocumentNumberResult> ReserveNumberAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<object, ReserveSecurityDocumentNumberResult>(
            $"{BasePath}/{id}/reserve-number",
            new { },
            cancellationToken);
    }

    private static string BuildPath(string path, string? search, string? documentType, bool? isActive)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Add($"search={Uri.EscapeDataString(search.Trim())}");
        }

        if (!string.IsNullOrWhiteSpace(documentType))
        {
            query.Add($"documentType={Uri.EscapeDataString(documentType.Trim())}");
        }

        if (isActive.HasValue)
        {
            query.Add($"isActive={(isActive.Value ? "true" : "false")}");
        }

        return query.Count == 0 ? path : $"{path}?{string.Join("&", query)}";
    }
}
