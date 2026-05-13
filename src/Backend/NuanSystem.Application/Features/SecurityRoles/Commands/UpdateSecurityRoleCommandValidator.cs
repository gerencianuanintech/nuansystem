using FluentValidation;

namespace NuanSystem.Application.Features.SecurityRoles.Commands;

public sealed class UpdateSecurityRoleCommandValidator : AbstractValidator<UpdateSecurityRoleCommand>
{
    public UpdateSecurityRoleCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0);

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
