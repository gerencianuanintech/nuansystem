namespace NuanSystem.Application.Features.SecurityUsers.Dtos;

public sealed record RoleDto(
    int Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive);

