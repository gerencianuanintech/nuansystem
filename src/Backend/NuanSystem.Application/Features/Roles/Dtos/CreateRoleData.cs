namespace NuanSystem.Application.Features.Roles.Dtos;

public sealed record CreateRoleData(
    string Code,
    string Name,
    string? Description,
    bool IsActive);
