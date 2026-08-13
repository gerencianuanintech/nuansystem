using FluentValidation;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Commands;

internal static class ItemBrandValidationRules
{
    public static void Apply<T>(AbstractValidator<T> validator,
        Func<T, string> code, Func<T, string> name, Func<T, string?> description,
        Func<T, int> sortOrder, Func<T, string?> externalSystem,
        Func<T, string?> externalCode, Func<T, string?> sapManufacturerCode,
        Func<T, string?> sapCode)
    {
        validator.RuleFor(x => code(x)).NotEmpty().MaximumLength(50).WithName("Code");
        validator.RuleFor(x => name(x)).NotEmpty().MaximumLength(150).WithName("Name");
        validator.RuleFor(x => description(x)).MaximumLength(500).WithName("Description");
        validator.RuleFor(x => sortOrder(x)).GreaterThanOrEqualTo(0).WithName("SortOrder");
        validator.RuleFor(x => externalSystem(x)).MaximumLength(50).WithName("ExternalSystem")
            .NotEmpty().When(x => !string.IsNullOrWhiteSpace(externalCode(x)));
        validator.RuleFor(x => externalCode(x)).MaximumLength(100).WithName("ExternalCode")
            .NotEmpty().When(x => !string.IsNullOrWhiteSpace(externalSystem(x)));
        validator.RuleFor(x => sapManufacturerCode(x)).MaximumLength(50).WithName("SapManufacturerCode");
        validator.RuleFor(x => sapCode(x)).MaximumLength(50).WithName("SapCode");
    }
}

public sealed class CreateItemBrandCommandValidator : AbstractValidator<CreateItemBrandCommand>
{
    public CreateItemBrandCommandValidator() => ItemBrandValidationRules.Apply(
        this, x => x.Code, x => x.Name, x => x.Description, x => x.SortOrder,
        x => x.ExternalSystem, x => x.ExternalCode, x => x.SapManufacturerCode, x => x.SapCode);
}

public sealed class UpdateItemBrandCommandValidator : AbstractValidator<UpdateItemBrandCommand>
{
    public UpdateItemBrandCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        ItemBrandValidationRules.Apply(
            this, x => x.Code, x => x.Name, x => x.Description, x => x.SortOrder,
            x => x.ExternalSystem, x => x.ExternalCode, x => x.SapManufacturerCode, x => x.SapCode);
    }
}

public sealed class DeleteItemBrandCommandValidator : AbstractValidator<DeleteItemBrandCommand>
{
    public DeleteItemBrandCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}
