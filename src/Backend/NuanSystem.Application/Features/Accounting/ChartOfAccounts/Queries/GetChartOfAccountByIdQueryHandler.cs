using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Accounting.ChartOfAccounts.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Accounting.ChartOfAccounts.Queries;

public sealed class GetChartOfAccountByIdQueryHandler(IChartOfAccountRepository accountRepository)
    : IQueryHandler<GetChartOfAccountByIdQuery, ChartOfAccountDto>
{
    public async Task<Result<ChartOfAccountDto>> Handle(GetChartOfAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.Id, cancellationToken);

        return account is null
            ? Result<ChartOfAccountDto>.Failure(
                "Cuenta contable no encontrada.",
                [new ApiError("ChartOfAccountNotFound", "No existe la cuenta contable indicada.", nameof(request.Id))])
            : Result<ChartOfAccountDto>.Success(account);
    }
}
