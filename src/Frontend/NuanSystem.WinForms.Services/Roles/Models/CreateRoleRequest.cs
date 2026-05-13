namespace NuanSystem.WinForms.Services.Roles.Models;

public sealed record CreateRoleRequest(
    string Code,
    string Name,
    string? Description,
    bool IsActive);
