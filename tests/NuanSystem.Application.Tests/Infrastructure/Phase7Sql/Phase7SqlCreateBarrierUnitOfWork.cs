using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed class Phase7SqlCreateBarrier
{
    private readonly TaskCompletionSource ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private int arrivals;
    public int Arrivals => Volatile.Read(ref arrivals);
    public Task ArriveAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var count = Interlocked.Increment(ref arrivals);
        if (count > 2) throw new Phase7SqlGuardException("PHASE7_SQL_CREATE_BARRIER_REUSED");
        if (count == 2) ready.TrySetResult();
        return ready.Task.WaitAsync(token);
    }
}

// Both contenders pass application duplicate checks before reaching the actual INSERT.
internal sealed class Phase7SqlCreateBarrierUnitOfWork(IBusinessPartnerUnitOfWork inner, Phase7SqlCreateBarrier barrier) : IBusinessPartnerUnitOfWork
{
    public Task<T> ExecuteAsync<T>(Func<IBusinessPartnerTransaction, CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default) =>
        inner.ExecuteAsync((context, ct) => operation(new Context(context, barrier), ct), cancellationToken);

    private sealed class Context(IBusinessPartnerTransaction inner, Phase7SqlCreateBarrier barrier) : IBusinessPartnerTransaction
    {
        public Task<BusinessPartnerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => inner.GetByIdAsync(id, cancellationToken);
        public Task<bool> ExistsByCodeAsync(string code, int? excludingId, CancellationToken cancellationToken = default) => inner.ExistsByCodeAsync(code, excludingId, cancellationToken);
        public Task<bool> ExistsByIdentificationAsync(int typeId, string number, int? excludingId, CancellationToken cancellationToken = default) => inner.ExistsByIdentificationAsync(typeId, number, excludingId, cancellationToken);
        public async Task<int> CreateAsync(CreateBusinessPartnerData partner, CancellationToken cancellationToken = default)
        {
            await barrier.ArriveAsync(cancellationToken);
            return await inner.CreateAsync(partner, cancellationToken);
        }
        public Task<bool> UpdateAsync(UpdateBusinessPartnerData partner, CancellationToken cancellationToken = default) => inner.UpdateAsync(partner, cancellationToken);
        public Task<bool> DeleteAsync(int id, int? userId, string? userName, CancellationToken cancellationToken = default) => inner.DeleteAsync(id, userId, userName, cancellationToken);
        public Task<Guid?> EnqueueAsync(BusinessPartnerDto partner, SyncOperation operation, CancellationToken cancellationToken = default) => inner.EnqueueAsync(partner, operation, cancellationToken);
    }
}
