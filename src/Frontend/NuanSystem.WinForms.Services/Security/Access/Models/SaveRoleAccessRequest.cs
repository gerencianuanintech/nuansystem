namespace NuanSystem.WinForms.Services.Security.Access.Models;

public sealed record SaveRoleAccessRequest(
    int RoleId,
    IReadOnlyCollection<SaveRoleAccessMenuRequest> Menus,
    IReadOnlyCollection<SaveRoleAccessOperationRequest> Operations);
