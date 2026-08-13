using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Queries;

public sealed record GetItemFamiliesQuery : IQuery<IReadOnlyCollection<ItemFamilyDto>>;
public sealed record GetItemFamilyLookupQuery(int? ItemGroupId) : IQuery<IReadOnlyCollection<ItemFamilyLookupDto>>;
public sealed record GetItemFamilyByIdQuery(int Id) : IQuery<ItemFamilyDto>;
public sealed record GetItemFamilyHistoryQuery(int Id) : IQuery<IReadOnlyCollection<ItemFamilyAuditChangeDto>>;
