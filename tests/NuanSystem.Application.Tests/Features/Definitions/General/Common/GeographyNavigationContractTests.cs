using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.Definitions.General.Common;

public sealed class GeographyNavigationContractTests
{
    [Fact]
    public void Migration171_ShouldPlaceGeographyUnderDefinitionsGeneralWithoutChangingFormKeys()
    {
        var sql = Read("database", "sql", "171_master_definitions_general_geography_navigation.sql");
        var shell = Read(
            "src", "Frontend", "NuanSystem.WinForms.ViewModels", "Shell", "ShellViewModel.cs");
        var initializer = Read(
            "src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerMasterDatabaseInitializer.cs");

        sql.Should().Contain("N'MENU.DEFINITIONS'")
            .And.Contain("N'Definiciones'")
            .And.Contain("N'MENU.GEOGRAPHY'")
            .And.Contain("Name = N'General'")
            .And.Contain("N'MENU.GEOGRAPHY.COUNTRIES'")
            .And.Contain("N'MENU.GEOGRAPHY.PROVINCES'")
            .And.Contain("N'MENU.GEOGRAPHY.CITIES'")
            .And.Contain("INSERT dbo.SecurityRoleMenus")
            .And.Contain("existing.IsAllowed = 1")
            .And.NotContain("DELETE FROM dbo.SecurityMenus")
            .And.NotContain("RolePermissions")
            .And.NotContain("SecurityRoleFormOperations");

        shell.Should().Contain("\"Modulo de configuracion / Definiciones / General\", \"Países\"")
            .And.Contain("\"countries\"")
            .And.Contain("\"provinces\"")
            .And.Contain("\"cities\"");

        initializer.Should().Contain("035_master_geography_security.sql")
            .And.Contain("171_master_definitions_general_geography_navigation.sql");
    }

    [Fact]
    public void Migration178_ShouldRepairConfigurationGeneralHierarchyAndAdminAccess()
    {
        var sql = Read(
            "database", "sql", "178_master_configuration_definitions_general_navigation_repair.sql");
        var initializer = Read(
            "src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerMasterDatabaseInitializer.cs");

        sql.Should().Contain("N'MENU.CONFIGURATION'")
            .And.Contain("ParentId = @ConfigurationMenuId")
            .And.Contain("N'MENU.DEFINITIONS'")
            .And.Contain("ParentId = @DefinitionsMenuId")
            .And.Contain("Code = N'MENU.GENERAL'")
            .And.Contain("Name = N'General'")
            .And.Contain("N'MENU.GENERAL.COUNTRIES'")
            .And.Contain("N'MENU.GENERAL.PROVINCES'")
            .And.Contain("N'MENU.GENERAL.CITIES'")
            .And.Contain("Code = N'ADMIN'")
            .And.Contain("MERGE dbo.SecurityRoleMenus")
            .And.Contain("MERGE dbo.SecurityRoleFormOperations")
            .And.Contain("N'countries', N'provinces', N'cities'")
            .And.Contain("N'new'")
            .And.Contain("N'edit'")
            .And.Contain("N'20260805.178'")
            .And.NotContain("DELETE FROM dbo.SecurityMenus")
            .And.NotContain("RolePermissions");

        initializer.Should().Contain(
            "178_master_configuration_definitions_general_navigation_repair.sql");
    }

    private static string Read(params string[] parts)
    {
        var root = FindRepositoryRoot();
        return File.ReadAllText(Path.Combine(new[] { root }.Concat(parts).ToArray()));
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, "database")))
            directory = directory.Parent;

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("No se encontró la raíz del repositorio.");
    }
}
