using FluentValidation;

namespace NuanSystem.Application.Features.SecurityMenus.Commands;

public sealed class CreateSecurityMenuCommandValidator : AbstractValidator<CreateSecurityMenuCommand>
{
    public CreateSecurityMenuCommandValidator()
    {
        RuleFor(command => command.Code).NotEmpty().MaximumLength(80);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(120);
        RuleFor(command => command.Description).MaximumLength(300);
        RuleFor(command => command.MenuType).InclusiveBetween(1, 3);
        RuleFor(command => command.FormKey).MaximumLength(120);
        RuleFor(command => command.IconLarge).MaximumLength(200);
        RuleFor(command => command.IconSmall).MaximumLength(200);
    }
}
