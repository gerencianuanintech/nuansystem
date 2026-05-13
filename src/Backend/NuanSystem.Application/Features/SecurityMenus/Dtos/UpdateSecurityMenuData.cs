namespace NuanSystem.Application.Features.SecurityMenus.Dtos;

public sealed record UpdateSecurityMenuData(
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
    bool IsActive,
    int? UpdatedByUserId,
    string? UpdatedByUserName);
