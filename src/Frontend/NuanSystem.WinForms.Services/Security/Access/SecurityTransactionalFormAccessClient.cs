using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.Services.Security.Access;

public sealed class SecurityTransactionalFormAccessClient(INuanApiClient apiClient) : ISecurityTransactionalFormAccessClient
{
    public async Task<IReadOnlyCollection<SecurityFormAccessFormItem>> GetFormsAsync(
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var path = $"/api/security/transactional-form-access/forms?onlyActive={onlyActive.ToString().ToLowerInvariant()}";

        if (!string.IsNullOrWhiteSpace(search))
        {
            path += $"&search={Uri.EscapeDataString(search.Trim())}";
        }

        return await apiClient.GetAsync<List<SecurityFormAccessFormItem>>(path, cancellationToken);
    }

    public async Task<IReadOnlyCollection<SecurityFormAccessOperationItem>> GetOperationsAsync(
        int roleId,
        int formId,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var path = $"/api/security/transactional-form-access/roles/{roleId}/forms/{formId}/operations?onlyActive={onlyActive.ToString().ToLowerInvariant()}";

        if (!string.IsNullOrWhiteSpace(search))
        {
            path += $"&search={Uri.EscapeDataString(search.Trim())}";
        }

        return await apiClient.GetAsync<List<SecurityFormAccessOperationItem>>(path, cancellationToken);
    }

    public Task<bool> SaveOperationsAsync(
        int roleId,
        int formId,
        SaveSecurityFormAccessRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveSecurityFormAccessRequest, bool>(
            $"/api/security/transactional-form-access/roles/{roleId}/forms/{formId}/operations",
            request,
            cancellationToken);
    }
}
