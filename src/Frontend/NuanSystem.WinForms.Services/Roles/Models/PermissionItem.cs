namespace NuanSystem.WinForms.Services.Roles.Models;

public sealed record PermissionItem(
    int Id,
    string ModuleCode,
    string ModuleName,
    string Code,
    string Name,
    string? Description,
    bool IsActive);
