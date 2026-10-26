using System.Runtime.ExceptionServices;
using System.Text.Json.Nodes;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Data;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

public enum Phase7SqlFailureCase
{
    AuditInsert = 5, OutboxInsert, SyncAuditInsert, AfterProcedure, ProcedureError, CancelAfterWrites, CancelDuringCommand
}

internal sealed class Phase7SqlRollbackSnapshot
{
    public static IReadOnlyList<string> RequiredTables { get; } = Array.AsReadOnly(new[]
    {
        "BusinessPartners", "BusinessPartnerAddresses", "BusinessPartnerContacts", "BusinessPartnerFiscalData",
        "BusinessPartnerAccountingSettings", "BusinessPartnerCreditSettings", "BusinessPartnerPurchaseSettings",
        "BusinessPartnerSapMapping", "BusinessPartnerBankAccounts", "BusinessPartnerSapFieldMappings",
        "BusinessPartnerRetentionSettings", "BusinessPartnerNotes", "BusinessPartnerAttachmentsMetadata",
        "AuditCatalogChanges", "LocalOutbox", "SyncAudit"
    });
    private readonly JsonObject tables = [];

    public Phase7SqlRollbackSnapshot(IReadOnlyDictionary<string, JsonArray> source)
    {
        if (source.Count != RequiredTables.Count || RequiredTables.Any(name => !source.ContainsKey(name) || source[name] is null))
            throw new Phase7SqlGuardException("PHASE7_SQL_ROLLBACK_SNAPSHOT_INCOMPLETE");
        foreach (var name in RequiredTables) tables[name] = source[name].DeepClone();
    }

    public static void AssertUnchanged(Phase7SqlRollbackSnapshot before, Phase7SqlRollbackSnapshot after)
    {
        // In one fixture rollback must preserve exact row identities, values and timestamps.
        // Sequence/IDENTITY allocation is not a table row and must not be included by the reader.
        if (!JsonNode.DeepEquals(before.tables, after.tables)) throw new Phase7SqlGuardException("PHASE7_SQL_ROLLBACK_STATE_CHANGED");
    }
}

internal sealed class Phase7SqlCancellationBarrier(Phase7SqlFailureCase fault)
{
    private readonly TaskCompletionSource reached = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public Task Reached => reached.Task;
    public bool Acknowledged => reached.Task.IsCompletedSuccessfully;
    public CancellationToken OperationToken { get; private set; }

    public void Acknowledge(Phase7SqlFailureCase point, CancellationToken token)
    {
        if (point != fault || point is not (Phase7SqlFailureCase.CancelAfterWrites or Phase7SqlFailureCase.CancelDuringCommand)
            || !token.CanBeCanceled || reached.Task.IsCompleted) throw new Phase7SqlGuardException("PHASE7_SQL_CANCELLATION_BARRIER_INVALID");
        OperationToken = token;
        if (!reached.TrySetResult()) throw new Phase7SqlGuardException("PHASE7_SQL_CANCELLATION_BARRIER_INVALID");
    }

    public Task WaitForCancellationAsync(Phase7SqlFailureCase point, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        Acknowledge(point, token);
        return Task.Delay(Timeout.InfiniteTimeSpan, token);
    }
}

internal sealed record Phase7SqlFailureEvidence(Phase7SqlFailureCase Case, Exception Failure, bool BarrierAcknowledged,
    Phase7SqlRollbackSnapshot Before, Phase7SqlRollbackSnapshot After);

internal static class Phase7SqlFailureScenarios
{
    public static Task<Phase7SqlFailureEvidence> RunOnFixtureAsync(Phase7SqlFixture.Phase7SqlTenant tenant,
        Phase7SqlFaultSessionOptions options, Func<IBusinessPartnerUnitOfWork, CancellationToken, Task> operation,
        Func<System.Data.IDbConnection, int, CancellationToken, Task<Phase7SqlRollbackSnapshot>> readSnapshot,
        TimeSpan timeout, Func<SqlException, bool>? expectedProcedureError = null, CancellationToken token = default)
    {
        options.Validate();
        var commandTimeout = tenant.CommandTimeoutSeconds;
        if (options.RunId != tenant.RunId || (options.Case == Phase7SqlFailureCase.CancelDuringCommand && commandTimeout < 6))
            throw new Phase7SqlGuardException("PHASE7_SQL_FAULT_SESSION_CONFIG");
        if (options.Case == Phase7SqlFailureCase.ProcedureError && expectedProcedureError is null)
            throw new Phase7SqlGuardException("PHASE7_SQL_PROCEDURE_ERROR_EXPECTATION_REQUIRED");
        return RunAsync(options.Case, async (barrier, ct) =>
        {
            var unit = tenant.CreateFailureUnitOfWork(options, async (label, c, t, innerToken) =>
            {
                if (options.Case == Phase7SqlFailureCase.AfterProcedure && label is "AfterCreate" or "AfterUpdate" or "AfterDelete")
                    throw new Phase7SqlAfterProcedureException();
                if (label != "AfterEnqueue") return;
                if (options.Case == Phase7SqlFailureCase.CancelAfterWrites)
                    await barrier.WaitForCancellationAsync(options.Case, innerToken);
                else if (options.Case == Phase7SqlFailureCase.CancelDuringCommand)
                    await Phase7SqlFaultSession.WaitForCancellationAsync((SqlConnection)c, (SqlTransaction)t, options, barrier, commandTimeout, innerToken);
            });
            await operation(unit, ct);
        }, ct => tenant.ObserveAsync((c, _, observerToken) => readSnapshot(c, commandTimeout, observerToken), ct),
            error => options.Case switch
            {
                Phase7SqlFailureCase.AuditInsert => error is SqlException { Number: 51505 },
                Phase7SqlFailureCase.OutboxInsert => error is SqlException { Number: 51506 },
                Phase7SqlFailureCase.SyncAuditInsert => error is SqlException { Number: 51507 },
                Phase7SqlFailureCase.AfterProcedure => error is Phase7SqlAfterProcedureException,
                Phase7SqlFailureCase.ProcedureError => error is SqlException sql && expectedProcedureError!(sql),
                _ => false
            }, timeout, token);
    }

