using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Commands;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Persistence.Repositories;
using NuanSystem.Persistence.Repositories.Sync;
using NuanSystem.Persistence.Tenancy;
using NuanSystem.Persistence.Transactions;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

// Trusted adapter supplied by the future SQL execution harness. No default adapter, secrets,
// appsettings, connection discovery, initializer or provisioning path exists in this fixture.
// Must verify the complete manifest against the SAME open connection, read-only, before returning.
internal interface IPhase7SqlDestinationVerifier
{
    Task VerifyAsync(Phase7SqlManifest manifest, Phase7SqlTarget target, bool observer,
        IDbConnection connection, IDbTransaction? transaction, CancellationToken token);
}

internal sealed record Phase7SqlConnections(ITenantConnectionFactory Writer, ITenantConnectionFactory Observer);

internal sealed class Phase7SqlFixture : IDisposable
{
    private readonly Phase7SqlApproval approval;
    private readonly Func<Phase7SqlGates> gates;
    private readonly TimeProvider clock;
    private readonly CancellationTokenSource lifetime;
    private readonly Dictionary<string, Phase7SqlTenant> tenants = [];
    private bool disposed;
    private int externalBoundaryAttempts;
    public int ExternalBoundaryAttempts=>Volatile.Read(ref externalBoundaryAttempts);

    public Phase7SqlFixture(string json, Phase7SqlApproval approval, Phase7SqlConnections tenantA,
        Phase7SqlConnections tenantB, IPhase7SqlDestinationVerifier verifier,
        TimeProvider? clock = null, Func<Phase7SqlGates>? gates = null, Phase7SqlSessionReader? readSession = null)
    {
        ArgumentNullException.ThrowIfNull(verifier);
        this.approval = approval;
        this.clock = clock ?? TimeProvider.System;
        this.gates = gates ?? Phase7SqlGates.ReadEnvironment;
        var validation = Phase7SqlManifestValidator.Validate(json, approval, this.gates(), this.clock.GetUtcNow());
        Manifest = validation.Manifest;
        Ledger = new(Manifest.RunId);
        lifetime = new(TimeSpan.FromSeconds(Manifest.RunTimeoutSeconds), this.clock);
        tenants.Add("A", new(this, Manifest.TenantA, tenantA, verifier, readSession));
        tenants.Add("B", new(this, Manifest.TenantB, tenantB, verifier, readSession));
    }

    public Phase7SqlManifest Manifest { get; }
    public Phase7SqlLedger Ledger { get; }
    public Phase7SqlTenant Tenant(string role) => tenants.TryGetValue(role, out var tenant) ? tenant : throw new Phase7SqlGuardException("PHASE7_SQL_ROLE_INVALID");

