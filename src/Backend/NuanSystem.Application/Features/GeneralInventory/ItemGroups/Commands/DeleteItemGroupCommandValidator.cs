using FluentValidation;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;

public sealed class DeleteItemGroupCommandValidator : AbstractValidator<DeleteItemGroupCommand>
{
    public DeleteItemGroupCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
    }
}
