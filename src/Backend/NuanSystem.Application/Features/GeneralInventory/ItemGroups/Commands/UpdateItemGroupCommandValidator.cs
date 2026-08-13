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
        RuleFor(command => command.IncomeAccountCode).MaximumLength(120);
        RuleFor(command => command.CostOfSalesAccountCode).MaximumLength(120);
        RuleFor(command => command.SalesReturnAccountCode).MaximumLength(120);
        RuleFor(command => command.PurchaseReturnAccountCode).MaximumLength(120);
        RuleFor(command => command.CostVarianceAccountCode).MaximumLength(120);
        RuleFor(command => command.InventoryAdjustmentAccountCode).MaximumLength(120);
        RuleFor(command => command.PurchaseExpenseAccountCode).MaximumLength(120);
        RuleFor(command => command.SortOrder).GreaterThanOrEqualTo(0);
        RuleFor(command => command.SapGroupCode).MaximumLength(100);
        RuleFor(command => command.SapCode).MaximumLength(50);
    }
}
