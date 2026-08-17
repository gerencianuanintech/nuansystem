using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Queries;

public sealed record GetItemSubgroupsQuery : IQuery<IReadOnlyCollection<ItemSubgroupDto>>;
public sealed record GetItemSubgroupLookupQuery(int? ItemFamilyId = null) : IQuery<IReadOnlyCollection<ItemSubgroupLookupDto>>;
public sealed record GetItemSubgroupByIdQuery(int Id) : IQuery<ItemSubgroupDto>;
public sealed record GetItemSubgroupHistoryQuery(int Id) : IQuery<IReadOnlyCollection<ItemSubgroupAuditChangeDto>>;
