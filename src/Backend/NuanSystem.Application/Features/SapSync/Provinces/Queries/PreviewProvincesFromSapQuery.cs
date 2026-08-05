using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SapSync.Provinces.Contracts;

namespace NuanSystem.Application.Features.SapSync.Provinces.Queries;

public sealed record PreviewProvincesFromSapQuery
    : IQuery<IReadOnlyCollection<SapProvincePreviewItemDto>>;
