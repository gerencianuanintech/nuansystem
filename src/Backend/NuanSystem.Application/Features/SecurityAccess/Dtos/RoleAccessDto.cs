namespace NuanSystem.Application.Features.SecurityAccess.Dtos;

public sealed record RoleAccessDto(
    IReadOnlyCollection<RoleAccessMenuDto> Menus,
    IReadOnlyCollection<RoleAccessOperationDto> Operations);
