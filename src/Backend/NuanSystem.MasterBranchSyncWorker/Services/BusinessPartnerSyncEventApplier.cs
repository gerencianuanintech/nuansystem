using System.Text.Json;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class BusinessPartnerSyncEventApplier(
    IBusinessPartnerSyncApplyRepository repository,
    ICompanyResolver companyResolver) : ISyncEntityEventApplier
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public bool CanApply(string entityName) =>
        string.Equals(entityName, SyncMasterBranchEntityCodes.BusinessPartner, StringComparison.OrdinalIgnoreCase);

    public async Task<SyncEventApplyResult> ApplyAsync(
        SyncEventApplyContext context,
        CancellationToken cancellationToken = default)
    {
        if (!CanApply(context.EntityName))
            return Terminal("Entidad canonica de socio no soportada.", "BP_SYNC_ENTITY_UNSUPPORTED");
        if (context.TargetCompanyId is null)
            return Terminal("El canonico requiere sucursal destino.", "BP_SYNC_TARGET_REQUIRED");
        if (context.Operation is not ("Created" or "Updated" or "Deleted" or "Disabled"))
            return Terminal("Operacion canonica no soportada.", "BP_SYNC_OPERATION_UNSUPPORTED");

        BusinessPartnerCanonicalPayloadV2 payload;
        try
        {
            payload = ReadPayload(context.PayloadJson, context.Operation);
        }
        catch (JsonException)
        {
            return Terminal("Payload canonico de socio no es JSON valido.", "SYNC_PAYLOAD_INVALID");
        }

        if (payload.SchemaVersion != BusinessPartnerSyncSchemaVersions.Canonical)
            return Terminal("Payload legacy de socio no soportado.", "BP_SYNC_LEGACY_PAYLOAD_UNSUPPORTED");
        if (!HasValidPayload(payload))
            return Terminal("Payload canonico de socio incompleto.", "SYNC_PAYLOAD_INVALID");
        if (payload.Partner.GlobalId != context.EntityGlobalId)
            return Terminal("El canonico no coincide con EntityGlobalId.", "BP_SYNC_GLOBAL_ID_MISMATCH");
        var source = await companyResolver.ResolveByIdAsync(context.SourceCompanyId, cancellationToken);
        if (source is null || !source.IsMaster)
            return Terminal("La empresa origen debe ser central.", "BP_SYNC_SOURCE_CENTRAL_REQUIRED");
        var target = await companyResolver.ResolveByIdAsync(context.TargetCompanyId.Value, cancellationToken);
        if (target is null || target.IsMaster)
            return Terminal("La empresa destino debe ser sucursal.", "BP_SYNC_TARGET_BRANCH_REQUIRED");
        if (target.ParentCompanyId != source.CompanyId)
            return Terminal("La sucursal destino no pertenece a la empresa central.", "BP_SYNC_PARENT_MISMATCH");
        if (!source.SyncEnabled || !target.SyncEnabled)
            return Terminal("La sincronizacion debe estar habilitada en origen y destino.", "BP_SYNC_DISABLED");

        var result = await repository.ApplyCanonicalAsync(target.CompanyId, context, payload, cancellationToken);
        return new SyncEventApplyResult(
            result.Applied,
            result.Message,
            result.ErrorCode,
            result.Retryable,
            result.Terminal);
    }

    private static BusinessPartnerCanonicalPayloadV2 ReadPayload(string payloadJson, string contextOperation)
    {
        using var document = JsonDocument.Parse(payloadJson);
        var payloadElement = BusinessPartnerSyncWireValidator.ValidateCanonicalEnvelope(
            document.RootElement,
            contextOperation);
        return payloadElement.Deserialize<BusinessPartnerCanonicalPayloadV2>(JsonOptions)
            ?? throw new JsonException("El canonico no pudo deserializarse.");
    }

    private static bool HasValidPayload(BusinessPartnerCanonicalPayloadV2 payload) =>
        payload.CanonicalVersion > 0 &&
        payload.Partner is not null &&
        payload.Partner.GlobalId != Guid.Empty &&
        HasText(payload.Partner.Code) &&
        HasText(payload.Partner.Name) &&
        payload.Partner.PartnerType is "Customer" or "Supplier" &&
        HasText(payload.Partner.IdentificationTypeCode) &&
        HasText(payload.Partner.IdentificationNumber) &&
        HasText(payload.Partner.NormalizedIdentificationNumber) &&
        payload.Partner.Addresses is not null &&
        payload.Partner.Contacts is not null &&
        payload.Partner.Addresses.All(item =>
            item is not null && item.GlobalId != Guid.Empty && HasText(item.AddressType) && HasText(item.Line1)) &&
        payload.Partner.Contacts.All(item =>
            item is not null && item.GlobalId != Guid.Empty && HasText(item.Name)) &&
        HasUniqueGlobalIds(payload.Partner.Addresses.Select(item => item.GlobalId)) &&
        HasUniqueGlobalIds(payload.Partner.Contacts.Select(item => item.GlobalId));

    private static bool HasUniqueGlobalIds(IEnumerable<Guid> ids)
    {
        var values = ids.ToArray();
        return values.Distinct().Count() == values.Length;
    }

    private static bool HasText(string? value) => !string.IsNullOrWhiteSpace(value);

    private static SyncEventApplyResult Terminal(string message, string errorCode) =>
        new(false, message, errorCode, Retryable: false, Terminal: true);
}

