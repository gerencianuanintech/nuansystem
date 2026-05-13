using FluentValidation;

namespace NuanSystem.Application.Features.SecurityForms.Commands;

public sealed class UpdateSecurityFormCommandValidator : AbstractValidator<UpdateSecurityFormCommand>
{
    public UpdateSecurityFormCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.Code).NotEmpty().MaximumLength(80);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(120);
        RuleFor(command => command.Description).MaximumLength(300);
        RuleFor(command => command.FormKey).NotEmpty().MaximumLength(120);
        RuleFor(command => command.FormType).InclusiveBetween(1, 5);
    }
}
