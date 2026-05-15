namespace NuanSystem.Application.Features.Accounting.ChartOfAccounts.Commands;

public sealed class CreateChartOfAccountCommandValidator : ChartOfAccountCommandValidator<CreateChartOfAccountCommand>
{
    public CreateChartOfAccountCommandValidator()
    {
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
