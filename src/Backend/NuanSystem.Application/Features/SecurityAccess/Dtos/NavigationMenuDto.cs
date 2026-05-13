namespace NuanSystem.Application.Features.SecurityAccess.Dtos;

public sealed record NavigationMenuDto(
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
