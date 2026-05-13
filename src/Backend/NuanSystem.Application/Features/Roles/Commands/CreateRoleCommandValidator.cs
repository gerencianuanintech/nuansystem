using FluentValidation;

namespace NuanSystem.Application.Features.Roles.Commands;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(command => command.Code)
            .NotEmpty()
            .MaximumLength(80)
            .Matches("^[A-Za-z0-9_.:-]+$");

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(command => command.Description)
            .MaximumLength(300)
            .When(command => !string.IsNullOrWhiteSpace(command.Description));
    }
}
