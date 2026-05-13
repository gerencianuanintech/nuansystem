using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Roles.Dtos;

namespace NuanSystem.Application.Features.Roles.Commands;

public sealed record CreateRoleCommand(
    string Code,
    string Name,
    string? Description,
    bool IsActive) : ICommand<RoleAdminDto>;
