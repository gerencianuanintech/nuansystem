using System.Data;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Persistence.Transactions;

namespace NuanSystem.Application.Tests.Architecture;

public sealed class SqlTransactionRunnerTests
{
    [Fact]
    public async Task Rollback_failure_preserves_the_original_operation_failure()
    {
        var (runner, _, transaction, _) = Create();
        var original = new InvalidOperationException("Operation failed");
        var rollback = new InvalidOperationException("Rollback failed");
        transaction.When(item => item.Rollback()).Do(_ => throw rollback);
        var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => runner.ExecuteInTenantTransactionAsync<int>(
            (_, _, _) => Task.FromException<int>(original)));
        Assert.Same(original, actual);
        Assert.Same(rollback, actual.Data["NuanSystem.Transaction.RollbackException"]);
        transaction.DidNotReceive().Commit();
    }

    [Fact]
    public async Task Pre_cancelled_operation_does_not_create_a_connection()
    {
        var (runner, _, _, factory) = Create();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => runner.ExecuteInTenantTransactionAsync<int>(
            (_, _, _) => Task.FromResult(1), cancellation.Token));
        factory.DidNotReceive().CreateConnection();
    }

    [Fact]
    public async Task Cancellation_after_operation_rolls_back_instead_of_committing()
    {
        var (runner, _, transaction, _) = Create();
        using var cancellation = new CancellationTokenSource();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => runner.ExecuteInTenantTransactionAsync<int>(
            (_, _, _) => { cancellation.Cancel(); return Task.FromResult(1); }, cancellation.Token));
        transaction.Received(1).Rollback();
        transaction.DidNotReceive().Commit();
    }

    [Fact]
    public async Task Commit_failure_is_propagated_without_retrying_the_operation()
    {
        var (runner, _, transaction, _) = Create();
        var original = new InvalidOperationException("Commit failed");
        transaction.When(item => item.Commit()).Do(_ => throw original);
        var calls = 0;
        var actual = await Assert.ThrowsAsync<InvalidOperationException>(() => runner.ExecuteInTenantTransactionAsync<int>(
            (_, _, _) => { calls++; return Task.FromResult(1); }));
        Assert.Same(original, actual);
        Assert.Equal(1, calls);
        transaction.Received(1).Commit();
        transaction.Received(1).Rollback();
    }

    [Fact]
    public async Task Cancellation_remains_the_primary_error_when_rollback_also_fails()
    {
        var (runner, _, transaction, _) = Create();
        using var cancellation = new CancellationTokenSource();
        var rollback = new InvalidOperationException("Rollback failed");
        transaction.When(item => item.Rollback()).Do(_ => throw rollback);
        var actual = await Assert.ThrowsAnyAsync<OperationCanceledException>(() => runner.ExecuteInTenantTransactionAsync<int>(
            (_, _, _) => { cancellation.Cancel(); return Task.FromResult(1); }, cancellation.Token));
        Assert.Equal(cancellation.Token, actual.CancellationToken);
        Assert.Same(rollback, actual.Data["NuanSystem.Transaction.RollbackException"]);
        transaction.DidNotReceive().Commit();
    }

    [Fact]
    public async Task Success_commits_once_and_disposes_connection_and_transaction()
    {
        var (runner, connection, transaction, _) = Create();
        Assert.Equal(42, await runner.ExecuteInTenantTransactionAsync((_, _, _) => Task.FromResult(42)));
        transaction.Received(1).Commit();
        transaction.DidNotReceive().Rollback();
        transaction.Received(1).Dispose();
        connection.Received(1).Dispose();
    }

    internal static (SqlTransactionRunner Runner, IDbConnection Connection, IDbTransaction Transaction, ITenantConnectionFactory Factory) Create()
    {
        var connection = Substitute.For<IDbConnection>();
        var transaction = Substitute.For<IDbTransaction>();
        connection.BeginTransaction().Returns(transaction);
        transaction.Connection.Returns(connection);
        var factory = Substitute.For<ITenantConnectionFactory>();
        factory.CreateConnection().Returns(connection);
        return (new SqlTransactionRunner(factory), connection, transaction, factory);
    }
}
