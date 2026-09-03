using FluentAssertions;
using NuanSystem.Application.Features.BusinessPartners.Commands;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.BusinessPartners.Exceptions;
using NuanSystem.Persistence.Repositories;

namespace NuanSystem.Application.Tests.Features.BusinessPartners;

public sealed class BusinessPartnerCommandPolicyTests
{
    private static readonly string[] SapManagedFields =
    [
        "SapCardCode", "SapCardType", "SapSyncStatus", "SapLastSyncAt", "SapLastError",
        "SapEnabled", "SapMode", "SapCompanyCode", "SapRetryCount", "SyncAsSupplier",
        "AllowManualSapRetry", "RequiresApprovalBeforeSapSync"
    ];

    [Fact]
    public void PublicCommands_DoNotExposeGeneratedImmutableOrSapManagedFields()
    {
        typeof(CreateBusinessPartnerCommand).GetProperty("Code").Should().BeNull();
        typeof(UpdateBusinessPartnerCommand).GetProperties().Select(property => property.Name)
            .Should().NotContain(["Code", "PartnerType", "IdentificationTypeId", "IdentificationNumber"]);

        foreach (var property in SapManagedFields)
        {
            typeof(CreateBusinessPartnerCommand).GetProperty(property).Should().BeNull();
            typeof(UpdateBusinessPartnerCommand).GetProperty(property).Should().BeNull();
        }
    }

    [Fact]
    public void PublicContracts_ExposeConcurrencyCanonicalAndStableChildIdentities()
    {
        typeof(UpdateBusinessPartnerCommand).GetProperty("ExpectedRowVersion").Should().NotBeNull();
        typeof(DeleteBusinessPartnerCommand).GetProperty("ExpectedRowVersion").Should().NotBeNull();
        typeof(BusinessPartnerDto).GetProperty("NormalizedIdentificationNumber").Should().NotBeNull();
        typeof(BusinessPartnerDto).GetProperty("CanonicalVersion").Should().NotBeNull();
        typeof(BusinessPartnerDto).GetProperty("RowVersion").Should().NotBeNull();
        typeof(BusinessPartnerDto).GetProperty("MasterSyncStatus").Should().NotBeNull();
        typeof(BusinessPartnerDto).GetProperty("MasterSyncMessage").Should().NotBeNull();
        typeof(BusinessPartnerAddressDto).GetProperty("GlobalId").Should().NotBeNull();
        typeof(BusinessPartnerAddressDto).GetProperty("ProvinceCode").Should().NotBeNull();
        typeof(BusinessPartnerAddressDto).GetProperty("CityCode").Should().NotBeNull();
        typeof(BusinessPartnerContactDto).GetProperty("GlobalId").Should().NotBeNull();
        typeof(BusinessPartnerContactDto).GetProperty("ContactTypeCode").Should().NotBeNull();
        typeof(BusinessPartnerContactDto).GetProperty("ContactChannelCode").Should().NotBeNull();
        typeof(BusinessPartnerLookupsDto).GetProperty("EditPolicy").Should().NotBeNull();
        typeof(BusinessPartnerUniqueConflictException).Assembly.GetReferencedAssemblies()
            .Should().NotContain(reference => reference.Name == "Microsoft.Data.SqlClient");
    }

    [Fact]
    public void TenantCrud230_UsesApplicationIdentityRoleAwareUniquenessAndOptimisticConcurrency()
    {
        var sql = Read("database", "sql", "230_tenant_business_partner_bidirectional_operations.sql");

        var create = Procedure(sql, "SP_NA_POST_BUSINESSPARTNERS_CREAR");
        create.Should().ContainAll("@GlobalId uniqueidentifier", "@NormalizedIdentificationNumber nvarchar(50)",
            "@CanonicalVersion bigint", "@MasterSyncStatus varchar(20)",
            "SP_NA_POST_BUSINESSPARTNER_CHILDREN_APPLY")
            .And.NotContain("SP_NA_POST_BUSINESSPARTNER_CANONICAL_UPSERT");

        var update = Procedure(sql, "SP_NA_PUT_BUSINESSPARTNERS_ACTUALIZAR");
        update.Should().ContainAll("@ExpectedRowVersion varbinary(8)", "WITH (UPDLOCK,HOLDLOCK)",
            "RowVersion=@ExpectedRowVersion", "CanonicalVersion=@CanonicalVersion",
            "MasterSyncStatus=@MasterSyncStatus", "SP_NA_POST_BUSINESSPARTNER_CHILDREN_APPLY")
            .And.NotContain("SET Code=@Code")
            .And.NotContain("SET PartnerType=@PartnerType")
            .And.NotContain("SET IdentificationTypeId=@IdentificationTypeId");

        var delete = Procedure(sql, "SP_NA_DELETE_BUSINESSPARTNERS_ELIMINAR");
        delete.Should().ContainAll("@ExpectedRowVersion varbinary(8)", "WITH (UPDLOCK,HOLDLOCK)",
            "RowVersion=@ExpectedRowVersion", "CanonicalVersion=@CanonicalVersion");

        Procedure(sql, "SP_NA_GET_BUSINESSPARTNERS_BUSCARPORIDENTIFICACION")
            .Should().ContainAll("@PartnerType nvarchar(20)", "@NormalizedIdentificationNumber nvarchar(50)",
                "PartnerType=@PartnerType", "NormalizedIdentificationNumber=@NormalizedIdentificationNumber",
                "IsActive=1", "IsDeleted=0");
        Procedure(sql, "SP_NA_POST_BUSINESSPARTNER_CHILDREN_APPLY")
            .Should().Contain("$.globalId");
    }

