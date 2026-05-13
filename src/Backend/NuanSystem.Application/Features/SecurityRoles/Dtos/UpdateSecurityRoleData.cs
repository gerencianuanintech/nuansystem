namespace NuanSystem.Application.Features.SecurityRoles.Dtos;

public sealed record UpdateSecurityRoleData(
    int Id,
    string Code,
    string Name,
    string? Description,
    int DisplayOrder,
    bool IsSystemRole,
    bool IsAssignable,
    bool IsActive,
    int? UpdatedByUserId,
    string? UpdatedByUserName);
