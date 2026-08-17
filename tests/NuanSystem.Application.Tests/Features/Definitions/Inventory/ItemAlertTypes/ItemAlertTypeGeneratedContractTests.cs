using FluentAssertions;
using NuanSystem.Application.Features.Definitions.Inventory.ItemAlertTypes.Dtos;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.ItemAlertTypes;

public sealed class ItemAlertTypeGeneratedContractTests
{
    [Fact]
    public void Identity_IsCanonical()
    {
        "/api/definitions/inventory/item-alert-types".Should().MatchRegex("^/api/[a-z0-9]+(?:-[a-z0-9]+)*(?:/[a-z0-9]+(?:-[a-z0-9]+)*)*$");
        "item-alert-types".Should().Be("item-alert-types");
        "GeneralInventoryItemAlertTypesRead".Should().NotBeNullOrWhiteSpace();
        "GeneralInventoryItemAlertTypesManage".Should().NotBeNullOrWhiteSpace();
        PermissionCodes.GeneralInventoryItemAlertTypesRead.Should().Be("GENERALINVENTORY.ITEMALERTTYPES.READ");
        PermissionCodes.GeneralInventoryItemAlertTypesManage.Should().Be("GENERALINVENTORY.ITEMALERTTYPES.MANAGE");
    }

    [Fact]
    public void Navigation_IsRegisteredEndToEnd()
    {
        var program = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms", "Program.cs");
        var mainForm = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Shell", "MainForm.cs");
        var shellViewModel = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.ViewModels", "Shell", "ShellViewModel.cs");

        program.Should().Contain("\"item-alert-types\" => CreateItemAlertTypesForm()");
        mainForm.Should().Contain("\"item-alert-types\" => generalInventoryCatalogFormFactory(module.Key)");
        shellViewModel.Should().Contain("\"item-alert-types\"");
    }

    [Fact]
    public void NavigationSql_RegistersApplicableOperationsSeparatelyFromRoleGrants()
    {
        var root = FindWorkspaceRoot();
        var navigationSqlPath = Directory.GetFiles(
            Path.Combine(root, "database", "sql"),
            "222_master_*_navigation.sql").Single();
        var navigationSql = File.ReadAllText(navigationSqlPath);

        navigationSql.Should().Contain("dbo.SecurityFormOperations");
        navigationSql.Should().Contain("dbo.SecurityRoleFormOperations");
        foreach (var operation in CanonicalOperations)
        {
            navigationSql.Should().Contain(operation);
        }
    }

    [Fact]
    public void SqlMigrations_UseApprovedVersionsAndPreserveDeleteRowCount()
    {
        var tenantSql = ReadWorkspaceFile("database", "sql", "221_tenant_item_alert_types_master.sql");
        var navigationSql = ReadWorkspaceFile("database", "sql", "222_master_definitions_inventory_item_alert_types_navigation.sql");
        var unicodeRepair = ReadWorkspaceFile("database", "sql", "225_master_item_alert_types_unicode_repair.sql");
        var initializer = ReadWorkspaceFile(
            "src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");

        tenantSql.Should().Contain("20260814.221");
        navigationSql.Should().Contain("20260814.222");
        navigationSql.Should().Contain("NCHAR(237)");
        unicodeRepair.Should().Contain("20260816.225")
            .And.Contain("NCHAR(237)")
            .And.NotContain("artículos");
        tenantSql.Should().Contain("DECLARE @Affected int=@@ROWCOUNT;");
        tenantSql.Should().Contain("SELECT @Affected;");
        navigationSql.Should().Contain("UPDATE dbo.SecurityRoleMenus SET IsAllowed=1,IsDeleted=0");
        initializer.IndexOf("224_master_item_auxiliary_navigation_hardening.sql", StringComparison.Ordinal)
            .Should().BeLessThan(initializer.IndexOf("225_master_item_alert_types_unicode_repair.sql", StringComparison.Ordinal));
    }

    [Fact]
    public void List_OffersEveryPersistedColumnEvenWhenItHasNoRows()
    {
        var listForm = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Definitions", "Inventory", "ItemAlertTypes",
            "ItemAlertTypesForm.cs");

        listForm.Should().Contain("GridView.Columns.AddField(field)");
        foreach (var field in PersistedFields)
        {
            listForm.Should().Contain($"nameof(ItemAlertTypeItem.{field})");
            typeof(ItemAlertTypeDto).GetProperty(field).Should().NotBeNull();
        }

        var frontendModel = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Services", "Definitions", "Inventory", "ItemAlertTypes",
            "Models", "ItemAlertTypeModels.cs");
        frontendModel.Should().Contain("public bool IsDeleted")
            .And.Contain("public int? CreatedByUserId")
            .And.Contain("public DateTime? UpdatedAt")
            .And.Contain("public DateTime? DeletedAt");
    }

    [Fact]
    public void ForwardHardening_PreservesAtomicDeleteAndDesignerSafety()
    {
        var listForm = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Definitions", "Inventory", "ItemAlertTypes",
            "ItemAlertTypesForm.cs");
        var tenantSql = ReadWorkspaceFile("database", "sql", "221_tenant_item_alert_types_master.sql");
        var tenantRepair = ReadWorkspaceFile("database", "sql", "223_tenant_item_auxiliary_delete_hardening.sql");

        listForm.Should().Contain("if (session is not null)");
        tenantSql.Should().Contain("DECLARE @OwnTransaction bit")
            .And.Contain("DECLARE @Affected int=@@ROWCOUNT;")
            .And.Contain("SELECT @Affected;");
        tenantRepair.Should().Contain("Version=N'20260815.223'")
            .And.Contain("SP_NA_DELETE_GENERAL_INVENTORY_ItemAlertTypes_ELIMINAR");
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
