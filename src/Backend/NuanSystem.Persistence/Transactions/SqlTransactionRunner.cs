using System.Data;
using System.Data.Common;
using NuanSystem.Application.Abstractions.Data;

namespace NuanSystem.Persistence.Transactions;

public sealed class SqlTransactionRunner(ITenantConnectionFactory connectionFactory) : ITransactionRunner
{
    public async Task ExecuteInTenantTransactionAsync(
        Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        await ExecuteInTenantTransactionAsync<object?>(
            async (connection, transaction, token) =>
            {
                await operation(connection, transaction, token);
                return null;
            },
            cancellationToken);
    }

    public async Task<T> ExecuteInTenantTransactionAsync<T>(
        Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await OpenAsync(connection, cancellationToken);

        using var transaction = connection.BeginTransaction();
        try
        {
            var result = await operation(connection, transaction, cancellationToken);
            transaction.Commit();
            return result;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static Task OpenAsync(IDbConnection connection, CancellationToken cancellationToken)
    {
        return connection is DbConnection dbConnection
            ? dbConnection.OpenAsync(cancellationToken)
            : Task.Run(connection.Open, cancellationToken);
    }
}
