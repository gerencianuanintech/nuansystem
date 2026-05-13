using FluentValidation;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Features.ConfigurationCompanies.Commands;

internal sealed class ConfigurationCompanyCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : class
{
    public ConfigurationCompanyCommandValidator()
    {
        RuleFor(command => Read<string>(command, "Code"))
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[A-Za-z0-9_-]+$")
            .WithMessage("El codigo solo puede contener letras, numeros, guion y guion bajo.");

        RuleFor(command => Read<string>(command, "CommercialName"))
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => Read<string?>(command, "LegalName"))
            .MaximumLength(250);

        RuleFor(command => Read<string?>(command, "TaxIdentification"))
            .MaximumLength(50);

        RuleFor(command => Read<string?>(command, "Address"))
            .MaximumLength(300);

        RuleFor(command => Read<string?>(command, "Phone"))
            .MaximumLength(30);

        RuleFor(command => Read<string?>(command, "Email"))
            .EmailAddress()
            .MaximumLength(256)
            .When(command => !string.IsNullOrWhiteSpace(Read<string?>(command, "Email")));

        RuleFor(command => Read<byte[]?>(command, "LogoImage"))
            .Must(image => image is null || image.Length <= 2 * 1024 * 1024)
            .WithMessage("El logo no debe superar 2 MB.");

        RuleFor(command => Read<string?>(command, "LogoImageContentType"))
            .MaximumLength(80);

        RuleFor(command => Read<string?>(command, "LogoImageFileName"))
            .MaximumLength(260);

        RuleFor(command => Read<DatabaseEngine>(command, "DatabaseEngine"))
            .IsInEnum()
            .Equal(DatabaseEngine.SqlServer)
            .WithMessage("Por ahora solo SQL Server esta implementado para companias.");

        RuleFor(command => Read<string>(command, "Server"))
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => Read<int?>(command, "Port"))
            .InclusiveBetween(1, 65535)
            .When(command => Read<int?>(command, "Port").HasValue);

        RuleFor(command => Read<string>(command, "DatabaseName"))
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(command => Read<string>(command, "DatabaseUser"))
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(command => Read<SapIntegrationMode>(command, "SapIntegrationMode"))
            .IsInEnum();

        RuleFor(command => Read<int>(command, "DisplayOrder"))
            .GreaterThanOrEqualTo(0);

        RuleFor(command => Read<string>(command, "TimeZoneId"))
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(command => Read<string>(command, "CultureCode"))
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(command => Read<string>(command, "CurrencyCode"))
            .NotEmpty()
            .Length(3);
    }

    private static TValue Read<TValue>(object source, string propertyName)
    {
        return (TValue)(source.GetType().GetProperty(propertyName)?.GetValue(source) ?? default(TValue)!);
    }
}
