using FluentValidation;

namespace NuanSystem.Application.Features.SecurityRoles.Commands;

public sealed class CreateSecurityRoleCommandValidator : AbstractValidator<CreateSecurityRoleCommand>
{
    public CreateSecurityRoleCommandValidator()
    {
        RuleFor(command => command.Code)
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(command => command.Description)
            .MaximumLength(300);

        RuleFor(command => command.DisplayOrder)
            .GreaterThanOrEqualTo(0);
    }
}
