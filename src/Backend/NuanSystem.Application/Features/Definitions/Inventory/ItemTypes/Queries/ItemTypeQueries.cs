using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Queries;

public sealed record GetItemTypesQuery : IQuery<IReadOnlyCollection<ItemTypeDto>>;
public sealed record GetItemTypeLookupQuery : IQuery<IReadOnlyCollection<ItemTypeLookupDto>>;
public sealed record GetItemTypeByIdQuery(int Id) : IQuery<ItemTypeDto>;
public sealed record GetItemTypeHistoryQuery(int Id) : IQuery<IReadOnlyCollection<ItemTypeAuditChangeDto>>;
