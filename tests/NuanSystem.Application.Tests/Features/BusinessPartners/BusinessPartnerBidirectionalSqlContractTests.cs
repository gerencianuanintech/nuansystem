using System.Text.RegularExpressions;
using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

public sealed class BusinessPartnerBidirectionalSqlContractTests
{
    [Fact]
    public void TenantFoundation_IsForwardOnlyAndRoleAware()
    {
        var sql = Read("database", "sql", "228_tenant_business_partner_bidirectional_foundation.sql");

        sql.Should().Contain("NormalizedIdentificationNumber")
            .And.Contain("CanonicalVersion")
            .And.Contain("MasterSyncStatus")
            .And.Contain("RowVersion")
            .And.Contain("BusinessPartnerSyncConflicts")
            .And.Contain("TargetCompanyId")
            .And.Contain("PartnerType, IdentificationTypeId, NormalizedIdentificationNumber")
            .And.Contain("THROW 52028")
            .And.Contain("LegacyReview")
            .And.Contain("Version = N'20260903.228'")
            .And.NotContain("DROP TABLE")
            .And.NotContain("DELETE FROM dbo.BusinessPartners")
            .And.NotContain("NuanSystem_Master");
    }

    [Fact]
    public void TenantFoundation_AddsStableChildIdentitiesAndFilteredUniqueIndexes()
    {
        var sql = Read("database", "sql", "228_tenant_business_partner_bidirectional_foundation.sql");

        sql.Should().Contain("ALTER TABLE dbo.BusinessPartnerAddresses ADD GlobalId uniqueidentifier NULL")
            .And.Contain("ALTER TABLE dbo.BusinessPartnerContacts ADD GlobalId uniqueidentifier NULL")
            .And.Contain("UX_BusinessPartnerAddresses_GlobalId")
            .And.Contain("UX_BusinessPartnerContacts_GlobalId")
            .And.Contain("UX_BusinessPartners_SapCardCode_Active")
            .And.Contain("WHERE IsDeleted = 0")
            .And.Contain("SET GlobalId = NEWID()")
            .And.Contain("ALTER COLUMN GlobalId uniqueidentifier NOT NULL");
    }

    [Fact]
    public void ReadinessReport_IsReadOnlyAndCoversEveryBlockingCondition()
    {
        var sql = Read("database", "sql", "manual", "check_business_partner_bidirectional_readiness.sql");

        sql.Should().Contain("MissingBusinessPartnerGlobalId")
            .And.Contain("DuplicateBusinessPartnerCode")
            .And.Contain("BusinessPartnerCodeTooLong")
            .And.Contain("DuplicateNormalizedIdentificationByRole")
            .And.Contain("LegacyBothBusinessPartner")
            .And.Contain("DuplicateSapCardCode")
            .And.Contain("SapCardCodeTooLong")
            .And.Contain("MissingAddressGlobalId")
            .And.Contain("MissingContactGlobalId")
            .And.Contain("PendingBusinessPartnerLocalOutbox")
            .And.Contain("PendingBusinessPartnerSyncOutbox")
            .And.Contain("PendingBusinessPartnerSyncInbox");

        Regex.IsMatch(
                sql,
                @"\b(INSERT|UPDATE|DELETE|MERGE|ALTER|CREATE|DROP)\b",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)
            .Should().BeFalse();
    }

    [Fact]
    public void TenantOperations_DefineTransactionalProposalCanonicalAndConflictContracts()
    {
        var sql = Read("database", "sql", "230_tenant_business_partner_bidirectional_operations.sql");
        string[] procedures =
        [
            "SP_NA_GET_BUSINESSPARTNER_CANONICAL_FORUPDATE",
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_ACCEPT",
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_CONFLICT",
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_REJECT",
            "SP_NA_POST_BUSINESSPARTNER_BRANCH_APPLY_PREFLIGHT",
            "SP_NA_POST_BUSINESSPARTNER_CANONICAL_APPLY",
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_RESULT_APPLY",
            "SP_NA_GET_BUSINESSPARTNER_SYNCCONFLICTS_LISTAR",
            "SP_NA_GET_BUSINESSPARTNER_SYNCCONFLICT_BUSCARPORID",
            "SP_NA_POST_BUSINESSPARTNER_SYNCCONFLICT_RESOLVER"
        ];

        sql.Should().ContainAll(procedures)
            .And.Contain("BEGIN TRANSACTION")
            .And.Contain("BEGIN TRY")
            .And.Contain("ROLLBACK TRANSACTION")
            .And.Contain("dbo.SyncInbox")
            .And.Contain("dbo.LocalOutbox")
            .And.Contain("BusinessPartnerSyncConflicts")
            .And.Contain("@CanonicalVersion")
            .And.Contain("CanonicalVersion = @CanonicalVersion")
            .And.Contain("@CurrentVersion > @CanonicalVersion")
            .And.Contain("Version = N'20260903.230'")
            .And.NotContain("NuanSystem_Master")
            .And.NotContain("SapSyncOutbox")
            .And.NotContain("DROP TABLE");
    }

    [Fact]
    public void TenantInitializer_RegistersFoundationBeforeOperationsAtTheEnd()
    {
        var source = Read(
            "src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerTenantDatabaseInitializer.cs");

        var foundation = source.IndexOf(
            "228_tenant_business_partner_bidirectional_foundation.sql",
            StringComparison.Ordinal);
        var operations = source.IndexOf(
            "230_tenant_business_partner_bidirectional_operations.sql",
            StringComparison.Ordinal);

        foundation.Should().BeGreaterThan(source.IndexOf(
            "226_tenant_sales_channels_master.sql",
            StringComparison.Ordinal));
        operations.Should().BeGreaterThan(foundation);
    }

    [Fact]
    public void ProposalResultApply_UsesLockedStaleAndStatusSpecificVersionOutcomes()
    {
        var procedure = Procedure("SP_NA_POST_BUSINESSPARTNER_PROPOSAL_RESULT_APPLY");

        procedure.Should().Contain("FROM dbo.BusinessPartners WITH (UPDLOCK,HOLDLOCK)")
            .And.Contain("@Status<>'Accepted' AND @CurrentVersion > @CanonicalVersion")
            .And.Contain("IF @Status='Rejected' AND @HasCanonical=1")
            .And.Contain("IF @Status='Conflict' AND @HasCanonical=0")
            .And.Contain("@Status='Rejected' AND @HasCanonical=0")
            .And.Contain("@CanonicalVersion<>0")
            .And.Contain("@CurrentVersion<>0")
            .And.Contain("SELECT 3 AS ResultCode")
            .And.NotContain("IF @HasCanonical=1 AND @CurrentVersion = @CanonicalVersion")
            .And.NotContain("CanonicalVersion=CASE");
    }

