namespace NuanSystem.MasterBranchSyncWorker.Services;

public interface IMasterBranchSyncWorkerProcessor
{
    Task<int> ProcessOnceAsync(CancellationToken cancellationToken = default);
}
