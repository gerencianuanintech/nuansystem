using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Queries;

public sealed record GetItemBrandsQuery : IQuery<IReadOnlyCollection<ItemBrandDto>>;
public sealed record GetItemBrandLookupQuery : IQuery<IReadOnlyCollection<ItemBrandLookupDto>>;
public sealed record GetItemBrandByIdQuery(int Id) : IQuery<ItemBrandDto>;
public sealed record GetItemBrandHistoryQuery(int Id) : IQuery<IReadOnlyCollection<ItemBrandAuditChangeDto>>;
