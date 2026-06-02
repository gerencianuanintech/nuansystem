using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders.Queries;

public sealed record GetPurchaseOrdersQuery : IQuery<IReadOnlyCollection<PurchaseOrderSummaryDto>>;

public sealed record GetPurchaseOrderByIdQuery(int Id) : IQuery<PurchaseOrderDto>;

public sealed record GetPurchaseOrderLookupsQuery : IQuery<PurchaseOrderLookupsDto>;
