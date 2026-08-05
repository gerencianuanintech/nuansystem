using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SapSync.Cities.Contracts;

namespace NuanSystem.Application.Features.SapSync.Cities.Queries;

public sealed record PreviewCitiesFromSapQuery
    : IQuery<IReadOnlyCollection<SapCityPreviewItemDto>>;
