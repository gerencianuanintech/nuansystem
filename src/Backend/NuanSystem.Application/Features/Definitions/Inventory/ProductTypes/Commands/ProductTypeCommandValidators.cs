using FluentValidation;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Commands;

internal static class ProductTypeValidationRules
{
    public static void Apply<T>(AbstractValidator<T> validator, Func<T, string> code, Func<T, string> name,
        Func<T, string?> description, Func<T, string> natureCode, Func<T, int> sortOrder)
    {
        validator.RuleFor(x => code(x)).NotEmpty().MaximumLength(50).WithName("Code");
        validator.RuleFor(x => name(x)).NotEmpty().MaximumLength(150).WithName("Name");
        validator.RuleFor(x => description(x)).MaximumLength(500).WithName("Description");
        validator.RuleFor(x => natureCode(x)).NotEmpty()
            .Must(value => !string.IsNullOrWhiteSpace(value) && ProductTypeNatureCodes.All.Contains(value.Trim()))
            .WithMessage("NatureCode no es valido.").WithName("NatureCode");
        validator.RuleFor(x => sortOrder(x)).GreaterThanOrEqualTo(0).WithName("SortOrder");
    }
}

public sealed class CreateProductTypeCommandValidator : AbstractValidator<CreateProductTypeCommand>
{
    public CreateProductTypeCommandValidator() => ProductTypeValidationRules.Apply(this,
        x => x.Code, x => x.Name, x => x.Description, x => x.NatureCode, x => x.SortOrder);
}

public sealed class UpdateProductTypeCommandValidator : AbstractValidator<UpdateProductTypeCommand>
{
    public UpdateProductTypeCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        ProductTypeValidationRules.Apply(this, x => x.Code, x => x.Name,
            x => x.Description, x => x.NatureCode, x => x.SortOrder);
    }
}

public sealed class DeleteProductTypeCommandValidator : AbstractValidator<DeleteProductTypeCommand>
{
    public DeleteProductTypeCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}
