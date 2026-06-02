using FluentValidation;

namespace NuanSystem.Application.Features.GeneralSupplier.Catalogs.Commands;

public sealed class CreateGeneralSupplierCatalogCommandValidator : AbstractValidator<CreateGeneralSupplierCatalogCommand>
{
    public CreateGeneralSupplierCatalogCommandValidator()
    {
        RuleFor(command => command.CatalogKey)
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(command => command.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(command => command.Description)
            .MaximumLength(500);
    }
}

