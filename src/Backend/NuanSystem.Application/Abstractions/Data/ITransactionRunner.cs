using System.Data;

namespace NuanSystem.Application.Abstractions.Data;

public interface ITransactionRunner
{
    Task ExecuteInTenantTransactionAsync(
        Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation,
        CancellationToken cancellationToken = default);

    Task<T> ExecuteInTenantTransactionAsync<T>(
        Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default);
}
