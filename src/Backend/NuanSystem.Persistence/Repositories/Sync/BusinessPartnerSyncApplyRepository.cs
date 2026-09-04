using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class BusinessPartnerSyncApplyRepository(ICompanyResolver companyResolver)
    : IBusinessPartnerSyncApplyRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    internal const string StableReferencesProcedure =
        "dbo.SP_NA_GET_BUSINESSPARTNER_STABLE_REFERENCES_RESOLVE";
    internal const string BranchApplyPreflightProcedure =
        "dbo.SP_NA_POST_BUSINESSPARTNER_BRANCH_APPLY_PREFLIGHT";
    internal const string CanonicalApplyProcedure =
        "dbo.SP_NA_POST_BUSINESSPARTNER_CANONICAL_APPLY";
    internal const string ProposalResultApplyProcedure =
        "dbo.SP_NA_POST_BUSINESSPARTNER_PROPOSAL_RESULT_APPLY";

    public async Task<BusinessPartnerSyncApplyResult> ApplyCanonicalAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        BusinessPartnerCanonicalPayloadV2 payload,
        CancellationToken cancellationToken = default)
    {
        var company = await ResolveBranchAsync(branchCompanyId, cancellationToken);
        await using var connection = CreateSqlConnection(company);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        try
        {
            var preflightRow = await connection.QuerySingleAsync<ApplyResultRow>(new CommandDefinition(
                BranchApplyPreflightProcedure,
                CreateCanonicalPreflightParameters(context, payload),
                transaction,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
            var preflightResult = MapCanonicalPreflightResult(preflightRow);
            if (preflightResult is not null)
            {
                await transaction.CommitAsync(cancellationToken);
                return preflightResult;
            }

            var references = await ResolveStableReferencesAsync(
                connection,
                transaction,
                payload.Partner,
                cancellationToken);
            if (!references.IsComplete)
            {
                await transaction.RollbackAsync(cancellationToken);
                return MissingReference();
            }

            var row = await connection.QuerySingleAsync<ApplyResultRow>(new CommandDefinition(
                CanonicalApplyProcedure,
                CreateCanonicalParameters(context, payload, references),
                transaction,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
            await transaction.CommitAsync(cancellationToken);
            return MapCanonicalResult(row);
        }
        catch
        {
            if (transaction.Connection is not null)
                await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task<BusinessPartnerSyncApplyResult> ApplyProposalResultAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        BusinessPartnerProposalResultPayloadV1 payload,
        CancellationToken cancellationToken = default)
    {
        var company = await ResolveBranchAsync(branchCompanyId, cancellationToken);
        await using var connection = CreateSqlConnection(company);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        try
        {
            var preflightRow = await connection.QuerySingleAsync<ApplyResultRow>(new CommandDefinition(
                BranchApplyPreflightProcedure,
                CreateProposalResultPreflightParameters(context, payload),
                transaction,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
            var preflightResult = MapProposalResultPreflightResult(preflightRow);
            if (preflightResult is not null)
            {
                await transaction.CommitAsync(cancellationToken);
                return preflightResult;
            }

            StableReferenceResolution? references = null;
            if (payload.Status == "Rejected" && payload.Canonical is not null)
            {
                references = await ResolveStableReferencesAsync(
                    connection,
                    transaction,
                    payload.Canonical,
                    cancellationToken);
                if (!references.IsComplete)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return MissingReference();
                }
            }

            var row = await connection.QuerySingleAsync<ApplyResultRow>(new CommandDefinition(
                ProposalResultApplyProcedure,
                CreateProposalResultParameters(context, payload, references),
                transaction,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
            await transaction.CommitAsync(cancellationToken);
            return MapProposalResult(row);
        }
        catch
        {
            if (transaction.Connection is not null)
                await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    private static async Task<StableReferenceResolution> ResolveStableReferencesAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        BusinessPartnerCanonicalSnapshot snapshot,
        CancellationToken cancellationToken)
    {
        var sorted = SortSnapshot(snapshot);
        using var grid = await connection.QueryMultipleAsync(new CommandDefinition(
            StableReferencesProcedure,
            new
            {
                sorted.IdentificationTypeCode,
                AddressesJson = JsonSerializer.Serialize(sorted.Addresses, JsonOptions),
                ContactsJson = JsonSerializer.Serialize(sorted.Contacts, JsonOptions)
            },
            transaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        var identification = await grid.ReadSingleAsync<BusinessPartnerProposalApplyRepository.IdentificationReferenceRow>();
        var addresses = (await grid.ReadAsync<BusinessPartnerProposalApplyRepository.AddressReferenceRow>()).AsList();
        var contacts = (await grid.ReadAsync<BusinessPartnerProposalApplyRepository.ContactReferenceRow>()).AsList();
        var resolved = BusinessPartnerProposalApplyRepository.ResolveStableReferences(
            sorted,
            identification,
            addresses,
            contacts);
        return new StableReferenceResolution(
            resolved.IsComplete,
            resolved.IdentificationTypeId,
            resolved.AddressesJson,
            resolved.ContactsJson);
    }

    internal static CanonicalApplyParameters CreateCanonicalParameters(
        SyncEventApplyContext context,
        BusinessPartnerCanonicalPayloadV2 payload,
        StableReferenceResolution references)
    {
        var partner = SortSnapshot(payload.Partner);
        var deleted = context.Operation == "Deleted";
        return new CanonicalApplyParameters(
            context.EventId,
            context.SourceCompanyId,
            partner.GlobalId,
            context.Operation,
            context.PayloadJson,
            partner.Code,
            partner.Name,
            partner.CommercialName,
            partner.PartnerType,
            references.IdentificationTypeId
                ?? throw new InvalidOperationException("Identification type reference is required."),
            partner.IdentificationNumber,
            partner.NormalizedIdentificationNumber,
            partner.Email,
            partner.Phone,
            partner.SapCardCode,
            payload.CanonicalVersion,
            !deleted && context.Operation != "Disabled" && partner.IsActive,
            deleted,
            references.AddressesJson
                ?? throw new InvalidOperationException("Resolved addresses are required."),
            references.ContactsJson
                ?? throw new InvalidOperationException("Resolved contacts are required."));
    }

    internal static PreflightParameters CreateCanonicalPreflightParameters(
        SyncEventApplyContext context,
        BusinessPartnerCanonicalPayloadV2 payload) =>
        new(
            context.EventId,
            context.SourceCompanyId,
            "BusinessPartner",
            context.EntityGlobalId,
            context.Operation,
            context.PayloadJson,
            payload.CanonicalVersion,
            CompareCanonicalVersion: true,
            EqualVersionIsReplay: true);

    internal static PreflightParameters CreateProposalResultPreflightParameters(
        SyncEventApplyContext context,
        BusinessPartnerProposalResultPayloadV1 payload) =>
        new(
            context.EventId,
            context.SourceCompanyId,
            "BusinessPartnerProposalResult",
            context.EntityGlobalId,
            "Updated",
            context.PayloadJson,
            payload.CanonicalVersion,
            CompareCanonicalVersion: payload.Status != "Accepted",
            EqualVersionIsReplay: false);

    internal static ProposalResultApplyParameters CreateProposalResultParameters(
        SyncEventApplyContext context,
        BusinessPartnerProposalResultPayloadV1 payload,
        StableReferenceResolution? references)
    {
        var restoreCanonical = payload.Status == "Rejected" && payload.Canonical is not null;
        var canonical = restoreCanonical ? SortSnapshot(payload.Canonical!) : null;
        var hasCanonicalVersion = payload.Status != "Accepted" && payload.Canonical is not null;
        return new ProposalResultApplyParameters(
            context.EventId,
            context.SourceCompanyId,
            payload.GlobalId,
            context.PayloadJson,
            payload.Status,
            payload.Message,
            payload.CanonicalVersion,
            hasCanonicalVersion,
            canonical?.Code,
            canonical?.Name,
            canonical?.CommercialName,
            canonical?.PartnerType,
            restoreCanonical
                ? references?.IdentificationTypeId
                    ?? throw new InvalidOperationException("Identification type reference is required.")
                : null,
            canonical?.IdentificationNumber,
            canonical?.NormalizedIdentificationNumber,
            canonical?.Email,
            canonical?.Phone,
            canonical?.SapCardCode,
            canonical?.IsActive ?? true,
            IsDeleted: false,
            restoreCanonical ? references!.AddressesJson! : "[]",
            restoreCanonical ? references!.ContactsJson! : "[]");
    }

    internal static BusinessPartnerSyncApplyResult MapCanonicalResult(ApplyResultRow row) =>
        row.ResultCode switch
        {
            1 => new(true, false, row.BusinessPartnerId, "Canonico de socio aplicado."),
            2 => new(true, true, row.BusinessPartnerId, "Canonico de socio ya aplicado."),
            3 => new(true, true, row.BusinessPartnerId, "Canonico anterior ignorado.", Ignored: true),
            4 => new(false, false, row.BusinessPartnerId, "El EventId ya pertenece a otro sobre.",
                "BP_SYNC_EVENT_ID_COLLISION", Terminal: true),
            5 => new(false, false, row.BusinessPartnerId, "El canonico contradice identidad inmutable local.",
                "BP_SYNC_CANONICAL_IDENTITY_CONFLICT", Terminal: true),
            _ => throw new InvalidOperationException($"Unexpected canonical apply result {row.ResultCode}.")
        };

    internal static BusinessPartnerSyncApplyResult? MapCanonicalPreflightResult(ApplyResultRow row) =>
        row.ResultCode switch
        {
            0 => null,
            2 => new(true, true, row.BusinessPartnerId, "Canonico de socio ya aplicado."),
            3 => new(true, true, row.BusinessPartnerId, "Canonico anterior ignorado.", Ignored: true),
            4 => new(false, false, row.BusinessPartnerId, "El EventId ya pertenece a otro sobre.",
                "BP_SYNC_EVENT_ID_COLLISION", Terminal: true),
            _ => throw new InvalidOperationException($"Unexpected canonical preflight result {row.ResultCode}.")
        };

    internal static BusinessPartnerSyncApplyResult MapProposalResult(ApplyResultRow row) =>
        row.ResultCode switch
        {
            1 => new(true, false, row.BusinessPartnerId, "Resultado de propuesta aplicado."),
            2 => new(true, true, row.BusinessPartnerId, "Resultado de propuesta ya aplicado."),
            3 => new(true, true, row.BusinessPartnerId, "Resultado canonico anterior ignorado.", Ignored: true),
            4 => new(false, false, row.BusinessPartnerId, "Resultado de propuesta no aplicable.",
                "BP_SYNC_RESULT_INVALID", Terminal: true),
            5 => new(false, false, row.BusinessPartnerId, "El resultado contradice identidad inmutable local.",
                "BP_SYNC_RESULT_IDENTITY_CONFLICT", Terminal: true),
            6 => new(false, false, row.BusinessPartnerId, "El EventId ya pertenece a otro sobre.",
                "BP_SYNC_EVENT_ID_COLLISION", Terminal: true),
            _ => throw new InvalidOperationException($"Unexpected proposal result apply result {row.ResultCode}.")
        };

    internal static BusinessPartnerSyncApplyResult? MapProposalResultPreflightResult(ApplyResultRow row) =>
        row.ResultCode switch
        {
            0 => null,
            2 => new(true, true, row.BusinessPartnerId, "Resultado de propuesta ya aplicado."),
            3 => new(true, true, row.BusinessPartnerId, "Resultado canonico anterior ignorado.", Ignored: true),
            4 => new(false, false, row.BusinessPartnerId, "El EventId ya pertenece a otro sobre.",
                "BP_SYNC_EVENT_ID_COLLISION", Terminal: true),
            _ => throw new InvalidOperationException($"Unexpected proposal result preflight result {row.ResultCode}.")
        };

    private async Task<CompanyConnectionInfo> ResolveBranchAsync(
        int branchCompanyId,
        CancellationToken cancellationToken)
    {
        var company = await companyResolver.ResolveByIdAsync(branchCompanyId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontro la sucursal destino {branchCompanyId}.");
        if (company.IsMaster)
            throw new InvalidOperationException($"La empresa {branchCompanyId} no es una sucursal.");
        return company;
    }

    private static SqlConnection CreateSqlConnection(CompanyConnectionInfo company) =>
        company.DatabaseEngine == DatabaseEngine.SqlServer
            ? new SqlConnection(company.ConnectionString)
            : throw new NotSupportedException(
                $"El motor {company.DatabaseEngine} no esta implementado para Sync BusinessPartner.");

    private static BusinessPartnerCanonicalSnapshot SortSnapshot(BusinessPartnerCanonicalSnapshot snapshot) =>
        snapshot with
        {
            Addresses = snapshot.Addresses.OrderBy(item => item.GlobalId).ToArray(),
            Contacts = snapshot.Contacts.OrderBy(item => item.GlobalId).ToArray()
        };

    private static BusinessPartnerSyncApplyResult MissingReference() =>
        new(false, false, null, "Una referencia estable requerida aun no existe en la sucursal.",
            "BP_SYNC_REFERENCE_NOT_FOUND", Retryable: true);

    internal sealed class ApplyResultRow
    {
        public int ResultCode { get; init; }
        public int? BusinessPartnerId { get; init; }
    }

    internal sealed record StableReferenceResolution(
        bool IsComplete,
        int? IdentificationTypeId,
        string? AddressesJson,
        string? ContactsJson);

    internal sealed record PreflightParameters(
        Guid EventId,
        int SourceCompanyId,
        string EntityName,
        Guid EntityGlobalId,
        string Operation,
        string PayloadJson,
        long CanonicalVersion,
        bool CompareCanonicalVersion,
        bool EqualVersionIsReplay);

    internal sealed record CanonicalApplyParameters(
        Guid EventId,
        int SourceCompanyId,
        Guid EntityGlobalId,
        string Operation,
        string PayloadJson,
        string Code,
        string Name,
        string? CommercialName,
        string PartnerType,
        int IdentificationTypeId,
        string IdentificationNumber,
        string NormalizedIdentificationNumber,
        string? Email,
        string? Phone,
        string? SapCardCode,
        long CanonicalVersion,
        bool IsActive,
        bool IsDeleted,
        string AddressesJson,
        string ContactsJson);

    internal sealed record ProposalResultApplyParameters(
        Guid EventId,
        int SourceCompanyId,
        Guid EntityGlobalId,
        string PayloadJson,
        string Status,
        string? Message,
        long CanonicalVersion,
        bool HasCanonical,
        string? Code,
        string? Name,
        string? CommercialName,
        string? PartnerType,
        int? IdentificationTypeId,
        string? IdentificationNumber,
        string? NormalizedIdentificationNumber,
        string? Email,
        string? Phone,
        string? SapCardCode,
        bool IsActive,
        bool IsDeleted,
        string AddressesJson,
        string ContactsJson);
}
