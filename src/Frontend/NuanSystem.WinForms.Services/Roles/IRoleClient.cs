using NuanSystem.WinForms.Services.Roles.Models;

namespace NuanSystem.WinForms.Services.Roles;

public interface IRoleClient
{
    Task<IReadOnlyCollection<RoleAdminItem>> GetAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PermissionItem>> GetPermissionsAsync(CancellationToken cancellationToken = default);
    Task<RoleAdminItem> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);
    Task<bool> AssignPermissionAsync(AssignRolePermissionRequest request, CancellationToken cancellationToken = default);
}
