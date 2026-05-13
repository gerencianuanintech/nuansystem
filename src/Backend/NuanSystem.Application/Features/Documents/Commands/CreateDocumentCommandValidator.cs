using FluentValidation;

namespace NuanSystem.Application.Features.Documents.Commands;

public sealed class CreateDocumentCommandValidator : AbstractValidator<CreateDocumentCommand>
{
    private static readonly string[] AllowedDocumentTypes = ["SalesOrder", "Delivery", "Invoice"];

    public CreateDocumentCommandValidator()
    {
        RuleFor(command => command.DocumentType)
            .NotEmpty()
            .Must(type => AllowedDocumentTypes.Contains(type, StringComparer.OrdinalIgnoreCase))
            .WithMessage("El tipo de documento permitido es SalesOrder, Delivery o Invoice.");

        RuleFor(command => command.CustomerId)
            .GreaterThan(0);

        RuleFor(command => command.DocumentDate)
            .NotEmpty();

        RuleFor(command => command.Currency)
            .NotEmpty()
            .Length(3);

        RuleFor(command => command.Lines)
            .NotEmpty()
            .WithMessage("El documento debe tener al menos una linea.");

        RuleForEach(command => command.Lines)
            .ChildRules(line =>
            {
                line.RuleFor(item => item.ItemId).GreaterThan(0);
                line.RuleFor(item => item.Quantity).GreaterThan(0);
                line.RuleFor(item => item.UnitPrice).GreaterThanOrEqualTo(0);
                line.RuleFor(item => item.TaxRate).InclusiveBetween(0, 1);
            });
    }
}
