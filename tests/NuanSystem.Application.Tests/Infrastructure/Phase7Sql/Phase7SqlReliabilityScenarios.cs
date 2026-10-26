using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Commands;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7SqlReliabilitySnapshot(IReadOnlyList<BusinessPartnerDto> Partners,
    IReadOnlyList<CreateLocalSyncOutboxData> Events, IReadOnlyList<Phase7NominalSyncAudit> SyncAudits, IReadOnlyList<Phase7NominalAudit> Audits);
internal sealed record Phase7SqlConcurrentAttempt(Result<BusinessPartnerDto>? Result, Exception? Error);
internal sealed record Phase7SqlRepeatedUpdateEvidence(Phase7SqlReliabilitySnapshot BeforeRepeat, Phase7SqlReliabilitySnapshot AfterRepeat)
{
    public int AuditDelta => AfterRepeat.Audits.Count - BeforeRepeat.Audits.Count;
    public int EventDelta => AfterRepeat.Events.Count - BeforeRepeat.Events.Count;
    public int SyncAuditDelta => AfterRepeat.SyncAudits.Count - BeforeRepeat.SyncAudits.Count;
}

internal static class Phase7SqlReliabilityScenarios
{
    public static Func<CancellationToken, Task<Phase7SqlReliabilitySnapshot>> Observer(Phase7SqlFixture.Phase7SqlTenant tenant,
        Func<System.Data.IDbConnection, int, CancellationToken, Task<Phase7SqlReliabilitySnapshot>> reader) =>
        token => tenant.ObserveAsync((c, timeout, ct) => reader(c, timeout, ct), token);

    public static Task TenantIsolationOnFixtureAsync(Phase7SqlFixture.Phase7SqlTenant a, Phase7SqlFixture.Phase7SqlTenant b,
        CreateBusinessPartnerCommand commandA, CreateBusinessPartnerCommand commandB,
        Func<System.Data.IDbConnection, int, CancellationToken, Task<Phase7SqlReliabilitySnapshot>> reader,
        CancellationToken token = default) => TenantIsolationAsync(a.CreateReliabilityScope().Unit, b.CreateReliabilityScope().Unit,
            commandA, commandB, Observer(a, reader), Observer(b, reader), a.CompanyId, b.CompanyId, token);

    public static Task RepeatedCreateOnFixtureAsync(Phase7SqlFixture.Phase7SqlTenant tenant, CreateBusinessPartnerCommand command,
        Func<System.Data.IDbConnection, int, CancellationToken, Task<Phase7SqlReliabilitySnapshot>> reader, CancellationToken token = default) =>
        RepeatedCreateAsync(tenant.CreateReliabilityScope().Unit, command, Observer(tenant, reader), tenant.CompanyId, token);

    public static Task<Phase7SqlRepeatedUpdateEvidence> RepeatedUpdateOnFixtureAsync(Phase7SqlFixture.Phase7SqlTenant tenant, CreateBusinessPartnerCommand command,
        Func<System.Data.IDbConnection, int, CancellationToken, Task<Phase7SqlReliabilitySnapshot>> reader, CancellationToken token = default) =>
        RepeatedUpdateAsync(tenant.CreateReliabilityScope().Unit, command, Observer(tenant, reader), tenant.CompanyId, token);

    public static Task<IReadOnlyList<Phase7SqlConcurrentAttempt>> ConcurrentCreateOnFixtureAsync(Phase7SqlFixture.Phase7SqlTenant tenant,
        CreateBusinessPartnerCommand command, Func<System.Data.IDbConnection, int, CancellationToken, Task<Phase7SqlReliabilitySnapshot>> reader,
        TimeSpan timeout, CancellationToken token = default) => ConcurrentCreateAsync(tenant.CreateIndependentScope().CreateReliabilityScope().Unit,
            tenant.CreateIndependentScope().CreateReliabilityScope().Unit, command, Observer(tenant, reader), tenant.CompanyId, IsExpectedSqlConflict, timeout, token);

