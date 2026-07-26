using FluentValidation;

namespace NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;

public sealed class DeleteWarehouseCommandValidator : AbstractValidator<DeleteWarehouseCommand>
{
    public DeleteWarehouseCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
    }
}
