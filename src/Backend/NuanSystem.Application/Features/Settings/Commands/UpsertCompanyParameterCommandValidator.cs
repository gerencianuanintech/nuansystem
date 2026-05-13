using FluentValidation;

namespace NuanSystem.Application.Features.Settings.Commands;

public sealed class UpsertCompanyParameterCommandValidator : AbstractValidator<UpsertCompanyParameterCommand>
{
    public UpsertCompanyParameterCommandValidator()
    {
        RuleFor(command => command.Key)
            .NotEmpty()
            .MaximumLength(120)
            .Matches("^[A-Za-z0-9_.:-]+$")
            .WithMessage("La clave solo puede contener letras, numeros, punto, guion, dos puntos o guion bajo.");

        RuleFor(command => command.Description)
            .MaximumLength(300)
            .When(command => !string.IsNullOrWhiteSpace(command.Description));
    }
}
