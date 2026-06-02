using FluentValidation;

namespace NuanSystem.Application.Features.GeneralInventory.Catalogs.Commands;

public sealed class DeleteGeneralInventoryCatalogCommandValidator : AbstractValidator<DeleteGeneralInventoryCatalogCommand>
{
    public DeleteGeneralInventoryCatalogCommandValidator()
    {
        RuleFor(command => command.CatalogKey)
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(command => command.Id)
            .GreaterThan(0);
    }
}
