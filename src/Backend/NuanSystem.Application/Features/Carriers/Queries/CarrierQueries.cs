using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Carriers.Dtos;

namespace NuanSystem.Application.Features.Carriers.Queries;

public sealed record GetCarriersQuery : IQuery<IReadOnlyCollection<CarrierListItemDto>>;
public sealed record GetCarrierLookupQuery : IQuery<IReadOnlyCollection<CarrierLookupDto>>;
public sealed record GetCarrierByIdQuery(int Id) : IQuery<CarrierDetailDto>;
public sealed record GetCarrierHistoryQuery(int Id) : IQuery<IReadOnlyCollection<CarrierAuditChangeDto>>;
