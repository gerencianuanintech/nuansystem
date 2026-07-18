using NuanSystem.Application.Features.Sync.Distribution;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncDistributionPolicyRepository
{
    Task<SyncDistributionPolicyDto?> GetByMatrixIdAsync(int matrixId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(UpdateSyncDistributionPolicyData data, CancellationToken cancellationToken = default);
}
