using System.Text.Json;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class BusinessPartnerProposalSyncEventApplier(
    IBusinessPartnerProposalApplyRepository repository,
    ICompanyResolver companyResolver) : ISyncEntityEventApplier
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly string[] ProposalMembers =
    [
        "schemaVersion", "globalId", "code", "partnerType", "identificationTypeCode",
        "identificationNumber", "normalizedIdentificationNumber", "baseCanonicalVersion",
        "originUserId", "originUserName", "base", "proposed", "changedFields"
    ];
    private static readonly string[] SnapshotMembers =
    [
        "globalId", "code", "name", "commercialName", "partnerType",
        "identificationTypeCode", "identificationNumber", "normalizedIdentificationNumber",
        "email", "phone", "sapCardCode", "isActive", "addresses", "contacts"
    ];
    private static readonly string[] AddressMembers =
    [
        "globalId", "addressType", "line1", "line2", "countryCode", "provinceCode",
        "cityCode", "postalCode", "latitude", "longitude", "isPrimary", "isActive"
    ];
    private static readonly string[] ContactMembers =
    [
        "globalId", "contactTypeCode", "contactChannelCode", "name", "position",
        "department", "phone", "extension", "mobile", "email", "language",
        "receivesNotifications", "isPrimary", "isActive", "notes"
    ];

    public bool CanApply(string entityName) =>
        string.Equals(
            entityName,
            SyncMasterBranchEntityCodes.BusinessPartnerProposal,
            StringComparison.OrdinalIgnoreCase);

    public async Task<SyncEventApplyResult> ApplyAsync(
        SyncEventApplyContext context,
        CancellationToken cancellationToken = default)
    {
        if (!CanApply(context.EntityName))
        {
            return Terminal("Entidad de propuesta no soportada.", "BP_SYNC_ENTITY_UNSUPPORTED");
        }

        if (context.TargetCompanyId is null)
        {
            return Terminal("La propuesta requiere empresa central destino.", "BP_SYNC_TARGET_REQUIRED");
        }

        BusinessPartnerProposalPayloadV1 proposal;
        try
        {
            proposal = ReadPayload(context.PayloadJson);
        }
        catch (JsonException)
        {
            return Terminal("Payload de propuesta de socio no es JSON valido.", "SYNC_PAYLOAD_INVALID");
        }

        if (proposal.SchemaVersion <= 0)
        {
            return Terminal("Payload de propuesta de socio incompleto.", "SYNC_PAYLOAD_INVALID");
        }

        if (proposal.SchemaVersion != BusinessPartnerSyncSchemaVersions.Proposal)
        {
            return Terminal("Schema de propuesta no soportado.", "BP_SYNC_PROPOSAL_SCHEMA_UNSUPPORTED");
        }

        if (!HasValidStructure(proposal))
        {
            return Terminal("Payload de propuesta de socio incompleto.", "SYNC_PAYLOAD_INVALID");
        }

        if (proposal.GlobalId == Guid.Empty || proposal.GlobalId != context.EntityGlobalId)
        {
            return Terminal("La propuesta no coincide con EntityGlobalId.", "BP_SYNC_GLOBAL_ID_MISMATCH");
        }

        if (context.Operation is not ("Created" or "Updated"))
        {
            return Terminal("Operacion de propuesta no soportada.", "BP_SYNC_OPERATION_UNSUPPORTED");
        }

        if (!OperationMatchesBase(context.Operation, proposal))
        {
            return Terminal("Operacion y snapshot base de propuesta no coinciden.", "SYNC_PAYLOAD_INVALID");
        }

        var source = await companyResolver.ResolveByIdAsync(context.SourceCompanyId, cancellationToken);
        if (source is null || source.IsMaster)
        {
            return Terminal("La empresa origen debe ser una sucursal registrada.", "BP_SYNC_SOURCE_BRANCH_REQUIRED");
        }

        var target = await companyResolver.ResolveByIdAsync(context.TargetCompanyId.Value, cancellationToken);
        if (target is null || !target.IsMaster)
        {
            return Terminal("La empresa destino debe ser central.", "BP_SYNC_TARGET_CENTRAL_REQUIRED");
        }

        if (source.ParentCompanyId != target.CompanyId)
        {
            return Terminal("La empresa central destino no es el padre de la sucursal.", "BP_SYNC_PARENT_MISMATCH");
        }

        if (!source.SyncEnabled || !target.SyncEnabled)
        {
            return Terminal("La sincronizacion debe estar habilitada en origen y destino.", "BP_SYNC_DISABLED");
        }

        var result = await repository.ApplyAsync(
            target.CompanyId,
            context,
            proposal,
            cancellationToken);

        return result.Outcome switch
        {
            BusinessPartnerProposalApplyOutcome.Accepted or
            BusinessPartnerProposalApplyOutcome.Rejected or
            BusinessPartnerProposalApplyOutcome.Conflict or
            BusinessPartnerProposalApplyOutcome.Duplicate =>
                new SyncEventApplyResult(true, result.Message, result.ErrorCode),

            BusinessPartnerProposalApplyOutcome.RetryableFailure =>
                new SyncEventApplyResult(false, result.Message, result.ErrorCode, Retryable: true),

            _ => Terminal(result.Message, result.ErrorCode)
        };
    }

    private static BusinessPartnerProposalPayloadV1 ReadPayload(string payloadJson)
    {
        using var document = JsonDocument.Parse(payloadJson);
        if (document.RootElement.ValueKind != JsonValueKind.Object ||
            !document.RootElement.TryGetProperty("payload", out var payloadElement))
        {
            throw new JsonException("El evento no contiene payload.");
        }

        if (!HasRequiredJsonMembers(payloadElement))
        {
            throw new JsonException("La propuesta no contiene todos los miembros requeridos.");
        }

        return payloadElement.Deserialize<BusinessPartnerProposalPayloadV1>(JsonOptions)
            ?? throw new JsonException("La propuesta no pudo deserializarse.");
    }

    private static bool HasRequiredJsonMembers(JsonElement payload)
    {
        if (payload.ValueKind != JsonValueKind.Object ||
            !HasMembers(payload, ProposalMembers) ||
            payload.GetProperty("proposed").ValueKind != JsonValueKind.Object ||
            !HasRequiredSnapshotMembers(payload.GetProperty("proposed")) ||
            payload.GetProperty("changedFields").ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        var baseElement = payload.GetProperty("base");
        return baseElement.ValueKind == JsonValueKind.Null ||
               baseElement.ValueKind == JsonValueKind.Object && HasRequiredSnapshotMembers(baseElement);
    }

    private static bool HasRequiredSnapshotMembers(JsonElement snapshot)
    {
        if (!HasMembers(snapshot, SnapshotMembers) ||
            snapshot.GetProperty("addresses").ValueKind != JsonValueKind.Array ||
            snapshot.GetProperty("contacts").ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        return snapshot.GetProperty("addresses").EnumerateArray().All(address =>
                   address.ValueKind == JsonValueKind.Object && HasMembers(address, AddressMembers)) &&
               snapshot.GetProperty("contacts").EnumerateArray().All(contact =>
                   contact.ValueKind == JsonValueKind.Object && HasMembers(contact, ContactMembers));
    }

    private static bool HasMembers(JsonElement element, IEnumerable<string> names) =>
        names.All(name => element.TryGetProperty(name, out _));

    private static bool HasValidStructure(BusinessPartnerProposalPayloadV1 proposal) =>
        proposal.GlobalId != Guid.Empty &&
        HasText(proposal.Code) &&
        HasText(proposal.PartnerType) &&
        HasText(proposal.IdentificationTypeCode) &&
        HasText(proposal.IdentificationNumber) &&
        HasText(proposal.NormalizedIdentificationNumber) &&
        proposal.BaseCanonicalVersion >= 0 &&
        proposal.ChangedFields is not null &&
        proposal.ChangedFields.All(HasText) &&
        proposal.Proposed is not null &&
        HasValidSnapshot(proposal.Proposed) &&
        (proposal.Base is null || HasValidSnapshot(proposal.Base));

    private static bool HasValidSnapshot(BusinessPartnerCanonicalSnapshot snapshot) =>
        snapshot.GlobalId != Guid.Empty &&
        HasText(snapshot.Code) &&
        HasText(snapshot.Name) &&
        HasText(snapshot.PartnerType) &&
        HasText(snapshot.IdentificationTypeCode) &&
        HasText(snapshot.IdentificationNumber) &&
        HasText(snapshot.NormalizedIdentificationNumber) &&
        snapshot.Addresses is not null &&
        snapshot.Addresses.All(address =>
            address is not null &&
            address.GlobalId != Guid.Empty &&
            HasText(address.AddressType) &&
            HasText(address.Line1)) &&
        snapshot.Contacts is not null &&
        snapshot.Contacts.All(contact =>
            contact is not null &&
            contact.GlobalId != Guid.Empty &&
            HasText(contact.Name));

    private static bool OperationMatchesBase(
        string operation,
        BusinessPartnerProposalPayloadV1 proposal) =>
        operation switch
        {
            "Created" => proposal.BaseCanonicalVersion == 0 && proposal.Base is null,
            "Updated" => proposal.BaseCanonicalVersion > 0 && proposal.Base is not null,
            _ => false
        };

    private static bool HasText(string? value) => !string.IsNullOrWhiteSpace(value);

    private static SyncEventApplyResult Terminal(string? message, string? errorCode) =>
        new(false, message, errorCode, Retryable: false, Terminal: true);
}
