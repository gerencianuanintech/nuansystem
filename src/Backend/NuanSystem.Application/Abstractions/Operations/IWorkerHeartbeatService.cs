using NuanSystem.Application.Features.Operations;

namespace NuanSystem.Application.Abstractions.Operations;

public interface IWorkerHeartbeatService
{
    Task BeatAsync(WorkerHeartbeatDto heartbeat, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<WorkerHeartbeatSnapshotDto>> GetByWorkerTypeAsync(string workerType, CancellationToken cancellationToken = default);
}
public interface IWorkerHeartbeatRepository
{
    Task UpsertAsync(WorkerHeartbeatDto heartbeat, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<WorkerHeartbeatSnapshotDto>> GetByWorkerTypeAsync(string workerType, CancellationToken cancellationToken = default);
}
