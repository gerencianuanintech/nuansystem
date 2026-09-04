using NuanSystem.Application.Features.BusinessPartners.Policies;

namespace NuanSystem.Application.Features.BusinessPartners.Sync;

public enum BusinessPartnerProposalApplyOutcome
{
    Accepted,
    Rejected,
    Conflict,
    Duplicate,
    RetryableFailure,
    TerminalFailure
}

public sealed record BusinessPartnerProposalApplyResult(
    BusinessPartnerProposalApplyOutcome Outcome,
    long CanonicalVersion,
    string? Message = null,
    string? ErrorCode = null);

public sealed record BusinessPartnerProposalCentralState(
    int BusinessPartnerId,
    long CanonicalVersion,
    BusinessPartnerCanonicalSnapshot Snapshot,
    byte[]? RowVersion = null);

public sealed record BusinessPartnerProposalDecision(
    BusinessPartnerProposalApplyOutcome Outcome,
    long CanonicalVersion,
    BusinessPartnerCanonicalSnapshot? Canonical,
    IReadOnlyCollection<string> ConflictFields,
    string? ErrorCode = null,
    string? Message = null);

public static class BusinessPartnerProposalReconciliationPolicy
{
    public static BusinessPartnerProposalDecision Evaluate(
        BusinessPartnerProposalPayloadV1 proposal,
        BusinessPartnerProposalCentralState? current,
        bool sameRoleIdentificationExists,
        BusinessPartnerSapCodePolicyData? sapPolicy,
        bool stableReferencesAvailable)
    {
        ArgumentNullException.ThrowIfNull(proposal);

        if (proposal.PartnerType is not "Customer" and not "Supplier" ||
            proposal.Proposed.PartnerType != proposal.PartnerType)
        {
            return Rejected(current, "BP_ROLE_INVALID", "El rol debe ser Customer o Supplier.");
        }

        if (proposal.GlobalId == Guid.Empty ||
            proposal.Proposed.GlobalId != proposal.GlobalId ||
            proposal.Base is not null && proposal.Base.GlobalId != proposal.GlobalId)
        {
            return Rejected(current, "BP_SYNC_GLOBAL_ID_MISMATCH", "La identidad global de la propuesta no coincide.");
        }

        var normalized = BusinessPartnerIdentityPolicy.NormalizeIdentification(proposal.IdentificationNumber);
        if (!string.Equals(normalized, proposal.NormalizedIdentificationNumber, StringComparison.Ordinal) ||
            !string.Equals(normalized, proposal.Proposed.NormalizedIdentificationNumber, StringComparison.Ordinal) ||
            !string.Equals(proposal.IdentificationNumber, proposal.Proposed.IdentificationNumber, StringComparison.Ordinal) ||
            !string.Equals(proposal.IdentificationTypeCode, proposal.Proposed.IdentificationTypeCode, StringComparison.Ordinal))
        {
            return Rejected(
                current,
                "BP_NORMALIZED_IDENTIFICATION_MISMATCH",
                "La identificacion normalizada no coincide con la politica central.");
        }

        if (!HasStableChildIdentities(proposal.Proposed))
        {
            return Rejected(
                current,
                "BP_CHILD_GLOBAL_ID_INVALID",
                "Direcciones y contactos requieren GlobalId no vacio y unico.");
        }

        if (!stableReferencesAvailable)
        {
            return new BusinessPartnerProposalDecision(
                BusinessPartnerProposalApplyOutcome.RetryableFailure,
                current?.CanonicalVersion ?? 0,
                current?.Snapshot,
                [],
                "BP_SYNC_REFERENCE_NOT_FOUND",
                "Una referencia estable requerida aun no existe en el tenant central.");
        }

        if (sameRoleIdentificationExists)
        {
            return Rejected(
                current,
                "BP_IDENTIFICATION_DUPLICATE",
                "La identificacion ya pertenece a otro socio activo del mismo rol.");
        }

        if (sapPolicy is null)
        {
            return new BusinessPartnerProposalDecision(
                BusinessPartnerProposalApplyOutcome.RetryableFailure,
                current?.CanonicalVersion ?? 0,
                current?.Snapshot,
                [],
                "BP_SAP_CODE_POLICY_REQUIRED",
                "La politica central de codigo SAP no esta disponible.");
        }

        var confirmedSapCardCode = string.IsNullOrWhiteSpace(current?.Snapshot.SapCardCode)
            ? null
            : current.Snapshot.SapCardCode;
        var calculatedSapCardCode = confirmedSapCardCode is null
            ? BusinessPartnerSapCardCodePolicy.CreateSapCardCode(
                sapPolicy,
                proposal.PartnerType,
                proposal.IdentificationTypeCode,
                normalized)
            : null;
        if (calculatedSapCardCode is { IsSuccess: false })
        {
            var error = calculatedSapCardCode.Errors.Single();
            return Rejected(current, error.Code, error.Message);
        }

        var sapCardCode = confirmedSapCardCode ?? calculatedSapCardCode!.Value;

        if (proposal.BaseCanonicalVersion == 0)
        {
            if (proposal.Base is not null)
            {
                return Rejected(
                    current,
                    "BP_SYNC_CREATE_BASE_NOT_ALLOWED",
                    "Una propuesta de alta no puede incluir snapshot base.");
            }

            if (current is not null)
            {
                return Conflict(current, "BP_SYNC_CREATE_IDENTITY_CONFLICT");
            }

            if (proposal.Proposed.IsActive ||
                !string.IsNullOrWhiteSpace(proposal.Proposed.SapCardCode))
            {
                return Rejected(
                    null,
                    "BP_PROTECTED_FIELD",
                    "Una propuesta de alta no puede definir activacion ni codigo SAP.");
            }

            var expectedCode = BusinessPartnerIdentityPolicy.CreateInternalCode(proposal.GlobalId);
            if (!string.Equals(expectedCode, proposal.Code, StringComparison.Ordinal) ||
                !string.Equals(expectedCode, proposal.Proposed.Code, StringComparison.Ordinal))
            {
                return Rejected(
                    current,
                    "BP_INTERNAL_CODE_MISMATCH",
                    "El codigo interno no coincide con la identidad global.");
            }

            return new BusinessPartnerProposalDecision(
                BusinessPartnerProposalApplyOutcome.Accepted,
                1,
                proposal.Proposed with { IsActive = true, SapCardCode = sapCardCode },
                []);
        }

        if (proposal.Base is null)
        {
            return Rejected(current, "BP_SYNC_BASE_REQUIRED", "Una actualizacion requiere snapshot base.");
        }

        if (current is null)
        {
            return Rejected(null, "BP_SYNC_CANONICAL_NOT_FOUND", "No existe el socio canonico a actualizar.");
        }

        var merge = new BusinessPartnerThreeWayMergeService().Merge(
            proposal.Base,
            proposal.Proposed,
            current.Snapshot);
        if (merge.Status == BusinessPartnerMergeStatus.Rejected)
        {
            return Rejected(current, merge.ErrorCode ?? "BP_PROTECTED_FIELD", "La propuesta modifica un campo protegido.");
        }

        if (merge.Status == BusinessPartnerMergeStatus.Conflict)
        {
            return new BusinessPartnerProposalDecision(
                BusinessPartnerProposalApplyOutcome.Conflict,
                current.CanonicalVersion,
                current.Snapshot,
                merge.ConflictFields,
                "BP_SYNC_CONFLICT",
                "La propuesta diverge del estado central en las mismas rutas.");
        }

        var merged = merge.Merged!;
        return new BusinessPartnerProposalDecision(
            BusinessPartnerProposalApplyOutcome.Accepted,
            checked(current.CanonicalVersion + 1),
            merged with
            {
                SapCardCode = sapCardCode
            },
            []);
    }

    private static bool HasStableChildIdentities(BusinessPartnerCanonicalSnapshot snapshot) =>
        HasStableIdentities(snapshot.Addresses.Select(item => item.GlobalId)) &&
        HasStableIdentities(snapshot.Contacts.Select(item => item.GlobalId));

    private static bool HasStableIdentities(IEnumerable<Guid> values)
    {
        var ids = values.ToArray();
        return ids.All(id => id != Guid.Empty) && ids.Distinct().Count() == ids.Length;
    }

    private static BusinessPartnerProposalDecision Rejected(
        BusinessPartnerProposalCentralState? current,
        string errorCode,
        string message) =>
        new(
            BusinessPartnerProposalApplyOutcome.Rejected,
            current?.CanonicalVersion ?? 0,
            current?.Snapshot,
            [],
            errorCode,
            message);

    private static BusinessPartnerProposalDecision Conflict(
        BusinessPartnerProposalCentralState? current,
        string errorCode) =>
        new(
            BusinessPartnerProposalApplyOutcome.Conflict,
            current?.CanonicalVersion ?? 0,
            current?.Snapshot,
            ["GlobalId"],
            errorCode,
            "La identidad de alta ya existe en el canonico central.");
}
