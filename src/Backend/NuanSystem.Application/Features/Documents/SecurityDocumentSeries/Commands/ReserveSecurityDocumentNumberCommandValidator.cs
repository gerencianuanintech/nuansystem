using FluentValidation;

namespace NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Commands;

public sealed class ReserveSecurityDocumentNumberCommandValidator
    : AbstractValidator<ReserveSecurityDocumentNumberCommand>
{
    public ReserveSecurityDocumentNumberCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
    }
}
