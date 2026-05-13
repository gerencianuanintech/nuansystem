namespace NuanSystem.WinForms.Services.SecurityUsers.Models;

public sealed record RoleItem(
    int Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive);

