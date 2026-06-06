using FluentValidation;

namespace NuanSystem.Application.Features.OperationalCatalogs.Commands;

public sealed class CreateOperationalCatalogCommandValidator : AbstractValidator<CreateOperationalCatalogCommand>
{
    public CreateOperationalCatalogCommandValidator()
    {
        Include(new OperationalCatalogCommandValidatorBase<CreateOperationalCatalogCommand>());
    }
}

public sealed class UpdateOperationalCatalogCommandValidator : AbstractValidator<UpdateOperationalCatalogCommand>
{
    public UpdateOperationalCatalogCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        Include(new OperationalCatalogCommandValidatorBase<UpdateOperationalCatalogCommand>());
    }
}

public sealed class DeleteOperationalCatalogCommandValidator : AbstractValidator<DeleteOperationalCatalogCommand>
{
    public DeleteOperationalCatalogCommandValidator()
    {
        RuleFor(command => command.CatalogKey).NotEmpty().MaximumLength(80);
        RuleFor(command => command.Id).GreaterThan(0);
    }
}

internal sealed class OperationalCatalogCommandValidatorBase<TCommand> : AbstractValidator<TCommand>
{
    public OperationalCatalogCommandValidatorBase()
    {
        RuleFor(command => ReadString(command, "CatalogKey")).NotEmpty().MaximumLength(80).WithName("CatalogKey");
        RuleFor(command => ReadString(command, "Code")).NotEmpty().MaximumLength(40).WithName("Code");
        RuleFor(command => ReadString(command, "Name")).NotEmpty().MaximumLength(150).WithName("Name");
        RuleFor(command => ReadNullableString(command, "Description")).MaximumLength(500).WithName("Description");
        RuleFor(command => ReadNullableString(command, "ParentCatalogKey")).MaximumLength(80).WithName("ParentCatalogKey");
        RuleFor(command => ReadNullableString(command, "ParentCode")).MaximumLength(40).WithName("ParentCode");
    }

    private static string ReadString(TCommand command, string propertyName)
    {
        return ReadNullableString(command, propertyName) ?? string.Empty;
    }

    private static string? ReadNullableString(TCommand command, string propertyName)
    {
        return command?.GetType().GetProperty(propertyName)?.GetValue(command) as string;
    }
}
