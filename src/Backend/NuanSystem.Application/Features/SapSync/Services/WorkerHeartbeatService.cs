using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class WorkerHeartbeatService(IWorkerHeartbeatRepository repository) : IWorkerHeartbeatService
{
    public Task BeatAsync(WorkerHeartbeatDto heartbeat, CancellationToken cancellationToken = default)
        => repository.UpsertAsync(heartbeat, cancellationToken);
}
