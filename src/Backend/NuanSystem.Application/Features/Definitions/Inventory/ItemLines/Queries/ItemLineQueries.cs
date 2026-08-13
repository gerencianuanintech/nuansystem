using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Queries;

public sealed record GetItemLinesQuery : IQuery<IReadOnlyCollection<ItemLineDto>>;
public sealed record GetItemLineLookupQuery : IQuery<IReadOnlyCollection<ItemLineLookupDto>>;
public sealed record GetItemLineByIdQuery(int Id) : IQuery<ItemLineDto>;
public sealed record GetItemLineHistoryQuery(int Id) : IQuery<IReadOnlyCollection<ItemLineAuditChangeDto>>;
