using System.Text.Json;
using System.Text.Json.Nodes;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.BusinessPartners.Commands;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Tests.Infrastructure.Phase7Sql;

internal sealed record Phase7NominalAudit(string EntityName, int RecordId, string Action, string FieldName, string? OldValue, string? NewValue, int? UserId, string? UserName);
internal sealed record Phase7NominalSyncAudit(Guid EventId, string Action, string NewStatus);
internal sealed record Phase7NominalSnapshot(BusinessPartnerDto Partner, bool IsDeleted,
    IReadOnlyList<Phase7NominalAudit> Audits, IReadOnlyList<CreateLocalSyncOutboxData> Events, IReadOnlyList<Phase7NominalSyncAudit> SyncAudits);

// Same reusable scenarios for offline adapters and the future authorized SQL observer.
// Snapshots must be detached, filtered to this fixture, and read AFTER the handler returns.
internal static class Phase7SqlNominalScenarios
{
    public static Task<IReadOnlyList<Phase7NominalSnapshot>> RunOnFixtureAsync(Phase7SqlFixture.Phase7SqlTenant tenant,
        bool legacy, bool isMaster, bool syncEnabled, CreateBusinessPartnerCommand create,
        Func<int, UpdateBusinessPartnerCommand> update,
        Func<System.Data.IDbConnection, int, int, CancellationToken, Task<Phase7NominalSnapshot>> readSnapshot,
        CancellationToken token = default) => RunAsync(
            new Phase7NominalCheckpointUnitOfWork(legacy ? tenant.CreateLegacyReference(isMaster, syncEnabled) : tenant.CreateUnitOfWork(isMaster, syncEnabled), tenant.CheckpointAsync),
            create, update, (id, ct) => tenant.ObserveAsync((c, timeout, observerToken) => readSnapshot(c, id, timeout, observerToken), ct),
            tenant.CompanyId, isMaster && syncEnabled, token);

    public static async Task<IReadOnlyList<Phase7NominalSnapshot>> RunAsync(IBusinessPartnerUnitOfWork unit,
        CreateBusinessPartnerCommand create, Func<int, UpdateBusinessPartnerCommand> update,
        Func<int, CancellationToken, Task<Phase7NominalSnapshot>> observe, int companyId, bool expectsSync,
        CancellationToken token = default)
    {
        Require((await new CreateBusinessPartnerCommandValidator().ValidateAsync(create, token)).IsValid, "COMMAND_INVALID");
        var created = await new CreateBusinessPartnerCommandHandler(unit).Handle(create, token);
        Require(created.IsSuccess && created.Value is not null, "CREATE_FAILED");
        var id = created.Value!.Id;
        var globalId = created.Value.GlobalId;
        Require(id > 0 && globalId != Guid.Empty, "IDENTITY_INVALID");
        var snapshots = new List<Phase7NominalSnapshot> { await observe(id, token) };
        AssertSnapshot(snapshots[0], id, globalId, companyId, 1, expectsSync, create.AuditUserId, create.AuditUserName);
        AssertRequestedCommercialFields(create, snapshots[0].Partner);
        var edit = update(id);
        Require(edit.Id == id && edit.Code.Trim().ToUpperInvariant() == create.Code.Trim().ToUpperInvariant(), "UPDATE_FIXTURE_INVALID");
        Require((await new UpdateBusinessPartnerCommandValidator().ValidateAsync(edit, token)).IsValid, "COMMAND_INVALID");
        var updated = await new UpdateBusinessPartnerCommandHandler(unit).Handle(edit, token);
        Require(updated.IsSuccess && updated.Value?.GlobalId == globalId, "UPDATE_IDENTITY_CHANGED");
        snapshots.Add(await observe(id, token));
        AssertSnapshot(snapshots[1], id, globalId, companyId, 2, expectsSync, edit.AuditUserId, edit.AuditUserName);
        AssertRequestedCommercialFields(edit, snapshots[1].Partner);
        Require(snapshots[1].Partner.Name == edit.Name.Trim(), "UPDATE_NOT_OBSERVED");
        var deleted = await new DeleteBusinessPartnerCommandHandler(unit).Handle(new(id, edit.AuditUserId, edit.AuditUserName), token);
        Require(deleted.IsSuccess && deleted.Value, "DELETE_FAILED");
        snapshots.Add(await observe(id, token));
        AssertSnapshot(snapshots[2], id, globalId, companyId, 3, expectsSync, edit.AuditUserId, edit.AuditUserName);
        return snapshots;
    }