    public static async Task SessionReuseOnFixtureAsync(Phase7SqlFixture.Phase7SqlTenant tenant, CreateBusinessPartnerCommand command,
        Func<System.Data.IDbConnection, int, CancellationToken, Task<Phase7SqlReliabilitySnapshot>> reader, CancellationToken token = default)
    {
        var first = tenant.CreateIndependentScope();
        var next = tenant.CreateIndependentScope();
        var observe = Observer(tenant, reader);
        Empty(await observe(token));
        var created = await Create(first.CreateReliabilityScope().Unit, command, token);
        var before = await observe(token);
        AssertState(before, created, tenant.CompanyId, [SyncOperation.Created]);
        var frozen = JsonSerializer.SerializeToNode(before);
        var read = await next.CreateReliabilityScope().Unit.ExecuteAsync((context, ct) => context.GetByIdAsync(created.Id, ct), token);
        Require(read?.GlobalId == created.GlobalId && read.Name == command.Name.Trim(), "REUSED_SCOPE_READ_MIXED");
        Require(JsonNode.DeepEquals(frozen, JsonSerializer.SerializeToNode(await observe(token))), "REUSED_SCOPE_CHANGED_STATE");
        var states = first.ScopeStates.Concat(next.ScopeStates).ToArray();
        Require(states.Length == 2 && states[0].DatabaseId == states[1].DatabaseId, "REUSED_SCOPE_STATE_INCOMPLETE");
        Phase7SqlScopeHygiene.AssertReuse(states, states[0].DatabaseName, states[0].DatabaseId);
    }

    public static async Task TenantIsolationAsync(IBusinessPartnerUnitOfWork a, IBusinessPartnerUnitOfWork b,
        CreateBusinessPartnerCommand commandA, CreateBusinessPartnerCommand commandB,
        Func<CancellationToken, Task<Phase7SqlReliabilitySnapshot>> observeA, Func<CancellationToken, Task<Phase7SqlReliabilitySnapshot>> observeB,
        int companyA, int companyB, CancellationToken token = default)
    {
        Require(companyA > 0 && companyB > 0 && companyA != companyB && commandA.Code.Trim().ToUpperInvariant() == commandB.Code.Trim().ToUpperInvariant()
            && commandA.IdentificationNumber.Trim() == commandB.IdentificationNumber.Trim() && commandA.Name.Trim() != commandB.Name.Trim(), "TENANT_FIXTURE_INVALID");
        Empty(await observeA(token));
        Empty(await observeB(token));
        var createdA = await Create(a, commandA, token);
        var createdB = await Create(b, commandB, token);
        var stateA = await observeA(token);
        var stateB = await observeB(token);
        AssertState(stateA, createdA, companyA, [SyncOperation.Created]);
        AssertState(stateB, createdB, companyB, [SyncOperation.Created]);
        Require(stateA.Partners[0].Name == commandA.Name.Trim() && stateB.Partners[0].Name == commandB.Name.Trim()
            && createdA.GlobalId != createdB.GlobalId && !stateA.Events.Select(e => e.EventId).Intersect(stateB.Events.Select(e => e.EventId)).Any(), "TENANT_DATA_MIXED");
        var readA = await a.ExecuteAsync((context, ct) => context.GetByIdAsync(createdA.Id, ct), token);
        var readB = await b.ExecuteAsync((context, ct) => context.GetByIdAsync(createdB.Id, ct), token);
        Require(readA?.GlobalId == createdA.GlobalId && readA.Name == commandA.Name.Trim()
            && readB?.GlobalId == createdB.GlobalId && readB.Name == commandB.Name.Trim(), "TENANT_READ_MIXED");
    }