    [Fact]
    public void ProposalResultApply_PreservesConflictRestoresRejectedAndConsumesAcceptedWithoutMutation()
    {
        var procedure = Procedure("SP_NA_POST_BUSINESSPARTNER_PROPOSAL_RESULT_APPLY");

        var acceptedGuard = procedure.IndexOf("IF @Status='Accepted'", StringComparison.Ordinal);
        var staleGuard = procedure.IndexOf(
            "IF @Status<>'Accepted' AND @CurrentVersion > @CanonicalVersion",
            StringComparison.Ordinal);
        var conflictGuard = procedure.IndexOf("IF @Status='Conflict'", StringComparison.Ordinal);
        var rejectedRestoreGuard = procedure.IndexOf(
            "IF @Status='Rejected' AND @HasCanonical=1",
            StringComparison.Ordinal);
        var canonicalUpsert = procedure.IndexOf(
            "EXEC dbo.SP_NA_POST_BUSINESSPARTNER_CANONICAL_UPSERT",
            StringComparison.Ordinal);

        acceptedGuard.Should().BeGreaterThan(0).And.BeLessThan(staleGuard);
        conflictGuard.Should().BeGreaterThan(staleGuard).And.BeLessThan(rejectedRestoreGuard);
        rejectedRestoreGuard.Should().BeGreaterThan(conflictGuard).And.BeLessThan(canonicalUpsert);
        procedure.Should().Contain("IF @Status='Rejected' AND @HasCanonical=0")
            .And.Contain("MasterSyncStatus='Conflict',MasterSyncMessage=@Message")
            .And.Contain("MasterSyncStatus='Rejected',MasterSyncMessage=@Message")
            .And.NotContain("IF @HasCanonical=1 AND @CurrentVersion = @CanonicalVersion")
            .And.NotContain("IF @HasCanonical=1\n        BEGIN");
    }

    [Fact]
    public void BranchApplyPreflight_ClosesInboxAndVersionOutcomesBeforeMutableReferences()
    {
        var procedure = Procedure("SP_NA_POST_BUSINESSPARTNER_BRANCH_APPLY_PREFLIGHT");

        procedure.Should().Contain("IF @@TRANCOUNT = 0")
            .And.Contain("requires an ambient transaction")
            .And.Contain("EXEC dbo.SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE")
            .And.Contain("FROM dbo.BusinessPartners WITH (UPDLOCK,HOLDLOCK)")
            .And.Contain("@CompareCanonicalVersion=1 AND @CurrentVersion>@CanonicalVersion")
            .And.Contain("@EqualVersionIsReplay=1 AND @CurrentVersion=@CanonicalVersion")
            .And.Contain("SET Status=N'Ignored'")
            .And.Contain("SET Status=N'Applied'")
            .And.ContainAll("SELECT 0 AS ResultCode", "SELECT 2 AS ResultCode", "SELECT 3 AS ResultCode", "SELECT 4 AS ResultCode")
            .And.NotContain("BEGIN TRANSACTION")
            .And.NotContain("SP_NA_GET_BUSINESSPARTNER_STABLE_REFERENCES_RESOLVE");
    }

    [Fact]
    public void BranchApplyPreflight_ExactDeadLetterReplayReturnsBeforeLocksWithoutMutation()
    {
        var procedure = Procedure("SP_NA_POST_BUSINESSPARTNER_BRANCH_APPLY_PREFLIGHT");
        var collisionGuard = procedure.IndexOf("IF @InboxEnvelopeResult=4", StringComparison.Ordinal);
        var deadLetterGuard = procedure.IndexOf("IF @InboxStatus=N'DeadLetter'", StringComparison.Ordinal);
        var partnerLock = procedure.IndexOf(
            "FROM dbo.BusinessPartners WITH (UPDLOCK,HOLDLOCK)",
            StringComparison.Ordinal);

        collisionGuard.Should().BeGreaterThan(-1).And.BeLessThan(deadLetterGuard);
        deadLetterGuard.Should().BeGreaterThan(-1).And.BeLessThan(partnerLock);
        var terminalBranch = procedure[deadLetterGuard..partnerLock];
        terminalBranch.Should().Contain("SELECT 5 AS ResultCode")
            .And.Contain("RETURN;")
            .And.NotContain("UPDATE dbo.SyncInbox")
            .And.NotContain("SP_NA_GET_BUSINESSPARTNER_STABLE_REFERENCES_RESOLVE")
            .And.NotContain("SP_NA_POST_BUSINESSPARTNER_CANONICAL_APPLY")
            .And.NotContain("SP_NA_POST_BUSINESSPARTNER_PROPOSAL_RESULT_APPLY");
    }

    [Theory]
    [InlineData("SP_NA_POST_BUSINESSPARTNER_CANONICAL_APPLY", "BpCanonicalApplySavepoint")]
    [InlineData("SP_NA_POST_BUSINESSPARTNER_PROPOSAL_RESULT_APPLY", "BpProposalResultSavepoint")]
    public void BranchApplyProcedures_PreserveCallerOwnedTransactions(string procedureName, string savepoint)
    {
        var procedure = Procedure(procedureName);

        procedure.Should().Contain("DECLARE @StartedTransaction bit = 0")
            .And.Contain("IF @@TRANCOUNT = 0")
            .And.Contain($"SAVE TRANSACTION {savepoint}")
            .And.Contain("IF @StartedTransaction = 1 COMMIT TRANSACTION")
            .And.Contain($"ROLLBACK TRANSACTION {savepoint}")
            .And.NotContain("IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;");
    }

