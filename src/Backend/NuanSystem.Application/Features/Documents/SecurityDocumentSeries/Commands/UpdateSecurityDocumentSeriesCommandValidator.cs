using FluentValidation;

namespace NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Commands;

public sealed class UpdateSecurityDocumentSeriesCommandValidator
    : AbstractValidator<UpdateSecurityDocumentSeriesCommand>
{
    public UpdateSecurityDocumentSeriesCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        Include(new SecurityDocumentSeriesCommandValidatorBase<UpdateSecurityDocumentSeriesCommand>());
    }
}
