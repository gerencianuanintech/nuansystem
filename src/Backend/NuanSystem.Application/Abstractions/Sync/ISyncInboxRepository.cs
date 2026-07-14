using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncInboxRepository
{
    Task<long> RegisterAsync(CreateSyncInboxEventData data, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SyncInboxDto>> GetPendingAsync(int take, CancellationToken cancellationToken = default);
    Task<SyncInboxDto?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task MarkDeadLetterAsync(long id, string errorMessage, CancellationToken cancellationToken = default);
}
