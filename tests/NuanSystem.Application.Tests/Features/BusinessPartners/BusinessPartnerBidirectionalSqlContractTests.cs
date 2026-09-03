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
    public void ProposalResultApply_UsesLockedHigherEqualAndLowerVersionOutcomes()
    {
        var procedure = Procedure("SP_NA_POST_BUSINESSPARTNER_PROPOSAL_RESULT_APPLY");

        procedure.Should().Contain("FROM dbo.BusinessPartners WITH (UPDLOCK,HOLDLOCK)")
            .And.Contain("@CurrentVersion > @CanonicalVersion")
            .And.Contain("@CurrentVersion = @CanonicalVersion")
            .And.Contain("@CurrentVersion < @CanonicalVersion")
            .And.Contain("SELECT 3 AS ResultCode")
            .And.NotContain("CanonicalVersion=CASE");
    }

    [Fact]
    public void ConflictResolver_RequiresConflictTokenAndRejectsAStaleLivePartner()
    {
        var procedure = Procedure("SP_NA_POST_BUSINESSPARTNER_SYNCCONFLICT_RESOLVER");

        procedure.Should().Contain("IF @ExpectedRowVersion IS NULL")
            .And.Contain("FROM dbo.BusinessPartners WITH (UPDLOCK,HOLDLOCK)")
            .And.Contain("@LiveCanonicalVersion <> @CurrentVersion")
            .And.Contain("@LiveBusinessPartnerRowVersion<>@PresentedBusinessPartnerRowVersion")
            .And.Contain("SELECT 4 AS ResultCode")
            .And.Contain("@CanonicalVersion=@LiveCanonicalVersion+1")
            .And.NotContain("@CanonicalVersion=@CurrentVersion+1");
    }

    [Fact]
    public void EventEnvelopeGuards_DeadLetterCollisionsAndPreserveIdenticalReplays()
    {
        var inboxGuard = Procedure("SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE");
        var outboxGuard = Procedure("SP_NA_POST_BUSINESSPARTNER_LOCALOUTBOX_ENSURE");

        inboxGuard.Should().ContainAll(
            "SourceCompanyId", "EntityName", "EntityGlobalId", "Operation", "PayloadJson",
            "COLLATE Latin1_General_100_BIN2", "Status=N'DeadLetter'", "@EnvelopeResult=4");
        outboxGuard.Should().ContainAll(
            "CompanyId", "TargetCompanyId", "CausationEventId", "EntityName", "EntityGlobalId",
            "EntityCode", "Operation", "PayloadJson", "COLLATE Latin1_General_100_BIN2",
            "Status=N'DeadLetter'", "@EnvelopeResult=4");

        string[] inboxProcedures =
        [
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_ACCEPT",
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_CONFLICT",
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_REJECT",
            "SP_NA_POST_BUSINESSPARTNER_CANONICAL_APPLY",
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_RESULT_APPLY"
        ];
        foreach (var name in inboxProcedures)
            Procedure(name).Should().Contain("EXEC dbo.SP_NA_POST_BUSINESSPARTNER_SYNCINBOX_ENSURE")
                .And.Contain("SELECT 4 AS ResultCode");

        string[] outboxProcedures =
        [
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_ACCEPT",
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_CONFLICT",
            "SP_NA_POST_BUSINESSPARTNER_PROPOSAL_REJECT",
            "SP_NA_POST_BUSINESSPARTNER_SYNCCONFLICT_RESOLVER"
        ];
        foreach (var name in outboxProcedures)
            Procedure(name).Should().Contain("EXEC dbo.SP_NA_POST_BUSINESSPARTNER_LOCALOUTBOX_ENSURE")
                .And.Contain("SELECT 4 AS ResultCode");
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
    public void SapCardCodeInputs_AreWideAndValidatedBeforeCanonicalPersistence()
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
            procedure.Should().Contain("@SapCardCode nvarchar(50)")
                .And.Contain("DATALENGTH(@SapCardCode) > 30");
        }
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

    private static string Root()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
            directory = directory.Parent;

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Repository root not found.");
    }
}
