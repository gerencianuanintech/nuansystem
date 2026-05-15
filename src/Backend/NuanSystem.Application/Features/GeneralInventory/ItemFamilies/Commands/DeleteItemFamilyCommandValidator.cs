using FluentValidation;

namespace NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Commands;

public sealed class DeleteItemFamilyCommandValidator : AbstractValidator<DeleteItemFamilyCommand>
{
    public DeleteItemFamilyCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
    }
}
