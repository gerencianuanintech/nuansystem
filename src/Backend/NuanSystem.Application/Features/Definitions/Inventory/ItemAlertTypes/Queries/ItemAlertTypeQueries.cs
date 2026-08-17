using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemAlertTypes.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemAlertTypes.Queries;

public sealed record GetItemAlertTypesQuery : IQuery<IReadOnlyCollection<ItemAlertTypeDto>>;
public sealed record GetItemAlertTypeLookupQuery : IQuery<IReadOnlyCollection<ItemAlertTypeLookupDto>>;
public sealed record GetItemAlertTypeByIdQuery(int Id) : IQuery<ItemAlertTypeDto>;
public sealed record GetItemAlertTypeHistoryQuery(int Id) : IQuery<IReadOnlyCollection<ItemAlertTypeAuditChangeDto>>;

