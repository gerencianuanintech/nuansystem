using NuanSystem.Application.Features.Roles.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IRoleAdminRepository
{
    Task<IReadOnlyCollection<RoleAdminDto>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PermissionDto>> GetPermissionsAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<int> CreateRoleAsync(CreateRoleData role, CancellationToken cancellationToken = default);
    Task AssignPermissionAsync(int roleId, int permissionId, CancellationToken cancellationToken = default);
    Task<RoleAdminDto?> GetRoleByIdAsync(int id, CancellationToken cancellationToken = default);
}
