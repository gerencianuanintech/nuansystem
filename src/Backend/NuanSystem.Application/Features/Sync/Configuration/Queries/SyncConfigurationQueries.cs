using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;

namespace NuanSystem.Application.Features.Sync.Configuration.Queries;

public sealed record GetSyncProfilesQuery(
    string? Search,
    int? CompanyId,
    bool? IsActive,
    string? ExecutionMode,
    int PageNumber,
    int PageSize,
    int? UserId) : IQuery<PagedResultDto<SyncProfileListItemDto>>;

public sealed record GetSyncProfileByIdQuery(int Id, int? UserId) : IQuery<SyncProfileApiDetailDto>;

public sealed record GetSyncConfigurationCatalogQuery(int? UserId) : IQuery<SyncConfigurationCatalogDto>;
