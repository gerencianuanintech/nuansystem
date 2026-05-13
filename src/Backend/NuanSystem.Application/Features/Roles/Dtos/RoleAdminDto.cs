namespace NuanSystem.Application.Features.Roles.Dtos;

public sealed record RoleAdminDto(
    int Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    IReadOnlyCollection<string> Permissions);
