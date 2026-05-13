using FluentValidation;

namespace NuanSystem.Application.Features.SecurityOperations.Commands;

public sealed class UpdateSecurityOperationCommandValidator : AbstractValidator<UpdateSecurityOperationCommand>
{
    public UpdateSecurityOperationCommandValidator()
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

        RuleFor(command => command.RibbonPageName)
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(command => command.RibbonGroupName)
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(command => command.ActionKey)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(command => command.IconLarge)
            .MaximumLength(200);

        RuleFor(command => command.IconSmall)
            .MaximumLength(200);
    }
}
