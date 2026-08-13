using FluentValidation;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Commands;

internal static class UnitMeasureValidationRules
{
    public static void Apply<T>(AbstractValidator<T> validator, Func<T, string> code,
        Func<T, string> name, Func<T, string?> description, Func<T, string?> symbol,
        Func<T, string> magnitudeCode, Func<T, int> sortOrder,
        Func<T, string?> externalSystem, Func<T, string?> externalCode)
    {
        validator.RuleFor(x => code(x)).NotEmpty().MaximumLength(50).WithName("Code");
        validator.RuleFor(x => name(x)).NotEmpty().MaximumLength(150).WithName("Name");
        validator.RuleFor(x => description(x)).MaximumLength(500).WithName("Description");
        validator.RuleFor(x => symbol(x)).MaximumLength(20).WithName("Symbol");
        validator.RuleFor(x => magnitudeCode(x)).NotEmpty()
            .Must(value => !string.IsNullOrWhiteSpace(value) && UnitMeasureMagnitudeCodes.All.Contains(value.Trim()))
            .WithMessage("MagnitudeCode no es valido.").WithName("MagnitudeCode");
        validator.RuleFor(x => sortOrder(x)).GreaterThanOrEqualTo(0).WithName("SortOrder");
        validator.RuleFor(x => externalSystem(x)).MaximumLength(50).WithName("ExternalSystem")
            .NotEmpty().When(x => !string.IsNullOrWhiteSpace(externalCode(x)));
        validator.RuleFor(x => externalCode(x)).MaximumLength(100).WithName("ExternalCode")
            .NotEmpty().When(x => !string.IsNullOrWhiteSpace(externalSystem(x)));
    }
}

public sealed class CreateUnitMeasureCommandValidator : AbstractValidator<CreateUnitMeasureCommand>
{
    public CreateUnitMeasureCommandValidator() => UnitMeasureValidationRules.Apply(this,
        x => x.Code, x => x.Name, x => x.Description, x => x.Symbol, x => x.MagnitudeCode,
        x => x.SortOrder, x => x.ExternalSystem, x => x.ExternalCode);
}

public sealed class UpdateUnitMeasureCommandValidator : AbstractValidator<UpdateUnitMeasureCommand>
{
    public UpdateUnitMeasureCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        UnitMeasureValidationRules.Apply(this, x => x.Code, x => x.Name, x => x.Description,
            x => x.Symbol, x => x.MagnitudeCode, x => x.SortOrder, x => x.ExternalSystem, x => x.ExternalCode);
    }
}

public sealed class DeleteUnitMeasureCommandValidator : AbstractValidator<DeleteUnitMeasureCommand>
{
    public DeleteUnitMeasureCommandValidator() => RuleFor(x => x.Id).GreaterThan(0);
}
