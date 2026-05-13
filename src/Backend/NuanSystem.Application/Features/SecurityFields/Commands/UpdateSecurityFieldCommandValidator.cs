using FluentValidation;

namespace NuanSystem.Application.Features.SecurityFields.Commands;

public sealed class UpdateSecurityFieldCommandValidator : AbstractValidator<UpdateSecurityFieldCommand>
{
    public UpdateSecurityFieldCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.FormId).GreaterThan(0);
        RuleFor(command => command.Code).NotEmpty().MaximumLength(80);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(120);
        RuleFor(command => command.FieldKey).NotEmpty().MaximumLength(120);
        RuleFor(command => command.Description).MaximumLength(300);
        RuleFor(command => command.ControlType).NotEmpty().MaximumLength(60);
        RuleFor(command => command.DataType).NotEmpty().MaximumLength(40);
        RuleFor(command => command.ValidationMessage).MaximumLength(300);
        RuleFor(command => command.DisplayOrder).GreaterThanOrEqualTo(0);
    }
}
