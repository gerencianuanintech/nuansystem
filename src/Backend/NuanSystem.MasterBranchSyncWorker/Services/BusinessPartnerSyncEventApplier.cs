using System.Text.Json;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

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

        BusinessPartnerCanonicalPayloadV2 payload;
        try
        {
            payload = ReadPayload(context.PayloadJson);
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
        if (!Enum.TryParse<SyncOperation>(context.Operation, false, out _))
            return Terminal("Operacion canonica no soportada.", "BP_SYNC_OPERATION_UNSUPPORTED");

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

    private static BusinessPartnerCanonicalPayloadV2 ReadPayload(string payloadJson)
    {
        using var document = JsonDocument.Parse(payloadJson);
        if (document.RootElement.ValueKind != JsonValueKind.Object ||
            !document.RootElement.TryGetProperty("payload", out var payloadElement) ||
            payloadElement.ValueKind != JsonValueKind.Object)
            throw new JsonException("El evento no contiene payload objeto.");
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
        payload.Partner.Addresses.All(item => item is not null && item.GlobalId != Guid.Empty) &&
        payload.Partner.Contacts.All(item => item is not null && item.GlobalId != Guid.Empty) &&
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