    [Fact]
    public void ConflictResolver_RequiresConflictTokenAndRevalidatesTheExactLockedLivePartner()
    {
        var procedure = Procedure("SP_NA_POST_BUSINESSPARTNER_SYNCCONFLICT_RESOLVER");

        procedure.Should().Contain("IF @ExpectedRowVersion IS NULL")
            .And.Contain("@ExpectedBusinessPartnerId int")
            .And.Contain("@ExpectedCanonicalVersion bigint")
            .And.Contain("@ExpectedBusinessPartnerRowVersion binary(8)")
            .And.Contain("FROM dbo.BusinessPartners WITH (UPDLOCK,HOLDLOCK)")
            .And.Contain("@LiveBusinessPartnerId<>@ExpectedBusinessPartnerId")
            .And.Contain("@LiveCanonicalVersion<>@ExpectedCanonicalVersion")
            .And.Contain("@LiveBusinessPartnerRowVersion<>@ExpectedBusinessPartnerRowVersion")
            .And.Contain("SELECT 4 AS ResultCode")
            .And.Contain("@CanonicalVersion=@ExpectedCanonicalVersion+1")
            .And.NotContain("@LiveCanonicalVersion <> @CurrentVersion")
            .And.NotContain("@LiveBusinessPartnerRowVersion<>@PresentedBusinessPartnerRowVersion")
            .And.NotContain("@CanonicalVersion=@CurrentVersion+1");
    }

    [Fact]
    public void ConflictResolver_ReceivesResolvedTenantIdentificationTypeInsteadOfReadingLocalIdFromWireSnapshot()
    {
        var procedure = Procedure("SP_NA_POST_BUSINESSPARTNER_SYNCCONFLICT_RESOLVER");

        procedure.Should().Contain("@IdentificationTypeId int")
            .And.Contain("@IdentificationTypeId=@IdentificationTypeId")
            .And.NotContain("JSON_VALUE(@ResolvedSnapshotJson,'$.identificationTypeId')");
    }

    [Fact]
    public void ConflictResolver_ClosesOutboundEntityAndTargetAgainstPersistedOrigin()
    {
        var procedure = Procedure("SP_NA_POST_BUSINESSPARTNER_SYNCCONFLICT_RESOLVER");

        procedure.Should().ContainAll(
                "@OutboundEntityName COLLATE Latin1_General_100_BIN2<>N'BusinessPartner'",
                "@Resolution='AcceptBranch'",
                "@TargetCompanyId IS NOT NULL",
                "@OutboundEntityName COLLATE Latin1_General_100_BIN2<>N'BusinessPartnerProposalResult'",
                "@TargetCompanyId<>@OriginCompanyId",
                "THROW 52030, 'Conflict outbound route is invalid.'")
            .And.NotContain("COALESCE(@TargetCompanyId,@OriginCompanyId)");
    }

    [Fact]
    public void ConflictResolver_PreservesCallerOwnedTransactionWithNamedSavepoint()
    {
        var procedure = Procedure("SP_NA_POST_BUSINESSPARTNER_SYNCCONFLICT_RESOLVER");

        procedure.Should().Contain("DECLARE @StartedTransaction bit = 0")
            .And.Contain("IF @@TRANCOUNT = 0")
            .And.Contain("SAVE TRANSACTION BpConflictResolveSavepoint")
            .And.Contain("IF @StartedTransaction = 1 COMMIT TRANSACTION")
            .And.Contain("ROLLBACK TRANSACTION BpConflictResolveSavepoint")
            .And.NotContain("IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;");
    }

    [Fact]
    public void ConflictList_ReturnsSnapshotsForSingleCallSafeDifferenceProjection()
    {
        var procedure = Procedure("SP_NA_GET_BUSINESSPARTNER_SYNCCONFLICTS_LISTAR");

        procedure.Should().ContainAll(
            "conflict.BaseSnapshotJson AS BaseSnapshotJson",
            "conflict.ProposedSnapshotJson AS ProposedSnapshotJson",
            "conflict.CanonicalSnapshotJson AS CanonicalSnapshotJson",
            "conflict.ConflictFieldsJson AS ConflictFieldsJson");
    }

    [Fact]
    public void ConflictDetail_LocksTheConflictForRepositoryPreflight()
    {
        var procedure = Procedure("SP_NA_GET_BUSINESSPARTNER_SYNCCONFLICT_BUSCARPORID");

        procedure.Should().Contain(
            "FROM dbo.BusinessPartnerSyncConflicts AS conflict WITH (UPDLOCK,HOLDLOCK)");
    }

    [Fact]
    public void EventEnvelopeGuards_DeadLetterCollisionsAndPreserveIdenticalReplays()
    {
        var inboxGuard = Procedure("SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE");
        var outboxGuard = Procedure("SP_NA_POST_BUSINESSPARTNER_LOCALOUTBOX_ENSURE");

        inboxGuard.Should().ContainAll(
            "SourceCompanyId", "EntityName", "EntityGlobalId", "Operation", "PayloadJson",
            "COLLATE Latin1_General_100_BIN2", "Status=N'DeadLetter'", "@EnvelopeResult=4")
            .And.NotContain("SET SourceCompanyId=")
            .And.NotContain("SET EntityName=")
            .And.NotContain("SET EntityGlobalId=")
            .And.NotContain("SET Operation=")
            .And.NotContain("SET PayloadJson=");
        outboxGuard.Should().ContainAll(
            "CompanyId", "TargetCompanyId", "CausationEventId", "EntityName", "EntityGlobalId",
            "EntityCode", "Operation", "PayloadJson", "COLLATE Latin1_General_100_BIN2",
            "Status=N'DeadLetter'", "@EnvelopeResult=4");

        string[] inboxProcedures =
        [
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_ACCEPT",
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_CONFLICT",
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_REJECT",
            "SP_NA_POST_BUSINESSPARTNER_BRANCH_APPLY_PREFLIGHT",
            "SP_NA_POST_BUSINESSPARTNER_CANONICAL_APPLY"
        ];
        foreach (var name in inboxProcedures)
            Procedure(name).Should().Contain("EXEC dbo.SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE")
                .And.Contain("SELECT 4 AS ResultCode");
        Procedure("SP_NA_POST_BUSINESSPARTNER_PROPOSAL_RESULT_APPLY")
            .Should().Contain("EXEC dbo.SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE")
            .And.Contain("SELECT 6 AS ResultCode");

        string[] outboxProcedures =
        [
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_ACCEPT",
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_CONFLICT",
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_REJECT"
        ];
        foreach (var name in outboxProcedures)
            Procedure(name).Should().Contain("EXEC dbo.SP_NA_POST_BUSINESSPARTNER_LOCALOUTBOX_ENSURE")
                .And.Contain("SELECT 4 AS ResultCode");
        Procedure("SP_NA_POST_BUSINESSPARTNER_SYNCCONFLICT_RESOLVER")
            .Should().Contain("IF @OutboxEnvelopeResult=4")
            .And.Contain("SELECT 5 AS ResultCode")
            .And.Contain("SELECT 4 AS ResultCode");
    }

