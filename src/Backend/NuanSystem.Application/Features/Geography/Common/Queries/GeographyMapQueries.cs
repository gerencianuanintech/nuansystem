using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Geography.Common.Dtos;

namespace NuanSystem.Application.Features.Geography.Common.Queries;

public sealed record ReverseGeocodeQuery(decimal Latitude, decimal Longitude) : IQuery<ReverseGeocodeResultDto>;
public sealed record GetStaticMapQuery(decimal Latitude, decimal Longitude) : IQuery<StaticMapResultDto>;
