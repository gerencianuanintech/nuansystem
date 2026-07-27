using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Dtos;

namespace NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Queries;

public sealed record GetPriceListsQuery : IQuery<IReadOnlyCollection<PriceListDto>>;
public sealed record GetPriceListLookupQuery(string? AppliesTo = null) : IQuery<IReadOnlyCollection<PriceListLookupDto>>;
public sealed record GetPriceListByIdQuery(int Id) : IQuery<PriceListDto>;
