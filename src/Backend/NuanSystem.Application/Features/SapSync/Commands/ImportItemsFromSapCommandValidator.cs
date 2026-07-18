using FluentValidation;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed class ImportItemsFromSapCommandValidator : AbstractValidator<ImportItemsFromSapCommand>
{
    public ImportItemsFromSapCommandValidator()
    {
        RuleFor(command => command.SapItemCodes)
            .Must(codes => codes is null || codes.Count <= 1000)
            .WithMessage("Puede seleccionar hasta 1000 articulos por importacion.");

        RuleForEach(command => command.SapItemCodes).NotEmpty().MaximumLength(100);

        RuleFor(command => command.SapItemCodes)
            .Must(codes => codes is null || codes.Distinct(StringComparer.OrdinalIgnoreCase).Count() == codes.Count)
            .WithMessage("La seleccion contiene codigos SAP duplicados.");
    }
}
