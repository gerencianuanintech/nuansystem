using System.Data;
using System.Text.Json;
using System.Text.Json.Nodes;
using NSubstitute;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Commands;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Persistence.Transactions;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

public enum Phase7FailureOperation { Create, Update, Delete }

// Staged in-memory persistence only. Its rollback models the expected SQL contract, not proof of it.
internal sealed class Phase7FailureOfflineHarness
{
    private readonly Phase7SqlFailureCase fault;
    private readonly Phase7FailureOperation operation;
    private readonly bool leakWrites;
    private readonly IBusinessPartnerUnitOfWork unit;
    private Dictionary<string, JsonArray> durable = Empty();
    private Dictionary<string, JsonArray> pending = Empty();
    private Phase7SqlCancellationBarrier? barrier;
    private bool inTransaction;
    private const int Id = 104;
    private readonly string code;
    public Exception ExpectedFailure { get; }
    public Exception? RollbackFailure { get; set; }
    public int Commits { get; private set; }
    public int Rollbacks { get; private set; }
    public int Observations { get; private set; }
    public int HandlerCalls { get; private set; }

    public Phase7FailureOfflineHarness(Phase7SqlFailureCase fault, Phase7FailureOperation operation = Phase7FailureOperation.Create, bool leakWrites = false)
    {
        this.fault = fault;
        this.operation = operation;
        this.leakWrites = leakWrites;
        code = $"P7_12345678_S{(int)fault:00}";
        ExpectedFailure = new SimulatedPersistenceFailure(fault);
        if (operation != Phase7FailureOperation.Create)
        {
            durable["BusinessPartners"].Add(new JsonObject
            {
                ["Id"] = Id, ["Code"] = code, ["Name"] = "Original", ["GlobalId"] = Guid.NewGuid(),
                ["IsActive"] = true, ["IsDeleted"] = false, ["IdentificationNumber"] = "P7_IDENT",
                ["IdentificationTypeId"] = 1, ["PartnerType"] = "Supplier", ["CreatedAt"] = "2026-10-24T00:00:00"
            });
            foreach (var table in Phase7SqlRollbackSnapshot.RequiredTables.Where(t => t.StartsWith("BusinessPartner", StringComparison.Ordinal) && t != "BusinessPartners"))
                durable[table].Add(new JsonObject { ["Id"] = 88, ["BusinessPartnerId"] = Id, ["SyntheticValue"] = "Original" });
        }
        var connection = Substitute.For<IDbConnection>();
        var transaction = Substitute.For<IDbTransaction>();
        connection.BeginTransaction().Returns(_ => { inTransaction = true; pending = Copy(durable); return transaction; });
        transaction.When(t => t.Commit()).Do(_ => { Commits++; durable = Copy(pending); inTransaction = false; });
        transaction.When(t => t.Rollback()).Do(_ =>
        {
            Rollbacks++;
            if (this.leakWrites) durable = Copy(pending);
            pending = Copy(durable);
            inTransaction = false;
            if (RollbackFailure is not null) throw RollbackFailure;
        });
        var factory = Substitute.For<ITenantConnectionFactory>();
        factory.CreateConnection().Returns(connection);
        var repository = Substitute.For<IBusinessPartnerRepository>();
        repository.GetByIdAsync(Id, connection, transaction, Arg.Any<CancellationToken>()).Returns(_ => pending["BusinessPartners"][0]!.Deserialize<BusinessPartnerDto>());
        repository.CreateAsync(Arg.Any<CreateBusinessPartnerData>(), connection, transaction, Arg.Any<CancellationToken>()).Returns(call =>
        {
            WriteBusiness(call.Arg<CreateBusinessPartnerData>());
            WriteAudit("INSERT");
            return Id;
        });
        repository.UpdateAsync(Arg.Any<UpdateBusinessPartnerData>(), connection, transaction, Arg.Any<CancellationToken>()).Returns(call =>
        {
            WriteBusiness(call.Arg<UpdateBusinessPartnerData>());
            WriteAudit("UPDATE");
            return true;
        });
        repository.DeleteAsync(Id, Arg.Any<int?>(), Arg.Any<string?>(), connection, transaction, Arg.Any<CancellationToken>()).Returns(_ =>
        {
            pending["BusinessPartners"][0]!["IsDeleted"] = true;
            pending["BusinessPartners"][0]!["IsActive"] = false;
            WriteAudit("DELETE");
            return true;
        });
        var outbox = Substitute.For<ILocalSyncOutboxRepository>();
        outbox.CreateAsync(Arg.Any<CreateLocalSyncOutboxData>(), connection, transaction, Arg.Any<CancellationToken>()).Returns(call =>
        {
            if (fault == Phase7SqlFailureCase.OutboxInsert) throw ExpectedFailure;
            var data = call.Arg<CreateLocalSyncOutboxData>();
            pending["LocalOutbox"].Add(JsonSerializer.SerializeToNode(data));
            if (fault == Phase7SqlFailureCase.SyncAuditInsert) throw ExpectedFailure;
            pending["SyncAudit"].Add(new JsonObject { ["EventId"] = data.EventId, ["Action"] = "Created", ["NewStatus"] = "Pending" });
            return 1L;
        });
        var company = new CompanyConnectionInfo(11, "P7", "Offline", DatabaseEngine.SqlServer, "", SapIntegrationMode.None, IsMaster: true, SyncEnabled: true);
        var context = Substitute.For<ICompanyContext>();
        context.HasActiveCompany.Returns(true);
        context.CurrentCompany.Returns(company);
        var writer = new BusinessPartnerLocalOutboxWriter(context, new SyncEventPayloadFactory(), outbox);
        unit = new Phase7NominalCheckpointUnitOfWork(new BusinessPartnerUnitOfWork(new SqlTransactionRunner(factory), repository, writer, context), async (label, token) =>
        {
            if (fault == Phase7SqlFailureCase.AfterProcedure && label is "AfterCreate" or "AfterUpdate" or "AfterDelete") throw ExpectedFailure;
            if (label == "AfterEnqueue" && fault is Phase7SqlFailureCase.CancelAfterWrites or Phase7SqlFailureCase.CancelDuringCommand)
                await barrier!.WaitForCancellationAsync(fault, token); // simulated command acknowledgement, not a SQL server signal
        });
    }

