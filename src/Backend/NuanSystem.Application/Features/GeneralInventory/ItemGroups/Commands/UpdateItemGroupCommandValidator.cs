using FluentValidation;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;

public sealed class UpdateItemGroupCommandValidator : AbstractValidator<UpdateItemGroupCommand>
{
    public UpdateItemGroupCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.Code).NotEmpty().MaximumLength(50);
        RuleFor(command => command.Name).NotEmpty().MaximumLength(150);
        RuleFor(command => command.Description).MaximumLength(500);
        RuleFor(command => command.InventoryAccountCode).MaximumLength(120);
        RuleFor(command => command.CostOfSalesAccountCode).MaximumLength(120);
        RuleFor(command => command.SalesAccountCode).MaximumLength(120);
        RuleFor(command => command.PurchaseAccountCode).MaximumLength(120);
        RuleFor(command => command.SapGroupCode).MaximumLength(100);
        RuleFor(command => command.SapCode).MaximumLength(50);
    }
}
