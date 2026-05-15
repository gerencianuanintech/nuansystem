using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Accounting.ChartOfAccounts.Dtos;

namespace NuanSystem.Application.Features.Accounting.ChartOfAccounts.Queries;

public sealed record GetChartOfAccountsQuery : IQuery<IReadOnlyCollection<ChartOfAccountDto>>;
