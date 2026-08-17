using FluentValidation;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Commands;

internal static class ItemSubgroupValidationRules
{
    public static void Apply<T>(AbstractValidator<T> validator, Func<T, int> itemFamilyId,
        Func<T, string> code, Func<T, string> name, Func<T, string?> description, Func<T, int> sortOrder)
    {
        validator.RuleFor(x => itemFamilyId(x)).GreaterThan(0).WithName("ItemFamilyId");
        validator.RuleFor(x => code(x)).NotEmpty().MaximumLength(50).WithName("Code");
        validator.RuleFor(x => name(x)).NotEmpty().MaximumLength(150).WithName("Name");
        validator.RuleFor(x => description(x)).MaximumLength(500).WithName("Description");
        validator.RuleFor(x => sortOrder(x)).GreaterThanOrEqualTo(0).WithName("SortOrder");
    }
}

public sealed class CreateItemSubgroupCommandValidator : AbstractValidator<CreateItemSubgroupCommand>
{
    public CreateItemSubgroupCommandValidator() => ItemSubgroupValidationRules.Apply(
        this, x => x.ItemFamilyId, x => x.Code, x => x.Name, x => x.Description, x => x.SortOrder);
}

public sealed class UpdateItemSubgroupCommandValidator : AbstractValidator<UpdateItemSubgroupCommand>
{
    public UpdateItemSubgroupCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        ItemSubgroupValidationRules.Apply(
            this, x => x.ItemFamilyId, x => x.Code, x => x.Name, x => x.Description, x => x.SortOrder);
    }
}

public sealed class DeleteItemSubgroupCommandValidator : AbstractValidator<DeleteItemSubgroupCommand>
{
    public DeleteItemSubgroupCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}
