using NuanSystem.WinForms.Services.SecurityRoles.Models;

namespace NuanSystem.WinForms.Services.SecurityRoles;

public interface ISecurityRoleClient
{
    Task<IReadOnlyCollection<SecurityRoleItem>> GetAsync(CancellationToken cancellationToken = default);
    Task<SecurityRoleItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SecurityRoleItem> CreateAsync(SaveSecurityRoleRequest request, CancellationToken cancellationToken = default);
    Task<SecurityRoleItem> UpdateAsync(int id, SaveSecurityRoleRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
