using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.SecurityRoles.Models;

namespace NuanSystem.WinForms.Services.SecurityRoles;

public sealed class SecurityRoleClient(INuanApiClient apiClient) : ISecurityRoleClient
{
    public async Task<IReadOnlyCollection<SecurityRoleItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<SecurityRoleItem>>("/api/security/roles", cancellationToken);
    }

    public Task<SecurityRoleItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<SecurityRoleItem>($"/api/security/roles/{id}", cancellationToken);
    }

    public Task<SecurityRoleItem> CreateAsync(SaveSecurityRoleRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveSecurityRoleRequest, SecurityRoleItem>("/api/security/roles", request, cancellationToken);
    }

    public Task<SecurityRoleItem> UpdateAsync(int id, SaveSecurityRoleRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveSecurityRoleRequest, SecurityRoleItem>($"/api/security/roles/{id}", request, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/security/roles/{id}", cancellationToken);
    }
}
