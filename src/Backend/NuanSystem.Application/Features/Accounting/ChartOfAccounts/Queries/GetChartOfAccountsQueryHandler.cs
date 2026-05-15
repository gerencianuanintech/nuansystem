using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Accounting.ChartOfAccounts.Dtos;

namespace NuanSystem.Application.Features.Accounting.ChartOfAccounts.Queries;

public sealed class GetChartOfAccountsQueryHandler(IChartOfAccountRepository accountRepository)
    : IQueryHandler<GetChartOfAccountsQuery, IReadOnlyCollection<ChartOfAccountDto>>
{
    public async Task<Result<IReadOnlyCollection<ChartOfAccountDto>>> Handle(
        GetChartOfAccountsQuery request,
        CancellationToken cancellationToken)
    {
        var accounts = await accountRepository.GetAllAsync(cancellationToken);

        return Result<IReadOnlyCollection<ChartOfAccountDto>>.Success(accounts);
    }
}
