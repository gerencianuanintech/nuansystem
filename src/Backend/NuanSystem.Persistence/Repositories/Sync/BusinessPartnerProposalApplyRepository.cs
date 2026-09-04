using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Policies;
using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class BusinessPartnerProposalApplyRepository(
    ICompanyResolver companyResolver,
    IBusinessPartnerSapCodePolicyRepository sapCodePolicyRepository,
    ISyncEventPayloadFactory payloadFactory) : IBusinessPartnerProposalApplyRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    internal const string InboxEnsureProcedure =
        "dbo.SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE";
    internal const string StableReferencesProcedure =
        "dbo.SP_NA_GET_BUSINESSPARTNER_STABLE_REFERENCES_RESOLVE";
    internal const string CanonicalForUpdateProcedure =
        "dbo.SP_NA_GET_BUSINESSPARTNER_CANONICAL_FORUPDATE";
    internal const string IdentificationExistsProcedure =
        "dbo.SP_NA_GET_BUSINESSPARTNERS_BUSCARPORIDENTIFICACION";
    internal const string AcceptProcedure =
        "dbo.SP_NA_POST_BUSINESSPARTNER_PROPOSAL_ACCEPT";
    internal const string ConflictProcedure =
        "dbo.SP_NA_POST_BUSINESSPARTNER_PROPOSAL_CONFLICT";
    internal const string RejectProcedure =
        "dbo.SP_NA_POST_BUSINESSPARTNER_PROPOSAL_REJECT";

    public async Task<BusinessPartnerProposalApplyResult> ApplyAsync(
        int centralCompanyId,
        SyncEventApplyContext context,
        BusinessPartnerProposalPayloadV1 proposal,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(proposal);

        var company = await companyResolver.ResolveByIdAsync(centralCompanyId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontro la empresa central {centralCompanyId}.");
        if (!company.IsMaster)
        {
            throw new InvalidOperationException($"La empresa {centralCompanyId} no es una empresa central.");
        }

        await using var connection = CreateSqlConnection(company);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        try
        {
            var earlyInboxResult = await EnsureInboxAsync(
                connection,
                transaction,
                context,
                cancellationToken);
            if (earlyInboxResult is not null)
            {
                await transaction.CommitAsync(cancellationToken);
                return earlyInboxResult;
            }

            var policyRecord = await sapCodePolicyRepository.GetByCompanyIdAsync(
                centralCompanyId,
                cancellationToken);
            var sapPolicy = ToEnabledPolicy(policyRecord);

            var current = await LoadCentralStateAsync(
                connection,
                transaction,
                proposal.GlobalId,
                cancellationToken);
            var proposedReferences = await ResolveStableReferencesAsync(
                connection,
                transaction,
                proposal.Proposed,
                cancellationToken);
            var sameRoleIdentificationExists = proposedReferences.IsComplete &&
                await SameRoleIdentificationExistsAsync(
                    connection,
                    transaction,
                    proposal.PartnerType,
                    proposedReferences.IdentificationTypeId!.Value,
                    proposal.NormalizedIdentificationNumber,
                    current?.BusinessPartnerId,
                    cancellationToken);

            var decision = BusinessPartnerProposalReconciliationPolicy.Evaluate(
                proposal,
                current,
                sameRoleIdentificationExists,
                sapPolicy,
                proposedReferences.IsComplete);

            if (decision.Outcome is BusinessPartnerProposalApplyOutcome.RetryableFailure or
                BusinessPartnerProposalApplyOutcome.TerminalFailure)
            {
                await transaction.RollbackAsync(cancellationToken);
                return ToApplyResult(decision);
            }

            BusinessPartnerProposalApplyResult result;
            switch (decision.Outcome)
            {
                case BusinessPartnerProposalApplyOutcome.Accepted:
                {
                    var canonicalReferences = await ResolveStableReferencesAsync(
                        connection,
                        transaction,
                        decision.Canonical!,
                        cancellationToken);
                    if (!canonicalReferences.IsComplete)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return new BusinessPartnerProposalApplyResult(
                            BusinessPartnerProposalApplyOutcome.RetryableFailure,
                            current?.CanonicalVersion ?? 0,
                            "Una referencia estable requerida aun no existe en el tenant central.",
                            "BP_SYNC_REFERENCE_NOT_FOUND");
                    }

                    var parameters = CreateAcceptParameters(
                        payloadFactory,
                        centralCompanyId,
                        context,
                        proposal,
                        decision,
                        current,
                        canonicalReferences);
                    var sqlResult = await connection.QuerySingleAsync<TerminalProcedureResultRow>(
                        new CommandDefinition(
                            AcceptProcedure,
                            parameters,
                            transaction,
                            commandType: CommandType.StoredProcedure,
                            cancellationToken: cancellationToken));
                    result = MapTerminalResult(sqlResult, decision, BusinessPartnerProposalApplyOutcome.Accepted);
                    break;
                }

                case BusinessPartnerProposalApplyOutcome.Conflict:
                {
                    var parameters = CreateConflictParameters(
                        payloadFactory,
                        centralCompanyId,
                        context,
                        proposal,
                        decision,
                        current);
                    var sqlResult = await connection.QuerySingleAsync<TerminalProcedureResultRow>(
                        new CommandDefinition(
                            ConflictProcedure,
                            parameters,
                            transaction,
                            commandType: CommandType.StoredProcedure,
                            cancellationToken: cancellationToken));
                    result = MapTerminalResult(sqlResult, decision, BusinessPartnerProposalApplyOutcome.Conflict);
                    break;
                }

                case BusinessPartnerProposalApplyOutcome.Rejected:
                {
                    var parameters = CreateRejectParameters(
                        payloadFactory,
                        centralCompanyId,
                        context,
                        proposal,
                        decision);
                    var sqlResult = await connection.QuerySingleAsync<TerminalProcedureResultRow>(
                        new CommandDefinition(
                            RejectProcedure,
                            parameters,
                            transaction,
                            commandType: CommandType.StoredProcedure,
                            cancellationToken: cancellationToken));
                    result = MapTerminalResult(sqlResult, decision, BusinessPartnerProposalApplyOutcome.Rejected);
                    break;
                }

                default:
                    throw new InvalidOperationException($"Unsupported proposal outcome {decision.Outcome}.");
            }

            await transaction.CommitAsync(cancellationToken);
            return result;
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

    private static async Task<BusinessPartnerProposalApplyResult?> EnsureInboxAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        SyncEventApplyContext context,
        CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters(CreateInboxEnsureParameters(context));
        parameters.Add("InboxId", dbType: DbType.Int64, direction: ParameterDirection.Output);
        parameters.Add(
            "InboxStatus",
            dbType: DbType.String,
            direction: ParameterDirection.Output,
            size: 30);
        parameters.Add("EnvelopeResult", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await connection.ExecuteAsync(new CommandDefinition(
            InboxEnsureProcedure,
            parameters,
            transaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        return MapEarlyInboxResult(new InboxEnsureResultRow
        {
            InboxId = parameters.Get<long>("InboxId"),
            InboxStatus = parameters.Get<string>("InboxStatus"),
            EnvelopeResult = parameters.Get<int>("EnvelopeResult")
        });
    }

    internal static InboxEnsureParameters CreateInboxEnsureParameters(SyncEventApplyContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return new InboxEnsureParameters(
            context.EventId,
            context.SourceCompanyId,
            context.EntityName,
            context.EntityGlobalId,
            context.Operation,
            context.PayloadJson);
    }

    internal static BusinessPartnerProposalApplyResult? MapEarlyInboxResult(
        InboxEnsureResultRow result)
    {
        ArgumentNullException.ThrowIfNull(result);

        if (result.EnvelopeResult == 4)
        {
            return new BusinessPartnerProposalApplyResult(
                BusinessPartnerProposalApplyOutcome.TerminalFailure,
                0,
                "El EventId ya pertenece a un sobre distinto.",
                "BP_SYNC_EVENT_ID_COLLISION");
        }

        if (string.Equals(result.InboxStatus, "Applied", StringComparison.Ordinal))
        {
            return new BusinessPartnerProposalApplyResult(
                BusinessPartnerProposalApplyOutcome.Duplicate,
                0,
                "El evento de propuesta ya fue consumido.");
        }

        if (string.Equals(result.InboxStatus, "Pending", StringComparison.Ordinal) &&
            result.EnvelopeResult is 1 or 2)
        {
            return null;
        }

        throw new InvalidOperationException(
            $"Unexpected SyncInbox guard result {result.EnvelopeResult}/{result.InboxStatus}.");
    }

    internal static async Task<BusinessPartnerProposalCentralState?> LoadCentralStateAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        Guid globalId,
        CancellationToken cancellationToken)
    {
        using var grid = await connection.QueryMultipleAsync(new CommandDefinition(
            CanonicalForUpdateProcedure,
            new { GlobalId = globalId },
            transaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        var partner = await grid.ReadSingleOrDefaultAsync<CanonicalRootRow>();
        var addresses = (await grid.ReadAsync<CanonicalAddressRow>()).AsList();
        var contacts = (await grid.ReadAsync<CanonicalContactRow>()).AsList();
        if (partner is null)
        {
            return null;
        }

        var snapshot = new BusinessPartnerCanonicalSnapshot(
            partner.GlobalId,
            partner.Code,
            partner.Name,
            partner.CommercialName,
            partner.PartnerType,
            partner.IdentificationTypeCode,
            partner.IdentificationNumber,
            partner.NormalizedIdentificationNumber,
            partner.Email,
            partner.Phone,
            partner.SapCardCode,
            partner.IsActive && !partner.IsDeleted,
            addresses
                .Where(item => !item.IsDeleted)
                .OrderBy(item => item.GlobalId)
                .Select(item => new BusinessPartnerAddressSnapshot(
                    item.GlobalId,
                    item.AddressType,
                    item.Line1,
                    item.Line2,
                    item.CountryCode,
                    item.ProvinceCode,
                    item.CityCode,
                    item.PostalCode,
                    item.Latitude,
                    item.Longitude,
                    item.IsPrimary,
                    item.IsActive))
                .ToArray(),
            contacts
                .Where(item => !item.IsDeleted)
                .OrderBy(item => item.GlobalId)
                .Select(item => new BusinessPartnerContactSnapshot(
                    item.GlobalId,
                    item.ContactTypeCode,
                    item.ContactChannelCode,
                    item.Name,
                    item.Position,
                    item.Department,
                    item.Phone,
                    item.Extension,
                    item.Mobile,
                    item.Email,
                    item.Language,
                    item.ReceivesNotifications,
                    item.IsPrimary,
                    item.IsActive,
                    item.Notes))
                .ToArray());

        return new BusinessPartnerProposalCentralState(
            partner.Id,
            partner.CanonicalVersion,
            snapshot,
            partner.RowVersion);
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
        var identification = await grid.ReadSingleAsync<IdentificationReferenceRow>();
        var addresses = (await grid.ReadAsync<AddressReferenceRow>()).AsList();
        var contacts = (await grid.ReadAsync<ContactReferenceRow>()).AsList();
        return ResolveStableReferences(sorted, identification, addresses, contacts);
    }

    private static async Task<bool> SameRoleIdentificationExistsAsync(
        SqlConnection connection,
        SqlTransaction transaction,
        string partnerType,
        int identificationTypeId,
        string normalizedIdentificationNumber,
        int? excludingId,
        CancellationToken cancellationToken) =>
        await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            IdentificationExistsProcedure,
            new
            {
                PartnerType = partnerType,
                IdentificationTypeId = identificationTypeId,
                NormalizedIdentificationNumber = normalizedIdentificationNumber,
                ExcluirId = excludingId
            },
            transaction,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken)) > 0;

    private static BusinessPartnerSapCodePolicyData? ToEnabledPolicy(
        BusinessPartnerSapCodePolicyRecord? policy)
    {
        if (policy is not { IsEnabled: true } ||
            string.IsNullOrWhiteSpace(policy.PassportIdentificationTypeCode) ||
            !Enum.TryParse<BusinessPartnerSapPrefixMode>(policy.PrefixMode, out var prefixMode))
        {
            return null;
        }

        return new BusinessPartnerSapCodePolicyData(
            prefixMode,
            policy.PassportIdentificationTypeCode.Trim());
    }

    private static BusinessPartnerProposalApplyResult ToApplyResult(
        BusinessPartnerProposalDecision decision) =>
        new(decision.Outcome, decision.CanonicalVersion, decision.Message, decision.ErrorCode);

    internal static BusinessPartnerProposalApplyResult MapTerminalResult(
        TerminalProcedureResultRow sqlResult,
        BusinessPartnerProposalDecision decision,
        BusinessPartnerProposalApplyOutcome intendedOutcome) =>
        sqlResult.ResultCode switch
        {
            1 => new(intendedOutcome, sqlResult.CanonicalVersion ?? decision.CanonicalVersion,
                decision.Message, decision.ErrorCode),
            2 => new(BusinessPartnerProposalApplyOutcome.Duplicate,
                sqlResult.CanonicalVersion ?? decision.CanonicalVersion,
                "El evento de propuesta ya fue consumido."),
            4 => new(BusinessPartnerProposalApplyOutcome.TerminalFailure,
                sqlResult.CanonicalVersion ?? decision.CanonicalVersion,
                "El EventId ya pertenece a un sobre distinto.",
                "BP_SYNC_EVENT_ID_COLLISION"),
            5 => new(BusinessPartnerProposalApplyOutcome.Conflict,
                sqlResult.CanonicalVersion ?? decision.CanonicalVersion,
                "La propuesta entro en conflicto con el canonico central.",
                "BP_SYNC_CONFLICT"),
            _ => throw new InvalidOperationException(
                $"Unexpected BusinessPartner proposal procedure result {sqlResult.ResultCode}.")
        };

    private static SqlConnection CreateSqlConnection(CompanyConnectionInfo company) =>
        company.DatabaseEngine == DatabaseEngine.SqlServer
            ? new SqlConnection(company.ConnectionString)
            : throw new NotSupportedException(
                $"El motor {company.DatabaseEngine} no esta implementado para propuestas BusinessPartner.");

    internal static Guid CreateDeterministicEventId(Guid proposalEventId, string outputKind)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputKind);

        var proposalBytes = proposalEventId.ToByteArray();
        var kindBytes = Encoding.UTF8.GetBytes(outputKind);
        var input = new byte[proposalBytes.Length + kindBytes.Length];
        proposalBytes.CopyTo(input, 0);
        kindBytes.CopyTo(input, proposalBytes.Length);
        var hash = SHA256.HashData(input);
        return new Guid(hash.AsSpan(0, 16));
    }

    internal static string CreateResultPayloadJson(
        ISyncEventPayloadFactory payloadFactory,
        int companyId,
        Guid proposalEventId,
        int originCompanyId,
        Guid globalId,
        string status,
        string? message,
        long canonicalVersion,
        BusinessPartnerCanonicalSnapshot? canonical)
    {
        ArgumentNullException.ThrowIfNull(payloadFactory);

        var payload = new BusinessPartnerProposalResultPayloadV1(
            BusinessPartnerSyncSchemaVersions.ProposalResult,
            globalId,
            proposalEventId,
            originCompanyId,
            status,
            message,
            canonicalVersion,
            canonical);

        return payloadFactory.CreatePayloadJson(new SyncPublishRequest(
            companyId,
            SyncMasterBranchEntityCodes.BusinessPartnerProposalResult,
            globalId,
            EntityCode: null,
            SyncOperation.Updated,
            payload,
            SourceSystem: null,
            SourceReference: proposalEventId.ToString("D")));
    }

    internal static ProposalAcceptParameters CreateAcceptParameters(
        ISyncEventPayloadFactory payloadFactory,
        int companyId,
        SyncEventApplyContext context,
        BusinessPartnerProposalPayloadV1 proposal,
        BusinessPartnerProposalDecision decision,
        BusinessPartnerProposalCentralState? current,
        StableReferenceResolution references)
    {
        var canonical = SortSnapshot(decision.Canonical
            ?? throw new InvalidOperationException("An accepted proposal requires a canonical snapshot."));
        var currentCanonical = current is null ? null : SortSnapshot(current.Snapshot);
        var resultEventId = CreateDeterministicEventId(context.EventId, "Conflict");

        return new ProposalAcceptParameters(
            context.EventId,
            companyId,
            context.SourceCompanyId,
            proposal.GlobalId,
            context.Operation,
            context.PayloadJson,
            canonical.Code,
            canonical.Name,
            canonical.CommercialName,
            canonical.PartnerType,
            references.IdentificationTypeId
                ?? throw new InvalidOperationException("Identification type reference is required."),
            canonical.IdentificationNumber,
            canonical.NormalizedIdentificationNumber,
            canonical.Email,
            canonical.Phone,
            canonical.SapCardCode,
            decision.CanonicalVersion,
            canonical.IsActive,
            IsDeleted: false,
            references.AddressesJson
                ?? throw new InvalidOperationException("Resolved addresses are required."),
            references.ContactsJson
                ?? throw new InvalidOperationException("Resolved contacts are required."),
            CreateDeterministicEventId(context.EventId, "Accepted"),
            CreateCanonicalPayloadJson(payloadFactory, companyId, context, canonical, decision.CanonicalVersion),
            proposal.BaseCanonicalVersion,
            SerializeSnapshot(proposal.Base),
            SerializeSnapshot(SortSnapshot(proposal.Proposed))!,
            SerializeSnapshot(currentCanonical),
            resultEventId,
            CreateResultPayloadJson(
                payloadFactory,
                companyId,
                context.EventId,
                context.SourceCompanyId,
                proposal.GlobalId,
                "Conflict",
                "La propuesta entro en conflicto con el canonico central.",
                current?.CanonicalVersion ?? 0,
                currentCanonical),
            proposal.OriginUserId,
            proposal.OriginUserName);
    }

    internal static ProposalConflictParameters CreateConflictParameters(
        ISyncEventPayloadFactory payloadFactory,
        int companyId,
        SyncEventApplyContext context,
        BusinessPartnerProposalPayloadV1 proposal,
        BusinessPartnerProposalDecision decision,
        BusinessPartnerProposalCentralState? current)
    {
        var currentCanonical = SortSnapshot(current?.Snapshot ?? decision.Canonical
            ?? throw new InvalidOperationException("A conflict requires a current canonical snapshot."));
        return new ProposalConflictParameters(
            context.EventId,
            companyId,
            context.SourceCompanyId,
            current?.BusinessPartnerId,
            proposal.GlobalId,
            context.Operation,
            context.PayloadJson,
            proposal.BaseCanonicalVersion,
            decision.CanonicalVersion,
            SerializeSnapshot(proposal.Base),
            SerializeSnapshot(SortSnapshot(proposal.Proposed))!,
            SerializeSnapshot(currentCanonical)!,
            JsonSerializer.Serialize(
                decision.ConflictFields.Distinct(StringComparer.Ordinal).OrderBy(path => path, StringComparer.Ordinal),
                JsonOptions),
            CreateDeterministicEventId(context.EventId, "Conflict"),
            CreateResultPayloadJson(
                payloadFactory,
                companyId,
                context.EventId,
                context.SourceCompanyId,
                proposal.GlobalId,
                "Conflict",
                decision.Message,
                decision.CanonicalVersion,
                currentCanonical),
            proposal.OriginUserId,
            proposal.OriginUserName);
    }

    internal static ProposalRejectParameters CreateRejectParameters(
        ISyncEventPayloadFactory payloadFactory,
        int companyId,
        SyncEventApplyContext context,
        BusinessPartnerProposalPayloadV1 proposal,
        BusinessPartnerProposalDecision decision)
    {
        var canonical = decision.Canonical is null ? null : SortSnapshot(decision.Canonical);
        return new ProposalRejectParameters(
            context.EventId,
            companyId,
            context.SourceCompanyId,
            proposal.GlobalId,
            context.Operation,
            context.PayloadJson,
            CreateDeterministicEventId(context.EventId, "Rejected"),
            CreateResultPayloadJson(
                payloadFactory,
                companyId,
                context.EventId,
                context.SourceCompanyId,
                proposal.GlobalId,
                "Rejected",
                decision.Message,
                decision.CanonicalVersion,
                canonical));
    }

    private static string CreateCanonicalPayloadJson(
        ISyncEventPayloadFactory payloadFactory,
        int companyId,
        SyncEventApplyContext context,
        BusinessPartnerCanonicalSnapshot canonical,
        long canonicalVersion)
    {
        var payload = new BusinessPartnerCanonicalPayloadV2(
            BusinessPartnerSyncSchemaVersions.Canonical,
            canonicalVersion,
            context.SourceCompanyId,
            context.EventId,
            canonical);
        var operation = Enum.Parse<SyncOperation>(context.Operation, ignoreCase: false);
        return payloadFactory.CreatePayloadJson(new SyncPublishRequest(
            companyId,
            SyncMasterBranchEntityCodes.BusinessPartner,
            canonical.GlobalId,
            canonical.Code,
            operation,
            payload,
            SourceSystem: null,
            SourceReference: context.EventId.ToString("D")));
    }

    private static string? SerializeSnapshot(BusinessPartnerCanonicalSnapshot? snapshot) =>
        snapshot is null ? null : JsonSerializer.Serialize(SortSnapshot(snapshot), JsonOptions);

    private static BusinessPartnerCanonicalSnapshot SortSnapshot(BusinessPartnerCanonicalSnapshot snapshot) =>
        snapshot with
        {
            Addresses = snapshot.Addresses.OrderBy(item => item.GlobalId).ToArray(),
            Contacts = snapshot.Contacts.OrderBy(item => item.GlobalId).ToArray()
        };

    internal static StableReferenceResolution ResolveStableReferences(
        BusinessPartnerCanonicalSnapshot snapshot,
        IdentificationReferenceRow identification,
        IReadOnlyCollection<AddressReferenceRow> addressReferences,
        IReadOnlyCollection<ContactReferenceRow> contactReferences)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(identification);
        ArgumentNullException.ThrowIfNull(addressReferences);
        ArgumentNullException.ThrowIfNull(contactReferences);

        if (identification.MatchCount != 1 || identification.IdentificationTypeId is null ||
            addressReferences.Count != snapshot.Addresses.Count ||
            contactReferences.Count != snapshot.Contacts.Count)
        {
            return StableReferenceResolution.Incomplete;
        }

        var addressesById = addressReferences.GroupBy(item => item.GlobalId).ToArray();
        var contactsById = contactReferences.GroupBy(item => item.GlobalId).ToArray();
        if (addressesById.Any(group => group.Count() != 1) ||
            contactsById.Any(group => group.Count() != 1))
        {
            return StableReferenceResolution.Incomplete;
        }

        var addressMap = addressesById.ToDictionary(group => group.Key, group => group.Single());
        var contactMap = contactsById.ToDictionary(group => group.Key, group => group.Single());
        var addresses = new List<ResolvedAddressWrite>(snapshot.Addresses.Count);
        foreach (var address in snapshot.Addresses.OrderBy(item => item.GlobalId))
        {
            if (!addressMap.TryGetValue(address.GlobalId, out var reference) ||
                !IsResolved(address.CountryCode, reference.CountryId, reference.CountryMatchCount) ||
                !IsResolved(address.ProvinceCode, reference.ProvinceId, reference.ProvinceMatchCount) ||
                !IsResolved(address.CityCode, reference.CityId, reference.CityMatchCount))
            {
                return StableReferenceResolution.Incomplete;
            }

            addresses.Add(new ResolvedAddressWrite(
                address.GlobalId,
                address.AddressType,
                address.Line1,
                address.Line2,
                reference.CountryId,
                reference.ProvinceId,
                reference.CityId,
                address.CountryCode,
                address.ProvinceCode,
                address.CityCode,
                address.PostalCode,
                address.Latitude,
                address.Longitude,
                address.IsPrimary,
                address.IsActive,
                IsDeleted: false));
        }

        var contacts = new List<ResolvedContactWrite>(snapshot.Contacts.Count);
        foreach (var contact in snapshot.Contacts.OrderBy(item => item.GlobalId))
        {
            if (!contactMap.TryGetValue(contact.GlobalId, out var reference) ||
                !IsResolved(contact.ContactTypeCode, reference.ContactTypeId, reference.ContactTypeMatchCount) ||
                !IsResolved(contact.ContactChannelCode, reference.ContactChannelId, reference.ContactChannelMatchCount))
            {
                return StableReferenceResolution.Incomplete;
            }

            contacts.Add(new ResolvedContactWrite(
                contact.GlobalId,
                reference.ContactTypeId,
                reference.ContactChannelId,
                contact.Name,
                contact.Position,
                contact.Department,
                contact.Phone,
                contact.Extension,
                contact.Mobile,
                contact.Email,
                contact.Language,
                contact.ReceivesNotifications,
                contact.IsPrimary,
                contact.IsActive,
                contact.Notes,
                IsDeleted: false));
        }

        return new StableReferenceResolution(
            true,
            identification.IdentificationTypeId,
            JsonSerializer.Serialize(addresses, JsonOptions),
            JsonSerializer.Serialize(contacts, JsonOptions));
    }

    private static bool IsResolved(string? code, int? id, int matchCount) =>
        string.IsNullOrWhiteSpace(code)
            ? id is null && matchCount == 0
            : id is not null && matchCount == 1;

    internal sealed record IdentificationReferenceRow(int? IdentificationTypeId, int MatchCount);

    internal sealed record AddressReferenceRow(
        Guid GlobalId,
        int? CountryId,
        int CountryMatchCount,
        int? ProvinceId,
        int ProvinceMatchCount,
        int? CityId,
        int CityMatchCount);

    internal sealed record ContactReferenceRow(
        Guid GlobalId,
        int? ContactTypeId,
        int ContactTypeMatchCount,
        int? ContactChannelId,
        int ContactChannelMatchCount);

    internal sealed record StableReferenceResolution(
        bool IsComplete,
        int? IdentificationTypeId,
        string? AddressesJson,
        string? ContactsJson)
    {
        public static readonly StableReferenceResolution Incomplete = new(false, null, null, null);
    }

    internal sealed record ProposalAcceptParameters(
        Guid ProposalEventId,
        int CompanyId,
        int SourceCompanyId,
        Guid BusinessPartnerGlobalId,
        string Operation,
        string ProposalPayloadJson,
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
        string ContactsJson,
        Guid CanonicalEventId,
        string CanonicalPayloadJson,
        long BaseCanonicalVersion,
        string? BaseSnapshotJson,
        string ProposedSnapshotJson,
        string? CurrentCanonicalSnapshotJson,
        Guid ResultEventId,
        string ResultPayloadJson,
        int? AuditUserId,
        string? AuditUserName);

    internal sealed record ProposalConflictParameters(
        Guid ProposalEventId,
        int CompanyId,
        int SourceCompanyId,
        int? BusinessPartnerId,
        Guid BusinessPartnerGlobalId,
        string Operation,
        string ProposalPayloadJson,
        long BaseCanonicalVersion,
        long CurrentCanonicalVersion,
        string? BaseSnapshotJson,
        string ProposedSnapshotJson,
        string CanonicalSnapshotJson,
        string ConflictFieldsJson,
        Guid ResultEventId,
        string ResultPayloadJson,
        int? AuditUserId,
        string? AuditUserName);

    internal sealed record ProposalRejectParameters(
        Guid ProposalEventId,
        int CompanyId,
        int SourceCompanyId,
        Guid BusinessPartnerGlobalId,
        string Operation,
        string ProposalPayloadJson,
        Guid ResultEventId,
        string ResultPayloadJson);

    private sealed record ResolvedAddressWrite(
        Guid GlobalId,
        string AddressType,
        string Line1,
        string? Line2,
        int? CountryId,
        int? ProvinceId,
        int? CityId,
        string? CountryCode,
        string? Province,
        string? City,
        string? PostalCode,
        decimal? Latitude,
        decimal? Longitude,
        bool IsPrimary,
        bool IsActive,
        bool IsDeleted);

    private sealed record ResolvedContactWrite(
        Guid GlobalId,
        int? ContactTypeId,
        int? ContactChannelId,
        string Name,
        string? Position,
        string? Department,
        string? Phone,
        string? Extension,
        string? Mobile,
        string? Email,
        string? Language,
        bool ReceivesNotifications,
        bool IsPrimary,
        bool IsActive,
        string? Notes,
        bool IsDeleted);

    private sealed class CanonicalRootRow
    {
        public int Id { get; init; }
        public Guid GlobalId { get; init; }
        public string Code { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? CommercialName { get; init; }
        public string PartnerType { get; init; } = string.Empty;
        public string IdentificationTypeCode { get; init; } = string.Empty;
        public string IdentificationNumber { get; init; } = string.Empty;
        public string NormalizedIdentificationNumber { get; init; } = string.Empty;
        public string? Email { get; init; }
        public string? Phone { get; init; }
        public string? SapCardCode { get; init; }
        public long CanonicalVersion { get; init; }
        public byte[] RowVersion { get; init; } = [];
        public bool IsActive { get; init; }
        public bool IsDeleted { get; init; }
    }

    private sealed class CanonicalAddressRow
    {
        public Guid GlobalId { get; init; }
        public string AddressType { get; init; } = string.Empty;
        public string Line1 { get; init; } = string.Empty;
        public string? Line2 { get; init; }
        public string? CountryCode { get; init; }
        public string? ProvinceCode { get; init; }
        public string? CityCode { get; init; }
        public string? PostalCode { get; init; }
        public decimal? Latitude { get; init; }
        public decimal? Longitude { get; init; }
        public bool IsPrimary { get; init; }
        public bool IsActive { get; init; }
        public bool IsDeleted { get; init; }
    }

    private sealed class CanonicalContactRow
    {
        public Guid GlobalId { get; init; }
        public string? ContactTypeCode { get; init; }
        public string? ContactChannelCode { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Position { get; init; }
        public string? Department { get; init; }
        public string? Phone { get; init; }
        public string? Extension { get; init; }
        public string? Mobile { get; init; }
        public string? Email { get; init; }
        public string? Language { get; init; }
        public bool ReceivesNotifications { get; init; }
        public bool IsPrimary { get; init; }
        public bool IsActive { get; init; }
        public string? Notes { get; init; }
        public bool IsDeleted { get; init; }
    }

    internal sealed class TerminalProcedureResultRow
    {
        public int ResultCode { get; init; }
        public int? BusinessPartnerId { get; init; }
        public long? CanonicalVersion { get; init; }
    }

    internal sealed record InboxEnsureParameters(
        Guid EventId,
        int SourceCompanyId,
        string EntityName,
        Guid EntityGlobalId,
        string Operation,
        string PayloadJson);

    internal sealed class InboxEnsureResultRow
    {
        public long InboxId { get; init; }
        public string InboxStatus { get; init; } = string.Empty;
        public int EnvelopeResult { get; init; }
    }
}
