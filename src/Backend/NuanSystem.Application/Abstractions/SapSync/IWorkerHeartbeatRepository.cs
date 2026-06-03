using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface IWorkerHeartbeatRepository
{
    Task UpsertAsync(WorkerHeartbeatDto heartbeat, CancellationToken cancellationToken = default);
}
