using System.Text.RegularExpressions;
using FluentValidation;

namespace NuanSystem.Application.Features.Sync.EntityDefinitions.Commands;

public sealed partial class CreateSyncEntityDefinitionCommandValidator : AbstractValidator<CreateSyncEntityDefinitionCommand>
{
    public CreateSyncEntityDefinitionCommandValidator()
    {
        RuleFor(command => command.Code)
            .NotEmpty()
            .MaximumLength(80)
            .Matches(TechnicalCodeRegex())
            .WithMessage("El codigo debe iniciar con una letra y solo puede contener letras, numeros, punto, guion o guion bajo.");

        ApplyDefinitionRules(
            this,
            command => command.Name,
            command => command.Description,
            command => command.DefaultExecutionOrder,
            command => command.DefaultKeyField,
            command => command.DefaultModifiedAtField,
            command => command.DependencyDefinitionIds);
    }

    internal static void ApplyDefinitionRules<T>(
        AbstractValidator<T> validator,
        System.Linq.Expressions.Expression<Func<T, string>> name,
        System.Linq.Expressions.Expression<Func<T, string?>> description,
        System.Linq.Expressions.Expression<Func<T, int>> executionOrder,
        System.Linq.Expressions.Expression<Func<T, string?>> keyField,
        System.Linq.Expressions.Expression<Func<T, string?>> modifiedAtField,
        System.Linq.Expressions.Expression<Func<T, IEnumerable<int>>> dependencies)
    {
        validator.RuleFor(name).NotEmpty().MaximumLength(120);
        validator.RuleFor(description).MaximumLength(500);
        validator.RuleFor(executionOrder).GreaterThanOrEqualTo(0);
        validator.RuleFor(keyField).MaximumLength(100).Must(BeTechnicalField).WithMessage("El campo clave solo puede contener letras, numeros y guion bajo.");
        validator.RuleFor(modifiedAtField).MaximumLength(100).Must(BeTechnicalField).WithMessage("El campo de modificacion solo puede contener letras, numeros y guion bajo.");
        validator.RuleFor(dependencies).NotNull();
        validator.RuleForEach(dependencies).GreaterThan(0);
        validator.RuleFor(dependencies)
            .Must(values => values is null || values.Distinct().Count() == values.Count())
            .WithMessage("No se permiten dependencias duplicadas.");
    }

    private static bool BeTechnicalField(string? value)
    {
        return string.IsNullOrWhiteSpace(value) || value.Trim().All(character => char.IsLetterOrDigit(character) || character == '_');
    }

    [GeneratedRegex("^[A-Za-z][A-Za-z0-9._-]*$", RegexOptions.CultureInvariant)]
    private static partial Regex TechnicalCodeRegex();
}

public sealed class UpdateSyncEntityDefinitionCommandValidator : AbstractValidator<UpdateSyncEntityDefinitionCommand>
{
    public UpdateSyncEntityDefinitionCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        CreateSyncEntityDefinitionCommandValidator.ApplyDefinitionRules(
            this,
            command => command.Name,
            command => command.Description,
            command => command.DefaultExecutionOrder,
            command => command.DefaultKeyField,
            command => command.DefaultModifiedAtField,
            command => command.DependencyDefinitionIds);
        RuleFor(command => command)
            .Must(command => command.DependencyDefinitionIds is null || !command.DependencyDefinitionIds.Contains(command.Id))
            .WithMessage("Una entidad no puede depender de si misma.")
            .WithName(nameof(UpdateSyncEntityDefinitionCommand.DependencyDefinitionIds));
    }
}

public sealed class DeleteSyncEntityDefinitionCommandValidator : AbstractValidator<DeleteSyncEntityDefinitionCommand>
{
    public DeleteSyncEntityDefinitionCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
    }
}
