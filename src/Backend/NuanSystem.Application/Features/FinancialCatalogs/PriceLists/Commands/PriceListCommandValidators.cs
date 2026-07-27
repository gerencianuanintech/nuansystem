using FluentValidation;

namespace NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Commands;

public sealed class CreatePriceListCommandValidator : AbstractValidator<CreatePriceListCommand>
{
    public CreatePriceListCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).MaximumLength(300);
        RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.AppliesTo).Must(PriceListRules.IsValidAppliesTo)
            .WithMessage("AppliesTo debe ser Sales, Purchasing o Both.");
    }
}

public sealed class UpdatePriceListCommandValidator : AbstractValidator<UpdatePriceListCommand>
{
    public UpdatePriceListCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).MaximumLength(300);
        RuleFor(x => x.CurrencyCode).NotEmpty().MaximumLength(3);
        RuleFor(x => x.AppliesTo).Must(PriceListRules.IsValidAppliesTo)
            .WithMessage("AppliesTo debe ser Sales, Purchasing o Both.");
    }
}

public sealed class DeletePriceListCommandValidator : AbstractValidator<DeletePriceListCommand>
{
    public DeletePriceListCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}

internal static class PriceListRules
{
    public static bool IsValidAppliesTo(string value) =>
        value is "Sales" or "Purchasing" or "Both";
}
