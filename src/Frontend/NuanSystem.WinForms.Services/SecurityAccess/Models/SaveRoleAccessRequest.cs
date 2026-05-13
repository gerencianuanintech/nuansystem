namespace NuanSystem.WinForms.Services.SecurityAccess.Models;

public sealed record SaveRoleAccessRequest(
    int RoleId,
    IReadOnlyCollection<SaveRoleAccessMenuRequest> Menus,
    IReadOnlyCollection<SaveRoleAccessOperationRequest> Operations);
