using FluentValidation;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemAlertTypes.Commands;

internal static class ItemAlertTypeValidationRules
{
    public static void Apply(AbstractValidator<CreateItemAlertTypeCommand> validator)
    {
        validator.RuleFor(x => x.Code).NotEmpty().MaximumLength(50).WithName("Code");
        validator.RuleFor(x => x.Name).NotEmpty().MaximumLength(150).WithName("Name");
        validator.RuleFor(x => x.Description).MaximumLength(500).WithName("Description");
        validator.RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0).WithName("SortOrder");
    }

    public static void Apply(AbstractValidator<UpdateItemAlertTypeCommand> validator)
    {
        validator.RuleFor(x => x.Code).NotEmpty().MaximumLength(50).WithName("Code");
        validator.RuleFor(x => x.Name).NotEmpty().MaximumLength(150).WithName("Name");
        validator.RuleFor(x => x.Description).MaximumLength(500).WithName("Description");
        validator.RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0).WithName("SortOrder");
    }
}

public sealed class CreateItemAlertTypeCommandValidator : AbstractValidator<CreateItemAlertTypeCommand>
{
    public CreateItemAlertTypeCommandValidator() => ItemAlertTypeValidationRules.Apply(this);
}

public sealed class UpdateItemAlertTypeCommandValidator : AbstractValidator<UpdateItemAlertTypeCommand>
{
    public UpdateItemAlertTypeCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        ItemAlertTypeValidationRules.Apply(this);
    }
}

public sealed class DeleteItemAlertTypeCommandValidator : AbstractValidator<DeleteItemAlertTypeCommand>
{
    public DeleteItemAlertTypeCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

