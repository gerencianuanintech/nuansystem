using System.Data;
using System.Text.RegularExpressions;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Persistence.Transactions;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlSession(int Spid, string DatabaseName, int TransactionCount, int TransactionState, int DatabaseId = 0);
internal sealed record Phase7SqlSample(Guid ExecutionId, string Checkpoint, Phase7SqlSession Session);
internal delegate Task<Phase7SqlSession> Phase7SqlSessionReader(IDbConnection connection, IDbTransaction? transaction, int timeoutSeconds, CancellationToken token);

// A decorator of the production runner, never an alternative transaction implementation.
internal sealed class Phase7SqlSessionProbe : ITransactionRunner
{
    private readonly SqlTransactionRunner runner;
    private readonly string database;
    private readonly int databaseId;
    private readonly int timeout;
    private readonly Action<IDbConnection, IDbTransaction?, CancellationToken> guard;
    private readonly Phase7SqlSessionReader read;
    private readonly List<Phase7SqlSample> samples = [];
    private int active, capturing, connections;
    private IDbConnection? connection;
    private IDbTransaction? transaction;
    private Phase7SqlSession? entry;
    private Guid execution;
    private bool invalid;

    public Phase7SqlSessionProbe(ITenantConnectionFactory source, string database, int timeoutSeconds,
        Action<IDbConnection, IDbTransaction?, CancellationToken> guard, Phase7SqlSessionReader? read = null, int databaseId = 0)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(guard);
        if (string.IsNullOrWhiteSpace(database) || timeoutSeconds is < 1 or > 120) throw new Phase7SqlGuardException("PHASE7_SQL_PROBE_CONFIG");
        this.database = database;
        this.databaseId = databaseId;
        timeout = timeoutSeconds;
        this.guard = guard;
        this.read = read ?? ReadSessionAsync;
        ConnectionFactory = new TrackingFactory(this, source);
        runner = new SqlTransactionRunner(ConnectionFactory);
    }

    public ITenantConnectionFactory ConnectionFactory { get; }
    public int WriteConnectionCount => Volatile.Read(ref connections);
    public IReadOnlyList<Phase7SqlSample> Samples { get { lock (samples) return samples.ToArray(); } }

    public async Task ExecuteInTenantTransactionAsync(Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        await ExecuteInTenantTransactionAsync<object?>(async (c, t, token) => { await operation(c, t, token); return null; }, cancellationToken);
    }

    public async Task<T> ExecuteInTenantTransactionAsync<T>(Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        cancellationToken.ThrowIfCancellationRequested();
        if (Interlocked.CompareExchange(ref active, 1, 0) != 0) throw new Phase7SqlGuardException("PHASE7_SQL_PROBE_BUSY");
        connections = 0;
        entry = null;
        invalid = false;
        execution = Guid.NewGuid();
        try
        {
            return await runner.ExecuteInTenantTransactionAsync(async (c, t, token) =>
            {
                connection = c;
                transaction = t;
                await CheckpointAsync("Entry", token);
                var result = await operation(c, t, token);
                await CheckpointAsync("BeforeCommit", token);
                if (invalid || connections != 1) throw new Phase7SqlGuardException("PHASE7_SQL_EXECUTION_INVALID");
                return result;
            }, cancellationToken);
        }
        finally { connection = null; transaction = null; entry = null; Volatile.Write(ref active, 0); }
    }

    public async Task CheckpointAsync(string checkpoint, CancellationToken token = default)
    {
        if (!Regex.IsMatch(checkpoint ?? "", "\\A[A-Za-z][A-Za-z0-9_]{0,47}\\z", RegexOptions.CultureInvariant))
            throw new Phase7SqlGuardException("PHASE7_SQL_CHECKPOINT_INVALID");
        if (connection is null || transaction is null || Volatile.Read(ref active) != 1)
            throw new Phase7SqlGuardException("PHASE7_SQL_PROBE_INACTIVE");
        if (Interlocked.CompareExchange(ref capturing, 1, 0) != 0) throw new Phase7SqlGuardException("PHASE7_SQL_PROBE_BUSY");
        try
        {
            token.ThrowIfCancellationRequested();
            guard(connection, transaction, token);
            var session = await read(connection, transaction, timeout, token);
            // Persist failed measurements too, without handles, SQL text, payloads or credentials.
            lock (samples) samples.Add(new(execution, checkpoint!, session));
            if (session.Spid <= 0 || !string.Equals(session.DatabaseName, database, StringComparison.Ordinal)
                || (databaseId > 0 && session.DatabaseId != databaseId)
                || session.TransactionCount != 1 || session.TransactionState != 1
                || (entry is not null && session.Spid != entry.Spid))
                throw new Phase7SqlGuardException("PHASE7_SQL_SESSION_DRIFT");
            entry ??= session;
        }
        catch { invalid = true; throw; }
        finally { Volatile.Write(ref capturing, 0); }
    }

    internal static Task<Phase7SqlSession> ReadSessionAsync(IDbConnection connection, IDbTransaction? transaction, int timeout, CancellationToken token) =>
        connection.QuerySingleAsync<Phase7SqlSession>(new CommandDefinition(
            "SELECT CAST(@@SPID AS int) AS Spid, DB_NAME() AS DatabaseName, @@TRANCOUNT AS TransactionCount, XACT_STATE() AS TransactionState, DB_ID() AS DatabaseId;",
            transaction: transaction, commandTimeout: timeout, cancellationToken: token));

    private sealed class TrackingFactory(Phase7SqlSessionProbe owner, ITenantConnectionFactory source) : ITenantConnectionFactory
    {
        public IDbConnection CreateConnection()
        {
            if (Volatile.Read(ref owner.active) != 1) throw new Phase7SqlGuardException("PHASE7_SQL_PROBE_INACTIVE");
            if (Interlocked.Increment(ref owner.connections) != 1) throw new Phase7SqlGuardException("PHASE7_SQL_WRITE_CONNECTION_COUNT");
            // The fixture wraps source with its authorization gate, before creating any handle.
            return source.CreateConnection();
        }
    }
}
