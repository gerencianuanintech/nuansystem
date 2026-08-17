using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.SalesChannels.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.SalesChannels.Queries;

public sealed record GetSalesChannelsQuery : IQuery<IReadOnlyCollection<SalesChannelDto>>;
public sealed record GetSalesChannelLookupQuery : IQuery<IReadOnlyCollection<SalesChannelLookupDto>>;
public sealed record GetSalesChannelByIdQuery(int Id) : IQuery<SalesChannelDto>;
public sealed record GetSalesChannelHistoryQuery(int Id) : IQuery<IReadOnlyCollection<SalesChannelAuditChangeDto>>;


