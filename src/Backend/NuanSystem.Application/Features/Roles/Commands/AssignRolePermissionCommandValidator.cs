using FluentValidation;

namespace NuanSystem.Application.Features.Roles.Commands;

public sealed class AssignRolePermissionCommandValidator : AbstractValidator<AssignRolePermissionCommand>
{
    public AssignRolePermissionCommandValidator()
    {
        RuleFor(command => command.RoleId).GreaterThan(0);
        RuleFor(command => command.PermissionId).GreaterThan(0);
    }
}
