using NuanSystem.Application.Features.SriDocuments.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISriDocumentQueueRepository : IRepository
{
    Task<SriDocumentQueuePersistenceResult> EnqueueAsync(EnqueueSriDocumentData data, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SriDocumentQueueListItemDto>> SearchAsync(SriDocumentQueueFilter filter, CancellationToken cancellationToken = default);
    Task<SriDocumentQueueDetailDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SriDocumentAttemptDto>> GetAttemptsAsync(long queueId, CancellationToken cancellationToken = default);
    Task<SriDocumentMonitorSummaryDto> GetMonitorSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SriDocumentMonitorListItemDto>> SearchMonitorAsync(SriDocumentMonitorFilter filter, CancellationToken cancellationToken = default);
    Task<SriDocumentMonitorDetailDto?> GetMonitorDetailAsync(long queueId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SriDocumentAuditDto>> GetAuditAsync(long queueId, CancellationToken cancellationToken = default);
    Task<SriAuthorizedXmlPersistenceResult> DownloadAuthorizedXmlAsync(SriAuthorizedXmlDownloadData data, CancellationToken cancellationToken = default);
    Task<SriDocumentQueueActionCode> CancelAsync(SriDocumentQueueActionData data, CancellationToken cancellationToken = default);
    Task<SriDocumentQueueActionCode> ReprocessAsync(SriDocumentQueueActionData data, CancellationToken cancellationToken = default);
}