    [Fact]
    public void ProposalTerminalProcedures_PreserveCallerTransactionWithNamedSavepoints()
    {
        var procedures = new Dictionary<string, string>
        {
            ["SP_NA_POST_BUSINESSPARTNER_PROPOSAL_ACCEPT"] = "BpProposalAcceptSavepoint",
            ["SP_NA_POST_BUSINESSPARTNER_PROPOSAL_CONFLICT"] = "BpProposalConflictSavepoint",
            ["SP_NA_POST_BUSINESSPARTNER_PROPOSAL_REJECT"] = "BpProposalRejectSavepoint"
        };

        foreach (var (procedureName, savepointName) in procedures)
        {
            var procedure = Procedure(procedureName);
            procedure.Should().Contain("DECLARE @StartedTransaction bit = 0")
                .And.Contain("IF @@TRANCOUNT = 0")
                .And.Contain("SET @StartedTransaction = 1")
                .And.Contain($"SAVE TRANSACTION {savepointName}")
                .And.Contain("IF @StartedTransaction = 1 COMMIT TRANSACTION")
                .And.Contain("IF @StartedTransaction = 1 AND XACT_STATE() <> 0")
                .And.Contain("ELSE IF XACT_STATE() = 1")
                .And.Contain($"ROLLBACK TRANSACTION {savepointName}")
                .And.NotContain("IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;");
        }
    }

    [Fact]
    public void ProposalTerminalProcedures_KeepOriginalEnvelopeSeparateFromConflictSnapshots()
    {
        var conflict = Procedure("SP_NA_POST_BUSINESSPARTNER_PROPOSAL_CONFLICT");
        conflict.Should().Contain("@Operation nvarchar(30)")
            .And.Contain("@ProposalPayloadJson nvarchar(max)")
            .And.Contain("@Operation=@Operation,@PayloadJson=@ProposalPayloadJson")
            .And.Contain("@ProposedSnapshotJson,@CanonicalSnapshotJson,@ConflictFieldsJson");

        var reject = Procedure("SP_NA_POST_BUSINESSPARTNER_PROPOSAL_REJECT");
        reject.Should().Contain("@Operation nvarchar(30)")
            .And.Contain("@Operation=@Operation,@PayloadJson=@ProposalPayloadJson");
    }

