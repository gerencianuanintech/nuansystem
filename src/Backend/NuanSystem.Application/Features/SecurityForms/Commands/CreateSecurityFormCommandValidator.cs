using FluentValidation;

namespace NuanSystem.Application.Features.SecurityForms.Commands;

public sealed class CreateSecurityFormCommandValidator : AbstractValidator<CreateSecurityFormCommand>
{
    public CreateSecurityFormCommandValidator()
    {
        RuleFor(command => command.Code).NotEmpty().MaximumLength(80);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(120);
        RuleFor(command => command.Description).MaximumLength(300);
        RuleFor(command => command.FormKey).NotEmpty().MaximumLength(120);
        RuleFor(command => command.FormType).InclusiveBetween(1, 5);
    }
}
