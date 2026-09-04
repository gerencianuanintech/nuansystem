using System.Security.Cryptography;
using System.Text;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.BusinessPartners.SyncConflicts;

public sealed class ResolveBusinessPartnerSyncConflictCommandHandler(
    ICompanyContext companyContext,
    IBusinessPartnerSyncConflictRepository repository)
    : ICommandHandler<ResolveBusinessPartnerSyncConflictCommand, BusinessPartnerSyncConflictDto>
{
    public async Task<Result<BusinessPartnerSyncConflictDto>> Handle(
        ResolveBusinessPartnerSyncConflictCommand request,
        CancellationToken cancellationToken)
    {
        var company = BusinessPartnerSyncConflictCompanyGuard.RequireCentral(companyContext);
        if (company is null)
        {
            return BusinessPartnerSyncConflictErrors.CompanyRequired<BusinessPartnerSyncConflictDto>();
        }

        if (!company.IsMaster)
        {
            return BusinessPartnerSyncConflictErrors.MasterRequired<BusinessPartnerSyncConflictDto>();
        }

        if (!ResolveBusinessPartnerSyncConflictCommandValidator.TryDecodeRowVersion(
                request.ExpectedRowVersion,
                out var expectedRowVersion))
        {
            return Invalid(
                "BP_SYNC_CONFLICT_ROW_VERSION_INVALID",
                "ExpectedRowVersion debe ser un rowversion base64 valido.",
                nameof(request.ExpectedRowVersion));
        }

        if (request.Resolution is not "AcceptBranch" and not "KeepCentral")
        {
            return Invalid(
                "BP_SYNC_CONFLICT_RESOLUTION_INVALID",
                "La resolucion debe ser AcceptBranch o KeepCentral.",
                nameof(request.Resolution));
        }

        var conflict = await repository.GetByIdAsync(company.CompanyId, request.ConflictId, cancellationToken);
        if (conflict is null)
        {
            return Invalid(
                "BP_SYNC_CONFLICT_NOT_FOUND",
                "No se encontro el conflicto de socio solicitado.",
                nameof(request.ConflictId));
        }

        if (conflict.Status == "Resolved")
        {
            return Result<BusinessPartnerSyncConflictDto>.Success(
                BusinessPartnerSyncConflictMapper.ToDto(conflict),
                "El conflicto ya habia sido resuelto.");
        }

        var reason = request.Reason.Trim();
        BusinessPartnerCanonicalSnapshot? resolvedSnapshot = null;
        BusinessPartnerSyncConflictOutboundEvent outbound;
        if (request.Resolution == "AcceptBranch")
        {
            if (!BusinessPartnerSyncConflictPaths.TryApply(
                    conflict.Canonical,
                    conflict.Proposed,
                    conflict.ConflictFields,
                    out resolvedSnapshot))
            {
                return Invalid(
                    "BP_SYNC_CONFLICT_PATH_INVALID",
                    "El conflicto contiene una ruta que no puede resolverse de forma segura.",
                    "ConflictFields");
            }

            var canonicalVersion = checked(conflict.CurrentCanonicalVersion + 1);
            var payload = new BusinessPartnerCanonicalPayloadV2(
                BusinessPartnerSyncSchemaVersions.Canonical,
                canonicalVersion,
                conflict.OriginCompanyId,
                conflict.ProposalEventId,
                resolvedSnapshot);
            outbound = CreateOutbound(
                company.CompanyId,
                conflict,
                "AcceptBranch",
                targetCompanyId: null,
                SyncMasterBranchEntityCodes.BusinessPartner,
                SyncOperation.Updated,
                entityCode: null,
                payload);
        }
        else
        {
            var payload = new BusinessPartnerProposalResultPayloadV1(
                BusinessPartnerSyncSchemaVersions.ProposalResult,
                conflict.BusinessPartnerGlobalId,
                conflict.ProposalEventId,
                conflict.OriginCompanyId,
                "Rejected",
                reason,
                conflict.CurrentCanonicalVersion,
                conflict.Canonical);
            outbound = CreateOutbound(
                company.CompanyId,
                conflict,
                "KeepCentral",
                conflict.OriginCompanyId,
                SyncMasterBranchEntityCodes.BusinessPartnerProposalResult,
                SyncOperation.Updated,
                entityCode: null,
                payload);
        }

        var resolution = await repository.ResolveAsync(
            new BusinessPartnerSyncConflictResolutionData(
                company.CompanyId,
                conflict.Id,
                request.Resolution,
                reason,
                expectedRowVersion,
                resolvedSnapshot,
                outbound,
                request.AuditUserId,
                Clean(request.AuditUserName)),
            cancellationToken);

        return resolution.Outcome switch
        {
            BusinessPartnerSyncConflictResolutionOutcome.Resolved => Result<BusinessPartnerSyncConflictDto>.Success(
                BusinessPartnerSyncConflictMapper.ToDto(resolution.Conflict
                    ?? throw new InvalidOperationException("La resolucion no devolvio el conflicto persistido.")),
                "Conflicto de socio resuelto correctamente."),
            BusinessPartnerSyncConflictResolutionOutcome.AlreadyResolved => Result<BusinessPartnerSyncConflictDto>.Success(
                BusinessPartnerSyncConflictMapper.ToDto(resolution.Conflict
                    ?? throw new InvalidOperationException("La resolucion idempotente no devolvio el conflicto persistido.")),
                "El conflicto ya habia sido resuelto."),
            BusinessPartnerSyncConflictResolutionOutcome.NotFound => Invalid(
                "BP_SYNC_CONFLICT_NOT_FOUND",
                "No se encontro el conflicto de socio solicitado.",
                nameof(request.ConflictId)),
            BusinessPartnerSyncConflictResolutionOutcome.ConcurrencyConflict => Invalid(
                "BP_SYNC_CONFLICT_CONCURRENCY_CONFLICT",
                "El conflicto o el socio central fue modificado. Recargue e intente nuevamente.",
                nameof(request.ExpectedRowVersion)),
            BusinessPartnerSyncConflictResolutionOutcome.ReferenceNotFound => Invalid(
                "BP_SYNC_REFERENCE_NOT_FOUND",
                "Una referencia estable requerida ya no existe en el tenant central.",
                "ConflictFields"),
            BusinessPartnerSyncConflictResolutionOutcome.OutboundEventCollision => Invalid(
                "BP_SYNC_EVENT_ID_COLLISION",
                "El identificador del evento de salida ya pertenece a otro sobre.",
                "EventId"),
            _ => throw new InvalidOperationException($"Resultado de resolucion no soportado: {resolution.Outcome}.")
        };
    }

    private static BusinessPartnerSyncConflictOutboundEvent CreateOutbound(
        int companyId,
        BusinessPartnerSyncConflictRecord conflict,
        string eventKind,
        int? targetCompanyId,
        string entityName,
        SyncOperation operation,
        string? entityCode,
        object payload) => new(
        CreateDeterministicEventId(conflict.ProposalEventId, eventKind),
        targetCompanyId,
        conflict.ProposalEventId,
        new SyncPublishRequest(
            companyId,
            entityName,
            conflict.BusinessPartnerGlobalId,
            entityCode,
            operation,
            payload,
            SourceSystem: null,
            SourceReference: conflict.ProposalEventId.ToString("D")));

    internal static Guid CreateDeterministicEventId(Guid proposalEventId, string eventKind)
    {
        var input = proposalEventId.ToByteArray().Concat(Encoding.UTF8.GetBytes(eventKind)).ToArray();
        return new Guid(SHA256.HashData(input).AsSpan(0, 16));
    }

    private static Result<BusinessPartnerSyncConflictDto> Invalid(
        string code,
        string message,
        string field) => Result<BusinessPartnerSyncConflictDto>.Failure(
        "No fue posible resolver el conflicto de socio.",
        [new ApiError(code, message, field)]);

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