    public static async Task RepeatedCreateAsync(IBusinessPartnerUnitOfWork unit, CreateBusinessPartnerCommand command,
        Func<CancellationToken, Task<Phase7SqlReliabilitySnapshot>> observe, int companyId, CancellationToken token = default)
    {
        Empty(await observe(token));
        var first = await Create(unit, command, token);
        var before = await observe(token);
        AssertState(before, first, companyId, [SyncOperation.Created]);
        var frozen = JsonSerializer.SerializeToNode(before);
        var second = await new CreateBusinessPartnerCommandHandler(unit).Handle(command, token);
        Require(!second.IsSuccess && second.Errors.Any(e => e.Code == "BusinessPartnerCodeAlreadyExists"), "REPEATED_CREATE_NOT_REJECTED");
        var after = await observe(token);
        Require(JsonNode.DeepEquals(frozen, JsonSerializer.SerializeToNode(after)), "REPEATED_CREATE_CHANGED_STATE");
        var read = await unit.ExecuteAsync((context, ct) => context.GetByIdAsync(first.Id, ct), token);
        Require(read?.GlobalId == first.GlobalId, "UNIT_NOT_REUSABLE");
    }

    public static async Task<Phase7SqlRepeatedUpdateEvidence> RepeatedUpdateAsync(IBusinessPartnerUnitOfWork unit, CreateBusinessPartnerCommand command,
        Func<CancellationToken, Task<Phase7SqlReliabilitySnapshot>> observe, int companyId, CancellationToken token = default)
    {
        Empty(await observe(token));
        var first = await Create(unit, command, token);
        var edit = Phase7NominalCommands.UpdateCommand(first.Id, command) with { Name = "Proveedor editado" };
        Require((await new UpdateBusinessPartnerCommandValidator().ValidateAsync(edit, token)).IsValid, "COMMAND_INVALID");
        var handler = new UpdateBusinessPartnerCommandHandler(unit);
        var once = await handler.Handle(edit, token);
        Require(once.IsSuccess && once.Value?.GlobalId == first.GlobalId, "FIRST_UPDATE_FAILED");
        var before = await observe(token);
        AssertState(before, first, companyId, [SyncOperation.Created, SyncOperation.Updated]);
        var commercial = RepeatedCommercial(before.Partners[0]);
        var twice = await handler.Handle(edit, token);
        Require(twice.IsSuccess && twice.Value?.GlobalId == first.GlobalId, "REPEATED_UPDATE_FAILED");
        var after = await observe(token);
        AssertState(after, first, companyId, [SyncOperation.Created, SyncOperation.Updated, SyncOperation.Updated]);
        var repeated = RepeatedCommercial(after.Partners[0]);
        Require(after.Partners[0].Name == edit.Name && JsonNode.DeepEquals(commercial, repeated), "REPEATED_UPDATE_CHANGED_IDENTITY_OR_BUSINESS");
        Require(after.Events.Take(before.Events.Count).SequenceEqual(before.Events) && after.SyncAudits.Take(before.SyncAudits.Count).SequenceEqual(before.SyncAudits)
            && after.Audits.Count >= before.Audits.Count && after.Audits.Take(before.Audits.Count).SequenceEqual(before.Audits), "REPEATED_UPDATE_REWROTE_HISTORY");
        return new(before, after); // observed audit delta, without inventing an exactly-once update guarantee
    }

