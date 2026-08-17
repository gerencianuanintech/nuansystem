using FluentAssertions;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.SalesChannels;

public sealed class SalesChannelGeneratedContractTests
{
    [Fact]
    public void Identity_IsCanonical()
    {
        "/api/definitions/inventory/sales-channels".Should().MatchRegex("^/api/[a-z0-9]+(?:-[a-z0-9]+)*(?:/[a-z0-9]+(?:-[a-z0-9]+)*)*$");
        "sales-channels".Should().Be("sales-channels");
        "GeneralInventorySalesChannelsRead".Should().NotBeNullOrWhiteSpace();
        "GeneralInventorySalesChannelsManage".Should().NotBeNullOrWhiteSpace();
        PermissionCodes.GeneralInventorySalesChannelsRead.Should().Be("GENERALINVENTORY.SALESCHANNELS.READ");
        PermissionCodes.GeneralInventorySalesChannelsManage.Should().Be("GENERALINVENTORY.SALESCHANNELS.MANAGE");
    }

    [Fact]
    public void Navigation_IsRegisteredEndToEnd()
    {
        var program = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms", "Program.cs");
        var mainForm = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Shell", "MainForm.cs");
        var shellViewModel = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.ViewModels", "Shell", "ShellViewModel.cs");

        program.Should().Contain("\"sales-channels\" => CreateSalesChannelsForm()");
        mainForm.Should().Contain("\"sales-channels\" => generalInventoryCatalogFormFactory(module.Key)");
        shellViewModel.Should().Contain("\"sales-channels\"");
    }

    [Fact]
    public void NavigationSql_RegistersApplicableOperationsSeparatelyFromRoleGrants()
    {
        var root = FindWorkspaceRoot();
        var navigationSqlPath = Directory.GetFiles(
            Path.Combine(root, "database", "sql"),
            "227_master_*_navigation.sql").Single();
        var navigationSql = File.ReadAllText(navigationSqlPath);

        navigationSql.Should().Contain("dbo.SecurityFormOperations");
        navigationSql.Should().Contain("dbo.SecurityRoleFormOperations");
        foreach (var operation in CanonicalOperations)
        {
            navigationSql.Should().Contain(operation);
        }
    }

    [Fact]
    public void GeneratedContracts_PreserveSqlResultsAndDesignerSafety()
    {
        var tenantSql = ReadWorkspaceFile("database", "sql", "226_tenant_sales_channels_master.sql");
        var navigationSql = ReadWorkspaceFile("database", "sql", "227_master_definitions_inventory_sales_channels_navigation.sql");
        var listForm = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Definitions", "Inventory", "SalesChannels",
            "SalesChannelsForm.cs");

        tenantSql.Should().Contain("20260817.226");
        navigationSql.Should().Contain("20260817.227");
        tenantSql.Should().Contain("DECLARE @Affected int=@@ROWCOUNT;");
        tenantSql.Should().Contain("SELECT @Affected;");
        navigationSql.Should().Contain("UPDATE dbo.SecurityRoleMenus SET IsAllowed=1,IsDeleted=0");
        listForm.Should().Contain("if (session is null) return;");
    }

    [Fact]
    public void List_OffersEveryPersistedColumnEvenWhenItHasNoRows()
    {
        var listForm = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Definitions", "Inventory", "SalesChannels",
            "SalesChannelsForm.cs");

        listForm.Should().Contain("GridView.Columns.AddField(field)");
        foreach (var field in PersistedFields)
        {
            listForm.Should().Contain($"nameof(SalesChannelItem.{field})");
        }
    }

    private static readonly string[] PersistedFields = ["Id", "GlobalId", "Code", "Name", "Description", "SortOrder", "IsActive", "CreatedByUserId", "CreatedByUserName", "CreatedAt", "UpdatedByUserId", "UpdatedByUserName", "UpdatedAt", "IsDeleted", "DeletedByUserId", "DeletedByUserName", "DeletedAt"];
    private static readonly string[] CanonicalOperations =
    [
        "ACTION.REFRESH", "ACTION.CONSULT", "ACTION.CREATE", "ACTION.UPDATE",
        "ACTION.DELETE", "ACTION.COPY", "ACTION.HISTORY", "ACTION.CUSTOMIZE_COLUMNS",
        "ACTION.EXPORT_EXCEL", "ACTION.EXPORT_PDF", "ACTION.EXPORT_JSON", "ACTION.EXPORT_XML"
    ];

    private static string ReadWorkspaceFile(params string[] segments) =>
        File.ReadAllText(Path.Combine(FindWorkspaceRoot(), Path.Combine(segments)));

    private static string FindWorkspaceRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("No se encontró la raíz del repositorio.");
    }
}


