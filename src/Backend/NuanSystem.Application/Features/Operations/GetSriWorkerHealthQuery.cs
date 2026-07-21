using NuanSystem.Application.Abstractions.Operations;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Operations;

public sealed record GetSriWorkerHealthQuery(WorkerHealthThresholds Thresholds) : IQuery<WorkerHealthReportDto>;

public sealed class GetSriWorkerHealthQueryHandler(IWorkerHeartbeatService heartbeatService)
    : IQueryHandler<GetSriWorkerHealthQuery, WorkerHealthReportDto>
{
    public async Task<Result<WorkerHealthReportDto>> Handle(GetSriWorkerHealthQuery request, CancellationToken cancellationToken)
    {
        request.Thresholds.Validate();
        var snapshots = await heartbeatService.GetByWorkerTypeAsync(WorkerTypes.Sri, cancellationToken);
        return Result<WorkerHealthReportDto>.Success(WorkerHealthEvaluator.Evaluate(snapshots, request.Thresholds, DateTime.UtcNow));
    }
}
