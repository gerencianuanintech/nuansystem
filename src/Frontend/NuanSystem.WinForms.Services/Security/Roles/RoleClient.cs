using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Security.Roles.Models;

namespace NuanSystem.WinForms.Services.Security.Roles;

public sealed class RoleClient(INuanApiClient apiClient) : IRoleClient
{
    public async Task<IReadOnlyCollection<RoleItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<RoleItem>>("/api/security/roles", cancellationToken);
    }

    public Task<RoleItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<RoleItem>($"/api/security/roles/{id}", cancellationToken);
    }

    public Task<RoleItem> CreateAsync(SaveRoleRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveRoleRequest, RoleItem>("/api/security/roles", request, cancellationToken);
    }

    public Task<RoleItem> UpdateAsync(int id, SaveRoleRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveRoleRequest, RoleItem>($"/api/security/roles/{id}", request, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/security/roles/{id}", cancellationToken);
    }
}
