using FluentValidation;

namespace NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Commands;

public sealed class CreateItemFamilyCommandValidator : AbstractValidator<CreateItemFamilyCommand>
{
    public CreateItemFamilyCommandValidator()
    {
        RuleFor(command => command.ItemGroupId).GreaterThan(0);
        RuleFor(command => command.Code).NotEmpty().MaximumLength(50);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(150);
        RuleFor(command => command.Description).MaximumLength(500);
        RuleFor(command => command.SapFamilyCode).MaximumLength(100);
        RuleFor(command => command.SapCode).MaximumLength(50);
    }
}
