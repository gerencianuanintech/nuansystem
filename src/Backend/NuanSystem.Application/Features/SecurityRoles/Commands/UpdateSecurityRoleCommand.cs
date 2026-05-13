using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityRoles.Dtos;

namespace NuanSystem.Application.Features.SecurityRoles.Commands;

public sealed record UpdateSecurityRoleCommand(
    int Id,
    string Code,
    string Name,
    string? Description,
    int DisplayOrder,
    bool IsSystemRole,
    bool IsAssignable,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<SecurityRoleDto>;
