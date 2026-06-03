using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface IWorkerHeartbeatService
{
    Task BeatAsync(WorkerHeartbeatDto heartbeat, CancellationToken cancellationToken = default);
}
