using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapSyncProfileRibbonIconContractTests
{
    [Fact]
    public void Migration167_MapsProfileActionsToCorporateRibbonIcons()
    {
        var sql = Read("database", "sql", "167_master_sap_sync_profile_ribbon_icons.sql");
        var initializer = Read("src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");
        var expected = new[]
        {
            (Operation: "ACTION.SAP_SYNC_PROFILES.VALIDATE", Icon: "validar_cuadro_check"),
            (Operation: "ACTION.SAP_SYNC_PROFILES.ACTIVATE", Icon: "activar_toggle_on"),
            (Operation: "ACTION.SAP_SYNC_PROFILES.DEACTIVATE", Icon: "desactivar_toggle_off")
        };

        sql.Should().Contain("N'20260804.167'")
            .And.Contain("N'20260731.160'")
            .And.NotContain("SecurityRoleFormOperations")
            .And.NotContain("RolePermissions");
        initializer.Should().Contain("167_master_sap_sync_profile_ribbon_icons.sql");

        foreach (var item in expected)
        {
            sql.Should().Contain($"N'{item.Operation}'")
                .And.Contain($"N'Ribbon/{item.Icon}_32.svg'")
                .And.Contain($"N'Ribbon/{item.Icon}_16.svg'");

            Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Assets", "Icons", "Ribbon", $"{item.Icon}_32.svg")
                .Should().Contain("width=\"32\"").And.Contain("stroke=\"#00B894\"");
            Read("src", "Frontend", "NuanSystem.WinForms.Forms", "Assets", "Icons", "Ribbon", $"{item.Icon}_16.svg")
                .Should().Contain("width=\"16\"").And.Contain("stroke=\"#00B894\"");
        }
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

        return directory?.FullName ?? throw new DirectoryNotFoundException("No se encontro la raiz del repositorio.");
    }
}
