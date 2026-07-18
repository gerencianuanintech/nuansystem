using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos;
using NuanSystem.Application.Features.Audit.Dtos;

namespace NuanSystem.Application.Features.Sync.EntityDefinitions.Queries;

public sealed record GetSyncEntityDefinitionsQuery(
    string? Search,
    bool? IsActive,
    int PageNumber,
    int PageSize) : IQuery<PagedResultDto<SyncEntityDefinitionListItemDto>>;

public sealed record GetSyncEntityDefinitionByIdQuery(int Id) : IQuery<SyncEntityDefinitionDetailDto>;

public sealed record GetSyncEntityDefinitionHistoryQuery(int Id) : IQuery<IReadOnlyCollection<SecurityChangeDto>>;

public sealed record GetSyncEntityDefinitionLookupQuery(int? IncludeId = null) : IQuery<IReadOnlyCollection<SyncEntityDefinitionLookupDto>>;
