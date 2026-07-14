using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.Services.Security.Access;

public sealed class SecurityRoleFormFieldAccessClient(INuanApiClient apiClient, string basePath) : ISecurityRoleFormFieldAccessClient
{
    public async Task<IReadOnlyCollection<SecurityFormFieldAccessItem>> GetFieldsAsync(
        int roleId,
        int formId,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>
        {
            $"onlyActive={(onlyActive ? "true" : "false")}"
        };

        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Add($"search={Uri.EscapeDataString(search.Trim())}");
        }

        return await apiClient.GetAsync<List<SecurityFormFieldAccessItem>>(
            $"{basePath}/roles/{roleId}/forms/{formId}/fields?{string.Join("&", query)}",
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<SecurityFormFieldAccessItem>> GetDocumentSeriesFieldsAsync(
        int roleId,
        int formId,
        int seriesId,
        string documentType,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>
        {
            $"documentType={Uri.EscapeDataString(documentType.Trim())}",
            $"onlyActive={(onlyActive ? "true" : "false")}"
        };

        if (!string.IsNullOrWhiteSpace(search))
        {
            query.Add($"search={Uri.EscapeDataString(search.Trim())}");
        }

        return await apiClient.GetAsync<List<SecurityFormFieldAccessItem>>(
            $"{basePath}/roles/{roleId}/forms/{formId}/series/{seriesId}/fields?{string.Join("&", query)}",
            cancellationToken);
    }

    public Task<bool> SaveAsync(
        int roleId,
        int formId,
        SaveSecurityFormFieldAccessRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveSecurityFormFieldAccessRequest, bool>(
            $"{basePath}/roles/{roleId}/forms/{formId}/fields",
            request,
            cancellationToken);
    }

    public Task<bool> SaveDocumentSeriesAsync(
        int roleId,
        int formId,
        int seriesId,
        string documentType,
        SaveSecurityFormFieldAccessRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = $"documentType={Uri.EscapeDataString(documentType.Trim())}";

        return apiClient.PutAsync<SaveSecurityFormFieldAccessRequest, bool>(
            $"{basePath}/roles/{roleId}/forms/{formId}/series/{seriesId}/fields?{query}",
            request,
            cancellationToken);
    }
}
