using System.Text.RegularExpressions;
using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapMasterDataMappingSqlContractTests
{
    [Fact]
    public void Master_migrations_define_idempotent_versioned_query_and_mapping_tables()
    {
        var querySql = Read("database", "sql", "231_master_sap_query_definitions.sql");
        var mappingSql = Read("database", "sql", "232_master_sap_field_mappings.sql");

        querySql.Should().Contain("Version = N'20260904.231'")
            .And.Contain("IF OBJECT_ID(N'dbo.SapQueryDefinitions', N'U') IS NULL")
            .And.Contain("CREATE TABLE dbo.SapQueryDefinitions")
            .And.Contain("IsApproved bit NOT NULL")
            .And.Contain("ISJSON(ParameterSchemaJson) = 1")
            .And.Contain("UX_SapQueryDefinitions_Entity_Transport_Version")
            .And.NotContain("DROP TABLE")
            .And.NotContain("DELETE FROM");

        mappingSql.Should().Contain("Version = N'20260904.232'")
            .And.Contain("IF OBJECT_ID(N'dbo.SapFieldMappings', N'U') IS NULL")
            .And.Contain("CREATE TABLE dbo.SapFieldMappings")
            .And.Contain("FK_SapFieldMappings_ProfileEntity")
            .And.Contain("CK_SapFieldMappings_Direction")
            .And.Contain("UX_SapFieldMappings_Current")
            .And.NotContain("DROP TABLE")
            .And.NotContain("DELETE FROM");

        Regex.Matches(querySql, "\\bGO\\b", RegexOptions.IgnoreCase).Count.Should().BeGreaterThanOrEqualTo(2);
        Regex.Matches(mappingSql, "\\bGO\\b", RegexOptions.IgnoreCase).Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void Mapping_tables_are_master_owned_and_do_not_reference_tenant_objects()
    {
        var querySql = Read("database", "sql", "231_master_sap_query_definitions.sql");
        var mappingSql = Read("database", "sql", "232_master_sap_field_mappings.sql");

        (querySql + mappingSql).Should().NotContain("SapSyncOutbox")
            .And.NotContain("NuanSystem_DEMO")
            .And.NotContain("Tenant");
    }

    [Fact]
    public void Master_initializer_registers_the_two_migrations_in_order()
    {
        var initializer = Read(
            "src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");
        var queryIndex = initializer.IndexOf("231_master_sap_query_definitions.sql", StringComparison.Ordinal);
        var mappingIndex = initializer.IndexOf("232_master_sap_field_mappings.sql", StringComparison.Ordinal);
        var priorIndex = initializer.IndexOf("227_master_definitions_inventory_sales_channels_navigation.sql", StringComparison.Ordinal);

        queryIndex.Should().BeGreaterThan(priorIndex);
        mappingIndex.Should().BeGreaterThan(queryIndex);
    }

    private static string Read(params string[] path)
        => File.ReadAllText(Path.Combine([Root(), .. path]));

    private static string Root()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
    }
}
