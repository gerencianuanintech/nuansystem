namespace NuanSystem.WinForms.Services.Roles.Models;

public sealed record RoleAdminItem(
    int Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    IReadOnlyCollection<string> Permissions);
