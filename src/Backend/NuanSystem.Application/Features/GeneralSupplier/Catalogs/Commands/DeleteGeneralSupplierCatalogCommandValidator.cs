using FluentValidation;

namespace NuanSystem.Application.Features.GeneralSupplier.Catalogs.Commands;

public sealed class DeleteGeneralSupplierCatalogCommandValidator : AbstractValidator<DeleteGeneralSupplierCatalogCommand>
{
    public DeleteGeneralSupplierCatalogCommandValidator()
    {
        RuleFor(command => command.CatalogKey)
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(command => command.Id)
            .GreaterThan(0);
    }
}

