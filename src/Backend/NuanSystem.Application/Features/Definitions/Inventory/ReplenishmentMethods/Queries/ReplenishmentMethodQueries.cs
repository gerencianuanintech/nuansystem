using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Queries;

public sealed record GetReplenishmentMethodsQuery : IQuery<IReadOnlyCollection<ReplenishmentMethodDto>>;
public sealed record GetReplenishmentMethodLookupQuery(string? IncludeCode = null) : IQuery<IReadOnlyCollection<ReplenishmentMethodLookupDto>>;
public sealed record GetReplenishmentMethodByIdQuery(int Id) : IQuery<ReplenishmentMethodDto>;
public sealed record GetReplenishmentMethodHistoryQuery(int Id) : IQuery<IReadOnlyCollection<ReplenishmentMethodAuditChangeDto>>;
