using FluentValidation;

namespace NuanSystem.Application.Features.SecurityRoles.Commands;

public sealed class DeleteSecurityRoleCommandValidator : AbstractValidator<DeleteSecurityRoleCommand>
{
    public DeleteSecurityRoleCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0);
    }
}
