using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

// Adds measurements inside the real unit's callback; never creates a surrounding transaction.
internal sealed class Phase7NominalCheckpointUnitOfWork(IBusinessPartnerUnitOfWork inner,
    Func<string, CancellationToken, Task> checkpoint) : IBusinessPartnerUnitOfWork
{
    public Task<T> ExecuteAsync<T>(Func<IBusinessPartnerTransaction, CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default) =>
        inner.ExecuteAsync((context, ct) => operation(new Context(context, checkpoint), ct), cancellationToken);

    private sealed class Context(IBusinessPartnerTransaction inner, Func<string, CancellationToken, Task> checkpoint) : IBusinessPartnerTransaction
    {
        private async Task<T> Measure<T>(Task<T> operation, string label, CancellationToken token)
        {
            var result = await operation;
            await checkpoint(label, token);
            return result;
        }
        public Task<BusinessPartnerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => inner.GetByIdAsync(id, cancellationToken);
        public Task<bool> ExistsByCodeAsync(string code, int? excludingId, CancellationToken cancellationToken = default) => inner.ExistsByCodeAsync(code, excludingId, cancellationToken);
        public Task<bool> ExistsByIdentificationAsync(int typeId, string number, int? excludingId, CancellationToken cancellationToken = default) => inner.ExistsByIdentificationAsync(typeId, number, excludingId, cancellationToken);
        public Task<int> CreateAsync(CreateBusinessPartnerData partner, CancellationToken cancellationToken = default) => Measure(inner.CreateAsync(partner, cancellationToken), "AfterCreate", cancellationToken);
        public Task<bool> UpdateAsync(UpdateBusinessPartnerData partner, CancellationToken cancellationToken = default) => Measure(inner.UpdateAsync(partner, cancellationToken), "AfterUpdate", cancellationToken);
        public Task<bool> DeleteAsync(int id, int? userId, string? userName, CancellationToken cancellationToken = default) => Measure(inner.DeleteAsync(id, userId, userName, cancellationToken), "AfterDelete", cancellationToken);
        public Task<Guid?> EnqueueAsync(BusinessPartnerDto partner, SyncOperation operation, CancellationToken cancellationToken = default) => Measure(inner.EnqueueAsync(partner, operation, cancellationToken), "AfterEnqueue", cancellationToken);
    }
}
