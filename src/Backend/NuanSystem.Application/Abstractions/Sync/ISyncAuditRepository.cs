using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncAuditRepository
{
    Task<long> AddAsync(CreateSyncAuditData data, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SyncAuditDto>> GetRecentAsync(int companyId, int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SyncAuditDto>> SearchAuditAsync(int companyId, SyncAuditQueryFilter filter, CancellationToken cancellationToken = default);
}
