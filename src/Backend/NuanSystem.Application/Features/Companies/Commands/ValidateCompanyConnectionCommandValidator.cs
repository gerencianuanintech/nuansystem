using FluentValidation;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Features.Companies.Commands;

public sealed class ValidateCompanyConnectionCommandValidator : AbstractValidator<ValidateCompanyConnectionCommand>
{
    public ValidateCompanyConnectionCommandValidator()
    {
        RuleFor(command => command.DatabaseEngine)
            .IsInEnum();

        RuleFor(command => command.DatabaseEngine)
            .Equal(DatabaseEngine.SqlServer)
            .WithMessage("Por ahora solo SQL Server esta implementado para validacion de conexion.");

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
    }
}