    private static void AssertSnapshot(Phase7NominalSnapshot snapshot, int id, Guid globalId, int companyId,
        int step, bool sync, int? userId, string? userName)
    {
        Require(snapshot.Partner.Id == id && snapshot.Partner.GlobalId == globalId, "OBSERVER_IDENTITY_CHANGED");
        Require(snapshot.IsDeleted == (step == 3) && (step != 3 || !snapshot.Partner.IsActive), "DELETE_STATE_INVALID");
        Require(snapshot.Audits.All(a => a.EntityName == "BusinessPartners" && a.RecordId == id), "AUDIT_SCOPE_INVALID");
        var action = step == 1 ? "INSERT" : step == 2 ? "UPDATE" : "DELETE";
        Require(snapshot.Audits.Any(a => a.Action == action && a.UserId == userId && a.UserName == userName), "AUDIT_MISSING");
        Require(snapshot.Partner.Addresses.All(a => a.BusinessPartnerId == id) && snapshot.Partner.Contacts.All(a => a.BusinessPartnerId == id)
            && snapshot.Partner.BankAccounts.All(a => a.BusinessPartnerId == id) && snapshot.Partner.RetentionSettings.All(a => a.BusinessPartnerId == id)
            && snapshot.Partner.Attachments.All(a => a.BusinessPartnerId == id) && snapshot.Partner.SapFieldMappings.All(a => a.BusinessPartnerId == id)
            && (snapshot.Partner.Notes is null || snapshot.Partner.Notes.BusinessPartnerId == id), "CHILD_SCOPE_INVALID");
        Require(snapshot.Events.Count == (sync ? step : 0) && snapshot.SyncAudits.Count == (sync ? step : 0), "INTENTION_COUNT_INVALID");
        if (!sync) return;
        Require(snapshot.Events.Select(e => e.EventId).Distinct().Count() == step, "EVENT_ID_INVALID");
        for (var i = 0; i < snapshot.Events.Count; i++)
        {
            var item = snapshot.Events[i];
            var operation = new[] { SyncOperation.Created, SyncOperation.Updated, SyncOperation.Deleted }[i];
            Require(item.EventId != Guid.Empty && item.CompanyId == companyId && item.EntityName == "BusinessPartner"
                && item.EntityGlobalId == globalId && item.EntityCode == snapshot.Partner.Code && item.Operation == operation, "EVENT_SCOPE_INVALID");
            var payload = JsonNode.Parse(item.PayloadJson)!.AsObject();
            Require(payload["globalId"]!.GetValue<Guid>() == globalId && payload["operation"]!.GetValue<string>() == operation.ToString()
                && payload["payload"]!["globalId"]!.GetValue<Guid>() == globalId, "PAYLOAD_IDENTITY_INVALID");
            AssertSafePayload(payload);
            Require(snapshot.SyncAudits.Count(a => a.EventId == item.EventId && a.Action == "Created" && a.NewStatus == "Pending") == 1, "SYNC_AUDIT_INVALID");
        }
    }

    internal static void AssertSafePayload(JsonNode node)
    {
        if (node is JsonObject obj)
            foreach (var (key, value) in obj)
            {
                Require(!new[] { "password", "secret", "token", "credential", "connectionstring", "apikey", "api_key", "sapcard", "sapcompany", "saplasterror", "sapfield" }
                    .Any(fragment => key.Contains(fragment, StringComparison.OrdinalIgnoreCase)), "PAYLOAD_SENSITIVE_FIELD");
                if (value is not null) AssertSafePayload(value);
            }
        else if (node is JsonArray array) foreach (var value in array) if (value is not null) AssertSafePayload(value);
    }

