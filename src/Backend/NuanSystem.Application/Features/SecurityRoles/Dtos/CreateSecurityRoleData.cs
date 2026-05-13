namespace NuanSystem.Application.Features.SecurityRoles.Dtos;

public sealed record CreateSecurityRoleData(
    string Code,
    string Name,
    string? Description,
    int DisplayOrder,
    bool IsSystemRole,
    bool IsAssignable,
    bool IsActive,
    int? CreatedByUserId,
    string? CreatedByUserName);
