using NuanSystem.WinForms.Services.Roles;
using NuanSystem.WinForms.Services.Roles.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Roles;

public sealed class RolesViewModel(IRoleClient roleClient) : ViewModelBase
{
    private IReadOnlyCollection<RoleAdminItem> roles = Array.Empty<RoleAdminItem>();
    private IReadOnlyCollection<PermissionItem> permissions = Array.Empty<PermissionItem>();
    private bool isBusy;

    public IReadOnlyCollection<RoleAdminItem> Roles
    {
        get => roles;
        private set => SetProperty(ref roles, value);
    }

    public IReadOnlyCollection<PermissionItem> Permissions
    {
        get => permissions;
        private set => SetProperty(ref permissions, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        private set => SetProperty(ref isBusy, value);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try
        {
            Roles = await roleClient.GetAsync(cancellationToken);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task LoadPermissionsAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try
        {
            Permissions = (await roleClient.GetPermissionsAsync(cancellationToken)).Where(permission => permission.IsActive).ToArray();
        }
        finally
        {
            IsBusy = false;
        }
    }

    public Task<RoleAdminItem> CreateAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        return roleClient.CreateAsync(request, cancellationToken);
    }

    public Task AssignPermissionAsync(int roleId, int permissionId, CancellationToken cancellationToken = default)
    {
        return roleClient.AssignPermissionAsync(new AssignRolePermissionRequest(roleId, permissionId), cancellationToken);
    }
}
