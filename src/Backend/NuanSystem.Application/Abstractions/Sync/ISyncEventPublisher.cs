using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncEventPublisher
{
    Task<Result<SyncPublishResult>> PublishAsync(
        SyncPublishRequest request,
        CancellationToken cancellationToken = default);
}
