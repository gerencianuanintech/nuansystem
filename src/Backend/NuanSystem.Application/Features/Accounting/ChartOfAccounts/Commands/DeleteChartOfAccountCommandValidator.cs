using FluentValidation;

namespace NuanSystem.Application.Features.Accounting.ChartOfAccounts.Commands;

public sealed class DeleteChartOfAccountCommandValidator : AbstractValidator<DeleteChartOfAccountCommand>
{
    public DeleteChartOfAccountCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
    }
}
