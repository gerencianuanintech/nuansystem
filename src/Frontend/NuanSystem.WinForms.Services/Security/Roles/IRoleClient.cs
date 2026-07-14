using NuanSystem.WinForms.Services.Security.Roles.Models;

namespace NuanSystem.WinForms.Services.Security.Roles;

public interface IRoleClient
{
    Task<IReadOnlyCollection<RoleItem>> GetAsync(CancellationToken cancellationToken = default);
    Task<RoleItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RoleItem> CreateAsync(SaveRoleRequest request, CancellationToken cancellationToken = default);
    Task<RoleItem> UpdateAsync(int id, SaveRoleRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
