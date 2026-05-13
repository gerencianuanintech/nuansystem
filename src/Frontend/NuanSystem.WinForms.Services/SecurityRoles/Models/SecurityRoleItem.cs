namespace NuanSystem.WinForms.Services.SecurityRoles.Models;

public sealed record SecurityRoleItem(
    int Id,
    string Code,
    string Name,
    string? Description,
    int DisplayOrder,
    bool IsSystemRole,
    bool IsAssignable,
    bool IsActive,
    IReadOnlyCollection<string> Permissions,
    int? CreatedByUserId,
    string? CreatedByUserName,
    DateTime CreatedAt,
    int? UpdatedByUserId,
    string? UpdatedByUserName,
    DateTime? UpdatedAt,
    int? DeletedByUserId,
    string? DeletedByUserName,
    DateTime? DeletedAt);
