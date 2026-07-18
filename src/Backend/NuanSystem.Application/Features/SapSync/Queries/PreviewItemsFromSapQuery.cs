using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Queries;

public sealed record PreviewItemsFromSapQuery(int Take = 200, string? Search = null)
    : IQuery<IReadOnlyCollection<SapItemPreviewItemDto>>;