    public static async Task<IReadOnlyList<Phase7SqlConcurrentAttempt>> ConcurrentCreateAsync(IBusinessPartnerUnitOfWork first,
        IBusinessPartnerUnitOfWork second, CreateBusinessPartnerCommand command,
        Func<CancellationToken, Task<Phase7SqlReliabilitySnapshot>> observe, int companyId,
        Func<Exception, bool> expectedConflict, TimeSpan timeout, CancellationToken token = default)
    {
        Require(!ReferenceEquals(first, second) && timeout >= TimeSpan.FromSeconds(1) && timeout <= TimeSpan.FromSeconds(30), "CONCURRENT_CASE_CONFIG");
        Require((await new CreateBusinessPartnerCommandValidator().ValidateAsync(command, token)).IsValid, "COMMAND_INVALID");
        using var budget = CancellationTokenSource.CreateLinkedTokenSource(token);
        budget.CancelAfter(timeout);
        Empty(await observe(budget.Token).WaitAsync(budget.Token));
        var barrier = new Phase7SqlCreateBarrier();
        async Task<Phase7SqlConcurrentAttempt> Attempt(IBusinessPartnerUnitOfWork unit)
        {
            try { return new(await new CreateBusinessPartnerCommandHandler(new Phase7SqlCreateBarrierUnitOfWork(unit, barrier)).Handle(command, budget.Token), null); }
            catch (Exception error) { return new(null, error); } // evidence, never an automatic retry
        }
        var all = Task.WhenAll(Attempt(first), Attempt(second));
        Phase7SqlConcurrentAttempt[] attempts;
        try { attempts = await all.WaitAsync(budget.Token); }
        catch (OperationCanceledException) when (budget.IsCancellationRequested)
        {
            try { await all.WaitAsync(TimeSpan.FromSeconds(2)); }
            catch (TimeoutException) { throw new Phase7SqlGuardException("PHASE7_SQL_CONCURRENT_OPERATIONS_NOT_STOPPED"); }
            token.ThrowIfCancellationRequested();
            throw new Phase7SqlGuardException("PHASE7_SQL_CONCURRENT_CASE_DEADLINE");
        }
        token.ThrowIfCancellationRequested();
        Require(!budget.IsCancellationRequested, "CONCURRENT_CASE_DEADLINE");
        Require(barrier.Arrivals == 2, "CONCURRENT_BARRIER_INCOMPLETE");
        var winners = attempts.Where(a => a.Result?.IsSuccess == true).ToArray();
        Require(winners.Length == 1, "CONCURRENT_WINNER_COUNT");
        var loser = attempts.Single(a => a.Result?.IsSuccess != true);
        var allowedConflict = loser.Error is not OperationCanceledException and not TimeoutException && loser.Error is not SqlException { Number: -2 }
            && (loser.Error is not null ? expectedConflict(loser.Error)
                : loser.Result!.Errors.Any(e => e.Code is "BusinessPartnerCodeAlreadyExists" or "BusinessPartnerIdentificationAlreadyExists"));
        if (!allowedConflict)
        {
            var error = new Phase7SqlGuardException("PHASE7_SQL_CONCURRENT_UNEXPECTED_FAILURE");
            if (loser.Error is not null) error.Data["Phase7Sql.ConcurrentFailure"] = loser.Error;
            throw error;
        }
        var state = await observe(budget.Token).WaitAsync(budget.Token);
        AssertState(state, winners[0].Result!.Value!, companyId, [SyncOperation.Created]);
        Require(state.Audits.Count(a => a.Action == "INSERT") == 1 && state.Audits.All(a => a.Action == "INSERT"), "CONCURRENT_PARTIAL_AUDIT");
        token.ThrowIfCancellationRequested();
        Require(!budget.IsCancellationRequested, "CONCURRENT_CASE_DEADLINE");
        return attempts;
    }

    public static bool IsExpectedSqlConflict(Exception error) => error is SqlException sql && sql.Errors.Cast<SqlError>().Any(e => e.Number is 2601 or 2627 or 1205);

    private static async Task<BusinessPartnerDto> Create(IBusinessPartnerUnitOfWork unit, CreateBusinessPartnerCommand command, CancellationToken token)
    {
        Require((await new CreateBusinessPartnerCommandValidator().ValidateAsync(command, token)).IsValid, "COMMAND_INVALID");
        var created = await new CreateBusinessPartnerCommandHandler(unit).Handle(command, token);
        Require(created.IsSuccess && created.Value is { Id: > 0 } && created.Value.GlobalId != Guid.Empty, "CREATE_FAILED");
        return created.Value!;
    }

