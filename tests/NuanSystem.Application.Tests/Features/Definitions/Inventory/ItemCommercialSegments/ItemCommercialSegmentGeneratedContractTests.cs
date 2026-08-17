using FluentAssertions;
using NuanSystem.Application.Features.Definitions.Inventory.ItemCommercialSegments.Dtos;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.ItemCommercialSegments;

public sealed class ItemCommercialSegmentGeneratedContractTests
{
    [Fact]
    public void Identity_IsCanonical()
    {
        "/api/definitions/inventory/item-commercial-segments".Should().MatchRegex("^/api/[a-z0-9]+(?:-[a-z0-9]+)*(?:/[a-z0-9]+(?:-[a-z0-9]+)*)*$");
        "item-commercial-segments".Should().Be("item-commercial-segments");
        "GeneralInventoryItemCommercialSegmentsRead".Should().NotBeNullOrWhiteSpace();
        "GeneralInventoryItemCommercialSegmentsManage".Should().NotBeNullOrWhiteSpace();
        PermissionCodes.GeneralInventoryItemCommercialSegmentsRead.Should().Be("GENERALINVENTORY.ITEMCOMMERCIALSEGMENTS.READ");
        PermissionCodes.GeneralInventoryItemCommercialSegmentsManage.Should().Be("GENERALINVENTORY.ITEMCOMMERCIALSEGMENTS.MANAGE");
    }

    [Fact]
    public void Navigation_IsRegisteredEndToEnd()
    {
        var program = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms", "Program.cs");
        var mainForm = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Shell", "MainForm.cs");
        var shellViewModel = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.ViewModels", "Shell", "ShellViewModel.cs");

        program.Should().Contain("\"item-commercial-segments\" => CreateItemCommercialSegmentsForm()");
        mainForm.Should().Contain("\"item-commercial-segments\" => generalInventoryCatalogFormFactory(module.Key)");
        shellViewModel.Should().Contain("\"item-commercial-segments\"");
    }

    [Fact]
    public void NavigationSql_RegistersApplicableOperationsAndForwardRepair()
    {
        var navigation = ReadWorkspaceFile("database", "sql", "218_master_definitions_inventory_item_commercial_segments_navigation.sql");
        var repair = ReadWorkspaceFile("database", "sql", "219_master_item_commercial_segments_form_operations_repair.sql");
        var initializer = ReadWorkspaceFile("src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");

        navigation.Should().Contain("dbo.SecurityFormOperations")
            .And.Contain("N'ACTION.CUSTOMIZE_COLUMNS'")
            .And.Contain("N'ACTION.EXPORT_XML'")
            .And.Contain("HasListView=1,HasEditView=1")
            .And.Contain("ORDER BY IsDeleted,Id")
            .And.Contain("UPDATE dbo.SecurityRoleMenus");
        repair.Should().Contain("Version=N'20260814.219'")
            .And.Contain("dbo.SecurityFormOperations")
            .And.Contain("(SELECT COUNT(*) FROM @ApplicableOperations)<>12");
        initializer.IndexOf("218_master_definitions_inventory_item_commercial_segments_navigation.sql", StringComparison.Ordinal)
            .Should().BeLessThan(initializer.IndexOf("219_master_item_commercial_segments_form_operations_repair.sql", StringComparison.Ordinal));
    }

    [Fact]
    public void List_DeclaresVisibleColumnsEvenWhenItHasNoRows()
    {
        var listForm = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Definitions", "Inventory",
            "ItemCommercialSegments", "ItemCommercialSegmentsForm.cs");

        listForm.Should().Contain("GridView.Columns.AddField(field)");
        foreach (var field in PersistedFields)
        {
            listForm.Should().Contain($"nameof(ItemCommercialSegmentItem.{field})");
        }

        var frontendModel = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Services", "Definitions", "Inventory",
            "ItemCommercialSegments", "Models", "ItemCommercialSegmentModels.cs");
        foreach (var field in PersistedFields)
        {
            frontendModel.Should().Contain($"public {FrontendType(field)} {field}");
            typeof(ItemCommercialSegmentDto).GetProperty(field).Should().NotBeNull();
        }
    }

    [Fact]
    public void GeneratedContracts_PreserveSqlResultsAndDesignerSafety()
    {
        var listForm = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Definitions", "Inventory",
            "ItemCommercialSegments", "ItemCommercialSegmentsForm.cs");
        var tenantSql = ReadWorkspaceFile("database", "sql", "217_tenant_item_commercial_segments_master.sql");
        var tenantRepair = ReadWorkspaceFile("database", "sql", "223_tenant_item_auxiliary_delete_hardening.sql");
        var masterRepair = ReadWorkspaceFile("database", "sql", "224_master_item_auxiliary_navigation_hardening.sql");
        var tenantInitializer = ReadWorkspaceFile(
            "src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerTenantDatabaseInitializer.cs");
        var masterInitializer = ReadWorkspaceFile(
            "src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");

        listForm.Should().Contain("if (session is null)");
        tenantSql.Should().Contain("DECLARE @Affected int=@@ROWCOUNT;")
            .And.Contain("SELECT @Affected;")
            .And.Contain("DECLARE @OwnTransaction bit");
        tenantRepair.Should().Contain("Version=N'20260815.223'")
            .And.Contain("SP_NA_DELETE_GENERAL_INVENTORY_ItemCommercialSegments_ELIMINAR")
            .And.NotContain("ItemCommercialSegments from migration 217 is required")
            .And.NotContain("ItemOrigins from migration 208 is required")
            .And.NotContain("ItemAlertTypes from migration 221 is required");
        masterRepair.Should().Contain("Version=N'20260815.224'")
            .And.Contain("FormKey=N'item-commercial-segments'")
            .And.Contain("UPDATE dbo.SecurityRoleMenus")
            .And.Contain("NOT EXISTS(")
            .And.Contain("WHERE source.OperationId=target.OperationId");
        tenantInitializer.IndexOf("221_tenant_item_alert_types_master.sql", StringComparison.Ordinal)
            .Should().BeLessThan(tenantInitializer.IndexOf("223_tenant_item_auxiliary_delete_hardening.sql", StringComparison.Ordinal));
        masterInitializer.IndexOf("222_master_definitions_inventory_item_alert_types_navigation.sql", StringComparison.Ordinal)
            .Should().BeLessThan(masterInitializer.IndexOf("224_master_item_auxiliary_navigation_hardening.sql", StringComparison.Ordinal));
    }

    private static readonly string[] PersistedFields =
    [
        "Id", "GlobalId", "Code", "Name", "Description", "SortOrder", "IsActive", "CreatedByUserId",
        "CreatedByUserName", "CreatedAt", "UpdatedByUserId", "UpdatedByUserName", "UpdatedAt", "IsDeleted",
        "DeletedByUserId", "DeletedByUserName", "DeletedAt"
    ];

    private static string FrontendType(string field) => field switch
    {
        "Id" or "SortOrder" => "int",
        "GlobalId" => "Guid",
        "Code" or "Name" => "string",
        "Description" or "CreatedByUserName" or "UpdatedByUserName" or "DeletedByUserName" => "string?",
        "IsActive" or "IsDeleted" => "bool",
        "CreatedByUserId" or "UpdatedByUserId" or "DeletedByUserId" => "int?",
        "CreatedAt" => "DateTime",
        "UpdatedAt" or "DeletedAt" => "DateTime?",
        _ => throw new ArgumentOutOfRangeException(nameof(field), field, null)
    };

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
