namespace NuanSystem.Application.Features.SecurityAccess.Dtos;

public sealed record RoleAccessMenuDto(
    int MenuId,
    int? ParentId,
    string Code,
    string Name,
    int MenuType,
    string? FormKey,
    bool IsAllowed);