    [Fact]
    public void ProposalAccept_DefensiveConflictPersistsSnapshotsAndPublishesOriginResultBeforeInboxApplied()
    {
        var accept = Procedure("SP_NA_POST_BUSINESSPARTNER_PROPOSAL_ACCEPT");
        accept.Should().ContainAll(
                "@BaseSnapshotJson nvarchar(max)",
                "@ProposedSnapshotJson nvarchar(max)",
                "@CurrentCanonicalSnapshotJson nvarchar(max)",
                "@ResultEventId uniqueidentifier",
                "@ResultPayloadJson nvarchar(max)",
                "@BaseSnapshotJson,@ProposedSnapshotJson",
                "@CurrentCanonicalSnapshotJson",
                "@EventId=@ResultEventId,@CompanyId=@CompanyId,@TargetCompanyId=@SourceCompanyId",
                "@CausationEventId=@ProposalEventId,@EntityName=N'BusinessPartnerProposalResult'",
                "@PayloadJson=@ResultPayloadJson")
            .And.NotContain("@ProposalPayloadJson,\r\n                     @CanonicalPayloadJson,N'[\"immutableIdentityRoleSapOrVersion\"]'");

        accept.IndexOf("@EventId=@ResultEventId", StringComparison.Ordinal)
            .Should().BeLessThan(accept.IndexOf("SET Status=N'Applied'", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("SP_NA_POST_BUSINESSPARTNER_PROPOSAL_ACCEPT")]
    [InlineData("SP_NA_POST_BUSINESSPARTNER_PROPOSAL_CONFLICT")]
    [InlineData("SP_NA_POST_BUSINESSPARTNER_PROPOSAL_REJECT")]
    public void ProposalTerminalProcedures_CreateDurableOutputBeforeInboxApplied(string procedureName)
    {
        var procedure = Procedure(procedureName);

        procedure.IndexOf("EXEC dbo.SP_NA_POST_BUSINESSPARTNER_LOCALOUTBOX_ENSURE", StringComparison.Ordinal)
            .Should().BeGreaterThan(-1)
            .And.BeLessThan(procedure.IndexOf("SET Status=N'Applied'", StringComparison.Ordinal));
    }

    [Fact]
    public void StableReferenceResolver_IsLockedCodeBasedAndFailClosedForEverySupportedReference()
    {
        var resolver = Procedure("SP_NA_GET_BUSINESSPARTNER_STABLE_REFERENCES_RESOLVE");

        resolver.Should().ContainAll(
                "@IdentificationTypeCode nvarchar(30)",
                "@AddressesJson nvarchar(max)",
                "@ContactsJson nvarchar(max)",
                "BusinessPartnerIdentificationTypes WITH (UPDLOCK,HOLDLOCK)",
                "Countries WITH (UPDLOCK,HOLDLOCK)",
                "Provinces WITH (UPDLOCK,HOLDLOCK)",
                "Cities WITH (UPDLOCK,HOLDLOCK)",
                "ContactTypes WITH (UPDLOCK,HOLDLOCK)",
                "ContactChannels WITH (UPDLOCK,HOLDLOCK)",
                "MatchCount")
            .And.Contain("OPENJSON(@AddressesJson)")
            .And.Contain("OPENJSON(@ContactsJson)");
    }

    [Fact]
    public void ProposalIdentificationCheck_LocksOnlyActiveSameRoleCandidateRangeAndExcludesCurrent()
    {
        var procedure = Procedure("SP_NA_GET_BUSINESSPARTNERS_BUSCARPORIDENTIFICACION");

        procedure.Should().Contain("FROM dbo.BusinessPartners WITH (UPDLOCK,HOLDLOCK)")
            .And.Contain("PartnerType=@PartnerType")
            .And.Contain("IdentificationTypeId=@IdentificationTypeId")
            .And.Contain("NormalizedIdentificationNumber=@NormalizedIdentificationNumber")
            .And.Contain("IsDeleted=0 AND IsActive=1")
            .And.Contain("@ExcluirId IS NULL OR Id<>@ExcluirId");
    }

    [Fact]
    public void EventEnvelopeGuards_RejectMissingRequiredValuesAndCompareEveryFieldNullSafely()
    {
        var inboxGuard = Procedure("SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE");
        var outboxGuard = Procedure("SP_NA_POST_BUSINESSPARTNER_LOCALOUTBOX_ENSURE");

        inboxGuard.Should().Contain(
                "IF @EventId IS NULL OR @SourceCompanyId IS NULL OR @EntityName IS NULL OR @EntityGlobalId IS NULL OR @Operation IS NULL OR @PayloadJson IS NULL")
            .And.Contain("SyncInbox envelope required fields cannot be null.")
            .And.ContainAll(
                "CASE WHEN @ExistingSourceCompanyId=@SourceCompanyId OR (@ExistingSourceCompanyId IS NULL AND @SourceCompanyId IS NULL) THEN 0 ELSE 1 END=1",
                "CASE WHEN @ExistingEntityName COLLATE Latin1_General_100_BIN2=@EntityName COLLATE Latin1_General_100_BIN2 OR (@ExistingEntityName IS NULL AND @EntityName IS NULL) THEN 0 ELSE 1 END=1",
                "CASE WHEN @ExistingEntityGlobalId=@EntityGlobalId OR (@ExistingEntityGlobalId IS NULL AND @EntityGlobalId IS NULL) THEN 0 ELSE 1 END=1",
                "CASE WHEN @ExistingOperation COLLATE Latin1_General_100_BIN2=@Operation COLLATE Latin1_General_100_BIN2 OR (@ExistingOperation IS NULL AND @Operation IS NULL) THEN 0 ELSE 1 END=1",
                "CASE WHEN @ExistingPayloadJson COLLATE Latin1_General_100_BIN2=@PayloadJson COLLATE Latin1_General_100_BIN2 OR (@ExistingPayloadJson IS NULL AND @PayloadJson IS NULL) THEN 0 ELSE 1 END=1");

        outboxGuard.Should().Contain(
                "IF @EventId IS NULL OR @CompanyId IS NULL OR @EntityName IS NULL OR @EntityGlobalId IS NULL OR @Operation IS NULL OR @PayloadJson IS NULL")
            .And.Contain("LocalOutbox envelope required fields cannot be null.")
            .And.ContainAll(
                "CASE WHEN @ExistingCompanyId=@CompanyId OR (@ExistingCompanyId IS NULL AND @CompanyId IS NULL) THEN 0 ELSE 1 END=1",
                "CASE WHEN @ExistingTargetCompanyId=@TargetCompanyId OR (@ExistingTargetCompanyId IS NULL AND @TargetCompanyId IS NULL) THEN 0 ELSE 1 END=1",
                "CASE WHEN @ExistingCausationEventId=@CausationEventId OR (@ExistingCausationEventId IS NULL AND @CausationEventId IS NULL) THEN 0 ELSE 1 END=1",
                "CASE WHEN @ExistingEntityName COLLATE Latin1_General_100_BIN2=@EntityName COLLATE Latin1_General_100_BIN2 OR (@ExistingEntityName IS NULL AND @EntityName IS NULL) THEN 0 ELSE 1 END=1",
                "CASE WHEN @ExistingEntityGlobalId=@EntityGlobalId OR (@ExistingEntityGlobalId IS NULL AND @EntityGlobalId IS NULL) THEN 0 ELSE 1 END=1",
                "CASE WHEN @ExistingEntityCode COLLATE Latin1_General_100_BIN2=@EntityCode COLLATE Latin1_General_100_BIN2 OR (@ExistingEntityCode IS NULL AND @EntityCode IS NULL) THEN 0 ELSE 1 END=1",
                "CASE WHEN @ExistingOperation COLLATE Latin1_General_100_BIN2=@Operation COLLATE Latin1_General_100_BIN2 OR (@ExistingOperation IS NULL AND @Operation IS NULL) THEN 0 ELSE 1 END=1",
                "CASE WHEN @ExistingPayloadJson COLLATE Latin1_General_100_BIN2=@PayloadJson COLLATE Latin1_General_100_BIN2 OR (@ExistingPayloadJson IS NULL AND @PayloadJson IS NULL) THEN 0 ELSE 1 END=1");

        inboxGuard.IndexOf("SyncInbox envelope required fields cannot be null.", StringComparison.Ordinal)
            .Should().BeLessThan(inboxGuard.IndexOf("FROM dbo.SyncInbox", StringComparison.Ordinal));
        outboxGuard.IndexOf("LocalOutbox envelope required fields cannot be null.", StringComparison.Ordinal)
            .Should().BeLessThan(outboxGuard.IndexOf("FROM dbo.LocalOutbox", StringComparison.Ordinal));
    }

    [Fact]
    public void CanonicalUpsert_ProtectsImmutableIdentityLegacyReviewAndConfirmedSapCode()
    {
        var procedure = Procedure("SP_NA_POST_BUSINESSPARTNER_CANONICAL_UPSERT");

        procedure.Should().ContainAll(
            "@ExistingCode", "@ExistingPartnerType", "@ExistingIdentificationTypeId",
            "@ExistingNormalizedIdentificationNumber", "@ExistingMasterSyncStatus",
            "@ExistingSapCardCode", "N'Both'", "'LegacyReview'",
            "Immutable BusinessPartner identity conflict", "Confirmed SapCardCode conflict")
            .And.Contain("IF NULLIF(LTRIM(RTRIM(@ExistingSapCardCode)),N'') IS NULL")
            .And.NotContain("SET SapCardCode = @SapCardCode\n        WHERE BusinessPartnerId");
    }

    [Fact]
    public void Foundation_UsesExplicitBinaryNormalizedCollationAndUnicodeVectors()
    {
        var sql = Read("database", "sql", "228_tenant_business_partner_bidirectional_foundation.sql");
        var readiness = Read("database", "sql", "manual", "check_business_partner_bidirectional_readiness.sql");

        sql.Should().Contain("NormalizedIdentificationNumber nvarchar(50) COLLATE Latin1_General_100_BIN2")
            .And.Contain("UPPER(LTRIM(RTRIM(IdentificationNumber)) COLLATE Latin1_General_100_BIN2)")
            .And.Contain("NCHAR(233)")
            .And.Contain("NCHAR(201)")
            .And.Contain("NCHAR(304)")
            .And.Contain("CK_BusinessPartners_CanonicalVersion CHECK (CanonicalVersion >= 0)");
        readiness.Should().Contain("COLLATE Latin1_General_100_BIN2");
    }

    [Fact]
    public void CanonicalQueries_EnumerateStableColumnsWithoutWildcardProjection()
    {
        var canonical = Procedure("SP_NA_GET_BUSINESSPARTNER_CANONICAL_FORUPDATE");
        var conflict = Procedure("SP_NA_GET_BUSINESSPARTNER_SYNCCONFLICT_BUSCARPORID");

        canonical.Should().ContainAll(
            "bp.Id AS Id", "bp.GlobalId AS GlobalId", "bp.RowVersion AS RowVersion",
            "addressItem.Id AS Id", "addressItem.GlobalId AS GlobalId",
            "contactItem.Id AS Id", "contactItem.GlobalId AS GlobalId")
            .And.NotContain("bp.*")
            .And.NotContain("addressItem.*")
            .And.NotContain("contactItem.*");
        conflict.Should().ContainAll(
            "conflict.Id AS Id", "conflict.BaseSnapshotJson AS BaseSnapshotJson",
            "conflict.RowVersion AS RowVersion", "bp.Code AS Code", "bp.Name AS Name")
            .And.NotContain("conflict.*");
    }

    [Fact]
    public void ReadinessReport_UsesTypedNullForMissingLegacyGlobalIdProjection()
    {
        var sql = Read("database", "sql", "manual", "check_business_partner_bidirectional_readiness.sql");

        sql.Should().Contain("CAST(NULL AS uniqueidentifier) AS GlobalId")
            .And.Contain("IF COL_LENGTH(N'dbo.BusinessPartners', N'GlobalId') IS NULL");
    }

    [Fact]
    public void Foundation_ValidatesColumnConstraintTableAndExactIndexShapes()
    {
        var sql = Read("database", "sql", "228_tenant_business_partner_bidirectional_foundation.sql");

        sql.Should().ContainAll(
            "system_type_id", "max_length", "is_nullable", "collation_name",
            "key_ordinal", "filter_definition", "is_unique", "is_not_trusted",
            "BusinessPartnerSyncConflicts has an incompatible shape",
            "UX_BusinessPartners_Identification_Active has an incompatible shape",
            "UX_BusinessPartners_SapCardCode_Active has an incompatible shape",
            "UX_BusinessPartnerAddresses_GlobalId has an incompatible shape",
            "UX_BusinessPartnerContacts_GlobalId has an incompatible shape")
            .And.Contain("THROW 52028")
            .And.NotContain("CanonicalVersion > 0");
    }

    [Fact]
    public void Foundation_ValidatesCompleteConflictMetadataExactDefinitionsAndStableChildDuplicates()
    {
        var sql = Read("database", "sql", "228_tenant_business_partner_bidirectional_foundation.sql");

        sql.Should().ContainAll(
                "UserTypeId", "PrecisionValue", "ScaleValue", "CollationName",
                "actual.user_type_id<>expected.UserTypeId",
                "actual.precision<>expected.PrecisionValue",
                "actual.scale<>expected.ScaleValue",
                "CASE WHEN actual.collation_name=expected.CollationName",
                "OR (actual.collation_name IS NULL AND expected.CollationName IS NULL)",
                "(N'CreatedAt',42,42,6,19,0,NULL,0)",
                "ExpectedDefinition", "NormalizedDefinition<>required.ExpectedDefinition",
                "N'isdeleted=0andisactive=1'",
                "N'sapcardcodeisnotnullandsapcardcode<>n''''",
                "Duplicate BusinessPartnerAddresses.GlobalId prevents unique index creation.",
                "Duplicate BusinessPartnerContacts.GlobalId prevents unique index creation.",
                "HAVING COUNT_BIG(1)>1")
            .And.NotContain("RequiredToken1")
            .And.NotContain("filter_definition LIKE");

        sql.IndexOf("Duplicate BusinessPartnerAddresses.GlobalId prevents unique index creation.", StringComparison.Ordinal)
            .Should().BeLessThan(sql.IndexOf("CREATE UNIQUE INDEX UX_BusinessPartnerAddresses_GlobalId", StringComparison.Ordinal));
        sql.IndexOf("Duplicate BusinessPartnerContacts.GlobalId prevents unique index creation.", StringComparison.Ordinal)
            .Should().BeLessThan(sql.IndexOf("CREATE UNIQUE INDEX UX_BusinessPartnerContacts_GlobalId", StringComparison.Ordinal));
    }

    [Fact]
    public void Foundation_CheckDefinitionNormalizationPreservesBooleanGrouping()
    {
        var sql = Read("database", "sql", "228_tenant_business_partner_bidirectional_foundation.sql");
        var baseCheckGuard = sql[
            sql.IndexOf("DECLARE @ExpectedBaseChecks", StringComparison.Ordinal)..
            sql.IndexOf("DECLARE @ExpectedBaseDefaults", StringComparison.Ordinal)];
        var conflictCheckGuard = sql[
            sql.IndexOf("DECLARE @ExpectedConflictChecks", StringComparison.Ordinal)..
            sql.IndexOf("BusinessPartnerSyncConflicts constraints have an incompatible shape.", StringComparison.Ordinal)];

        baseCheckGuard.Should().NotContain("N'(',N''")
            .And.NotContain("N')',N''");
        conflictCheckGuard.Should().NotContain("N'(',N''")
            .And.NotContain("N')',N''")
            .And.Contain(
                "(N'CK_BusinessPartnerSyncConflicts_ResolutionState',N'((status=''open''andresolutionisnullandresolvedatisnull)or(status=''resolved''andresolutionisnotnullandnullif(ltrim(rtrim(resolutionreason)),n'''')isnotnullandresolvedatisnotnull))')")
            .And.NotContain(
                "N'((status=''open''andresolutionisnullandresolvedatisnull)or(status=''resolved''))andresolutionisnotnullandnullif(ltrim(rtrim(resolutionreason)),n'''')isnotnullandresolvedatisnotnull'");
    }

    [Fact]
    public void SapCardCodeInputs_AreWideAndPreserveExistingConfirmedMappings()
    {
        string[] procedures =
        [
            "SP_NA_POST_BUSINESSPARTNER_CANONICAL_UPSERT",
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_ACCEPT",
            "SP_NA_POST_BUSINESSPARTNER_CANONICAL_APPLY",
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_RESULT_APPLY"
        ];

        foreach (var name in procedures)
        {
            var procedure = Procedure(name);
            procedure.Should().Contain("@SapCardCode nvarchar(50)");
        }

        Procedure("SP_NA_POST_BUSINESSPARTNER_CANONICAL_UPSERT")
            .Should().Contain("NULLIF(LTRIM(RTRIM(@ExistingSapCardCode)),N'') IS NULL")
            .And.Contain("DATALENGTH(@SapCardCode) > 30");
        Procedure("SP_NA_POST_BUSINESSPARTNER_PROPOSAL_ACCEPT")
            .Should().NotContain("IF DATALENGTH(@SapCardCode) > 30");
        Procedure("SP_NA_POST_BUSINESSPARTNER_CANONICAL_APPLY")
            .Should().Contain("DATALENGTH(@SapCardCode) > 30");
        Procedure("SP_NA_POST_BUSINESSPARTNER_PROPOSAL_RESULT_APPLY")
            .Should().Contain("DATALENGTH(@SapCardCode) > 30");
    }

    [Fact]
    public void MasterGovernance_RegistersDirectionsPolicyAndClosedRouting()
    {
        var sql = Read("database", "sql", "229_master_business_partner_bidirectional_governance.sql");

        sql.Should().Contain("BusinessPartnerSapCodePolicies")
            .And.Contain("BranchToMaster")
            .And.Contain("BusinessPartnerProposal")
            .And.Contain("BusinessPartnerProposalResult")
            .And.Contain("TargetCompanyId")
            .And.Contain("ParentCompanyId")
            .And.Contain("SYNC.BUSINESS_PARTNER_CONFLICTS.VIEW")
            .And.Contain("SYNC.BUSINESS_PARTNER_CONFLICTS.RESOLVE");
    }

    [Fact]
    public void MasterGovernance_AllowsOnlyExactProductionOrSessionBoundDisposableTestDatabase()
    {
        var sql = Read("database", "sql", "229_master_business_partner_bidirectional_governance.sql");
        var firstPrerequisite = sql.IndexOf("IF OBJECT_ID(N'dbo.Companies'", StringComparison.Ordinal);
        var firstDdl = sql.IndexOf("CREATE TABLE dbo.BusinessPartnerSapCodePolicies", StringComparison.Ordinal);
        var productionGuard = sql.IndexOf("DB_NAME()=N'NuanSystem_Master'", StringComparison.Ordinal);
        var testPrefixGuard = sql.IndexOf("N'NuanSystem[_]Test[_]Master[_]%'", StringComparison.Ordinal);
        var sessionGuard = sql.IndexOf(
            "SESSION_CONTEXT(N'NUANSYSTEM_INTEGRATION_TEST_MASTER_DATABASE')",
            StringComparison.Ordinal);

        productionGuard.Should().BeGreaterThan(-1).And.BeLessThan(firstPrerequisite);
        testPrefixGuard.Should().BeGreaterThan(-1).And.BeLessThan(firstPrerequisite);
        sessionGuard.Should().BeGreaterThan(-1).And.BeLessThan(firstPrerequisite);
        firstPrerequisite.Should().BeGreaterThan(sessionGuard).And.BeLessThan(firstDdl);
        sql.Should().Contain("CONVERT(nvarchar(128), SESSION_CONTEXT")
            .And.Contain("=DB_NAME()")
            .And.NotContain("LIKE N'NuanSystem_Test_Master_%'");
    }

    [Fact]
    public void SqlIntegrationHarness_IsExplicitOptInSerializedAndUsesOnlyDisposableNames()
    {
        var source = Read(
            "tests", "NuanSystem.Application.Tests", "Features", "Sync",
            "BusinessPartnerBidirectionalSqlIntegrationTests.cs");

        source.Should().Contain("[SqlServerIntegrationFact]")
            .And.Contain("DisableParallelization = true")
            .And.Contain("NUANSYSTEM_SQL_INTEGRATION_ADMIN_CONNECTION")
            .And.Contain("Initial Catalog=master")
            .And.Contain("^NuanSystem_Test_Master_[0-9a-f]{32}$")
            .And.Contain("^NuanSystem_Test_Tenant_(Central|BranchA|BranchB)_[0-9a-f]{32}$")
            .And.Contain("createdDatabaseRegistry")
            .And.Contain("Disposable database name already exists")
            .And.NotContain("appsettings.json")
            .And.NotContain("Console.Write")
            .And.NotContain("NuanSystem_Prod");
    }

    [Fact]
    public void SqlIntegrationHarness_BindsReadOnlySessionContextAndVerifiesOwnershipBeforeCleanup()
    {
        var source = Read(
            "tests", "NuanSystem.Application.Tests", "Features", "Sync",
            "BusinessPartnerBidirectionalSqlIntegrationTests.cs");
        var markerCheck = source.IndexOf("VerifyMarkerAsync(databaseName)", StringComparison.Ordinal);
        var drop = source.IndexOf("DROP DATABASE", StringComparison.Ordinal);

        source.Should().Contain("NUANSYSTEM_INTEGRATION_TEST_MASTER_DATABASE")
            .And.Contain("@read_only=1")
            .And.Contain("NuanSystemIntegrationTestMarker")
            .And.Contain("RunId=@RunId AND DatabaseName=@DatabaseName")
            .And.Contain("Database is not in this fixture's exact creation registry")
            .And.Contain("Fixture refuses script batch with USE")
            .And.Contain("MasterMigration_RejectsAbsentOrMismatchedBindingAndAcceptsReadOnlyExactContext")
            .And.Contain("ExecuteMasterMigrationWithMismatchedBindingAsync")
            .And.Contain("sys.sp_getapplock")
            .And.Contain("sys.sp_releaseapplock")
            .And.Contain("SqlCommandBuilder")
            .And.Contain("QuoteIdentifier(databaseName)")
            .And.Contain("SQL fixture initialization and safe cleanup both failed");
        markerCheck.Should().BeGreaterThan(-1).And.BeLessThan(drop);
    }

    [Fact]
    public void SqlIntegrationHarness_CoversMigrationRerunRoleUniquenessVersionsInboxOutboxAndRollback()
    {
        var source = Read(
            "tests", "NuanSystem.Application.Tests", "Features", "Sync",
            "BusinessPartnerBidirectionalSqlIntegrationTests.cs");

        source.Should().ContainAll(
                "228_tenant_business_partner_bidirectional_foundation.sql",
                "229_master_business_partner_bidirectional_governance.sql",
                "230_tenant_business_partner_bidirectional_operations.sql",
                "UX_BusinessPartners_Identification_Active",
                "SP_NA_POST_BUSINESSPARTNER_CANONICAL_UPSERT",
                "CanonicalVersion",
                "SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE",
                "SP_NA_POST_BUSINESSPARTNER_LOCALOUTBOX_ENSURE",
                "RollbackAsync",
                "ClearAllPools")
            .And.Contain("Canonical identification belongs to another BusinessPartner");
    }

    [Fact]
    public void MasterPolicy_IsDisabledByDefaultCentralOnlyAuditedAndSecretFree()
    {
        var sql = Read("database", "sql", "229_master_business_partner_bidirectional_governance.sql");
        var get = MasterProcedure("SP_NA_GET_BUSINESSPARTNERSAPCODEPOLICY_BUSCARPOREMPRESAID");
        var upsert = MasterProcedure("SP_NA_PUT_BUSINESSPARTNERSAPCODEPOLICY_GUARDAR");

        sql.Should().ContainAll(
            "CompanyId int NOT NULL CONSTRAINT PK_BusinessPartnerSapCodePolicies PRIMARY KEY",
            "IsEnabled bit NOT NULL CONSTRAINT DF_BusinessPartnerSapCodePolicies_IsEnabled DEFAULT (0)",
            "PrefixMode varchar(20) NOT NULL",
            "PassportIdentificationTypeCode nvarchar(30) NOT NULL",
            "RowVersion rowversion NOT NULL",
            "CK_BusinessPartnerSapCodePolicies_PrefixMode CHECK (PrefixMode IN ('NationalForeign','RoleOnly'))");
        get.Should().ContainAll(
                "policy.CompanyId", "policy.IsEnabled", "policy.PrefixMode",
                "policy.PassportIdentificationTypeCode", "policy.UpdatedAt", "policy.RowVersion",
                "company.IsActive=1", "company.IsMaster=1", "company.IsDeleted=0")
            .And.NotContain("Password")
            .And.NotContain("Secret")
            .And.NotContain("ConnectionString")
            .And.NotContain("Token");
        upsert.Should().ContainAll(
            "@ExpectedRowVersion varbinary(8)=NULL", "WITH (UPDLOCK,HOLDLOCK)",
            "company.IsActive=1", "company.IsMaster=1", "company.IsDeleted=0",
            "THROW 52229", "AuditSyncConfigurationChanges", "BEGIN TRANSACTION", "ROLLBACK TRANSACTION");
    }

    [Fact]
    public void MasterGovernance_AddsCausationAndDoesNotActivateProfilesFlagsOrDestinations()
    {
        var sql = Read("database", "sql", "229_master_business_partner_bidirectional_governance.sql");

        sql.Should().Contain("ALTER TABLE dbo.SyncOutbox ADD CausationEventId uniqueidentifier NULL")
            .And.Contain("N'BusinessPartnerProposal', N'Propuestas de socios de negocio'")
            .And.Contain("N'BusinessPartnerProposalResult', N'Resultados de propuestas de socios de negocio'")
            .And.Contain("DefaultModifiedAtField,IsSystem,IsActive")
            .And.Contain("NULL,1,0,N'Sistema'")
            .And.NotContain("N'BUSINESS-PARTNER-BIDIRECTIONAL'")
            .And.NotContain("SET IsActive=1 WHERE Id")
            .And.NotContain("INSERT dbo.SyncEntityConfigurations")
            .And.NotContain("INSERT INTO dbo.SyncEntityConfigurations")
            .And.NotContain("INSERT dbo.SyncOutboxTargets")
            .And.NotContain("INSERT INTO dbo.SyncOutboxTargets")
            .And.NotContain("SapCompanySettings")
            .And.NotContain("SapSyncOutbox")
            .And.NotContain("SriDocument")
            .And.NotContain("ServiceLayer");
    }

    [Fact]
    public void MasterGovernance_SeedsConflictPermissionsOnlyForAdminAndIsRegisteredAfter227()
    {
        var sql = Read("database", "sql", "229_master_business_partner_bidirectional_governance.sql");
        var initializer = Read(
            "src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerMasterDatabaseInitializer.cs");
        var readme = Read("database", "sql", "README.md");

        sql.Should().ContainAll(
                "N'SYNC.BUSINESS_PARTNER_CONFLICTS.VIEW'",
                "N'SYNC.BUSINESS_PARTNER_CONFLICTS.RESOLVE'",
                "FROM dbo.Roles WHERE Code=N'ADMIN' AND IsDeleted=0",
                "INSERT dbo.RolePermissions(RoleId,PermissionId)")
            .And.NotContain("CROSS JOIN dbo.Roles");

        initializer.IndexOf(
                "229_master_business_partner_bidirectional_governance.sql",
                StringComparison.Ordinal)
            .Should().BeGreaterThan(initializer.IndexOf(
                "227_master_definitions_inventory_sales_channels_navigation.sql",
                StringComparison.Ordinal));
        readme.IndexOf("228_tenant_business_partner_bidirectional_foundation.sql", StringComparison.Ordinal)
            .Should().BeLessThan(readme.IndexOf("229_master_business_partner_bidirectional_governance.sql", StringComparison.Ordinal));
        readme.IndexOf("229_master_business_partner_bidirectional_governance.sql", StringComparison.Ordinal)
            .Should().BeLessThan(readme.IndexOf("230_tenant_business_partner_bidirectional_operations.sql", StringComparison.Ordinal));
    }

    private static string Read(params string[] parts) =>
        File.ReadAllText(Path.Combine([Root(), .. parts]));

    private static string Procedure(string name)
    {
        var sql = Read("database", "sql", "230_tenant_business_partner_bidirectional_operations.sql");
        var match = Regex.Match(
            sql,
            $@"CREATE OR ALTER PROCEDURE dbo\.{Regex.Escape(name)}\b[\s\S]*?\r?\nGO\b",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        match.Success.Should().BeTrue($"procedure {name} must exist as its own SQL batch");
        return match.Value;
    }

    private static string MasterProcedure(string name)
    {
        var sql = Read("database", "sql", "229_master_business_partner_bidirectional_governance.sql");
        var match = Regex.Match(
            sql,
            $@"CREATE OR ALTER PROCEDURE dbo\.{Regex.Escape(name)}\b[\s\S]*?\r?\nGO\b",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        match.Success.Should().BeTrue($"procedure {name} must exist as its own SQL batch");
        return match.Value;
    }

    private static string Root()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
            directory = directory.Parent;

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Repository root not found.");
    }
}
