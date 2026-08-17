using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemCommercialSegments.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemCommercialSegments.Queries;

public sealed record GetItemCommercialSegmentsQuery : IQuery<IReadOnlyCollection<ItemCommercialSegmentDto>>;
public sealed record GetItemCommercialSegmentLookupQuery : IQuery<IReadOnlyCollection<ItemCommercialSegmentLookupDto>>;
public sealed record GetItemCommercialSegmentByIdQuery(int Id) : IQuery<ItemCommercialSegmentDto>;
public sealed record GetItemCommercialSegmentHistoryQuery(int Id) : IQuery<IReadOnlyCollection<ItemCommercialSegmentAuditChangeDto>>;

