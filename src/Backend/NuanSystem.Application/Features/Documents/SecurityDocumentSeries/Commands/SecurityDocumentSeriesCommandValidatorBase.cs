using FluentValidation;

namespace NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Commands;

internal sealed class SecurityDocumentSeriesCommandValidatorBase<TCommand>
    : AbstractValidator<TCommand>
{
    public SecurityDocumentSeriesCommandValidatorBase()
    {
        RuleFor(command => GetString(command, "DocumentType"))
            .NotEmpty()
            .MaximumLength(40)
            .WithName("DocumentType");

        RuleFor(command => GetString(command, "Code"))
            .NotEmpty()
            .MaximumLength(40)
            .WithName("Code");

        RuleFor(command => GetString(command, "Name"))
            .NotEmpty()
            .MaximumLength(150)
            .WithName("Name");

        RuleFor(command => GetNullableString(command, "Description"))
            .MaximumLength(500)
            .WithName("Description");

        RuleFor(command => GetString(command, "Prefix"))
            .NotEmpty()
            .MaximumLength(20)
            .WithName("Prefix");

        RuleFor(command => GetString(command, "Establishment"))
            .NotEmpty()
            .MaximumLength(10)
            .WithName("Establishment");

        RuleFor(command => GetString(command, "EmissionPoint"))
            .NotEmpty()
            .MaximumLength(10)
            .WithName("EmissionPoint");

        RuleFor(command => GetInt(command, "InitialNumber"))
            .GreaterThanOrEqualTo(0)
            .WithName("InitialNumber");

        RuleFor(command => GetInt(command, "CurrentNumber"))
            .GreaterThanOrEqualTo(0)
            .WithName("CurrentNumber");

        RuleFor(command => GetInt(command, "NextNumber"))
            .GreaterThan(0)
            .WithName("NextNumber");

        RuleFor(command => GetInt(command, "NumberLength"))
            .InclusiveBetween(1, 18)
            .WithName("NumberLength");

        RuleFor(command => GetNullableString(command, "SapObjectType"))
            .MaximumLength(40)
            .WithName("SapObjectType");

        RuleFor(command => GetNullableString(command, "SapSeriesName"))
            .MaximumLength(150)
            .WithName("SapSeriesName");

        RuleFor(command => GetNullableInt(command, "SapSeriesId"))
            .GreaterThan(0)
            .When(command => GetNullableInt(command, "SapSeriesId").HasValue)
            .WithName("SapSeriesId");

        RuleFor(command => command)
            .Must(command => GetInt(command, "NextNumber") > GetInt(command, "CurrentNumber"))
            .WithMessage("El siguiente numero debe ser mayor al numero actual.");
    }

    private static string GetString(TCommand command, string propertyName)
    {
        return GetNullableString(command, propertyName) ?? string.Empty;
    }

    private static string? GetNullableString(TCommand command, string propertyName)
    {
        return command?.GetType().GetProperty(propertyName)?.GetValue(command) as string;
    }

    private static int GetInt(TCommand command, string propertyName)
    {
        return (int)(command?.GetType().GetProperty(propertyName)?.GetValue(command) ?? 0);
    }

    private static int? GetNullableInt(TCommand command, string propertyName)
    {
        return command?.GetType().GetProperty(propertyName)?.GetValue(command) as int?;
    }
}