    private static void Empty(Phase7SqlReliabilitySnapshot state) => Require(state.Partners.Count == 0 && state.Events.Count == 0
        && state.SyncAudits.Count == 0 && state.Audits.Count == 0, "RELIABILITY_FIXTURE_NOT_EMPTY");

    private static JsonObject RepeatedCommercial(BusinessPartnerDto partner)
    {
        var node = JsonSerializer.SerializeToNode(partner)!.AsObject();
        node.Remove("UpdatedAt");
        foreach (var key in new[] { "Addresses", "Contacts", "BankAccounts", "RetentionSettings", "SapFieldMappings", "Attachments" })
        {
            foreach (var row in node[key]!.AsArray())
            {
                Require(row!["Id"]!.GetValue<int>() > 0 && row["BusinessPartnerId"]!.GetValue<int>() == partner.Id, "RELIABILITY_CHILD_SCOPE");
                row.AsObject().Remove("Id"); // effective SPs replace children; keep parent identity and all business FK/value fields
            }
        }
        Require(partner.Notes is null || partner.Notes.BusinessPartnerId == partner.Id, "RELIABILITY_CHILD_SCOPE");
        return node;
    }

    private static void AssertState(Phase7SqlReliabilitySnapshot state, BusinessPartnerDto identity, int companyId, SyncOperation[] operations)
    {
        Require(state.Partners.Count == 1 && state.Partners[0].Id == identity.Id && state.Partners[0].GlobalId == identity.GlobalId
            && state.Partners[0].Code == identity.Code && state.Partners[0].IdentificationNumber == identity.IdentificationNumber, "RELIABILITY_IDENTITY_INVALID");
        Require(state.Events.Count == operations.Length && state.SyncAudits.Count == operations.Length
            && state.Events.Select(e => e.EventId).Distinct().Count() == operations.Length, "RELIABILITY_EVENT_COUNT");
        Require(state.Audits.Any(a => a.Action == "INSERT") && state.Audits.All(a => a.EntityName == "BusinessPartners" && a.RecordId == identity.Id), "RELIABILITY_AUDIT_INVALID");
        for (var i = 0; i < operations.Length; i++)
        {
            var item = state.Events[i];
            Require(item.EventId != Guid.Empty && item.CompanyId == companyId && item.EntityName == "BusinessPartner" && item.EntityCode == identity.Code
                && item.EntityGlobalId == identity.GlobalId && item.Operation == operations[i], "RELIABILITY_EVENT_SCOPE");
            Require(state.SyncAudits.Count(a => a.EventId == item.EventId && a.Action == "Created" && a.NewStatus == "Pending") == 1, "RELIABILITY_SYNC_AUDIT_INVALID");
            var payload = JsonNode.Parse(item.PayloadJson)!;
            Require(payload["globalId"]!.GetValue<Guid>() == identity.GlobalId && payload["operation"]!.GetValue<string>() == operations[i].ToString()
                && payload["entityName"]!.GetValue<string>() == "BusinessPartner" && payload["code"]!.GetValue<string>() == identity.Code
                && payload["payload"]!["globalId"]!.GetValue<Guid>() == identity.GlobalId
                && payload["payload"]!["code"]!.GetValue<string>() == identity.Code
                && payload["payload"]!["identificationNumber"]!.GetValue<string>() == identity.IdentificationNumber
                && payload["payload"]!["name"]!.GetValue<string>() == (operations[i] == SyncOperation.Created ? identity.Name : state.Partners[0].Name), "RELIABILITY_PAYLOAD_IDENTITY");
            Phase7SqlNominalScenarios.AssertSafePayload(payload);
        }
    }

    internal static void Require(bool condition, string code)
    {
        if (!condition) throw new Phase7SqlGuardException($"PHASE7_SQL_{code}");
    }
}