    private static void AssertRequestedCommercialFields(object command, BusinessPartnerDto partner)
    {
        var expected = JsonSerializer.SerializeToNode(command)!.AsObject();
        var observed = JsonSerializer.SerializeToNode(partner)!.AsObject();
        foreach (var (key, value) in expected)
        {
            if (key.StartsWith("Sap", StringComparison.Ordinal) || key is "AuditUserId" or "AuditUserName" || !observed.ContainsKey(key)) continue;
            if (value is null)
            {
                Require(key == "CreditStatus" ? observed[key]?.GetValue<string>() == "Normal"
                    : observed[key] is JsonArray empty ? empty.Count == 0 : observed[key] is null, "COMMERCIAL_FIELDS_MISMATCH");
                continue;
            }
            var wanted = value.DeepClone();
            if (wanted is JsonValue scalar && scalar.TryGetValue<string>(out var text))
                wanted = JsonValue.Create(key is "Code" or "PreferredCurrencyCode" or "PurchaseCurrencyCode" ? text.Trim().ToUpperInvariant() : text.Trim())!;
            var actual = observed[key]?.DeepClone();
            if (actual is not null) Normalize(actual);
            if (wanted is JsonArray or JsonObject) Normalize(wanted);
            // Child DTOs also include display fields. Compare the fields explicitly supplied by the command.
            Require(ContainsRequested(wanted, actual), "COMMERCIAL_FIELDS_MISMATCH");
        }
    }

    private static bool ContainsRequested(JsonNode? expected, JsonNode? actual)
    {
        if (expected is JsonObject obj && actual is JsonObject actualObj)
            return obj.All(p => actualObj.ContainsKey(p.Key) && ContainsRequested(p.Value, actualObj[p.Key]));
        if (expected is JsonArray rows && actual is JsonArray actualRows)
            return rows.Count == actualRows.Count && rows.Select((row, i) => ContainsRequested(row, actualRows[i])).All(x => x);
        return JsonNode.DeepEquals(expected, actual);
    }

    public static void AssertEquivalent(IReadOnlyList<Phase7NominalSnapshot> reference, IReadOnlyList<Phase7NominalSnapshot> pilot)
    {
        Require(reference.Count == 3 && pilot.Count == 3, "EQUIVALENCE_INCOMPLETE");
        foreach (var evidence in new[] { reference, pilot })
        {
            var first = evidence[0].Partner;
            Require(first.Id > 0 && first.GlobalId != Guid.Empty && evidence.All(s => s.Partner.Id == first.Id && s.Partner.GlobalId == first.GlobalId), "EQUIVALENCE_IDENTITY_DRIFT");
        }
        for (var i = 0; i < 3; i++)
        {
            Require(JsonNode.DeepEquals(Canonical(reference[i]), Canonical(pilot[i])), "EQUIVALENCE_MISMATCH");
        }
    }

    private static JsonNode Canonical(Phase7NominalSnapshot snapshot)
    {
        // Only generated row/event identity, per-tenant company identity and timestamps differ.
        // Business FK ids (IdentificationTypeId, AccountId, etc.) remain comparable, never stripped.
        var root = JsonSerializer.SerializeToNode(snapshot)!;
        Normalize(root);
        // Raw JSON strings are parsed so generated GlobalId inside event payloads can be normalized.
        foreach (var item in root["Events"]!.AsArray())
        {
            var payload = JsonNode.Parse(item!["PayloadJson"]!.GetValue<string>())!;
            Normalize(payload);
            item["PayloadJson"] = payload;
        }
        return root;
    }

    private static void Normalize(JsonNode node)
    {
        if (node is JsonObject obj)
        {
            foreach (var key in obj.Select(x => x.Key).ToArray())
            {
                if (new[] { "Id", "RecordId", "BusinessPartnerId", "GlobalId", "EntityGlobalId", "EventId", "CompanyId", "CreatedAt", "UpdatedAt", "globalId" }.Contains(key)) obj.Remove(key);
                else if (obj[key] is { } value) Normalize(value);
            }
        }
        else if (node is JsonArray array) foreach (var value in array) if (value is not null) Normalize(value);
    }

    private static void Require(bool condition, string code)
    {
        if (!condition) throw new Phase7SqlGuardException($"PHASE7_SQL_{code}");
    }
}
