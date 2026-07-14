using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Features.Sync.Queries;

public sealed class GetSyncDashboardQueryHandler(
    ICompanyContext companyContext,
    ISyncOutboxRepository repository)
    : IQueryHandler<GetSyncDashboardQuery, SyncDashboardDto>
{
    public async Task<Result<SyncDashboardDto>> Handle(GetSyncDashboardQuery request, CancellationToken cancellationToken)
    {
        var dashboard = await repository.GetDashboardAsync(SyncCompanyContext.GetActiveCompanyId(companyContext), request.Take, cancellationToken);
        return Result<SyncDashboardDto>.Success(dashboard);
    }
}

public sealed class GetSyncSummaryQueryHandler(
    ICompanyContext companyContext,
    ISyncOutboxRepository repository)
    : IQueryHandler<GetSyncSummaryQuery, SyncSummaryDto>
{
    public async Task<Result<SyncSummaryDto>> Handle(GetSyncSummaryQuery request, CancellationToken cancellationToken)
    {
        var summary = await repository.GetSummaryAsync(SyncCompanyContext.GetActiveCompanyId(companyContext), cancellationToken);
        return Result<SyncSummaryDto>.Success(summary);
    }
}

public sealed class GetSyncOutboxQueryHandler(
    ICompanyContext companyContext,
    ISyncOutboxRepository repository)
    : IQueryHandler<GetSyncOutboxQuery, IReadOnlyCollection<SyncOutboxListItemDto>>
{
    public async Task<Result<IReadOnlyCollection<SyncOutboxListItemDto>>> Handle(GetSyncOutboxQuery request, CancellationToken cancellationToken)
    {
        var events = await repository.SearchOutboxAsync(SyncCompanyContext.GetActiveCompanyId(companyContext), request.Filter, cancellationToken);
        return Result<IReadOnlyCollection<SyncOutboxListItemDto>>.Success(events);
    }
}

public sealed class GetSyncOutboxDetailQueryHandler(
    ICompanyContext companyContext,
    ISyncOutboxRepository repository)
    : IQueryHandler<GetSyncOutboxDetailQuery, SyncOutboxDetailDto>
{
    public async Task<Result<SyncOutboxDetailDto>> Handle(GetSyncOutboxDetailQuery request, CancellationToken cancellationToken)
    {
        var detail = await repository.GetOutboxDetailAsync(SyncCompanyContext.GetActiveCompanyId(companyContext), request.Id, cancellationToken);

        return detail is null
            ? Result<SyncOutboxDetailDto>.Failure("Evento de sincronizacion no encontrado.")
            : Result<SyncOutboxDetailDto>.Success(detail);
    }
}

public sealed class GetSyncOutboxTargetsQueryHandler(
    ICompanyContext companyContext,
    ISyncOutboxRepository repository)
    : IQueryHandler<GetSyncOutboxTargetsQuery, IReadOnlyCollection<SyncOutboxTargetDto>>
{
    public async Task<Result<IReadOnlyCollection<SyncOutboxTargetDto>>> Handle(GetSyncOutboxTargetsQuery request, CancellationToken cancellationToken)
    {
        var targets = await repository.GetTargetsAsync(SyncCompanyContext.GetActiveCompanyId(companyContext), request.OutboxId, cancellationToken);
        return Result<IReadOnlyCollection<SyncOutboxTargetDto>>.Success(targets);
    }
}

public sealed class GetSyncAuditQueryHandler(
    ICompanyContext companyContext,
    ISyncAuditRepository repository)
    : IQueryHandler<GetSyncAuditQuery, IReadOnlyCollection<SyncAuditDto>>
{
    public async Task<Result<IReadOnlyCollection<SyncAuditDto>>> Handle(GetSyncAuditQuery request, CancellationToken cancellationToken)
    {
        var entries = await repository.SearchAuditAsync(SyncCompanyContext.GetActiveCompanyId(companyContext), request.Filter, cancellationToken);
        return Result<IReadOnlyCollection<SyncAuditDto>>.Success(entries);
    }
}

file static class SyncCompanyContext
{
    public static int GetActiveCompanyId(ICompanyContext companyContext)
    {
        if (!companyContext.HasActiveCompany || companyContext.CurrentCompany is null)
        {
            throw new InvalidOperationException("No hay empresa activa para consultar la sincronizacion.");
        }

        return companyContext.CurrentCompany.CompanyId;
    }
}
