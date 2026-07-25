namespace NuanSystem.MasterBranchSyncWorker.Services;

public interface ILocalSyncOutboxRelay
{
    Task<int> ProcessOnceAsync(CancellationToken cancellationToken = default);
}