    [Fact]
    public void DapperBoundary_UsesCrud230ParametersAndConvertsReadRowVersionToBase64()
    {
        var repository = Read("src", "Backend", "NuanSystem.Persistence", "Repositories", "BusinessPartnerRepository.cs");
        repository.Should().ContainAll(
            "partner.GlobalId", "partner.NormalizedIdentificationNumber", "partner.CanonicalVersion",
            "partner.MasterSyncStatus", "partner.ExpectedRowVersion",
            "Convert.ToBase64String(metadata.RowVersion)",
            "IdentificationTypeCodeProcedure");

        var updateParameters = repository[
            repository.IndexOf("private static object ToParameters(UpdateBusinessPartnerData", StringComparison.Ordinal)..
            repository.IndexOf("private sealed class BusinessPartnerCanonicalMetadataRow", StringComparison.Ordinal)];
        updateParameters.Should().NotContain("partner.Code")
            .And.NotContain("partner.IdentificationNumber")
            .And.NotContain("partner.SapCardCode")
            .And.Contain("partner.ExpectedRowVersion");
    }

    [Fact]
    public void LocalReads230_ReturnAggregateCanonicalMetadataAndRowVersionAtomically()
    {
        var sql = Read("database", "sql", "230_tenant_business_partner_bidirectional_operations.sql");
        var repository = Read("src", "Backend", "NuanSystem.Persistence", "Repositories", "BusinessPartnerRepository.cs");

        var list = Procedure(sql, "SP_NA_GET_BUSINESSPARTNERS_LISTAR");
        list.Should().ContainAll(
            "bp.NormalizedIdentificationNumber AS NormalizedIdentificationNumber",
            "bp.CanonicalVersion AS CanonicalVersion",
            "bp.MasterSyncStatus AS MasterSyncStatus",
            "bp.MasterSyncMessage AS MasterSyncMessage",
            "bp.RowVersion AS RowVersion");

        var detail = Procedure(sql, "SP_NA_GET_BUSINESSPARTNERS_BUSCARPORID");
        detail.Should().ContainAll(
                "bp.NormalizedIdentificationNumber AS NormalizedIdentificationNumber",
                "bp.CanonicalVersion AS CanonicalVersion",
                "bp.MasterSyncStatus AS MasterSyncStatus",
                "bp.MasterSyncMessage AS MasterSyncMessage",
                "bp.RowVersion AS RowVersion",
                "addressItem.GlobalId AS GlobalId", "addressItem.IsDeleted=0",
                "contactItem.GlobalId AS GlobalId", "contactItem.IsDeleted=0",
                "province.Code AS ProvinceCode", "city.Code AS CityCode",
                "contactType.Code AS ContactTypeCode", "contactChannel.Code AS ContactChannelCode");
        repository.Should().Contain("splitOn: \"NormalizedIdentificationNumber\"")
            .And.NotContain("CanonicalMetadataListProcedure")
            .And.NotContain("LocalReadProcedure");
    }

