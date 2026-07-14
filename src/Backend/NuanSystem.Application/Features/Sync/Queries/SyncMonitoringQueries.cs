using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Features.Sync.Queries;

public sealed record GetSyncDashboardQuery(int Take = 10) : IQuery<SyncDashboardDto>;

public sealed record GetSyncSummaryQuery : IQuery<SyncSummaryDto>;

public sealed record GetSyncOutboxQuery(SyncOutboxQueryFilter Filter) : IQuery<IReadOnlyCollection<SyncOutboxListItemDto>>;

public sealed record GetSyncOutboxDetailQuery(long Id) : IQuery<SyncOutboxDetailDto>;

public sealed record GetSyncOutboxTargetsQuery(long OutboxId) : IQuery<IReadOnlyCollection<SyncOutboxTargetDto>>;

public sealed record GetSyncAuditQuery(SyncAuditQueryFilter Filter) : IQuery<IReadOnlyCollection<SyncAuditDto>>;
