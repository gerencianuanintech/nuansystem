using FluentValidation;

namespace NuanSystem.Application.Features.SapSync.Commands;

public sealed class SendDocumentToSapCommandValidator : AbstractValidator<SendDocumentToSapCommand>
{
    public SendDocumentToSapCommandValidator()
    {
        RuleFor(command => command.DocumentId)
            .GreaterThan(0);
    }
}
