using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.Services.Security.Access;

public interface ISecurityAccessClient
{
    Task<IReadOnlyCollection<NavigationMenuItem>> GetNavigationAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<FormOperationAccessItem>> GetFormOperationsAsync(string formKey, CancellationToken cancellationToken = default);

    Task<RoleAccessItem> GetRoleAccessAsync(int roleId, CancellationToken cancellationToken = default);

    Task<bool> SaveRoleAccessAsync(int roleId, SaveRoleAccessRequest request, CancellationToken cancellationToken = default);
}
