using FluentValidation;
using System.Linq.Expressions;

namespace NuanSystem.Application.Features.Accounting.ChartOfAccounts.Commands;

public abstract class ChartOfAccountCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : class
{
    private static readonly string[] AccountTypes =
    [
        "ASSET",
        "LIABILITY",
        "EQUITY",
        "INCOME",
        "EXPENSE",
        "COST",
        "ORDER"
    ];

    protected void ApplyRules(
        Expression<Func<TCommand, int>> companyId,
        Expression<Func<TCommand, string>> code,
        Expression<Func<TCommand, string>> name,
        Expression<Func<TCommand, string?>> description,
        Expression<Func<TCommand, string?>> externalCode,
        Expression<Func<TCommand, string>> accountType,
        Expression<Func<TCommand, string?>> accountClass,
        Expression<Func<TCommand, int?>> parentAccountId,
        Expression<Func<TCommand, string?>> currencyCode)
    {
        RuleFor(companyId).GreaterThan(0).WithName("CompanyId");
        RuleFor(code).NotEmpty().MaximumLength(50).WithName("Code");
        RuleFor(name).NotEmpty().MaximumLength(200).WithName("Name");
        RuleFor(description).MaximumLength(500).WithName("Description");
        RuleFor(externalCode).MaximumLength(50).WithName("ExternalCode");
        RuleFor(accountType)
            .NotEmpty()
            .MaximumLength(30)
            .Must(type => AccountTypes.Contains(type.Trim().ToUpperInvariant()))
            .WithMessage("El tipo de cuenta no es valido.")
            .WithName("AccountType");
        RuleFor(accountClass).MaximumLength(30).WithName("AccountClass");
        RuleFor(parentAccountId)
            .Must(id => !id.HasValue || id.Value > 0)
            .WithMessage("La cuenta padre no es valida.")
            .WithName("ParentAccountId");
        RuleFor(currencyCode)
            .MaximumLength(3)
            .Must(value => string.IsNullOrWhiteSpace(value) || value.Trim().Length == 3)
            .WithMessage("La moneda debe tener 3 caracteres.")
            .WithName("CurrencyCode");
    }
}
