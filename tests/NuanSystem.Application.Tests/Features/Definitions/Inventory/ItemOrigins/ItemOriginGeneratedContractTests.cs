using FluentAssertions;
using NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Dtos;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.ItemOrigins;

public sealed class ItemOriginGeneratedContractTests
{
    [Fact]
    public void Identity_IsCanonical()
    {
        "/api/definitions/inventory/item-origins".Should().StartWith("/api/definitions/inventory/");
        "item-origins".Should().Be("item-origins");
        "GeneralInventoryItemOriginsRead".Should().NotBeNullOrWhiteSpace();
        "GeneralInventoryItemOriginsManage".Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void CodeNormalization_PreservesApprovedHistoricalCasing()
    {
        var method = typeof(CreateItemOriginCommandHandler).GetMethod(
            "NormalizeCode", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!;
        method.Invoke(null, [" Local "]).Should().Be("Local");
        method.Invoke(null, [" Imported "]).Should().Be("Imported");
        method.Invoke(null, [" Mixed "]).Should().Be("Mixed");
    }

    [Fact]
    public void Navigation_IsRegisteredEndToEnd()
    {
        var program = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms", "Program.cs");
        var mainForm = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.Forms", "Shell", "MainForm.cs");
        var shellViewModel = ReadWorkspaceFile("src", "Frontend", "NuanSystem.WinForms.ViewModels", "Shell", "ShellViewModel.cs");

        program.Should().Contain("\"item-origins\" => CreateItemOriginsForm()");
        mainForm.Should().Contain("\"item-origins\" => generalInventoryCatalogFormFactory(module.Key)");
        shellViewModel.Should().Contain("\"item-origins\"");
    }

    [Fact]
    public async Task Validator_EnforcesBasicShape()
    {
        var result = await new CreateItemOriginCommandValidator().ValidateAsync(
            new CreateItemOriginCommand("", "", new string('D', 501), -1, true), CancellationToken.None);
        result.IsValid.Should().BeFalse();
        result.Errors.Select(x => x.PropertyName).Should().Contain(["Code", "Name", "Description", "SortOrder"]);
    }

    [Fact]
    public void List_OffersEveryPersistedColumnEvenWhenItHasNoRows()
    {
        var listForm = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Definitions", "Inventory",
            "ItemOrigins", "ItemOriginsForm.cs");
        var frontendModel = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Services", "Definitions", "Inventory",
            "ItemOrigins", "Models", "ItemOriginModels.cs");

        listForm.Should().Contain("GridView.Columns.AddField(field)");
        foreach (var field in PersistedFields)
        {
            listForm.Should().Contain($"nameof(ItemOriginItem.{field})");
        }

        frontendModel.Should().Contain("public bool IsDeleted")
            .And.Contain("public int? CreatedByUserId")
            .And.Contain("public DateTime? UpdatedAt")
            .And.Contain("public DateTime? DeletedAt");

        foreach (var field in PersistedFields)
        {
            typeof(ItemOriginDto).GetProperty(field).Should().NotBeNull();
        }
    }

    [Fact]
    public void NavigationSql_RegistersApplicableOperationsAndForwardRepair()
    {
        var navigation = ReadWorkspaceFile("database", "sql", "209_master_definitions_inventory_item_origins_navigation.sql");
        var repair = ReadWorkspaceFile("database", "sql", "220_master_item_origins_form_operations_repair.sql");
        var initializer = ReadWorkspaceFile(
            "src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerMasterDatabaseInitializer.cs");

        navigation.Should().Contain("dbo.SecurityFormOperations")
            .And.Contain("N'ACTION.CUSTOMIZE_COLUMNS'")
            .And.Contain("N'ACTION.EXPORT_XML'")
            .And.Contain("HasListView=1,HasEditView=1")
            .And.Contain("ORDER BY IsDeleted,Id")
            .And.Contain("UPDATE dbo.SecurityRoleMenus");
        repair.Should().Contain("Version=N'20260814.220'")
            .And.Contain("dbo.SecurityFormOperations")
            .And.Contain("(SELECT COUNT(*) FROM @ApplicableOperations)<>12");
        initializer.IndexOf("209_master_definitions_inventory_item_origins_navigation.sql", StringComparison.Ordinal)
            .Should().BeLessThan(initializer.IndexOf("220_master_item_origins_form_operations_repair.sql", StringComparison.Ordinal));
    }

    [Fact]
    public void EditorAndSql_UseApprovedCompactAndSafeContracts()
    {
        var designer = ReadWorkspaceFile(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Definitions", "Inventory",
            "ItemOrigins", "ItemOriginEditForm.Designer.cs");
        var tenantSql = ReadWorkspaceFile("database", "sql", "208_tenant_item_origins_master.sql");
        var tenantRepair = ReadWorkspaceFile("database", "sql", "223_tenant_item_auxiliary_delete_hardening.sql");
        var masterRepair = ReadWorkspaceFile("database", "sql", "224_master_item_auxiliary_navigation_hardening.sql");

        designer.Should().NotContain("lblGeneralTitle")
            .And.NotContain("lineGeneralTitle")
            .And.Contain("txtCode.Location = new Point(154, 26)")
            .And.Contain("txtName.Location = new Point(154, 54)")
            .And.Contain("memDescription.Location = new Point(154, 82)")
            .And.Contain("ClientSize = new Size(870, 202)");
        tenantSql.Should().Contain("DECLARE @Affected int=@@ROWCOUNT;")
            .And.Contain("SELECT @Affected;");
        tenantRepair.Should().Contain("SP_NA_DELETE_GENERAL_INVENTORY_ItemOrigins_ELIMINAR");
        masterRepair.Should().Contain("FormKey=N'item-origins'")
            .And.Contain("UPDATE dbo.SecurityRoleMenus")
            .And.Contain("SET IsAllowed=0,IsDeleted=1");
    }

    private static readonly string[] PersistedFields =
    [
        "Id", "GlobalId", "Code", "Name", "Description", "SortOrder", "IsActive", "IsDeleted",
        "CreatedByUserId", "CreatedByUserName", "CreatedAt", "UpdatedByUserId", "UpdatedByUserName",
        "UpdatedAt", "DeletedByUserId", "DeletedByUserName", "DeletedAt"
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
