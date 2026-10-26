using System.Data;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

// Reference adapter: forwards the current handlers to the pre-pilot ADO contracts.
// Deliberately independent of BusinessPartnerUnitOfWork; not a copy of a historical handler.
internal sealed class Phase7LegacyBusinessPartnerUnitOfWork(ITransactionRunner runner,
    IBusinessPartnerRepository repository, IBusinessPartnerLocalOutboxWriter writer) : IBusinessPartnerUnitOfWork
{
    public Task<T> ExecuteAsync<T>(Func<IBusinessPartnerTransaction, CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default) =>
        runner.ExecuteInTenantTransactionAsync((c, t, ct) => operation(new LegacyContext(repository, writer, c, t), ct), cancellationToken);

    private sealed class LegacyContext(IBusinessPartnerRepository repository, IBusinessPartnerLocalOutboxWriter writer,
        IDbConnection connection, IDbTransaction transaction) : IBusinessPartnerTransaction
    {
        public Task<BusinessPartnerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => repository.GetByIdAsync(id, connection, transaction, cancellationToken);
        public Task<bool> ExistsByCodeAsync(string code, int? excludingId, CancellationToken cancellationToken = default) => repository.ExistsByCodeAsync(code, excludingId, connection, transaction, cancellationToken);
        public Task<bool> ExistsByIdentificationAsync(int typeId, string number, int? excludingId, CancellationToken cancellationToken = default) => repository.ExistsByIdentificationAsync(typeId, number, excludingId, connection, transaction, cancellationToken);
        public Task<int> CreateAsync(CreateBusinessPartnerData partner, CancellationToken cancellationToken = default) => repository.CreateAsync(partner, connection, transaction, cancellationToken);
        public Task<bool> UpdateAsync(UpdateBusinessPartnerData partner, CancellationToken cancellationToken = default) => repository.UpdateAsync(partner, connection, transaction, cancellationToken);
        public Task<bool> DeleteAsync(int id, int? userId, string? userName, CancellationToken cancellationToken = default) => repository.DeleteAsync(id, userId, userName, connection, transaction, cancellationToken);
        public Task<Guid?> EnqueueAsync(BusinessPartnerDto partner, SyncOperation operation, CancellationToken cancellationToken = default) => writer.EnqueueAsync(partner, operation, connection, transaction, cancellationToken);
    }
}
