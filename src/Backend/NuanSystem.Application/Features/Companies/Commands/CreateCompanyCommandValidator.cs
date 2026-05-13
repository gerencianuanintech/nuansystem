using FluentValidation;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Features.Companies.Commands;

public sealed class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(command => command.Code)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[A-Za-z0-9_-]+$")
            .WithMessage("El codigo solo puede contener letras, numeros, guion y guion bajo.");

        RuleFor(command => command.CommercialName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.LegalName)
            .MaximumLength(250);

        RuleFor(command => command.TaxIdentification)
            .MaximumLength(50);

        RuleFor(command => command.DatabaseEngine)
            .IsInEnum()
            .Equal(DatabaseEngine.SqlServer)
            .WithMessage("Por ahora solo SQL Server esta implementado para empresas.");

        RuleFor(command => command.Server)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.Port)
            .InclusiveBetween(1, 65535)
            .When(command => command.Port.HasValue);

        RuleFor(command => command.DatabaseName)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(command => command.DatabaseUser)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(command => command.DatabasePassword)
            .NotEmpty();

        RuleFor(command => command.SapIntegrationMode)
            .IsInEnum();
    }
}
