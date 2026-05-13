namespace NuanSystem.Application.Features.Roles.Dtos;

public sealed record PermissionDto(
    int Id,
    string ModuleCode,
    string ModuleName,
    string Code,
    string Name,
    string? Description,
    bool IsActive);
