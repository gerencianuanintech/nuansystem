using System.Security.Cryptography;
using System.Text;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.BusinessPartners.SyncConflicts;

public sealed class BusinessPartnerSyncConflictResolutionPlanner
    : IBusinessPartnerSyncConflictResolutionPlanner
{
    public BusinessPartnerSyncConflictResolutionPlan? CreatePlan(
        int companyId,
        BusinessPartnerSyncConflictRecord conflict,
        BusinessPartnerSyncConflictLiveCanonicalState live,
        string resolution,
        string reason)
    {
        ArgumentNullException.ThrowIfNull(conflict);
        ArgumentNullException.ThrowIfNull(live);

        BusinessPartnerCanonicalSnapshot? resolvedSnapshot = null;
        BusinessPartnerSyncConflictOutboundEvent outbound;
        if (resolution == "AcceptBranch")
        {
            if (!BusinessPartnerSyncConflictPaths.TryApply(
                    live.Snapshot,
                    conflict.Proposed,
                    conflict.ConflictFields,
                    out resolvedSnapshot))
            {
                return null;
            }

            var canonicalVersion = checked(live.CanonicalVersion + 1);
            var payload = new BusinessPartnerCanonicalPayloadV2(
                BusinessPartnerSyncSchemaVersions.Canonical,
                canonicalVersion,
                conflict.OriginCompanyId,
                conflict.ProposalEventId,
                resolvedSnapshot);
            outbound = CreateOutbound(
                companyId,
                conflict,
                "AcceptBranch",
                targetCompanyId: null,
                SyncMasterBranchEntityCodes.BusinessPartner,
                payload);
        }
        else if (resolution == "KeepCentral")
        {
            var payload = new BusinessPartnerProposalResultPayloadV1(
                BusinessPartnerSyncSchemaVersions.ProposalResult,
                conflict.BusinessPartnerGlobalId,
                conflict.ProposalEventId,
                conflict.OriginCompanyId,
                "Rejected",
                reason,
                live.CanonicalVersion,
                live.Snapshot);
            outbound = CreateOutbound(
                companyId,
                conflict,
                "KeepCentral",
                conflict.OriginCompanyId,
                SyncMasterBranchEntityCodes.BusinessPartnerProposalResult,
                payload);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
        }

        return new BusinessPartnerSyncConflictResolutionPlan(
            live.BusinessPartnerId,
            live.CanonicalVersion,
            live.RowVersion,
            resolvedSnapshot,
            outbound);
    }

    private static BusinessPartnerSyncConflictOutboundEvent CreateOutbound(
        int companyId,
        BusinessPartnerSyncConflictRecord conflict,
        string eventKind,
        int? targetCompanyId,
        string entityName,
        object payload) => new(
        CreateDeterministicEventId(conflict.ProposalEventId, eventKind),
        targetCompanyId,
        conflict.ProposalEventId,
        new SyncPublishRequest(
            companyId,
            entityName,
            conflict.BusinessPartnerGlobalId,
            EntityCode: null,
            SyncOperation.Updated,
            payload,
            SourceSystem: null,
            SourceReference: conflict.ProposalEventId.ToString("D")));

    internal static Guid CreateDeterministicEventId(Guid proposalEventId, string eventKind)
    {
        var input = proposalEventId.ToByteArray().Concat(Encoding.UTF8.GetBytes(eventKind)).ToArray();
        return new Guid(SHA256.HashData(input).AsSpan(0, 16));
    }
}
