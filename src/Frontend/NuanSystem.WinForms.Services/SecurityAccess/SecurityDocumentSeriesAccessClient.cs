using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.SecurityAccess.Models;

namespace NuanSystem.WinForms.Services.SecurityAccess;

public sealed class SecurityDocumentSeriesAccessClient(INuanApiClient apiClient) : ISecurityDocumentSeriesAccessClient
{
    private const string BasePath = "/api/security/document-series-access";

    public async Task<IReadOnlyCollection<SecurityDocumentSeriesAccessItem>> GetSeriesAsync(
        int roleId,
        string formKey,
        string? search,
        string? documentType,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>
        {
            $"formKey={Uri.EscapeDataString(formKey)}"
        };

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

        return await apiClient.GetAsync<List<SecurityDocumentSeriesAccessItem>>(
            $"{BasePath}/roles/{roleId}/series?{string.Join("&", query)}",
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<SecurityDocumentSeriesOperationAccessItem>> GetOperationsAsync(
        int roleId,
        int seriesId,
        string formKey,
        string documentType,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>
        {
            $"formKey={Uri.EscapeDataString(formKey)}",
            $"documentType={Uri.EscapeDataString(documentType)}",
            $"onlyActive={(onlyActive ? "true" : "false")}"
        };

        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Add($"search={Uri.EscapeDataString(search.Trim())}");
        }

        return await apiClient.GetAsync<List<SecurityDocumentSeriesOperationAccessItem>>(
            $"{BasePath}/roles/{roleId}/series/{seriesId}/operations?{string.Join("&", query)}",
            cancellationToken);
    }

    public Task<bool> SaveAsync(
        int roleId,
        int seriesId,
        string formKey,
        string documentType,
        SaveSecurityDocumentSeriesAccessRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = string.Join(
            "&",
            $"formKey={Uri.EscapeDataString(formKey)}",
            $"documentType={Uri.EscapeDataString(documentType)}");

        return apiClient.PutAsync<SaveSecurityDocumentSeriesAccessRequest, bool>(
            $"{BasePath}/roles/{roleId}/series/{seriesId}/operations?{query}",
            request,
            cancellationToken);
    }
}
