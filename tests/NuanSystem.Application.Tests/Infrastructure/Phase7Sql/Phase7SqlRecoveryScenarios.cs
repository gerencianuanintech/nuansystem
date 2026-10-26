using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.BusinessPartners.Commands;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlScopeState(int Spid, string DatabaseName, int DatabaseId, int TransactionCount, int TransactionState, bool HasFaultMarkers);
internal delegate Task<Phase7SqlScopeState> Phase7SqlScopeStateReader(IDbConnection connection, IDbTransaction transaction, int timeout, CancellationToken token);
internal sealed record Phase7SqlReliabilityScope(IBusinessPartnerUnitOfWork Unit, Action InvalidateCompany, Action RestoreCompany);
internal sealed class Phase7SqlOwnerClosedException() : Exception("PHASE7_SQL_OWNER_CLOSED_BEFORE_COMMIT");

internal static class Phase7SqlScopeHygiene
{
    public static Task<Phase7SqlScopeState> ReadAsync(IDbConnection connection, IDbTransaction transaction, int timeout, CancellationToken token) =>
        connection.QuerySingleAsync<Phase7SqlScopeState>(new CommandDefinition("""
SELECT CAST(@@SPID AS int) AS Spid, DB_NAME() AS DatabaseName, DB_ID() AS DatabaseId,
       @@TRANCOUNT AS TransactionCount, XACT_STATE() AS TransactionState,
       CAST(CASE WHEN SESSION_CONTEXT(N'P7_RunId') IS NOT NULL
                   OR SESSION_CONTEXT(N'P7_CompanyId') IS NOT NULL
                   OR SESSION_CONTEXT(N'P7_CaseCode') IS NOT NULL
                   OR SESSION_CONTEXT(N'P7_FaultPoint') IS NOT NULL THEN 1 ELSE 0 END AS bit) AS HasFaultMarkers;
""", transaction: transaction, commandTimeout: timeout, cancellationToken: token));

    public static void AssertClean(Phase7SqlScopeState state, string database, int databaseId)
    {
        Phase7SqlReliabilityScenarios.Require(state.Spid > 0 && state.DatabaseName == database && state.DatabaseId == databaseId
            && state.TransactionCount == 1 && state.TransactionState == 1 && !state.HasFaultMarkers, "SCOPE_NOT_CLEAN");
    }

    public static void AssertReuse(IReadOnlyList<Phase7SqlScopeState> states, string database, int databaseId)
    {
        foreach (var state in states) AssertClean(state, database, databaseId);
        Phase7SqlReliabilityScenarios.Require(states.Count >= 2 && states.GroupBy(s => s.Spid).Any(group => group.Count() >= 2), "PHYSICAL_SESSION_REUSE_NOT_OBSERVED");
    }
}

internal static class Phase7SqlRecoveryScenarios
{
    public static async Task<Exception> CloseBeforeCommitOnFixtureAsync(Phase7SqlFixture.Phase7SqlTenant tenant, CreateBusinessPartnerCommand command,
        Func<IDbConnection, int, CancellationToken, Task<Phase7SqlRollbackSnapshot>> reader, CancellationToken token = default)
    {
        var owner = tenant.CreateIndependentScope();
        var failure = await CloseBeforeCommitAsync(owner.CreateReliabilityScope(pooling: false, closeAfterEnqueue: true).Unit, command,
            ct => tenant.ObserveAsync((c, timeout, observerToken) => reader(c, timeout, observerToken), ct), token);
        if (owner.TotalWriteConnections != 1 || owner.Samples.Count(s => s.Checkpoint == "AfterEnqueue") != 1)
        {
            var error = new Phase7SqlGuardException("PHASE7_SQL_OWNER_CLOSE_RECONNECTED_OR_REPLAYED");
            error.Data["Phase7Sql.PrimaryFailure"] = failure;
            throw error;
        }
        return failure;
    }

    public static async Task InvalidContextsOnFixtureAsync(Phase7SqlFixture.Phase7SqlTenant tenant,
        Func<IDbConnection, int, CancellationToken, Task<Phase7SqlRollbackSnapshot>> reader, CancellationToken token = default)
    {
        var before = await tenant.ObserveAsync((c, timeout, ct) => reader(c, timeout, ct), token);
        var independent = tenant.CreateIndependentScope();
        await InvalidContextsAsync(independent.CreateReliabilityScope(), () => independent.TotalWriteConnections, token);
        var after = await tenant.ObserveAsync((c, timeout, ct) => reader(c, timeout, ct), token);
        Phase7SqlRollbackSnapshot.AssertUnchanged(before, after);
    }

