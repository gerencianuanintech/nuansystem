using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Commands;

public sealed record SaveRoleAccessCommand(
    int RoleId,
    IReadOnlyCollection<SaveRoleAccessMenuData> Menus,
    IReadOnlyCollection<SaveRoleAccessOperationData> Operations,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;