    public static async Task<Phase7SqlFailureEvidence> RunAsync(Phase7SqlFailureCase fault,
        Func<Phase7SqlCancellationBarrier, CancellationToken, Task> operation,
        Func<CancellationToken, Task<Phase7SqlRollbackSnapshot>> observe,
        Func<Exception, bool> expectedFailure, TimeSpan timeout, CancellationToken token = default)
    {
        if (!Enum.IsDefined(fault) || timeout < TimeSpan.FromSeconds(1) || timeout > TimeSpan.FromSeconds(30))
            throw new Phase7SqlGuardException("PHASE7_SQL_FAILURE_CASE_CONFIG");
        token.ThrowIfCancellationRequested();
        using var budget = CancellationTokenSource.CreateLinkedTokenSource(token);
        budget.CancelAfter(timeout);
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(budget.Token);
        var before = await observe(budget.Token).WaitAsync(budget.Token);
        var barrier = new Phase7SqlCancellationBarrier(fault);
        async Task Invoke() => await operation(barrier, cancellation.Token);
        var task = Invoke();
        Exception? failure = null;
        var cancels = fault is Phase7SqlFailureCase.CancelAfterWrites or Phase7SqlFailureCase.CancelDuringCommand;
        try
        {
            if (cancels)
            {
                await Task.WhenAny(barrier.Reached, task).WaitAsync(budget.Token);
                if (barrier.Acknowledged) cancellation.Cancel();
            }
            try { await task.WaitAsync(budget.Token); }
            catch (Exception error) when (task.IsCompleted && !budget.IsCancellationRequested) { failure = error; }
            budget.Token.ThrowIfCancellationRequested();
        }
        catch (OperationCanceledException) when (budget.IsCancellationRequested)
        {
            cancellation.Cancel();
            // Never read rollback snapshots while the operation is still unwinding.
            try { await task.WaitAsync(TimeSpan.FromSeconds(2)); }
            catch (Exception) when (task.IsCompleted) { /* Observed terminal failure while draining. */ }
            catch (TimeoutException) { throw new Phase7SqlGuardException("PHASE7_SQL_OPERATION_NOT_STOPPED"); }
            token.ThrowIfCancellationRequested();
            throw new Phase7SqlGuardException("PHASE7_SQL_FAILURE_CASE_DEADLINE");
        }

        Phase7SqlRollbackSnapshot after;
        try { after = await observe(budget.Token).WaitAsync(budget.Token); } // independent of the cancelled operation token
        catch (Exception error)
        {
            if (failure is not null) error.Data["Phase7Sql.PrimaryFailure"] = failure;
            throw;
        }
        try { Phase7SqlRollbackSnapshot.AssertUnchanged(before, after); }
        catch (Phase7SqlGuardException error)
        {
            if (failure is not null) error.Data["Phase7Sql.PrimaryFailure"] = failure;
            throw;
        }
        if (failure is null) throw new Phase7SqlGuardException("PHASE7_SQL_EXPECTED_FAILURE_NOT_RAISED");
        if (cancels)
        {
            if (!barrier.Acknowledged || !cancellation.IsCancellationRequested || failure is not OperationCanceledException cancelled
                || !barrier.OperationToken.IsCancellationRequested || cancelled.CancellationToken != barrier.OperationToken)
                throw new Phase7SqlGuardException("PHASE7_SQL_CANCELLATION_NOT_ACKNOWLEDGED");
        }
        else if (failure is OperationCanceledException or TimeoutException || failure is SqlException { Number: -2 } || !expectedFailure(failure))
            ExceptionDispatchInfo.Capture(failure).Throw();
        token.ThrowIfCancellationRequested();
        if (budget.IsCancellationRequested) throw new Phase7SqlGuardException("PHASE7_SQL_FAILURE_CASE_DEADLINE");
        return new(fault, failure, barrier.Acknowledged, before, after);
    }
}

internal sealed class Phase7SqlAfterProcedureException() : Exception("PHASE7_SQL_AFTER_PROCEDURE");
