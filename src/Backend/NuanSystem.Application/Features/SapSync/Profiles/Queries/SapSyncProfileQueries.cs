using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.SapSync.Profiles.Queries;

public sealed record GetSapSyncProfilesQuery(
    SapSyncProfileListRequest Filter,
    int UserId) : IQuery<SapSyncPagedResult<SapSyncProfileListItemDto>>;

public sealed record GetSapSyncProfileByIdQuery(
    long Id,
    int UserId) : IQuery<SapSyncProfileDto>;

public sealed record GetSapSyncProfileCatalogQuery(
    int UserId) : IQuery<SapSyncProfileCatalogDto>;