internal static class BusinessPartnerSyncWireValidator
{
    private static readonly string[] RootProperties =
        ["entityName", "globalId", "code", "operation", "payload"];
    private static readonly string[] CanonicalPayloadProperties =
        ["schemaVersion", "canonicalVersion", "originCompanyId", "causationEventId", "partner"];
    private static readonly string[] ResultPayloadProperties =
        ["schemaVersion", "globalId", "proposalEventId", "originCompanyId", "status", "message", "canonicalVersion", "canonical"];
    private static readonly string[] SnapshotProperties =
    [
        "globalId", "code", "name", "commercialName", "partnerType", "identificationTypeCode",
        "identificationNumber", "normalizedIdentificationNumber", "email", "phone", "sapCardCode",
        "isActive", "addresses", "contacts"
    ];
    private static readonly string[] AddressProperties =
    [
        "globalId", "addressType", "line1", "line2", "countryCode", "provinceCode", "cityCode",
        "postalCode", "latitude", "longitude", "isPrimary", "isActive"
    ];
    private static readonly string[] ContactProperties =
    [
        "globalId", "contactTypeCode", "contactChannelCode", "name", "position", "department", "phone",
        "extension", "mobile", "email", "language", "receivesNotifications", "isPrimary", "isActive", "notes"
    ];

    internal static JsonElement ValidateCanonicalEnvelope(JsonElement root, string contextOperation)
    {
        var payload = ValidateRoot(root, codeRequired: true);
        ValidateExactObject(payload, CanonicalPayloadProperties);
        RequireInt32(payload, "schemaVersion");
        RequireInt64(payload, "canonicalVersion");
        RequireNullableInt32(payload, "originCompanyId");
        RequireNullableGuid(payload, "causationEventId");
        var partner = RequireObject(payload, "partner");
        ValidateSnapshot(partner);
        RequireEnvelopeValue(root, "entityName", "BusinessPartner");
        RequireEnvelopeValue(root, "operation", contextOperation);
        if (RequireGuid(root, "globalId") != RequireGuid(partner, "globalId"))
            Invalid("The envelope and canonical globalId values differ.");
        if (!string.Equals(
                Required(root, "code").GetString(),
                Required(partner, "code").GetString(),
                StringComparison.Ordinal))
            Invalid("The envelope and canonical code values differ.");
        return payload;
    }

    internal static JsonElement ValidateResultEnvelope(JsonElement root)
    {
        var payload = ValidateRoot(root, codeRequired: false);
        ValidateExactObject(payload, ResultPayloadProperties);
        RequireInt32(payload, "schemaVersion");
        RequireGuid(payload, "globalId");
        RequireGuid(payload, "proposalEventId");
        RequireInt32(payload, "originCompanyId");
        RequireString(payload, "status", nonBlank: true);
        RequireNullableString(payload, "message");
        RequireInt64(payload, "canonicalVersion");
        var canonical = Required(payload, "canonical");
        if (canonical.ValueKind == JsonValueKind.Object)
            ValidateSnapshot(canonical);
        else if (canonical.ValueKind != JsonValueKind.Null)
            Invalid("canonical must be an object or null.");
        RequireEnvelopeValue(root, "entityName", "BusinessPartnerProposalResult");
        RequireEnvelopeValue(root, "operation", "Updated");
        if (RequireGuid(root, "globalId") != RequireGuid(payload, "globalId"))
            Invalid("The envelope and result globalId values differ.");
        return payload;
    }

