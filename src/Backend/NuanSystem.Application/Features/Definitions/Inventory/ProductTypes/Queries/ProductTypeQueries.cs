using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Queries;

public sealed record GetProductTypesQuery : IQuery<IReadOnlyCollection<ProductTypeDto>>;
public sealed record GetProductTypeLookupQuery : IQuery<IReadOnlyCollection<ProductTypeLookupDto>>;
public sealed record GetProductTypeByIdQuery(int Id) : IQuery<ProductTypeDto>;
public sealed record GetProductTypeHistoryQuery(int Id) : IQuery<IReadOnlyCollection<ProductTypeAuditChangeDto>>;
