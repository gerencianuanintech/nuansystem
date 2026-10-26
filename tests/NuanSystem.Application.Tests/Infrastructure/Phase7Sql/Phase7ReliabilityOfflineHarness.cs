using System.Data;
using System.Text.Json;
using System.Text.Json.Nodes;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.BusinessPartners.Commands;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Persistence.Tenancy;
using NuanSystem.Persistence.Transactions;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

// Simulates committed rows, unique reservations and a physical-session pool. Never contacts SQL.
internal sealed class Phase7ReliabilityOfflineHarness(int companyId)
{
    private readonly object gate = new();
    private readonly Dictionary<int, BusinessPartnerDto> rows = [];
    private readonly List<CreateLocalSyncOutboxData> events = [];
    private readonly List<Phase7NominalAudit> audits = [];
    private readonly List<Phase7NominalSyncAudit> syncAudits = [];
    private readonly HashSet<string> reservations = [];
    private readonly Queue<PhysicalSession> pool = [];
    private readonly List<Phase7SqlScopeState> sessions = [];
    private int nextId = 100, nextSpid = 50, connections;
    public int Connections => Volatile.Read(ref connections);
    public IReadOnlyList<Phase7SqlScopeState> Sessions { get { lock (gate) return sessions.ToArray(); } }
    public bool EnforceUnique { get; set; } = true;
    public bool DirtySessions { get; set; }
    public Exception? ConflictFailure { get; set; }
    public bool SimulateDeadlock { get; set; }
    public int RepositoryReads { get; private set; }
    public int OwnedClosures { get; private set; }
    private string Database => $"Offline_{companyId}";

    public CreateBusinessPartnerCommand Command(string name = "Proveedor sintetico") =>
        Phase7NominalCommands.CreateCommand(1, "P7_12345678_IDENT", "P7_12345678_S12") with { Name = name, PartnerType = "Supplier" };
    public bool IsExpectedConflict(Exception error) => error is SimulatedUniqueConflict or SimulatedDeadlock;

