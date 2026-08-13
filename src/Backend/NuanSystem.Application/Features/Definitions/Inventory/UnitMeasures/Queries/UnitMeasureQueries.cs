using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Queries;

public sealed record GetUnitMeasuresQuery : IQuery<IReadOnlyCollection<UnitMeasureDto>>;
public sealed record GetUnitMeasureLookupQuery : IQuery<IReadOnlyCollection<UnitMeasureLookupDto>>;
public sealed record GetUnitMeasureByIdQuery(int Id) : IQuery<UnitMeasureDto>;
public sealed record GetUnitMeasureHistoryQuery(int Id) : IQuery<IReadOnlyCollection<UnitMeasureAuditChangeDto>>;
