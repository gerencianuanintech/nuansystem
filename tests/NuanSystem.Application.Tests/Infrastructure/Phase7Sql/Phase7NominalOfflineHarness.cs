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

// In-memory persistence double, never evidence that a SQL procedure committed/audited correctly.
internal sealed class Phase7NominalOfflineHarness
{
    private readonly IBusinessPartnerUnitOfWork unit;
    private readonly IDbTransaction transaction = Substitute.For<IDbTransaction>();
    private readonly List<Phase7NominalAudit> audits = [];
    private readonly List<CreateLocalSyncOutboxData> events = [];
    private readonly List<Phase7NominalSyncAudit> syncAudits = [];
    private readonly int id;
    private readonly int companyId;
    private readonly bool expectsSync;
    private BusinessPartnerDto? partner;
    private bool deleted;
    public int Commits { get; private set; }
    public List<(string Label, int CompletedCommits)> Checkpoints { get; } = [];

    public Phase7NominalOfflineHarness(bool legacy, bool isMaster = true, bool syncEnabled = true, bool retainEmailOnUpdate = false)
    {
        id = legacy ? 104 : 211;
        companyId = legacy ? 11 : 12;
        expectsSync = isMaster && syncEnabled;
        var connection = Substitute.For<IDbConnection>();
        connection.BeginTransaction().Returns(transaction);
        transaction.When(t => t.Commit()).Do(_ => Commits++);
        var factory = Substitute.For<ITenantConnectionFactory>();
        factory.CreateConnection().Returns(connection);
        var repository = Substitute.For<IBusinessPartnerRepository>();
        repository.GetByIdAsync(id, connection, transaction, Arg.Any<CancellationToken>()).Returns(_ => Clone(partner!));
        repository.CreateAsync(Arg.Any<CreateBusinessPartnerData>(), connection, transaction, Arg.Any<CancellationToken>()).Returns(call =>
        {
            var data = call.Arg<CreateBusinessPartnerData>();
            partner = Project(data, Guid.NewGuid());
            Audit("INSERT", "Code", null, data.Code, data.CreatedByUserId, data.CreatedByUserName);
            return id;
        });
        repository.UpdateAsync(Arg.Any<UpdateBusinessPartnerData>(), connection, transaction, Arg.Any<CancellationToken>()).Returns(call =>
        {
            var data = call.Arg<UpdateBusinessPartnerData>();
            var oldName = partner!.Name;
            var oldEmail = partner.Email;
            partner = Project(data, partner.GlobalId);
            if (retainEmailOnUpdate) partner.Email = oldEmail;
            Audit("UPDATE", "Name", oldName, data.Name, data.UpdatedByUserId, data.UpdatedByUserName);
            return true;
        });
        repository.DeleteAsync(id, Arg.Any<int?>(), Arg.Any<string?>(), connection, transaction, Arg.Any<CancellationToken>()).Returns(call =>
        {
            deleted = true;
            partner!.IsActive = false;
            Audit("DELETE", "IsDeleted", "0", "1", call.ArgAt<int?>(1), call.ArgAt<string?>(2));
            return true;
        });
        var context = Substitute.For<ICompanyContext>();
        context.HasActiveCompany.Returns(true);
        context.CurrentCompany.Returns(new CompanyConnectionInfo(companyId, $"P7_{companyId}", "Offline", DatabaseEngine.SqlServer,
            "", SapIntegrationMode.None, IsMaster: isMaster, SyncEnabled: syncEnabled));
        var outbox = Substitute.For<ILocalSyncOutboxRepository>();
        outbox.CreateAsync(Arg.Any<CreateLocalSyncOutboxData>(), connection, transaction, Arg.Any<CancellationToken>()).Returns(call =>
        {
            var item = call.Arg<CreateLocalSyncOutboxData>();
            events.Add(item);
            syncAudits.Add(new(item.EventId, "Created", "Pending"));
            return (long)events.Count;
        });
        var writer = new BusinessPartnerLocalOutboxWriter(context, new SyncEventPayloadFactory(), outbox);
        var runner = new SqlTransactionRunner(factory);
        IBusinessPartnerUnitOfWork composed = legacy ? new Phase7LegacyBusinessPartnerUnitOfWork(runner, repository, writer)
            : new BusinessPartnerUnitOfWork(runner, repository, writer, context);
        unit = new Phase7NominalCheckpointUnitOfWork(composed, (label, token) =>
        {
            token.ThrowIfCancellationRequested();
            Checkpoints.Add((label, Commits));
            return Task.CompletedTask;
        });
    }

    public Task<IReadOnlyList<Phase7NominalSnapshot>> RunAsync(CancellationToken token = default)
    {
        // Id=1 is a synthetic lookup of this memory double, never a SQL seed assumption.
        var command = Phase7NominalCommands.CreateCommand(1, "P7_12345678_IDENT", "P7_12345678_S01", "SAP-EXCLUDED") with
        {
            Code = "P7_12345678_S01", Name = "Proveedor de laboratorio", PartnerType = "Supplier",
            Addresses = [new(null, null, null, "Main", "Direccion sintetica", null, "EC", null, null, null, null, null, true, true)],
            Contacts = [new(null, null, "Contacto sintetico", null, null, null, null, null, "contact@example.test", null, false, true, true, null)],
            Notes = new("Nota interna", "Compras", "Pagos", null)
        };
        return Phase7SqlNominalScenarios.RunAsync(unit, command, key => Phase7NominalCommands.UpdateCommand(key, command) with
        {
            Code = command.Code, PartnerType = command.PartnerType, Name = "Proveedor editado",
            Email = null, Remarks = null,
            Addresses = command.Addresses!.Select(a => a with { Line1 = "Direccion editada" }).ToArray(), Contacts = command.Contacts, Notes = command.Notes
        }, (_, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            if (Commits != audits.Count) throw new InvalidOperationException("Snapshot requested before handler commit.");
            return Task.FromResult(new Phase7NominalSnapshot(Clone(partner!), deleted, audits.ToArray(), events.ToArray(), syncAudits.ToArray()));
        }, companyId, expectsSync, token);
    }

    private void Audit(string action, string field, string? before, string? after, int? userId, string? userName) =>
        audits.Add(new("BusinessPartners", id, action, field, before, after, userId, userName));

    private BusinessPartnerDto Project(object data, Guid globalId)
    {
        var node = JsonSerializer.SerializeToNode(data)!;
        node["Id"] = id;
        node["GlobalId"] = globalId;
        foreach (var key in new[] { "Addresses", "Contacts", "BankAccounts", "RetentionSettings", "SapFieldMappings", "Attachments" })
        {
            node[key] ??= new JsonArray();
            if (node[key] is JsonArray rows)
                for (var index = 0; index < rows.Count; index++) { rows[index]!["Id"] = id * 10 + index; rows[index]!["BusinessPartnerId"] = id; }
        }
        if (node["Notes"] is JsonObject notes) notes["BusinessPartnerId"] = id;
        return node.Deserialize<BusinessPartnerDto>()!;
    }

    private static BusinessPartnerDto Clone(BusinessPartnerDto value) => JsonSerializer.Deserialize<BusinessPartnerDto>(JsonSerializer.Serialize(value))!;
}
