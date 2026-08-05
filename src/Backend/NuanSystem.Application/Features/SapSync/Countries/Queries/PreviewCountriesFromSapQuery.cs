using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SapSync.Countries.Contracts;

namespace NuanSystem.Application.Features.SapSync.Countries.Queries;

public sealed record PreviewCountriesFromSapQuery
    : IQuery<IReadOnlyCollection<SapCountryPreviewItemDto>>;
