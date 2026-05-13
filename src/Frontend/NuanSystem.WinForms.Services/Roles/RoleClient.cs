using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Roles.Models;

namespace NuanSystem.WinForms.Services.Roles;

public sealed class RoleClient(INuanApiClient apiClient) : IRoleClient
{
    public async Task<IReadOnlyCollection<RoleAdminItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<RoleAdminItem>>("/api/roles", cancellationToken);
    }

    public async Task<IReadOnlyCollection<PermissionItem>> GetPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<PermissionItem>>("/api/roles/permissions", cancellationToken);
    }

    public Task<RoleAdminItem> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<CreateRoleRequest, RoleAdminItem>("/api/roles", request, cancellationToken);
    }

    public Task<bool> AssignPermissionAsync(AssignRolePermissionRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<AssignRolePermissionRequest, bool>("/api/roles/assign-permission", request, cancellationToken);
    }
}
