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
            .And.Contain("CanonicalVersion > @CanonicalVersion")
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

    private static string Read(params string[] parts) =>
        File.ReadAllText(Path.Combine([Root(), .. parts]));

    private static string Root()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
            directory = directory.Parent;

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Repository root not found.");
    }
}
