using NuanSystem.WinForms.Services.Security.Roles;
using NuanSystem.WinForms.Services.Security.Roles.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Security.Roles;

public sealed class RolesViewModel(IRoleClient roleClient)
    : CrudViewModel<RoleItem, SaveRoleRequest>
{
    public IReadOnlyCollection<RoleItem> Roles => Items;

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(roleClient.GetAsync, cancellationToken);
    }

    public Task<RoleItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return roleClient.GetByIdAsync(id, cancellationToken);
    }

    public override Task CreateAsync(SaveRoleRequest request, CancellationToken cancellationToken = default)
    {
        return roleClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveRoleRequest request, CancellationToken cancellationToken = default)
    {
        return roleClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return roleClient.DeleteAsync(id, cancellationToken);
    }
}