    private static JsonElement ValidateRoot(JsonElement root, bool codeRequired)
    {
        ValidateExactObject(root, RootProperties, ["correlationId"]);
        RequireString(root, "entityName", nonBlank: true);
        RequireGuid(root, "globalId");
        var code = Required(root, "code");
        if (codeRequired)
            RequireStringValue(code, "code", nonBlank: true);
        else if (code.ValueKind != JsonValueKind.Null)
            Invalid("code must be null for a result event.");
        RequireString(root, "operation", nonBlank: true);
        if (root.TryGetProperty("correlationId", out var correlationId))
            RequireStringValue(correlationId, "correlationId", nonBlank: true);
        return RequireObject(root, "payload");
    }

    private static void ValidateSnapshot(JsonElement snapshot)
    {
        ValidateExactObject(snapshot, SnapshotProperties);
        RequireGuid(snapshot, "globalId");
        RequireString(snapshot, "code", nonBlank: true);
        RequireString(snapshot, "name", nonBlank: true);
        RequireNullableString(snapshot, "commercialName");
        RequireString(snapshot, "partnerType", nonBlank: true);
        RequireString(snapshot, "identificationTypeCode", nonBlank: true);
        RequireString(snapshot, "identificationNumber", nonBlank: true);
        RequireString(snapshot, "normalizedIdentificationNumber", nonBlank: true);
        RequireNullableString(snapshot, "email");
        RequireNullableString(snapshot, "phone");
        RequireNullableString(snapshot, "sapCardCode");
        RequireBoolean(snapshot, "isActive");

        foreach (var address in RequireArray(snapshot, "addresses").EnumerateArray())
            ValidateAddress(address);
        foreach (var contact in RequireArray(snapshot, "contacts").EnumerateArray())
            ValidateContact(contact);
    }

    private static void ValidateAddress(JsonElement address)
    {
        ValidateExactObject(address, AddressProperties);
        RequireGuid(address, "globalId");
        RequireString(address, "addressType", nonBlank: true);
        RequireString(address, "line1", nonBlank: true);
        RequireNullableString(address, "line2");
        RequireNullableStableCode(address, "countryCode");
        RequireNullableStableCode(address, "provinceCode");
        RequireNullableStableCode(address, "cityCode");
        RequireNullableString(address, "postalCode");
        RequireNullableDecimal(address, "latitude");
        RequireNullableDecimal(address, "longitude");
        RequireBoolean(address, "isPrimary");
        RequireBoolean(address, "isActive");
    }

    private static void ValidateContact(JsonElement contact)
    {
        ValidateExactObject(contact, ContactProperties);
        RequireGuid(contact, "globalId");
        RequireNullableStableCode(contact, "contactTypeCode");
        RequireNullableStableCode(contact, "contactChannelCode");
        RequireString(contact, "name", nonBlank: true);
        RequireNullableString(contact, "position");
        RequireNullableString(contact, "department");
        RequireNullableString(contact, "phone");
        RequireNullableString(contact, "extension");
        RequireNullableString(contact, "mobile");
        RequireNullableString(contact, "email");
        RequireNullableString(contact, "language");
        RequireBoolean(contact, "receivesNotifications");
        RequireBoolean(contact, "isPrimary");
        RequireBoolean(contact, "isActive");
        RequireNullableString(contact, "notes");
    }

    private static void ValidateExactObject(
        JsonElement element,
        IReadOnlyCollection<string> required,
        IReadOnlyCollection<string>? optional = null)
    {
        if (element.ValueKind != JsonValueKind.Object)
            Invalid("Expected an object.");

        var allowed = new HashSet<string>(required, StringComparer.Ordinal);
        if (optional is not null)
            allowed.UnionWith(optional);
        var present = new HashSet<string>(StringComparer.Ordinal);
        foreach (var property in element.EnumerateObject())
        {
            if (!allowed.Contains(property.Name) || !present.Add(property.Name))
                Invalid($"Unexpected or duplicate property {property.Name}.");
        }

        if (required.Any(name => !present.Contains(name)))
            Invalid("A required property is missing.");
    }

