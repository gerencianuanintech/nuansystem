namespace NuanSystem.WinForms.Services.Security.Users.Models;

public sealed record RoleItem(
    int Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive);

