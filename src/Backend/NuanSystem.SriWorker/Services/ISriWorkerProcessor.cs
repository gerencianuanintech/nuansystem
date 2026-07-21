namespace NuanSystem.SriWorker.Services;

public interface ISriWorkerProcessor
{
    Task<int> ProcessOnceAsync(CancellationToken cancellationToken = default);
}
