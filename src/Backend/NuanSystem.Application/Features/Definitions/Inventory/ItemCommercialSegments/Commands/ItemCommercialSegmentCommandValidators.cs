using FluentValidation;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemCommercialSegments.Commands;

internal static class ItemCommercialSegmentValidationRules
{
    public static void Apply(AbstractValidator<CreateItemCommercialSegmentCommand> validator)
    {
        validator.RuleFor(x => x.Code).NotEmpty().MaximumLength(50).WithName("Code");
        validator.RuleFor(x => x.Name).NotEmpty().MaximumLength(150).WithName("Name");
        validator.RuleFor(x => x.Description).MaximumLength(500).WithName("Description");
        validator.RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0).WithName("SortOrder");
    }

    public static void Apply(AbstractValidator<UpdateItemCommercialSegmentCommand> validator)
    {
        validator.RuleFor(x => x.Code).NotEmpty().MaximumLength(50).WithName("Code");
        validator.RuleFor(x => x.Name).NotEmpty().MaximumLength(150).WithName("Name");
        validator.RuleFor(x => x.Description).MaximumLength(500).WithName("Description");
        validator.RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0).WithName("SortOrder");
    }
}

public sealed class CreateItemCommercialSegmentCommandValidator : AbstractValidator<CreateItemCommercialSegmentCommand>
{
    public CreateItemCommercialSegmentCommandValidator() => ItemCommercialSegmentValidationRules.Apply(this);
}

public sealed class UpdateItemCommercialSegmentCommandValidator : AbstractValidator<UpdateItemCommercialSegmentCommand>
{
    public UpdateItemCommercialSegmentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        ItemCommercialSegmentValidationRules.Apply(this);
    }
}

public sealed class DeleteItemCommercialSegmentCommandValidator : AbstractValidator<DeleteItemCommercialSegmentCommand>
{
    public DeleteItemCommercialSegmentCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}
