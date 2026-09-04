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

        if (proposal.SchemaVersion != BusinessPartnerSyncSchemaVersions.Proposal)
        {
            return Terminal("Schema de propuesta no soportado.", "BP_SYNC_PROPOSAL_SCHEMA_UNSUPPORTED");
        }

        if (proposal.Proposed is null ||
            proposal.Proposed.Addresses is null ||
            proposal.Proposed.Contacts is null ||
            proposal.ChangedFields is null ||
            proposal.Base is { Addresses: null } or { Contacts: null })
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
        if (!document.RootElement.TryGetProperty("payload", out var payloadElement))
        {
            throw new JsonException("El evento no contiene payload.");
        }

        return payloadElement.Deserialize<BusinessPartnerProposalPayloadV1>(JsonOptions)
            ?? throw new JsonException("La propuesta no pudo deserializarse.");
    }

    private static SyncEventApplyResult Terminal(string? message, string? errorCode) =>
        new(false, message, errorCode, Retryable: false, Terminal: true);
}
