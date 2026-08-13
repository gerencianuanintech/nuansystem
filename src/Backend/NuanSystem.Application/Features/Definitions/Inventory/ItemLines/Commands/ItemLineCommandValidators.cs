using FluentValidation;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Commands;

internal static class ItemLineValidationRules
{
    public static void Apply<T>(AbstractValidator<T> validator, Func<T, string> code, Func<T, string> name,
        Func<T, string?> description, Func<T, int> sortOrder)
    {
        validator.RuleFor(x => code(x)).NotEmpty().MaximumLength(50).WithName("Code");
        validator.RuleFor(x => name(x)).NotEmpty().MaximumLength(150).WithName("Name");
        validator.RuleFor(x => description(x)).MaximumLength(500).WithName("Description");
        validator.RuleFor(x => sortOrder(x)).GreaterThanOrEqualTo(0).WithName("SortOrder");
    }
}

public sealed class CreateItemLineCommandValidator : AbstractValidator<CreateItemLineCommand>
{
    public CreateItemLineCommandValidator() => ItemLineValidationRules.Apply(
        this, x => x.Code, x => x.Name, x => x.Description, x => x.SortOrder);
}

public sealed class UpdateItemLineCommandValidator : AbstractValidator<UpdateItemLineCommand>
{
    public UpdateItemLineCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        ItemLineValidationRules.Apply(this, x => x.Code, x => x.Name, x => x.Description, x => x.SortOrder);
    }
}

public sealed class DeleteItemLineCommandValidator : AbstractValidator<DeleteItemLineCommand>
{
    public DeleteItemLineCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}