    public Phase7SqlReliabilityScope Scope(bool closeAfterEnqueue = false)
    {
        State? state = null;
        var factory = Substitute.For<ITenantConnectionFactory>();
        factory.CreateConnection().Returns(_ =>
        {
            Interlocked.Increment(ref connections);
            var current = new State();
            state = current;
            var connection = Substitute.For<IDbConnection>();
            var tx = Substitute.For<IDbTransaction>();
            current.Connection = connection;
            connection.State.Returns(_ => current.Open ? ConnectionState.Open : ConnectionState.Closed);
            tx.Connection.Returns(connection);
            connection.When(c => c.Open()).Do(_ =>
            {
                lock (gate)
                {
                    current.Physical = !closeAfterEnqueue && pool.Count > 0 ? pool.Dequeue() : new PhysicalSession(++nextSpid);
                    current.Open = true;
                    if (DirtySessions) current.Physical.Dirty = true;
                }
            });
            connection.BeginTransaction().Returns(_ => { current.Active = true; return tx; });
            tx.When(t => t.Commit()).Do(_ =>
            {
                lock (gate)
                {
                    if (!current.Open) throw new InvalidOperationException("Offline owner connection closed.");
                    if (current.Partner is not null) rows[current.Partner.Id] = Clone(current.Partner);
                    events.AddRange(current.Events);
                    audits.AddRange(current.Audits);
                    syncAudits.AddRange(current.SyncAudits);
                    Release(current);
                }
            });
            tx.When(t => t.Rollback()).Do(_ =>
            {
                lock (gate)
                {
                    Release(current);
                    if (!current.Open) throw new InvalidOperationException("Offline transaction was rolled back by owner close.");
                }
            });
            void Close()
            {
                lock (gate)
                {
                    Release(current);
                    current.Open = false;
                    if (current.Physical is not null) { if (!closeAfterEnqueue) pool.Enqueue(current.Physical); current.Physical = null; }
                }
            }
            connection.When(c => c.Close()).Do(_ => Close());
            connection.When(c => c.Dispose()).Do(_ => Close());
            return connection;
        });
        var repository = Substitute.For<IBusinessPartnerRepository>();
        repository.ExistsByCodeAsync(Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<IDbConnection>(), Arg.Any<IDbTransaction>(), Arg.Any<CancellationToken>()).Returns(call =>
        {
            lock (gate) return rows.Values.Any(p => p.Code == call.ArgAt<string>(0) && p.Id != call.ArgAt<int?>(1));
        });
        repository.ExistsByIdentificationAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<int?>(), Arg.Any<IDbConnection>(), Arg.Any<IDbTransaction>(), Arg.Any<CancellationToken>()).Returns(call =>
        {
            lock (gate) return rows.Values.Any(p => p.IdentificationTypeId == call.ArgAt<int>(0) && p.IdentificationNumber == call.ArgAt<string>(1) && p.Id != call.ArgAt<int?>(2));
        });
        repository.CreateAsync(Arg.Any<CreateBusinessPartnerData>(), Arg.Any<IDbConnection>(), Arg.Any<IDbTransaction>(), Arg.Any<CancellationToken>()).Returns(call =>
        {
            lock (gate)
            {
                var data = call.Arg<CreateBusinessPartnerData>();
                var keys = new[] { $"C:{data.Code}", $"I:{data.IdentificationTypeId}:{data.IdentificationNumber}" };
                if (EnforceUnique && (keys.Any(reservations.Contains) || rows.Values.Any(p => p.Code == data.Code || (p.IdentificationTypeId == data.IdentificationTypeId && p.IdentificationNumber == data.IdentificationNumber))))
                    throw ConflictFailure ?? (SimulateDeadlock ? new SimulatedDeadlock() : new SimulatedUniqueConflict());
                if (EnforceUnique) foreach (var key in keys) { reservations.Add(key); state!.Reservations.Add(key); }
                state!.Partner = Project(data, ++nextId, Guid.NewGuid());
                state.Audits.Add(new("BusinessPartners", state.Partner.Id, "INSERT", "Code", null, data.Code, data.CreatedByUserId, data.CreatedByUserName));
                return state.Partner.Id;
            }
        });
        repository.GetByIdAsync(Arg.Any<int>(), Arg.Any<IDbConnection>(), Arg.Any<IDbTransaction>(), Arg.Any<CancellationToken>()).Returns(call =>
        {
            lock (gate)
            {
                RepositoryReads++;
                var id = call.ArgAt<int>(0);
                var partner = state!.Partner?.Id == id ? state.Partner : rows.GetValueOrDefault(id);
                return partner is null ? null : Clone(partner);
            }
        });
        repository.UpdateAsync(Arg.Any<UpdateBusinessPartnerData>(), Arg.Any<IDbConnection>(), Arg.Any<IDbTransaction>(), Arg.Any<CancellationToken>()).Returns(call =>
        {
            lock (gate)
            {
                var data = call.Arg<UpdateBusinessPartnerData>();
                var old = rows[data.Id];
                state!.Partner = Project(data, data.Id, old.GlobalId);
                state.Partner.CreatedAt = old.CreatedAt;
                state.Partner.CreatedByUserId = old.CreatedByUserId;
                state.Partner.CreatedByUserName = old.CreatedByUserName;
                state.Partner.UpdatedAt = (old.UpdatedAt ?? old.CreatedAt).AddSeconds(1);
                if (old.Name != data.Name) state.Audits.Add(new("BusinessPartners", data.Id, "UPDATE", "Name", old.Name, data.Name, data.UpdatedByUserId, data.UpdatedByUserName));
                return true;
            }
        });
        var context = new CompanyContext();
        var original = new NuanSystem.Application.Abstractions.Tenancy.CompanyConnectionInfo(companyId, $"P7_{companyId}", "Offline", DatabaseEngine.SqlServer,
            "", SapIntegrationMode.None, IsMaster: true, SyncEnabled: true);
        context.SetCurrentCompany(original);
        var outbox = Substitute.For<ILocalSyncOutboxRepository>();
        outbox.CreateAsync(Arg.Any<CreateLocalSyncOutboxData>(), Arg.Any<IDbConnection>(), Arg.Any<IDbTransaction>(), Arg.Any<CancellationToken>()).Returns(call =>
        {
            var item = call.Arg<CreateLocalSyncOutboxData>();
            state!.Events.Add(item);
            state.SyncAudits.Add(new(item.EventId, "Created", "Pending"));
            return 1L;
        });
        var probe = new Phase7SqlSessionProbe(factory, Database, 30, (_, _, ct) => ct.ThrowIfCancellationRequested(),
            (_, _, _, ct) =>
            {
                ct.ThrowIfCancellationRequested();
                return Task.FromResult(new Phase7SqlSession(state!.Physical!.Spid, Database, state.Active ? 1 : 0, state.Active ? 1 : 0, companyId + 100));
            }, companyId + 100);
        var runner = new HygienicRunner(probe, () =>
        {
            var sample = new Phase7SqlScopeState(state!.Physical!.Spid, Database, companyId + 100, state.Active ? 1 : 0, state.Active ? 1 : 0, state.Physical.Dirty);
            lock (gate) sessions.Add(sample);
            Phase7SqlScopeHygiene.AssertClean(sample, Database, companyId + 100);
        });
        var unit = new BusinessPartnerUnitOfWork(runner, repository, new BusinessPartnerLocalOutboxWriter(context, new SyncEventPayloadFactory(), outbox), context);
        var measured = new Phase7NominalCheckpointUnitOfWork(unit, async (label, ct) =>
        {
            await probe.CheckpointAsync(label, ct);
            if (closeAfterEnqueue && label == "AfterEnqueue")
            {
                if (state!.Partner is null || state.Events.Count != 1 || state.SyncAudits.Count != 1) throw new Phase7SqlGuardException("PHASE7_SQL_OWNER_CLOSE_BEFORE_WRITES");
                OwnedClosures++;
                state.Connection!.Close();
                throw new Phase7SqlOwnerClosedException();
            }
        });
        return new(measured, () =>
        {
            if (state is not { Active: true, Open: true }) throw new Phase7SqlGuardException("PHASE7_SQL_RECOVERY_SCOPE_INACTIVE");
            context.SetCurrentCompany(original with { CompanyId = companyId == 1 ? 2 : 1 });
        }, () => context.SetCurrentCompany(original));
    }

    public Task<Phase7SqlReliabilitySnapshot> ObserveAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        lock (gate) return Task.FromResult(new Phase7SqlReliabilitySnapshot(rows.Values.OrderBy(p => p.Id).Select(Clone).ToArray(), events.ToArray(), syncAudits.ToArray(), audits.ToArray()));
    }

    public async Task<Phase7SqlRollbackSnapshot> RollbackSnapshotAsync(CancellationToken token)
    {
        var observed = await ObserveAsync(token);
        var tables = Phase7SqlRollbackSnapshot.RequiredTables.ToDictionary(t => t, _ => new JsonArray());
        tables["BusinessPartners"] = JsonSerializer.SerializeToNode(observed.Partners)!.AsArray();
        tables["LocalOutbox"] = JsonSerializer.SerializeToNode(observed.Events)!.AsArray();
        tables["SyncAudit"] = JsonSerializer.SerializeToNode(observed.SyncAudits)!.AsArray();
        tables["AuditCatalogChanges"] = JsonSerializer.SerializeToNode(observed.Audits)!.AsArray();
        return new(tables);
    }

    private void Release(State state)
    {
        foreach (var key in state.Reservations) reservations.Remove(key);
        state.Reservations.Clear();
        state.Active = false;
    }
    private static BusinessPartnerDto Project(object data, int id, Guid globalId)
    {
        var node = JsonSerializer.SerializeToNode(data)!;
        node["Id"] = id;
        node["GlobalId"] = globalId;
        node["CreatedAt"] = new DateTime(2026, 10, 24);
        foreach (var key in new[] { "Addresses", "Contacts", "BankAccounts", "RetentionSettings", "SapFieldMappings", "Attachments" })
        {
            node[key] ??= new JsonArray();
            var children = node[key]!.AsArray();
            for (var i = 0; i < children.Count; i++) { children[i]!["Id"] = id * 100 + i + 1; children[i]!["BusinessPartnerId"] = id; }
        }
        if (node["Notes"] is JsonObject notes) notes["BusinessPartnerId"] = id;
        return node.Deserialize<BusinessPartnerDto>()!;
    }
    private static BusinessPartnerDto Clone(BusinessPartnerDto partner) => JsonSerializer.Deserialize<BusinessPartnerDto>(JsonSerializer.Serialize(partner))!;
    private sealed class SimulatedUniqueConflict() : Exception("Offline unique constraint conflict.");
    private sealed class SimulatedDeadlock() : Exception("Offline deadlock victim.");
    private sealed class PhysicalSession(int spid) { public int Spid { get; } = spid; public bool Dirty; }
    private sealed class State
    {
        public IDbConnection? Connection;
        public PhysicalSession? Physical;
        public bool Open, Active;
        public BusinessPartnerDto? Partner;
        public List<string> Reservations { get; } = [];
        public List<CreateLocalSyncOutboxData> Events { get; } = [];
        public List<Phase7NominalAudit> Audits { get; } = [];
        public List<Phase7NominalSyncAudit> SyncAudits { get; } = [];
    }
    private sealed class HygienicRunner(ITransactionRunner runner, Action validate) : ITransactionRunner
    {
        public Task<T> ExecuteInTenantTransactionAsync<T>(Func<IDbConnection, IDbTransaction, CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default) =>
            runner.ExecuteInTenantTransactionAsync((c, t, ct) => { validate(); return operation(c, t, ct); }, cancellationToken);
        public async Task ExecuteInTenantTransactionAsync(Func<IDbConnection, IDbTransaction, CancellationToken, Task> operation, CancellationToken cancellationToken = default) =>
            await ExecuteInTenantTransactionAsync<object?>(async (c, t, ct) => { await operation(c, t, ct); return null; }, cancellationToken);
    }
}
