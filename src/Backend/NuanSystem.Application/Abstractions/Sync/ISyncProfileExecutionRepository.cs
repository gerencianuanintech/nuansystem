using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.Execution.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncProfileExecutionRepository
{
    Task<int> CreateAsync(CreateSyncProfileExecutionData data, CancellationToken cancellationToken = default);
    Task<bool> StartAsync(int executionId, CancellationToken cancellationToken = default);
    Task<bool> CompleteAsync(CompleteSyncProfileExecutionData data, CancellationToken cancellationToken = default);
    Task<bool> CancelAsync(int executionId, string? cancelledBy, CancellationToken cancellationToken = default);
    Task<SyncProfileExecutionDetailDto?> GetByIdAsync(int executionId, CancellationToken cancellationToken = default);
    Task<PagedResultDto<SyncProfileExecutionListItemDto>> SearchAsync(SyncProfileExecutionFilter filter, CancellationToken cancellationToken = default);
    Task<int?> GetActiveExecutionAsync(int syncProfileId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SyncProfileExecutionDetailDto>> GetPendingAsync(int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<DueSyncProfileDto>> GetDueProfilesAsync(DateTimeOffset utcNow, CancellationToken cancellationToken = default);
    Task<bool> MarkScheduledAsync(int syncProfileId, DateTimeOffset nextExecutionAt, CancellationToken cancellationToken = default);
    Task<int> UpsertDetailAsync(SyncProfileExecutionDetailUpdate data, CancellationToken cancellationToken = default);
}
