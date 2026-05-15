using FluentValidation;

namespace NuanSystem.Application.Features.Accounting.ChartOfAccounts.Commands;

public sealed class UpdateChartOfAccountCommandValidator : ChartOfAccountCommandValidator<UpdateChartOfAccountCommand>
{
    public UpdateChartOfAccountCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);

        ApplyRules(
            command => command.CompanyId,
            command => command.Code,
            command => command.Name,
            command => command.Description,
            command => command.ExternalCode,
            command => command.AccountType,
            command => command.AccountClass,
            command => command.ParentAccountId,
            command => command.CurrencyCode);
    }
}
