using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.Roles.Commands;

public sealed record AssignRolePermissionCommand(int RoleId, int PermissionId) : ICommand<bool>;
