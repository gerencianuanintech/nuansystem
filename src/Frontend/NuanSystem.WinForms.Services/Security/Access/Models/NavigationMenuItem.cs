namespace NuanSystem.WinForms.Services.Security.Access.Models;

public sealed record NavigationMenuItem(
    int Id,
    int? ParentId,
    string Code,
    string Name,
    string? Description,
    int MenuType,
    string? FormKey,
    string? IconLarge,
    string? IconSmall,
    int DisplayOrder,
    bool IsVisible,
    bool IsActive);
