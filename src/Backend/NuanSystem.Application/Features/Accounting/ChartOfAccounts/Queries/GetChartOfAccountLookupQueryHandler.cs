using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Accounting.ChartOfAccounts.Dtos;
using NuanSystem.Application.Common.Models;

namespace NuanSystem.Application.Features.Accounting.ChartOfAccounts.Queries;

public sealed class GetChartOfAccountLookupQueryHandler(IChartOfAccountRepository accountRepository)
    : IQueryHandler<GetChartOfAccountLookupQuery, IReadOnlyCollection<ChartOfAccountLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<ChartOfAccountLookupDto>>> Handle(
        GetChartOfAccountLookupQuery request,
        CancellationToken cancellationToken)
    {
        var accounts = await accountRepository.GetLookupAsync(cancellationToken);

        return Result<IReadOnlyCollection<ChartOfAccountLookupDto>>.Success(accounts);
    }
}
