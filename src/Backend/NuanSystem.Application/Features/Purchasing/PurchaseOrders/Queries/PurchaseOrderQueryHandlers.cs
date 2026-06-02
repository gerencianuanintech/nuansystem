using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders.Queries;

public sealed class GetPurchaseOrdersQueryHandler(IPurchaseOrderRepository repository)
    : IQueryHandler<GetPurchaseOrdersQuery, IReadOnlyCollection<PurchaseOrderSummaryDto>>
{
    public async Task<Result<IReadOnlyCollection<PurchaseOrderSummaryDto>>> Handle(
        GetPurchaseOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await repository.GetAllAsync(cancellationToken);
        return Result<IReadOnlyCollection<PurchaseOrderSummaryDto>>.Success(orders);
    }
}

public sealed class GetPurchaseOrderByIdQueryHandler(IPurchaseOrderRepository repository)
    : IQueryHandler<GetPurchaseOrderByIdQuery, PurchaseOrderDto>
{
    public async Task<Result<PurchaseOrderDto>> Handle(
        GetPurchaseOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await repository.GetByIdAsync(request.Id, cancellationToken);
        return order is null
            ? Result<PurchaseOrderDto>.Failure("No se encontro la orden de compra.")
            : Result<PurchaseOrderDto>.Success(order);
    }
}

public sealed class GetPurchaseOrderLookupsQueryHandler(IPurchaseOrderRepository repository)
    : IQueryHandler<GetPurchaseOrderLookupsQuery, PurchaseOrderLookupsDto>
{
    public async Task<Result<PurchaseOrderLookupsDto>> Handle(
        GetPurchaseOrderLookupsQuery request,
        CancellationToken cancellationToken)
    {
        var lookups = await repository.GetLookupsAsync(cancellationToken);
        return Result<PurchaseOrderLookupsDto>.Success(lookups);
    }
}