    public Task<Phase7SqlFailureEvidence> RunAsync(CancellationToken token = default) => Phase7SqlFailureScenarios.RunAsync(fault, async (signal, ct) =>
    {
        barrier = signal;
        HandlerCalls++;
        var create = Phase7NominalCommands.CreateCommand(1, "P7_IDENT", code) with { PartnerType = "Supplier" };
        if (operation == Phase7FailureOperation.Create)
        {
            var result = await new CreateBusinessPartnerCommandHandler(unit).Handle(create, ct);
            if (!result.IsSuccess) throw new InvalidOperationException("Offline create did not succeed.");
        }
        else if (operation == Phase7FailureOperation.Update)
        {
            var result = await new UpdateBusinessPartnerCommandHandler(unit).Handle(Phase7NominalCommands.UpdateCommand(Id, create) with { Name = "Edited" }, ct);
            if (!result.IsSuccess) throw new InvalidOperationException("Offline update did not succeed.");
        }
        else
        {
            var result = await new DeleteBusinessPartnerCommandHandler(unit).Handle(new(Id, 7, "admin"), ct);
            if (!result.IsSuccess) throw new InvalidOperationException("Offline delete did not succeed.");
        }
    }, ct =>
    {
        ct.ThrowIfCancellationRequested();
        if (inTransaction) throw new InvalidOperationException("Observer entered before transaction completion.");
        Observations++;
        return Task.FromResult(new Phase7SqlRollbackSnapshot(durable));
    }, error => ReferenceEquals(error, ExpectedFailure), TimeSpan.FromSeconds(5), token);

    private void WriteBusiness(object data)
    {
        var row = JsonSerializer.SerializeToNode(data)!;
        row["Id"] = Id;
        row["GlobalId"] = pending["BusinessPartners"].Count == 0 ? Guid.NewGuid() : pending["BusinessPartners"][0]!["GlobalId"]!.GetValue<Guid>();
        row["IsDeleted"] = false;
        pending["BusinessPartners"] = new JsonArray(row);
        pending["BusinessPartnerAddresses"] = new JsonArray(new JsonObject { ["BusinessPartnerId"] = Id, ["Line1"] = "Changed" });
    }

    private void WriteAudit(string action)
    {
        if (fault is Phase7SqlFailureCase.AuditInsert or Phase7SqlFailureCase.ProcedureError) throw ExpectedFailure;
        pending["AuditCatalogChanges"].Add(new JsonObject { ["RecordId"] = Id, ["Action"] = action });
    }

    private static Dictionary<string, JsonArray> Empty() => Phase7SqlRollbackSnapshot.RequiredTables.ToDictionary(name => name, _ => new JsonArray(), StringComparer.Ordinal);
    private static Dictionary<string, JsonArray> Copy(Dictionary<string, JsonArray> source) => source.ToDictionary(p => p.Key, p => (JsonArray)p.Value.DeepClone(), StringComparer.Ordinal);
    private sealed class SimulatedPersistenceFailure(Phase7SqlFailureCase fault) : Exception($"Offline persistence failure S{(int)fault:00}");
}
