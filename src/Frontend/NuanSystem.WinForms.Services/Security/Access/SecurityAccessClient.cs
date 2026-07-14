using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.Services.Security.Access;

public sealed class SecurityAccessClient(INuanApiClient apiClient) : ISecurityAccessClient
{
    public async Task<IReadOnlyCollection<NavigationMenuItem>> GetNavigationAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<NavigationMenuItem>>("/api/security/navigation/me", cancellationToken);
    }

    public async Task<IReadOnlyCollection<FormOperationAccessItem>> GetFormOperationsAsync(string formKey, CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<FormOperationAccessItem>>($"/api/security/forms/{Uri.EscapeDataString(formKey)}/operations/me", cancellationToken);
    }

    public Task<RoleAccessItem> GetRoleAccessAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<RoleAccessItem>($"/api/security/roles/{roleId}/access", cancellationToken);
    }

    public Task<bool> SaveRoleAccessAsync(int roleId, SaveRoleAccessRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveRoleAccessRequest, bool>($"/api/security/roles/{roleId}/access", request, cancellationToken);
    }
}
