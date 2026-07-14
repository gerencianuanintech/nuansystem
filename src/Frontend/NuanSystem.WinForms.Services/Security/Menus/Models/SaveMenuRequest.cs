namespace NuanSystem.WinForms.Services.Security.Menus.Models;

public sealed record SaveMenuRequest(
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
