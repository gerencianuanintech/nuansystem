namespace NuanSystem.WinForms.Services.Security.Roles.Models;

public sealed record SaveRoleRequest(
    string Code,
    string Name,
    string? Description,
    int DisplayOrder,
    bool IsSystemRole,
    bool IsAssignable,
    bool IsActive);
