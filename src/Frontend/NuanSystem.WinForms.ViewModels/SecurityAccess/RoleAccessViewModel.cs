using NuanSystem.WinForms.Services.Roles;
using NuanSystem.WinForms.Services.Roles.Models;
using NuanSystem.WinForms.Services.SecurityAccess;
using NuanSystem.WinForms.Services.SecurityAccess.Models;

namespace NuanSystem.WinForms.ViewModels.SecurityAccess;

public sealed class RoleAccessViewModel(IRoleClient roleClient, ISecurityAccessClient securityAccessClient)
{
    public IReadOnlyCollection<RoleAdminItem> Roles { get; private set; } = Array.Empty<RoleAdminItem>();

    public RoleAccessItem? Access { get; private set; }

    public async Task LoadRolesAsync(CancellationToken cancellationToken = default)
    {
        Roles = await roleClient.GetAsync(cancellationToken);
    }

    public async Task LoadAccessAsync(int roleId, CancellationToken cancellationToken = default)
    {
        Access = await securityAccessClient.GetRoleAccessAsync(roleId, cancellationToken);
    }

    public Task SaveAsync(
        int roleId,
        IReadOnlyCollection<SaveRoleAccessMenuRequest> menus,
        IReadOnlyCollection<SaveRoleAccessOperationRequest> operations,
        CancellationToken cancellationToken = default)
    {
        var request = new SaveRoleAccessRequest(roleId, menus, operations);
        return securityAccessClient.SaveRoleAccessAsync(roleId, request, cancellationToken);
    }
}