    private static JsonElement Required(JsonElement parent, string name) =>
        parent.TryGetProperty(name, out var value)
            ? value
            : throw new JsonException($"Missing {name}.");

    private static JsonElement RequireObject(JsonElement parent, string name)
    {
        var value = Required(parent, name);
        if (value.ValueKind != JsonValueKind.Object)
            Invalid($"{name} must be an object.");
        return value;
    }

    private static JsonElement RequireArray(JsonElement parent, string name)
    {
        var value = Required(parent, name);
        if (value.ValueKind != JsonValueKind.Array)
            Invalid($"{name} must be an array.");
        return value;
    }

    private static void RequireString(JsonElement parent, string name, bool nonBlank) =>
        RequireStringValue(Required(parent, name), name, nonBlank);

    private static void RequireStringValue(JsonElement value, string name, bool nonBlank)
    {
        if (value.ValueKind != JsonValueKind.String ||
            (nonBlank && string.IsNullOrWhiteSpace(value.GetString())))
            Invalid($"{name} must be a string{(nonBlank ? " with content" : string.Empty)}.");
    }

    private static void RequireNullableString(JsonElement parent, string name)
    {
        var value = Required(parent, name);
        if (value.ValueKind is not (JsonValueKind.String or JsonValueKind.Null))
            Invalid($"{name} must be a string or null.");
    }

    private static void RequireNullableStableCode(JsonElement parent, string name)
    {
        var value = Required(parent, name);
        if (value.ValueKind == JsonValueKind.Null)
            return;
        RequireStringValue(value, name, nonBlank: true);
    }

    private static Guid RequireGuid(JsonElement parent, string name)
    {
        var value = Required(parent, name);
        var parsed = Guid.Empty;
        if (value.ValueKind != JsonValueKind.String || !value.TryGetGuid(out parsed) || parsed == Guid.Empty)
            Invalid($"{name} must be a non-empty GUID.");
        return parsed;
    }

    private static void RequireEnvelopeValue(JsonElement root, string name, string expected)
    {
        if (!string.Equals(Required(root, name).GetString(), expected, StringComparison.Ordinal))
            Invalid($"The envelope {name} value is inconsistent.");
    }

    private static void RequireNullableGuid(JsonElement parent, string name)
    {
        var value = Required(parent, name);
        if (value.ValueKind == JsonValueKind.Null)
            return;
        if (value.ValueKind != JsonValueKind.String || !value.TryGetGuid(out var parsed) || parsed == Guid.Empty)
            Invalid($"{name} must be a non-empty GUID or null.");
    }

    private static void RequireInt32(JsonElement parent, string name)
    {
        var value = Required(parent, name);
        if (value.ValueKind != JsonValueKind.Number || !value.TryGetInt32(out _))
            Invalid($"{name} must be an integer.");
    }

    private static void RequireNullableInt32(JsonElement parent, string name)
    {
        var value = Required(parent, name);
        if (value.ValueKind == JsonValueKind.Null)
            return;
        if (value.ValueKind != JsonValueKind.Number || !value.TryGetInt32(out _))
            Invalid($"{name} must be an integer or null.");
    }

    private static void RequireInt64(JsonElement parent, string name)
    {
        var value = Required(parent, name);
        if (value.ValueKind != JsonValueKind.Number || !value.TryGetInt64(out _))
            Invalid($"{name} must be an integer.");
    }

    private static void RequireNullableDecimal(JsonElement parent, string name)
    {
        var value = Required(parent, name);
        if (value.ValueKind == JsonValueKind.Null)
            return;
        if (value.ValueKind != JsonValueKind.Number || !value.TryGetDecimal(out _))
            Invalid($"{name} must be a number or null.");
    }

    private static void RequireBoolean(JsonElement parent, string name)
    {
        var value = Required(parent, name);
        if (value.ValueKind is not (JsonValueKind.True or JsonValueKind.False))
            Invalid($"{name} must be a boolean.");
    }

    [System.Diagnostics.CodeAnalysis.DoesNotReturn]
    private static void Invalid(string message) => throw new JsonException(message);
}
