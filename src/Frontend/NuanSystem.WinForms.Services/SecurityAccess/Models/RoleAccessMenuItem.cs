namespace NuanSystem.WinForms.Services.SecurityAccess.Models;

public sealed record RoleAccessMenuItem(
    int MenuId,
    int? ParentId,
    string Code,
    string Name,
    int MenuType,
    string? FormKey,
    bool IsAllowed);
