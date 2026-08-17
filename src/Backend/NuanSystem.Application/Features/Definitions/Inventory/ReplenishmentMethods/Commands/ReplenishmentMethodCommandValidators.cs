using FluentValidation;

namespace NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Commands;

internal static class ReplenishmentMethodValidationRules
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

public sealed class CreateReplenishmentMethodCommandValidator : AbstractValidator<CreateReplenishmentMethodCommand>
{
    public CreateReplenishmentMethodCommandValidator() => ReplenishmentMethodValidationRules.Apply(this, x => x.Code, x => x.Name, x => x.Description, x => x.SortOrder);
}

public sealed class UpdateReplenishmentMethodCommandValidator : AbstractValidator<UpdateReplenishmentMethodCommand>
{
    public UpdateReplenishmentMethodCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        ReplenishmentMethodValidationRules.Apply(this, x => x.Code, x => x.Name, x => x.Description, x => x.SortOrder);
    }
}

public sealed class DeleteReplenishmentMethodCommandValidator : AbstractValidator<DeleteReplenishmentMethodCommand>
{
    public DeleteReplenishmentMethodCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}
