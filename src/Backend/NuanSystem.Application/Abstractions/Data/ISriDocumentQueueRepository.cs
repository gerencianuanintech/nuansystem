using NuanSystem.Application.Features.SriDocuments.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISriDocumentQueueRepository : IRepository
{
    Task<SriDocumentQueuePersistenceResult> EnqueueAsync(EnqueueSriDocumentData data, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SriDocumentQueueListItemDto>> SearchAsync(SriDocumentQueueFilter filter, CancellationToken cancellationToken = default);
    Task<SriDocumentQueueDetailDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SriDocumentAttemptDto>> GetAttemptsAsync(long queueId, CancellationToken cancellationToken = default);
    Task<SriDocumentQueueActionCode> CancelAsync(SriDocumentQueueActionData data, CancellationToken cancellationToken = default);
    Task<SriDocumentQueueActionCode> ReprocessAsync(SriDocumentQueueActionData data, CancellationToken cancellationToken = default);
}
