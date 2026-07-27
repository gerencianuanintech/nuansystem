using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Queries;

public sealed class GetPriceListsQueryHandler(IPriceListRepository repository)
    : IQueryHandler<GetPriceListsQuery, IReadOnlyCollection<PriceListDto>>
{
    public async Task<Result<IReadOnlyCollection<PriceListDto>>> Handle(GetPriceListsQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<PriceListDto>>.Success(await repository.GetAllAsync(cancellationToken));
}

public sealed class GetPriceListLookupQueryHandler(IPriceListRepository repository)
    : IQueryHandler<GetPriceListLookupQuery, IReadOnlyCollection<PriceListLookupDto>>
{
    public async Task<Result<IReadOnlyCollection<PriceListLookupDto>>> Handle(GetPriceListLookupQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<PriceListLookupDto>>.Success(await repository.GetLookupAsync(request.AppliesTo, cancellationToken));
}

public sealed class GetPriceListByIdQueryHandler(IPriceListRepository repository)
    : IQueryHandler<GetPriceListByIdQuery, PriceListDto>
{
    public async Task<Result<PriceListDto>> Handle(GetPriceListByIdQuery request, CancellationToken cancellationToken)
    {
        var priceList = await repository.GetByIdAsync(request.Id, cancellationToken);
        return priceList is null
            ? Result<PriceListDto>.Failure("Lista de precios no encontrada.",
                [new ApiError("PRICE_LIST_NOT_FOUND", "El registro no existe.", nameof(request.Id))])
            : Result<PriceListDto>.Success(priceList);
    }
}
