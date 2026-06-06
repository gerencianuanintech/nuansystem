using FluentValidation;

namespace NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Commands;

public sealed class DeleteSecurityDocumentSeriesCommandValidator
    : AbstractValidator<DeleteSecurityDocumentSeriesCommand>
{
    public DeleteSecurityDocumentSeriesCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
    }
}