    public static async Task<Exception> CloseBeforeCommitAsync(IBusinessPartnerUnitOfWork unit, CreateBusinessPartnerCommand command,
        Func<CancellationToken, Task<Phase7SqlRollbackSnapshot>> observe, CancellationToken token = default)
    {
        Phase7SqlReliabilityScenarios.Require((await new CreateBusinessPartnerCommandValidator().ValidateAsync(command, token)).IsValid, "COMMAND_INVALID");
        var before = await observe(token);
        var counted = new CountingUnit(unit);
        Phase7SqlOwnerClosedException primary;
        try
        {
            await new CreateBusinessPartnerCommandHandler(counted).Handle(command, token);
            throw new Phase7SqlGuardException("PHASE7_SQL_OWNER_CLOSE_NOT_RAISED");
        }
        catch (Phase7SqlOwnerClosedException error) { primary = error; }
        if (counted.Callbacks != 1)
        {
            var error = new Phase7SqlGuardException("PHASE7_SQL_OWNER_CALLBACK_REPLAYED");
            error.Data["Phase7Sql.PrimaryFailure"] = primary;
            throw error;
        }
        try { Phase7SqlRollbackSnapshot.AssertUnchanged(before, await observe(token)); }
        catch (Exception error) { error.Data["Phase7Sql.PrimaryFailure"] = primary; throw; }
        token.ThrowIfCancellationRequested();
        return primary; // no retry: uncertain commit after response loss is a separate reconciliation case
    }

    public static async Task InvalidContextsAsync(Phase7SqlReliabilityScope scope, Func<int> connectionCount, CancellationToken token = default)
    {
        IBusinessPartnerTransaction? escaped = null;
        var count = connectionCount();
        await scope.Unit.ExecuteAsync((context, _) => { escaped = context; return Task.FromResult(1); }, token);
        await Expect<ObjectDisposedException>(() => escaped!.GetByIdAsync(1, token));
        Phase7SqlReliabilityScenarios.Require(connectionCount() == count + 1, "ESCAPED_CONTEXT_OPENED_CONNECTION");

        count = connectionCount();
        var failure = new InvalidOperationException("P7_SCOPE_CALLBACK_FAILURE");
        try
        {
            await scope.Unit.ExecuteAsync<int>((context, _) => { escaped = context; return Task.FromException<int>(failure); }, token);
            throw new Phase7SqlGuardException("PHASE7_SQL_SCOPE_FAILURE_NOT_RAISED");
        }
        catch (InvalidOperationException error) when (ReferenceEquals(error, failure)) { }
        await Expect<ObjectDisposedException>(() => escaped!.GetByIdAsync(1, token));
        Phase7SqlReliabilityScenarios.Require(connectionCount() == count + 1, "FAILED_CONTEXT_OPENED_CONNECTION");

        count = connectionCount();
        await scope.Unit.ExecuteAsync(async (_, ct) =>
        {
            await Expect<InvalidOperationException>(() => scope.Unit.ExecuteAsync((_, _) => Task.FromResult(1), ct), IsOverlap);
            return 1;
        }, token);
        Phase7SqlReliabilityScenarios.Require(connectionCount() == count + 1, "NESTED_EXECUTION_OPENED_CONNECTION");

        count = connectionCount();
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var active = scope.Unit.ExecuteAsync(async (_, ct) => { entered.TrySetResult(); await release.Task.WaitAsync(ct); return 1; }, token);
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5), token);
            await Expect<InvalidOperationException>(() => scope.Unit.ExecuteAsync((_, _) => Task.FromResult(1), token), IsOverlap);
        }
        finally { release.TrySetResult(); await active.WaitAsync(TimeSpan.FromSeconds(5)); } // drain even when the caller token was cancelled
        Phase7SqlReliabilityScenarios.Require(connectionCount() == count + 1, "OVERLAP_OPENED_CONNECTION");

        count = connectionCount();
        try
        {
            await Expect<InvalidOperationException>(() => scope.Unit.ExecuteAsync<int>(async (context, ct) =>
            {
                scope.InvalidateCompany();
                await context.GetByIdAsync(1, ct);
                throw new Phase7SqlGuardException("PHASE7_SQL_COMPANY_CHANGE_NOT_REJECTED");
            }, token), e => e.Message.StartsWith("La empresa activa cambio", StringComparison.Ordinal));
        }
        finally { scope.RestoreCompany(); }
        Phase7SqlReliabilityScenarios.Require(connectionCount() == count + 1, "COMPANY_CHANGE_OPENED_CONNECTION");
        Phase7SqlReliabilityScenarios.Require(await scope.Unit.ExecuteAsync((_, _) => Task.FromResult(42), token) == 42, "UNIT_NOT_REUSABLE");
    }

    private static bool IsOverlap(InvalidOperationException error) => error.Message.Contains("no admite operaciones anidadas o simultaneas", StringComparison.Ordinal);
    private sealed class CountingUnit(IBusinessPartnerUnitOfWork inner) : IBusinessPartnerUnitOfWork
    {
        private int callbacks;
        public int Callbacks => Volatile.Read(ref callbacks);
        public Task<T> ExecuteAsync<T>(Func<IBusinessPartnerTransaction, CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default) =>
            inner.ExecuteAsync((context, token) => { Interlocked.Increment(ref callbacks); return operation(context, token); }, cancellationToken);
    }
    private static async Task Expect<T>(Func<Task> operation, Func<T, bool>? expected = null) where T : Exception
    {
        try { await operation(); }
        catch (T error) when (expected is null || expected(error)) { return; }
        throw new Phase7SqlGuardException("PHASE7_SQL_INVALID_CONTEXT_ACCEPTED");
    }
}
