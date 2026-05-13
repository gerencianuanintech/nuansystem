namespace NuanSystem.WinForms.Services.SecurityMenus.Models;

public sealed record SaveSecurityMenuRequest(
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
