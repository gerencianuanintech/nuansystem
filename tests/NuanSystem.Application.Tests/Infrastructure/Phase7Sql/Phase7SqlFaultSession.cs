using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlFaultSessionOptions(Guid RunId, string CaseCode, Phase7SqlFailureCase Case)
{
    public string CanonicalCaseCode => CaseCode.ToUpperInvariant(); // same normalization as the real CRUD handlers
    public void Validate()
    {
        var prefix = $"P7_{RunId:N}"[..11] + $"_S{(int)Case:00}";
        if (RunId == Guid.Empty || !Enum.IsDefined(Case) || !Regex.IsMatch(CaseCode ?? "", $"\\A{prefix}[A-Za-z0-9]{{0,4}}\\z", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            throw new Phase7SqlGuardException("PHASE7_SQL_FAULT_SESSION_OPTIONS");
    }
}

internal static class Phase7SqlFaultSession
{
    public static void RequireDedicatedConnection(string connectionString)
    {
        try
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            if (builder.Pooling || builder.MultipleActiveResultSets || builder.Enlist)
                throw new Phase7SqlGuardException("PHASE7_SQL_FAULT_SESSION_NOT_DEDICATED");
        }
        catch (ArgumentException) { throw new Phase7SqlGuardException("PHASE7_SQL_FAULT_SESSION_NOT_DEDICATED"); }
    }

    public static async Task<T> RunAsync<T>(IDbConnection connection, IDbTransaction transaction, Phase7SqlFaultSessionOptions options,
        int companyId, int timeout, Func<Task<T>> operation, CancellationToken token)
    {
        options.Validate();
        if (companyId <= 0 || timeout is < 1 or > 120 || connection is not SqlConnection sql || transaction is not SqlTransaction tx
            || !ReferenceEquals(tx.Connection, sql) || sql.State != ConnectionState.Open)
            throw new Phase7SqlGuardException("PHASE7_SQL_FAULT_SESSION_CONNECTION");
        RequireDedicatedConnection(sql.ConnectionString);
        return await RunWithCleanupAsync(async () =>
        {
            using var command = sql.CreateCommand();
            command.Transaction = tx;
            command.CommandTimeout = Math.Min(timeout, 5);
            command.CommandText = """
EXEC sys.sp_set_session_context @key=N'P7_RunId',@value=@RunId,@read_only=0;
EXEC sys.sp_set_session_context @key=N'P7_CompanyId',@value=@CompanyId,@read_only=0;
EXEC sys.sp_set_session_context @key=N'P7_CaseCode',@value=@CaseCode,@read_only=0;
EXEC sys.sp_set_session_context @key=N'P7_FaultPoint',@value=@Point,@read_only=0;
""";
            command.Parameters.Add("@RunId", SqlDbType.NVarChar, 36).Value = options.RunId.ToString("D");
            command.Parameters.Add("@CompanyId", SqlDbType.Int).Value = companyId;
            command.Parameters.Add("@CaseCode", SqlDbType.NVarChar, 50).Value = options.CanonicalCaseCode;
            command.Parameters.Add("@Point", SqlDbType.NVarChar, 40).Value = options.Case.ToString();
            await command.ExecuteNonQueryAsync(token);
            return await operation();
        }, async cleanupToken =>
        {
            using var command = sql.CreateCommand();
            command.Transaction = tx;
            command.CommandTimeout = 2;
            command.CommandText = """
EXEC sys.sp_set_session_context @key=N'P7_FaultPoint',@value=NULL,@read_only=0;
EXEC sys.sp_set_session_context @key=N'P7_CaseCode',@value=NULL,@read_only=0;
EXEC sys.sp_set_session_context @key=N'P7_CompanyId',@value=NULL,@read_only=0;
EXEC sys.sp_set_session_context @key=N'P7_RunId',@value=NULL,@read_only=0;
""";
            await command.ExecuteNonQueryAsync(cleanupToken);
        });
    }

    internal static async Task<T> RunWithCleanupAsync<T>(Func<Task<T>> operation, Func<CancellationToken, Task> cleanup)
    {
        Exception? primary = null;
        try { return await operation(); }
        catch (Exception error) { primary = error; throw; }
        finally
        {
            using var budget = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            try { await cleanup(budget.Token); }
            catch (Exception error) when (primary is not null) { primary.Data["Phase7Sql.SessionCleanupException"] = error; }
        }
    }

    public static async Task WaitForCancellationAsync(SqlConnection connection, SqlTransaction transaction,
        Phase7SqlFaultSessionOptions options, Phase7SqlCancellationBarrier barrier, int timeout, CancellationToken token)
    {
        options.Validate();
        if (options.Case != Phase7SqlFailureCase.CancelDuringCommand || timeout is < 6 or > 120
            || connection.State != ConnectionState.Open || !ReferenceEquals(transaction.Connection, connection))
            throw new Phase7SqlGuardException("PHASE7_SQL_WAIT_COMMAND_CONFIG");
        RequireDedicatedConnection(connection.ConnectionString);
        var acknowledgement = $"P7_WAIT_{Guid.NewGuid():N}";
        void OnMessage(object sender, SqlInfoMessageEventArgs args)
        {
            if (args.Errors.Cast<SqlError>().Any(e => e.Message == acknowledgement))
                barrier.Acknowledge(Phase7SqlFailureCase.CancelDuringCommand, token);
        }
        connection.InfoMessage += OnMessage;
        try
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandTimeout = timeout;
            command.CommandText = "RAISERROR(@Acknowledgement,10,1) WITH NOWAIT; WAITFOR DELAY @Delay;";
            command.Parameters.Add("@Acknowledgement", SqlDbType.NVarChar, 64).Value = acknowledgement;
            command.Parameters.Add("@Delay", SqlDbType.VarChar, 8).Value = "00:00:05";
            await command.ExecuteNonQueryAsync(token);
            throw new Phase7SqlGuardException("PHASE7_SQL_WAIT_NOT_CANCELLED");
        }
        finally { connection.InfoMessage -= OnMessage; }
    }
}