    internal void EnsureAuthorized(CancellationToken token)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        token.ThrowIfCancellationRequested();
        lifetime.Token.ThrowIfCancellationRequested();
        // Do not reapply the original full-run budget at every checkpoint. The run's monotonic
        // deadline is enforced by lifetime; its wall-clock approval can still expire/revoke gates.
        var now = clock.GetUtcNow();
        var currentGates = gates();
        if (!currentGates.StandardEnabled || !currentGates.IsolatedEnabled) throw new Phase7SqlGuardException("PHASE7_SQL_GATES_DISABLED");
        if (now < Manifest.NotBeforeUtc || now >= Manifest.ExpiresAtUtc || now < approval.NotBeforeUtc || now >= approval.ExpiresAtUtc)
            throw new Phase7SqlGuardException("PHASE7_SQL_WINDOW_INVALID");
    }

    internal CancellationTokenSource Link(CancellationToken token)
    {
        EnsureAuthorized(token);
        return CancellationTokenSource.CreateLinkedTokenSource(token, lifetime.Token);
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        lifetime.Cancel();
        lifetime.Dispose();
    }

    internal sealed class Phase7SqlTenant
    {
        private readonly Phase7SqlFixture fixture;
        private readonly Phase7SqlTarget target;
        private readonly Phase7SqlConnections connections;
        private readonly IPhase7SqlDestinationVerifier verifier;
        private readonly Phase7SqlSessionReader read;
        private int observers, activity, totalWriteConnections;
        private readonly List<Phase7SqlScopeState> scopeStates = [];

        internal Phase7SqlTenant(Phase7SqlFixture fixture, Phase7SqlTarget target, Phase7SqlConnections connections,
            IPhase7SqlDestinationVerifier verifier, Phase7SqlSessionReader? readSession)
        {
            this.fixture = fixture;
            this.target = target;
            this.connections = connections;
            this.verifier = verifier;
            read = readSession ?? Phase7SqlSessionProbe.ReadSessionAsync;
            Probe = new(new AuthorizedFactory(fixture, target, false, connections.Writer, () => Interlocked.Increment(ref totalWriteConnections)), target.DatabaseName, fixture.Manifest.CommandTimeoutSeconds,
                (_, _, token) => fixture.EnsureAuthorized(token), read, target.DatabaseId);
        }

        private Phase7SqlSessionProbe Probe { get; }
        public Guid EvidenceScopeId {get;}=Guid.NewGuid();
        public int WriteConnectionCount => Probe.WriteConnectionCount;
        public int TotalWriteConnections => Volatile.Read(ref totalWriteConnections);
        public IReadOnlyList<Phase7SqlSample> Samples => Probe.Samples;
        public Task CheckpointAsync(string checkpoint, CancellationToken token = default) => Probe.CheckpointAsync(checkpoint, token);
        public int ObserverConnectionCount => Volatile.Read(ref observers);
        public IReadOnlyList<Phase7SqlScopeState> ScopeStates { get { lock (scopeStates) return scopeStates.ToArray(); } }
        private readonly List<Phase7SqlTenant> independentScopes = [];
        public IReadOnlyList<Phase7SqlTenant> IndependentScopes { get { lock(independentScopes)return independentScopes.ToArray(); } }

        public Phase7SqlTenant CreateIndependentScope()
        {
            fixture.EnsureAuthorized(default);
            var scope = new Phase7SqlTenant(fixture, target, connections, verifier, read);
            lock(independentScopes)independentScopes.Add(scope);
            return scope;
        }

        public Phase7SqlReliabilityScope CreateReliabilityScope(bool pooling = true, bool closeAfterEnqueue = false, Phase7SqlScopeStateReader? readState = null)
        {
            if (closeAfterEnqueue && pooling) throw new Phase7SqlGuardException("PHASE7_SQL_RECOVERY_REQUIRES_DEDICATED_SESSION");
            var context = new CompanyContext();
            var runner = new ReliabilityRunner(this, pooling, closeAfterEnqueue, readState ?? Phase7SqlScopeHygiene.ReadAsync);
            var unit = new Phase7NominalCheckpointUnitOfWork(ComposeUnit(false, true, true, runner, context), runner.CheckpointAsync);
            var original = context.CurrentCompany!;
            return new(unit, () => { runner.RequireActive(); context.SetCurrentCompany(original with { CompanyId = original.CompanyId == 1 ? 2 : 1 }); }, () => context.SetCurrentCompany(original));
        }

        // No generic service provider: only this pilot's real repository/writer/UoW are composed.
        public IBusinessPartnerUnitOfWork CreateUnitOfWork(bool isMaster = true, bool syncEnabled = true) => ComposeUnit(false, isMaster, syncEnabled);

        public IBusinessPartnerUnitOfWork CreateLegacyReference(bool isMaster = true, bool syncEnabled = true) => ComposeUnit(true, isMaster, syncEnabled);

        public int CompanyId => target.CompanyId;
        public Guid RunId => fixture.Manifest.RunId;
        public int CommandTimeoutSeconds => fixture.Manifest.CommandTimeoutSeconds;

        public IBusinessPartnerUnitOfWork CreateFailureUnitOfWork(Phase7SqlFaultSessionOptions options,
            Func<string, IDbConnection, IDbTransaction, CancellationToken, Task> hook)
        {
            ArgumentNullException.ThrowIfNull(hook);
            options.Validate();
            if (options.RunId != fixture.Manifest.RunId) throw new Phase7SqlGuardException("PHASE7_SQL_FAULT_SESSION_RUN_MISMATCH");
            var runner = new FaultRunner(this, options, hook);
            return new Phase7NominalCheckpointUnitOfWork(ComposeUnit(false, true, true, runner), runner.CheckpointAsync);
        }

        private IBusinessPartnerUnitOfWork ComposeUnit(bool legacy, bool isMaster, bool syncEnabled, ITransactionRunner? runner = null, CompanyContext? companyContext = null)
        {
            var context = companyContext ?? new CompanyContext();
            context.SetCurrentCompany(new(target.CompanyId, $"P7_{target.Role}", $"Phase7 {target.Role}",
                DatabaseEngine.SqlServer, "", SapIntegrationMode.None, IsMaster: isMaster, SyncEnabled: syncEnabled));
            var repository = new BusinessPartnerRepository(Probe.ConnectionFactory);
            var barrier = new ExternalBarrier(()=>Interlocked.Increment(ref fixture.externalBoundaryAttempts));
            var outbox = new LocalSyncOutboxRepository(barrier, barrier);
            var writer = new BusinessPartnerLocalOutboxWriter(context, new SyncEventPayloadFactory(), outbox);
            runner ??= new VerifiedRunner(this);
            return legacy ? new Phase7LegacyBusinessPartnerUnitOfWork(runner, repository, writer)
                : new BusinessPartnerUnitOfWork(runner, repository, writer, context);
        }

        // Test infrastructure only: delegates see ADO; Application continues using its existing contract.
        public async Task<T> ExecuteAsync<T>(Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation, CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(operation);
            if (Interlocked.CompareExchange(ref activity, 1, 0) != 0) throw new Phase7SqlGuardException("PHASE7_SQL_TENANT_BUSY");
            try
            {
                using var linked = fixture.Link(token);
                return await Probe.ExecuteInTenantTransactionAsync(async (c, t, ct) =>
                {
                    await verifier.VerifyAsync(fixture.Manifest, target, false, c, t, ct);
                    fixture.EnsureAuthorized(ct);
                    return await operation(c, t, ct);
                }, linked.Token);
            }
            finally { Volatile.Write(ref activity, 0); }
        }

        // Call after ExecuteAsync has completed, including its rollback. No NOLOCK or ambient test transaction.
        public async Task<T> ObserveAsync<T>(Func<IDbConnection, int, CancellationToken, Task<T>> snapshot, CancellationToken token = default)
        {
            ArgumentNullException.ThrowIfNull(snapshot);
            if (Interlocked.CompareExchange(ref activity, 1, 0) != 0) throw new Phase7SqlGuardException("PHASE7_SQL_TENANT_BUSY");
            try
            {
                using var linked = fixture.Link(token);
                linked.CancelAfter(TimeSpan.FromSeconds(fixture.Manifest.CommandTimeoutSeconds));
                using var connection = new AuthorizedFactory(fixture, target, true, connections.Observer).CreateConnection();
                Interlocked.Increment(ref observers);
                if (connection is DbConnection db) await db.OpenAsync(linked.Token);
                else { linked.Token.ThrowIfCancellationRequested(); connection.Open(); }
                await verifier.VerifyAsync(fixture.Manifest, target, true, connection, null, linked.Token);
                var session = await read(connection, null, fixture.Manifest.CommandTimeoutSeconds, linked.Token);
                if (session.DatabaseName != target.DatabaseName || session.DatabaseId != target.DatabaseId || session.Spid <= 0
                    || session.TransactionCount != 0 || session.TransactionState != 0)
                    throw new Phase7SqlGuardException("PHASE7_SQL_OBSERVER_SESSION");
                fixture.EnsureAuthorized(linked.Token);
                return await snapshot(connection, fixture.Manifest.CommandTimeoutSeconds, linked.Token);
            }
            finally { Volatile.Write(ref activity, 0); }
        }

        private sealed class VerifiedRunner(Phase7SqlTenant tenant) : ITransactionRunner
        {
            public Task<T> ExecuteInTenantTransactionAsync<T>(Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default) => tenant.ExecuteAsync(operation, cancellationToken);
            public async Task ExecuteInTenantTransactionAsync(Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation, CancellationToken cancellationToken = default) =>
                await tenant.ExecuteAsync<object?>(async (c, t, ct) => { await operation(c, t, ct); return null; }, cancellationToken);
        }

        private sealed class ReliabilityRunner(Phase7SqlTenant tenant, bool pooling, bool closeAfterEnqueue, Phase7SqlScopeStateReader readState) : ITransactionRunner
        {
            private IDbConnection? connection;
            public void RequireActive()
            {
                if (connection is null) throw new Phase7SqlGuardException("PHASE7_SQL_RECOVERY_SCOPE_INACTIVE");
            }
            public Task<T> ExecuteInTenantTransactionAsync<T>(Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default) =>
                tenant.ExecuteAsync(async (c, t, ct) =>
                {
                    if (c is SqlConnection sql && new SqlConnectionStringBuilder(sql.ConnectionString).Pooling != pooling)
                        throw new Phase7SqlGuardException("PHASE7_SQL_SCOPE_POOLING_MISMATCH");
                    var sample = await readState(c, t, tenant.CommandTimeoutSeconds, ct);
                    lock (tenant.scopeStates) tenant.scopeStates.Add(sample);
                    Phase7SqlScopeHygiene.AssertClean(sample, tenant.target.DatabaseName, tenant.target.DatabaseId);
                    connection = c;
                    try { return await operation(c, t, ct); }
                    finally { connection = null; }
                }, cancellationToken);
            public async Task ExecuteInTenantTransactionAsync(Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation, CancellationToken cancellationToken = default) =>
                await ExecuteInTenantTransactionAsync<object?>(async (c, t, ct) => { await operation(c, t, ct); return null; }, cancellationToken);
            public async Task CheckpointAsync(string label, CancellationToken token)
            {
                if (connection is null) throw new Phase7SqlGuardException("PHASE7_SQL_RECOVERY_SCOPE_INACTIVE");
                await tenant.CheckpointAsync(label, token);
                if (closeAfterEnqueue && label == "AfterEnqueue")
                {
                    connection.Close();
                    throw new Phase7SqlOwnerClosedException(); // no post-close command that could implicitly reopen this handle
                }
            }
        }

        private sealed class FaultRunner(Phase7SqlTenant tenant, Phase7SqlFaultSessionOptions options,
            Func<string, IDbConnection, IDbTransaction, CancellationToken, Task> hook) : ITransactionRunner
        {
            private IDbConnection? connection;
            private IDbTransaction? transaction;
            public Task<T> ExecuteInTenantTransactionAsync<T>(Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default) =>
                tenant.ExecuteAsync((c, t, ct) => Phase7SqlFaultSession.RunAsync(c, t, options, tenant.target.CompanyId,
                    tenant.fixture.Manifest.CommandTimeoutSeconds, async () =>
                    {
                        connection = c;
                        transaction = t;
                        try { return await operation(c, t, ct); }
                        finally { connection = null; transaction = null; }
                    }, ct), cancellationToken);

            public async Task ExecuteInTenantTransactionAsync(Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation, CancellationToken cancellationToken = default) =>
                await ExecuteInTenantTransactionAsync<object?>(async (c, t, ct) => { await operation(c, t, ct); return null; }, cancellationToken);

            public async Task CheckpointAsync(string label, CancellationToken token)
            {
                if (connection is null || transaction is null) throw new Phase7SqlGuardException("PHASE7_SQL_FAULT_SESSION_INACTIVE");
                await tenant.CheckpointAsync(label, token);
                await hook(label, connection, transaction, token);
            }
        }
    }

    private sealed class AuthorizedFactory(Phase7SqlFixture fixture, Phase7SqlTarget target, bool observer, ITenantConnectionFactory source, Action? beforeCreate = null) : ITenantConnectionFactory
    {
        public IDbConnection CreateConnection()
        {
            fixture.EnsureAuthorized(default);
            beforeCreate?.Invoke();
            var connection = source.CreateConnection();
            try
            {
                if (connection.State != ConnectionState.Closed) throw new Phase7SqlGuardException("PHASE7_SQL_CONNECTION_NOT_CLOSED");
                if (connection is SqlConnection sql) ValidateConnectionString(sql.ConnectionString, fixture.Manifest, target, observer);
                return connection;
            }
            catch { connection.Dispose(); throw; }
        }
    }

    internal static void ValidateConnectionString(string connectionString, Phase7SqlManifest manifest, Phase7SqlTarget target, bool observer)
    {
        try
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            if (!string.Equals(builder.DataSource, manifest.Server.DataSource, StringComparison.OrdinalIgnoreCase)
                || builder.InitialCatalog != target.DatabaseName || builder.IntegratedSecurity
                || builder.UserID != (observer ? target.ObserverPrincipal : target.ApplicationPrincipal)
                || string.IsNullOrEmpty(builder.Password) || builder.AttachDBFilename.Length != 0 || builder.UserInstance
                || builder.FailoverPartner.Length != 0 || builder.PersistSecurityInfo || builder.TrustServerCertificate
                || builder.Encrypt == SqlConnectionEncryptOption.Optional || builder.MultipleActiveResultSets
                || builder.Enlist || builder.ConnectTimeout < 1 || builder.ConnectTimeout > manifest.CommandTimeoutSeconds)
                throw new Phase7SqlGuardException("PHASE7_SQL_CONNECTION_REJECTED");
        }
        catch (ArgumentException) { throw new Phase7SqlGuardException("PHASE7_SQL_CONNECTION_REJECTED"); }
    }

    internal sealed class ExternalBarrier(Action? onRejected=null) : IMasterConnectionFactory, ICompanyResolver
    {
        private Phase7SqlGuardException Rejected() {onRejected?.Invoke();return new("PHASE7_SQL_EXTERNAL_ACCESS");}
        public IDbConnection CreateConnection() => throw Rejected();
        public Task<CompanyConnectionInfo?> ResolveByCodeAsync(string companyCode, CancellationToken cancellationToken = default) => throw Rejected();
        public Task<CompanyConnectionInfo?> ResolveByIdAsync(int companyId, CancellationToken cancellationToken = default) => throw Rejected();
        public Task<CompanyConnectionInfo?> ResolveByCodeForUserAsync(string companyCode, int userId, CancellationToken cancellationToken = default) => throw Rejected();
    }
}
