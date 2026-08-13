using FluentValidation;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Commands;

internal static class ItemFamilyValidationRules
{
    public static void Apply<T>(AbstractValidator<T> validator,
        Func<T, int> groupId, Func<T, string> code, Func<T, string> name,
        Func<T, string?> description, Func<T, int> sortOrder,
        Func<T, string?> externalSystem, Func<T, string?> externalCode,
        Func<T, string?> sapFamilyCode, Func<T, string?> sapCode)
    {
        validator.RuleFor(x => groupId(x)).GreaterThan(0).WithName("ItemGroupId");
        validator.RuleFor(x => code(x)).NotEmpty().MaximumLength(50).WithName("Code");
        validator.RuleFor(x => name(x)).NotEmpty().MaximumLength(150).WithName("Name");
        validator.RuleFor(x => description(x)).MaximumLength(500).WithName("Description");
        validator.RuleFor(x => sortOrder(x)).GreaterThanOrEqualTo(0).WithName("SortOrder");
        validator.RuleFor(x => externalSystem(x)).MaximumLength(50).WithName("ExternalSystem")
            .NotEmpty().When(x => !string.IsNullOrWhiteSpace(externalCode(x)));
        validator.RuleFor(x => externalCode(x)).MaximumLength(100).WithName("ExternalCode")
            .NotEmpty().When(x => !string.IsNullOrWhiteSpace(externalSystem(x)));
        validator.RuleFor(x => sapFamilyCode(x)).MaximumLength(100).WithName("SapFamilyCode");
        validator.RuleFor(x => sapCode(x)).MaximumLength(50).WithName("SapCode");
    }
}

public sealed class CreateItemFamilyCommandValidator : AbstractValidator<CreateItemFamilyCommand>
{
    public CreateItemFamilyCommandValidator() => ItemFamilyValidationRules.Apply(
        this, x => x.ItemGroupId, x => x.Code, x => x.Name, x => x.Description,
        x => x.SortOrder, x => x.ExternalSystem, x => x.ExternalCode,
        x => x.SapFamilyCode, x => x.SapCode);
}

public sealed class UpdateItemFamilyCommandValidator : AbstractValidator<UpdateItemFamilyCommand>
{
    public UpdateItemFamilyCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        ItemFamilyValidationRules.Apply(
            this, x => x.ItemGroupId, x => x.Code, x => x.Name, x => x.Description,
            x => x.SortOrder, x => x.ExternalSystem, x => x.ExternalCode,
            x => x.SapFamilyCode, x => x.SapCode);
    }
}

public sealed class DeleteItemFamilyCommandValidator : AbstractValidator<DeleteItemFamilyCommand>
{
    public DeleteItemFamilyCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}
