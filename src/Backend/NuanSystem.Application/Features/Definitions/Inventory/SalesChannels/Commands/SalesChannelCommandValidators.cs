using FluentValidation;

namespace NuanSystem.Application.Features.Definitions.Inventory.SalesChannels.Commands;

internal static class SalesChannelValidationRules
{
    public static void Apply(AbstractValidator<CreateSalesChannelCommand> validator)
    {
        validator.RuleFor(x => x.Code).NotEmpty().MaximumLength(50).WithName("Code");
        validator.RuleFor(x => x.Name).NotEmpty().MaximumLength(150).WithName("Name");
        validator.RuleFor(x => x.Description).MaximumLength(500).WithName("Description");
        validator.RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0).WithName("SortOrder");
    }

    public static void Apply(AbstractValidator<UpdateSalesChannelCommand> validator)
    {
        validator.RuleFor(x => x.Code).NotEmpty().MaximumLength(50).WithName("Code");
        validator.RuleFor(x => x.Name).NotEmpty().MaximumLength(150).WithName("Name");
        validator.RuleFor(x => x.Description).MaximumLength(500).WithName("Description");
        validator.RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0).WithName("SortOrder");
    }
}

public sealed class CreateSalesChannelCommandValidator : AbstractValidator<CreateSalesChannelCommand>
{
    public CreateSalesChannelCommandValidator() => SalesChannelValidationRules.Apply(this);
}

public sealed class UpdateSalesChannelCommandValidator : AbstractValidator<UpdateSalesChannelCommand>
{
    public UpdateSalesChannelCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        SalesChannelValidationRules.Apply(this);
    }
}

public sealed class DeleteSalesChannelCommandValidator : AbstractValidator<DeleteSalesChannelCommand>
{
    public DeleteSalesChannelCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}


