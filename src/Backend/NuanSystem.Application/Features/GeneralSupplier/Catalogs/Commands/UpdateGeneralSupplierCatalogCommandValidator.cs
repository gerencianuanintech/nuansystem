using FluentValidation;

namespace NuanSystem.Application.Features.GeneralSupplier.Catalogs.Commands;

public sealed class UpdateGeneralSupplierCatalogCommandValidator : AbstractValidator<UpdateGeneralSupplierCatalogCommand>
{
    public UpdateGeneralSupplierCatalogCommandValidator()
    {
        RuleFor(command => command.CatalogKey)
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(command => command.Id)
            .GreaterThan(0);

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

