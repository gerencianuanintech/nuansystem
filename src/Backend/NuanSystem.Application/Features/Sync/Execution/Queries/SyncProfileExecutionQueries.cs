using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.Execution.Dtos;

namespace NuanSystem.Application.Features.Sync.Execution.Queries;

public sealed record GetSyncProfileExecutionsQuery(
    int? ProfileId,
    string? Status,
    string? ExecutionType,
    DateTimeOffset? DateFrom,
    DateTimeOffset? DateTo,
    int PageNumber,
    int PageSize) : IQuery<PagedResultDto<SyncProfileExecutionListItemDto>>;

public sealed record GetSyncProfileExecutionByIdQuery(int ExecutionId) : IQuery<SyncProfileExecutionDetailDto>;
