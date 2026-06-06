using FluentValidation;

namespace NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Commands;

public sealed class CreateSecurityDocumentSeriesCommandValidator
    : AbstractValidator<CreateSecurityDocumentSeriesCommand>
{
    public CreateSecurityDocumentSeriesCommandValidator()
    {
        Include(new SecurityDocumentSeriesCommandValidatorBase<CreateSecurityDocumentSeriesCommand>());
    }
}
