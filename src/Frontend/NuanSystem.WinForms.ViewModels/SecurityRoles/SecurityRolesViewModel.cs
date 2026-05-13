using NuanSystem.WinForms.Services.SecurityRoles;
using NuanSystem.WinForms.Services.SecurityRoles.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.SecurityRoles;

public sealed class SecurityRolesViewModel(ISecurityRoleClient roleClient)
    : CrudViewModel<SecurityRoleItem, SaveSecurityRoleRequest>
{
    public IReadOnlyCollection<SecurityRoleItem> Roles => Items;

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(roleClient.GetAsync, cancellationToken);
    }

    public Task<SecurityRoleItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return roleClient.GetByIdAsync(id, cancellationToken);
    }

    public override Task CreateAsync(SaveSecurityRoleRequest request, CancellationToken cancellationToken = default)
    {
        return roleClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveSecurityRoleRequest request, CancellationToken cancellationToken = default)
    {
        return roleClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return roleClient.DeleteAsync(id, cancellationToken);
    }
}
