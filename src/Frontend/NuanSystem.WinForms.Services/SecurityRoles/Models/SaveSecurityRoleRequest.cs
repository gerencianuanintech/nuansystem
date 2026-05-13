namespace NuanSystem.WinForms.Services.SecurityRoles.Models;

public sealed record SaveSecurityRoleRequest(
    string Code,
    string Name,
    string? Description,
    int DisplayOrder,
    bool IsSystemRole,
    bool IsAssignable,
    bool IsActive);