    [Fact]
    public void TenantCrud230_ClassifiesOnlyNamedUniqueIndexCollisions()
    {
        var sql = Read("database", "sql", "230_tenant_business_partner_bidirectional_operations.sql");

        var create = Procedure(sql, "SP_NA_POST_BUSINESSPARTNERS_CREAR");
        create.Should().ContainAll(
                "@IsActive=1 AND EXISTS",
                "WITH (UPDLOCK,HOLDLOCK,INDEX(UX_BusinessPartners_Identification_Active))",
                "WITH (UPDLOCK,HOLDLOCK,INDEX(UX_BusinessPartners_Code_Active))",
                "WITH (UPDLOCK,HOLDLOCK,INDEX(UX_BusinessPartners_SapCardCode_Active))",
                "SELECT CAST(-1 AS int)",
                "SELECT CAST(-2 AS int)",
                "SELECT CAST(-3 AS int)")
            .And.Contain("THROW;");
        CatchBlock(create).Should().ContainAll(
                "IF @StartedTransaction=1 AND XACT_STATE()<>0 ROLLBACK TRANSACTION",
                "ELSE IF @StartedTransaction=0 AND XACT_STATE()=1 ROLLBACK TRANSACTION BPCreate230",
                "THROW;")
            .And.NotContain("ERROR_NUMBER()")
            .And.NotContain("ERROR_MESSAGE()")
            .And.NotContain("SELECT CAST(-");

        var update = Procedure(sql, "SP_NA_PUT_BUSINESSPARTNERS_ACTUALIZAR");
        update.Should().ContainAll(
                "@CurrentIsActive=0 AND @IsActive=1 AND EXISTS",
                "WITH (UPDLOCK,HOLDLOCK,INDEX(UX_BusinessPartners_Identification_Active))",
                "SELECT CAST(-1 AS int)")
            .And.Contain("THROW;");
        CatchBlock(update).Should().ContainAll(
                "IF @StartedTransaction=1 AND XACT_STATE()<>0 ROLLBACK TRANSACTION",
                "ELSE IF @StartedTransaction=0 AND XACT_STATE()=1 ROLLBACK TRANSACTION BPUpdate230",
                "THROW;")
            .And.NotContain("ERROR_NUMBER()")
            .And.NotContain("ERROR_MESSAGE()")
            .And.NotContain("SELECT CAST(-");

        var repository = Read("src", "Backend", "NuanSystem.Persistence", "Repositories", "BusinessPartnerRepository.cs");
        repository.Should().Contain("ExecuteScalarAsync<int>")
            .And.Contain("Task<int> UpdateAsync");
    }

    [Theory]
    [InlineData(2601, "Cannot insert duplicate key row with unique index 'UX_BusinessPartners_Identification_Active'.", BusinessPartnerUniqueConflictKind.Identification)]
    [InlineData(2627, "Violation of UNIQUE KEY constraint 'UX_BusinessPartners_Code_Active'.", BusinessPartnerUniqueConflictKind.Code)]
    [InlineData(2601, "Cannot insert duplicate key row with unique index 'UX_BusinessPartners_SapCardCode_Active'.", BusinessPartnerUniqueConflictKind.SapCardCode)]
    public void Persistence_ClassifiesOnlyExactBusinessPartnerUniqueIndexes(
        int errorNumber,
        string message,
        BusinessPartnerUniqueConflictKind expectedKind)
    {
        var classified = BusinessPartnerRepository.TryClassifyUniqueConflict(errorNumber, message, out var actualKind);

        classified.Should().BeTrue();
        actualKind.Should().Be(expectedKind);
    }

    [Theory]
    [InlineData(547, "Violation of UNIQUE KEY constraint 'UX_BusinessPartners_Code_Active'.")]
    [InlineData(2601, "Cannot insert duplicate key row with unique index 'UX_BusinessPartners_Code_Active_Copy'.")]
    [InlineData(2627, "Violation of UNIQUE KEY constraint 'UX_OtherEntity_Code_Active'.")]
    public void Persistence_DoesNotClassifyUnrelatedOrSimilarlyNamedSqlErrors(int errorNumber, string message)
    {
        BusinessPartnerRepository.TryClassifyUniqueConflict(errorNumber, message, out _).Should().BeFalse();
    }

    [Fact]
    public void DeleteValidator_RequiresAnEightByteBase64RowVersion()
    {
        var validator = new DeleteBusinessPartnerCommandValidator();

        validator.Validate(new DeleteBusinessPartnerCommand(1, "AQID")).IsValid.Should().BeFalse();
        validator.Validate(new DeleteBusinessPartnerCommand(1, "AQIDBAUGBwg=")).IsValid.Should().BeTrue();
    }

    private static string Procedure(string sql, string name)
    {
        var start = sql.IndexOf($"CREATE OR ALTER PROCEDURE dbo.{name}", StringComparison.Ordinal);
        start.Should().BeGreaterThanOrEqualTo(0);
        var end = sql.IndexOf("\nGO", start, StringComparison.Ordinal);
        end.Should().BeGreaterThan(start);
        return sql[start..end];
    }

    private static string CatchBlock(string procedure)
    {
        var start = procedure.LastIndexOf("BEGIN CATCH", StringComparison.Ordinal);
        start.Should().BeGreaterThanOrEqualTo(0);
        return procedure[start..];
    }

    private static string Read(params string[] segments)
    {
        var root = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine([root, "NuanSystem.sln"])))
        {
            root = Directory.GetParent(root)?.FullName
                ?? throw new InvalidOperationException("Repository root not found.");
        }

        return File.ReadAllText(Path.Combine([root, .. segments]));
    }
}
