using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Queries;
public sealed record GetItemOriginsQuery : IQuery<IReadOnlyCollection<ItemOriginDto>>;
public sealed record GetItemOriginLookupQuery(string? IncludeCode = null) : IQuery<IReadOnlyCollection<ItemOriginLookupDto>>;
public sealed record GetItemOriginByIdQuery(int Id) : IQuery<ItemOriginDto>;
public sealed record GetItemOriginHistoryQuery(int Id) : IQuery<IReadOnlyCollection<ItemOriginAuditChangeDto>>;
