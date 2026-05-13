namespace NuanSystem.WinForms.Services.Roles.Models;

public sealed record AssignRolePermissionRequest(
    int RoleId,
    int PermissionId);
