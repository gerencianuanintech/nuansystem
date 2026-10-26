using System.Data;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Transactions;

public sealed class BusinessPartnerUnitOfWork(
    ITransactionRunner runner,
    IBusinessPartnerRepository repository,
    IBusinessPartnerLocalOutboxWriter writer,
    ICompanyContext companyContext) : IBusinessPartnerUnitOfWork
{
    private int active;

    public async Task<T> ExecuteAsync<T>(Func<IBusinessPartnerTransaction, CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        cancellationToken.ThrowIfCancellationRequested();
        var company = companyContext.CurrentCompany;
        if (!companyContext.HasActiveCompany || company is null)
            throw new InvalidOperationException("No hay empresa activa para la operacion transaccional.");
        if (Interlocked.CompareExchange(ref active, 1, 0) != 0)
            throw new InvalidOperationException("La unidad transaccional de socios no admite operaciones anidadas o simultaneas.");
        try
        {
            return await runner.ExecuteInTenantTransactionAsync(async (connection, transaction, token) =>
            {
                using var context = new Context(repository, writer, connection, transaction, () =>
                    companyContext.HasActiveCompany && ReferenceEquals(companyContext.CurrentCompany, company), token);
                context.EnsureUsable(token);
                var result = await operation(context, token);
                context.EnsureUsable(token);
                return result;
            }, cancellationToken);
        }
        finally
        {
            Volatile.Write(ref active, 0);
        }
    }

    private sealed class Context(
        IBusinessPartnerRepository repository,
        IBusinessPartnerLocalOutboxWriter writer,
        IDbConnection connection,
        IDbTransaction transaction,
        Func<bool> sameCompany,
        CancellationToken operationToken) : IBusinessPartnerTransaction, IDisposable
    {
        private bool disposed;

        public void EnsureUsable(CancellationToken token)
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            operationToken.ThrowIfCancellationRequested();
            token.ThrowIfCancellationRequested();
            if (!sameCompany())
                throw new InvalidOperationException("La empresa activa cambio durante la operacion transaccional.");
        }

        public Task<BusinessPartnerDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return RunAsync(token => repository.GetByIdAsync(id, connection, transaction, token), cancellationToken);
        }

        public Task<bool> ExistsByCodeAsync(string code, int? excludingId, CancellationToken cancellationToken = default)
        {
            return RunAsync(token => repository.ExistsByCodeAsync(code, excludingId, connection, transaction, token), cancellationToken);
        }

        public Task<bool> ExistsByIdentificationAsync(int typeId, string number, int? excludingId, CancellationToken cancellationToken = default)
        {
            return RunAsync(token => repository.ExistsByIdentificationAsync(typeId, number, excludingId, connection, transaction, token), cancellationToken);
        }

        public Task<int> CreateAsync(CreateBusinessPartnerData partner, CancellationToken cancellationToken = default)
        {
            return RunAsync(token => repository.CreateAsync(partner, connection, transaction, token), cancellationToken);
        }

        public Task<bool> UpdateAsync(UpdateBusinessPartnerData partner, CancellationToken cancellationToken = default)
        {
            return RunAsync(token => repository.UpdateAsync(partner, connection, transaction, token), cancellationToken);
        }

        public Task<bool> DeleteAsync(int id, int? userId, string? userName, CancellationToken cancellationToken = default)
        {
            return RunAsync(token => repository.DeleteAsync(id, userId, userName, connection, transaction, token), cancellationToken);
        }

        public Task<Guid?> EnqueueAsync(BusinessPartnerDto partner, SyncOperation operation, CancellationToken cancellationToken = default)
        {
            return RunAsync(token => writer.EnqueueAsync(partner, operation, connection, transaction, token), cancellationToken);
        }

        private async Task<T> RunAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken token)
        {
            EnsureUsable(token);
            using var linked = operationToken.CanBeCanceled && token.CanBeCanceled && token != operationToken
                ? CancellationTokenSource.CreateLinkedTokenSource(operationToken, token) : null;
            var effectiveToken = linked?.Token ?? (token.CanBeCanceled ? token : operationToken);
            var result = await operation(effectiveToken);
            EnsureUsable(token);
            return result;
        }

        public void Dispose() => disposed = true;
    }
}
