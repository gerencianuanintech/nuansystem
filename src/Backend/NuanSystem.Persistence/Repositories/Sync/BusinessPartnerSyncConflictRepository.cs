using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.BusinessPartners.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class BusinessPartnerSyncConflictRepository(
    ITenantConnectionFactory connectionFactory,
    ISyncEventPayloadFactory payloadFactory,
    IBusinessPartnerSyncConflictResolutionPlanner resolutionPlanner) : IBusinessPartnerSyncConflictRepository
{
    internal const string ListProcedure =
        "dbo.SP_NA_GET_BUSINESSPARTNER_SYNCCONFLICTS_LISTAR";
    internal const string GetByIdProcedure =
        "dbo.SP_NA_GET_BUSINESSPARTNER_SYNCCONFLICT_BUSCARPORID";
    internal const string ResolveProcedure =
        "dbo.SP_NA_POST_BUSINESSPARTNER_SYNCCONFLICT_RESOLVER";
    internal const string StableReferencesProcedure =
        "dbo.SP_NA_GET_BUSINESSPARTNER_STABLE_REFERENCES_RESOLVE";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyCollection<BusinessPartnerSyncConflictRecord>> ListAsync(
        int companyId,
        string? status,
        CancellationToken cancellationToken = default)
    {
        EnsureCompanyId(companyId);
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<ConflictRow>(new CommandDefinition(
            ListProcedure,
            new { Status = status },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        return rows.Select(ToRecord).ToArray();
    }

    public async Task<BusinessPartnerSyncConflictRecord?> GetByIdAsync(
        int companyId,
        long conflictId,
        CancellationToken cancellationToken = default)
    {
        EnsureCompanyId(companyId);
        using var connection = connectionFactory.CreateConnection();
        var row = await connection.QuerySingleOrDefaultAsync<ConflictRow>(new CommandDefinition(
            GetByIdProcedure,
            new { Id = conflictId },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        return row is null ? null : ToRecord(row);
    }

    public async Task<BusinessPartnerSyncConflictResolutionResult> ResolveAsync(
        BusinessPartnerSyncConflictResolutionData resolution,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(resolution);
        EnsureCompanyId(resolution.CompanyId);

        var connection = connectionFactory.CreateConnection() as SqlConnection
            ?? throw new NotSupportedException(
                "La resolucion de conflictos BusinessPartner requiere SQL Server.");
        await using (connection)
        {
            await connection.OpenAsync(cancellationToken);
            await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);
            try
            {
                var lockedRow = await connection.QuerySingleOrDefaultAsync<ConflictRow>(new CommandDefinition(
                    GetByIdProcedure,
                    new { Id = resolution.ConflictId },
                    transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));
                if (lockedRow is null)
                {
                    await transaction.CommitAsync(cancellationToken);
                    return new BusinessPartnerSyncConflictResolutionResult(
                        BusinessPartnerSyncConflictResolutionOutcome.NotFound,
                        null);
                }

                var lockedConflict = ToRecord(lockedRow);
                if (lockedConflict.Status == "Resolved")
                {
                    await transaction.CommitAsync(cancellationToken);
                    return new BusinessPartnerSyncConflictResolutionResult(
                        BusinessPartnerSyncConflictResolutionOutcome.AlreadyResolved,
                        lockedConflict);
                }

                if (!lockedConflict.RowVersion.SequenceEqual(resolution.ExpectedRowVersion))
                {
                    await transaction.CommitAsync(cancellationToken);
                    return new BusinessPartnerSyncConflictResolutionResult(
                        BusinessPartnerSyncConflictResolutionOutcome.ConcurrencyConflict,
                        null);
                }

                var current = await BusinessPartnerProposalApplyRepository.LoadCentralStateAsync(
                    connection,
                    transaction,
                    lockedConflict.BusinessPartnerGlobalId,
                    cancellationToken);
                if (current?.RowVersion is not { Length: 8 })
                {
                    await transaction.CommitAsync(cancellationToken);
                    return new BusinessPartnerSyncConflictResolutionResult(
                        BusinessPartnerSyncConflictResolutionOutcome.ConcurrencyConflict,
                        null);
                }

                var live = new BusinessPartnerSyncConflictLiveCanonicalState(
                    current.BusinessPartnerId,
                    current.CanonicalVersion,
                    current.RowVersion,
                    current.Snapshot);
                var plan = resolutionPlanner.CreatePlan(
                    resolution.CompanyId,
                    lockedConflict,
                    live,
                    resolution.Resolution,
                    resolution.Reason);
                if (plan is null)
                {
                    await transaction.CommitAsync(cancellationToken);
                    return new BusinessPartnerSyncConflictResolutionResult(
                        BusinessPartnerSyncConflictResolutionOutcome.InvalidConflictPath,
                        null);
                }

                var references = plan.ResolvedSnapshot is null
                    ? null
                    : await ResolveStableReferencesAsync(
                        connection,
                        transaction,
                        plan.ResolvedSnapshot,
                        cancellationToken);
                if (references is { IsComplete: false })
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return new BusinessPartnerSyncConflictResolutionResult(
                        BusinessPartnerSyncConflictResolutionOutcome.ReferenceNotFound,
                        null);
                }

                var sqlResult = await connection.QuerySingleAsync<ResolveResultRow>(new CommandDefinition(
                    ResolveProcedure,
                    new
                    {
                        Id = resolution.ConflictId,
                        resolution.Resolution,
                        ResolutionReason = resolution.Reason,
                        resolution.ExpectedRowVersion,
                        resolution.CompanyId,
                        plan.ExpectedBusinessPartnerId,
                        plan.ExpectedCanonicalVersion,
                        plan.ExpectedBusinessPartnerRowVersion,
                        IdentificationTypeId = references?.IdentificationTypeId,
                        ResolvedSnapshotJson = SerializeSnapshot(plan.ResolvedSnapshot),
                        AddressesJson = references?.AddressesJson ?? "[]",
                        ContactsJson = references?.ContactsJson ?? "[]",
                        OutboundEventId = plan.OutboundEvent.EventId,
                        OutboundEntityName = plan.OutboundEvent.PublishRequest.EntityName,
                        OutboundPayloadJson = payloadFactory.CreatePayloadJson(
                            plan.OutboundEvent.PublishRequest),
                        plan.OutboundEvent.TargetCompanyId,
                        resolution.AuditUserId,
                        resolution.AuditUserName
                    },
                    transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));

                if (sqlResult.ResultCode == 4)
                {
                    await transaction.CommitAsync(cancellationToken);
                    return new BusinessPartnerSyncConflictResolutionResult(
                        BusinessPartnerSyncConflictResolutionOutcome.ConcurrencyConflict,
                        null);
                }

                if (sqlResult.ResultCode == 5)
                {
                    await transaction.CommitAsync(cancellationToken);
                    return new BusinessPartnerSyncConflictResolutionResult(
                        BusinessPartnerSyncConflictResolutionOutcome.OutboundEventCollision,
                        null);
                }

                if (sqlResult.ResultCode is not (1 or 2))
                {
                    throw new InvalidOperationException(
                        $"Unexpected BusinessPartner conflict resolution result {sqlResult.ResultCode}.");
                }

                var savedRow = await connection.QuerySingleOrDefaultAsync<ConflictRow>(new CommandDefinition(
                    GetByIdProcedure,
                    new { Id = resolution.ConflictId },
                    transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));
                await transaction.CommitAsync(cancellationToken);
                return new BusinessPartnerSyncConflictResolutionResult(
                    sqlResult.ResultCode == 1
                        ? BusinessPartnerSyncConflictResolutionOutcome.Resolved
                        : BusinessPartnerSyncConflictResolutionOutcome.AlreadyResolved,
                    savedRow is null ? null : ProjectResolvedRecord(ToRecord(savedRow), live, plan));
            }
            catch
            {
                if (transaction.Connection is not null)
                {
                    await transaction.RollbackAsync(CancellationToken.None);
                }

                throw;
            }
        }
    }

    private static async Task<BusinessPartnerProposalApplyRepository.StableReferenceResolution>
        ResolveStableReferencesAsync(
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
        var identification = await grid.ReadSingleAsync<
            BusinessPartnerProposalApplyRepository.IdentificationReferenceRow>();
        var addresses = (await grid.ReadAsync<
            BusinessPartnerProposalApplyRepository.AddressReferenceRow>()).AsList();
        var contacts = (await grid.ReadAsync<
            BusinessPartnerProposalApplyRepository.ContactReferenceRow>()).AsList();
        return BusinessPartnerProposalApplyRepository.ResolveStableReferences(
            sorted,
            identification,
            addresses,
            contacts);
    }

    internal static BusinessPartnerSyncConflictRecord ToRecord(ConflictRow row) => new(
        row.Id,
        row.ProposalEventId,
        row.BusinessPartnerId,
        row.BusinessPartnerGlobalId,
        row.OriginCompanyId,
        row.BaseCanonicalVersion,
        row.CurrentCanonicalVersion,
        DeserializeOptionalSnapshot(row.BaseSnapshotJson),
        DeserializeRequiredSnapshot(row.ProposedSnapshotJson, "proposed"),
        DeserializeRequiredSnapshot(row.CanonicalSnapshotJson, "canonical"),
        DeserializeConflictFields(row.ConflictFieldsJson),
        row.Status,
        row.Resolution,
        row.ResolutionReason,
        row.CreatedByUserId,
        row.CreatedByUserName,
        row.CreatedAt,
        row.ResolvedByUserId,
        row.ResolvedByUserName,
        row.ResolvedAt,
        row.RowVersion,
        row.Code,
        row.Name);

    internal static BusinessPartnerSyncConflictRecord ProjectResolvedRecord(
        BusinessPartnerSyncConflictRecord persisted,
        BusinessPartnerSyncConflictLiveCanonicalState live,
        BusinessPartnerSyncConflictResolutionPlan plan)
    {
        var canonical = plan.ResolvedSnapshot ?? live.Snapshot;
        var canonicalVersion = plan.ResolvedSnapshot is null
            ? live.CanonicalVersion
            : checked(live.CanonicalVersion + 1);
        return persisted with
        {
            BusinessPartnerId = live.BusinessPartnerId,
            CurrentCanonicalVersion = canonicalVersion,
            Canonical = canonical,
            Code = canonical.Code,
            Name = canonical.Name
        };
    }

    private static BusinessPartnerCanonicalSnapshot DeserializeRequiredSnapshot(
        string json,
        string snapshotName) =>
        JsonSerializer.Deserialize<BusinessPartnerCanonicalSnapshot>(json, JsonOptions)
        ?? throw new InvalidOperationException(
            $"El snapshot {snapshotName} del conflicto BusinessPartner no tiene una raiz valida.");

    private static BusinessPartnerCanonicalSnapshot? DeserializeOptionalSnapshot(string? json) =>
        json is null ? null : DeserializeRequiredSnapshot(json, "base");

    private static IReadOnlyCollection<string> DeserializeConflictFields(string json)
    {
        var paths = JsonSerializer.Deserialize<string[]>(json, JsonOptions)
            ?? throw new InvalidOperationException(
                "Las rutas del conflicto BusinessPartner no tienen una raiz valida.");
        if (paths.Length == 0 || paths.Any(string.IsNullOrWhiteSpace))
        {
            throw new InvalidOperationException(
                "El conflicto BusinessPartner no contiene rutas resolubles.");
        }

        return paths;
    }

    private static string? SerializeSnapshot(BusinessPartnerCanonicalSnapshot? snapshot) =>
        snapshot is null ? null : JsonSerializer.Serialize(SortSnapshot(snapshot), JsonOptions);

    private static BusinessPartnerCanonicalSnapshot SortSnapshot(BusinessPartnerCanonicalSnapshot snapshot) =>
        snapshot with
        {
            Addresses = snapshot.Addresses.OrderBy(item => item.GlobalId).ToArray(),
            Contacts = snapshot.Contacts.OrderBy(item => item.GlobalId).ToArray()
        };

    private static void EnsureCompanyId(int companyId)
    {
        if (companyId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(companyId));
        }
    }

    internal sealed class ConflictRow
    {
        public long Id { get; init; }
        public Guid ProposalEventId { get; init; }
        public int? BusinessPartnerId { get; init; }
        public Guid BusinessPartnerGlobalId { get; init; }
        public int OriginCompanyId { get; init; }
        public long BaseCanonicalVersion { get; init; }
        public long CurrentCanonicalVersion { get; init; }
        public string? BaseSnapshotJson { get; init; }
        public string ProposedSnapshotJson { get; init; } = string.Empty;
        public string CanonicalSnapshotJson { get; init; } = string.Empty;
        public string ConflictFieldsJson { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string? Resolution { get; init; }
        public string? ResolutionReason { get; init; }
        public int? CreatedByUserId { get; init; }
        public string? CreatedByUserName { get; init; }
        public DateTime CreatedAt { get; init; }
        public int? ResolvedByUserId { get; init; }
        public string? ResolvedByUserName { get; init; }
        public DateTime? ResolvedAt { get; init; }
        public byte[] RowVersion { get; init; } = [];
        public string? Code { get; init; }
        public string? Name { get; init; }
    }

    private sealed class ResolveResultRow
    {
        public int ResultCode { get; init; }
        public long ConflictId { get; init; }
    }
}
