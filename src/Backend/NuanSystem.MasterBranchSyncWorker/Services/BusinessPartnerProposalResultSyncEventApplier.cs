using System.Text.Json;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class BusinessPartnerProposalResultSyncEventApplier(
    IBusinessPartnerSyncApplyRepository repository,
    ICompanyResolver companyResolver) : ISyncEntityEventApplier
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public bool CanApply(string entityName) =>
        string.Equals(entityName, SyncMasterBranchEntityCodes.BusinessPartnerProposalResult, StringComparison.OrdinalIgnoreCase);

    public async Task<SyncEventApplyResult> ApplyAsync(
        SyncEventApplyContext context,
        CancellationToken cancellationToken = default)
    {
        if (!CanApply(context.EntityName))
            return Terminal("Entidad de resultado no soportada.", "BP_SYNC_ENTITY_UNSUPPORTED");
        if (context.TargetCompanyId is null)
            return Terminal("El resultado requiere sucursal destino.", "BP_SYNC_TARGET_REQUIRED");
        if (context.Operation != "Updated")
            return Terminal("Operacion de resultado no soportada.", "BP_SYNC_OPERATION_UNSUPPORTED");

        BusinessPartnerProposalResultPayloadV1 payload;
        try
        {
            payload = ReadPayload(context.PayloadJson);
        }
        catch (JsonException)
        {
            return Terminal("Payload de resultado de socio no es JSON valido.", "SYNC_PAYLOAD_INVALID");
        }

        if (payload.SchemaVersion != BusinessPartnerSyncSchemaVersions.ProposalResult)
            return Terminal("Schema de resultado no soportado.", "BP_SYNC_RESULT_SCHEMA_UNSUPPORTED");
        if (!HasValidPayload(payload))
            return Terminal("Payload de resultado de socio incompleto.", "SYNC_PAYLOAD_INVALID");
        if (payload.Status is not ("Accepted" or "Rejected" or "Conflict"))
            return Terminal("Estado de resultado no soportado.", "BP_SYNC_RESULT_STATUS_UNSUPPORTED");
        if (payload.Status == "Conflict" && payload.Canonical is null)
            return Terminal("Payload de resultado de socio incompleto.", "SYNC_PAYLOAD_INVALID");
        if (payload.Status == "Rejected" && payload.Canonical is null && payload.CanonicalVersion != 0)
            return Terminal("Payload de resultado de socio incompleto.", "SYNC_PAYLOAD_INVALID");
        if (payload.GlobalId != context.EntityGlobalId)
            return Terminal("El resultado no coincide con EntityGlobalId.", "BP_SYNC_GLOBAL_ID_MISMATCH");
        if (context.TargetCompanyId != payload.OriginCompanyId)
            return Terminal("El resultado solo puede llegar a su sucursal origen.", "BP_SYNC_RESULT_TARGET_MISMATCH");
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

        var result = await repository.ApplyProposalResultAsync(target.CompanyId, context, payload, cancellationToken);
        return new SyncEventApplyResult(
            result.Applied,
            result.Message,
            result.ErrorCode,
            result.Retryable,
            result.Terminal);
    }

    private static BusinessPartnerProposalResultPayloadV1 ReadPayload(string payloadJson)
    {
        using var document = JsonDocument.Parse(payloadJson);
        var payloadElement = BusinessPartnerSyncWireValidator.ValidateResultEnvelope(document.RootElement);
        return payloadElement.Deserialize<BusinessPartnerProposalResultPayloadV1>(JsonOptions)
            ?? throw new JsonException("El resultado no pudo deserializarse.");
    }

    private static bool HasValidPayload(BusinessPartnerProposalResultPayloadV1 payload) =>
        payload.GlobalId != Guid.Empty &&
        payload.ProposalEventId != Guid.Empty &&
        payload.OriginCompanyId > 0 &&
        payload.CanonicalVersion >= 0 &&
        (payload.Canonical is null || HasValidCanonical(payload.Canonical, payload.GlobalId));

    private static bool HasValidCanonical(BusinessPartnerCanonicalSnapshot canonical, Guid globalId) =>
        canonical.GlobalId == globalId &&
        !string.IsNullOrWhiteSpace(canonical.Code) &&
        !string.IsNullOrWhiteSpace(canonical.Name) &&
        canonical.PartnerType is "Customer" or "Supplier" &&
        !string.IsNullOrWhiteSpace(canonical.IdentificationTypeCode) &&
        !string.IsNullOrWhiteSpace(canonical.IdentificationNumber) &&
        !string.IsNullOrWhiteSpace(canonical.NormalizedIdentificationNumber) &&
        canonical.Addresses is not null &&
        canonical.Contacts is not null &&
        canonical.Addresses.All(item =>
            item is not null && item.GlobalId != Guid.Empty &&
            !string.IsNullOrWhiteSpace(item.AddressType) && !string.IsNullOrWhiteSpace(item.Line1)) &&
        canonical.Contacts.All(item =>
            item is not null && item.GlobalId != Guid.Empty && !string.IsNullOrWhiteSpace(item.Name)) &&
        HasUniqueGlobalIds(canonical.Addresses.Select(item => item.GlobalId)) &&
        HasUniqueGlobalIds(canonical.Contacts.Select(item => item.GlobalId));

    private static bool HasUniqueGlobalIds(IEnumerable<Guid> ids)
    {
        var values = ids.ToArray();
        return values.Distinct().Count() == values.Length;
    }

    private static SyncEventApplyResult Terminal(string message, string errorCode) =>
        new(false, message, errorCode, Retryable: false, Terminal: true);
}
