using FluentValidation;

namespace NuanSystem.Application.Features.TaxCatalogs.Taxes.Commands;

public sealed class CreateTaxCommandValidator : AbstractValidator<CreateTaxCommand>
{
    public CreateTaxCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Rate).InclusiveBetween(0m, 1m)
            .WithMessage("Rate debe usar el contrato decimal entre 0 y 1.");
    }
}

public sealed class UpdateTaxCommandValidator : AbstractValidator<UpdateTaxCommand>
{
    public UpdateTaxCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Rate).InclusiveBetween(0m, 1m)
            .WithMessage("Rate debe usar el contrato decimal entre 0 y 1.");
    }
}

public sealed class DeleteTaxCommandValidator : AbstractValidator<DeleteTaxCommand>
{
    public DeleteTaxCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}
