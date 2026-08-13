using System.Runtime.CompilerServices;
using FluentAssertions;
using NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Commands;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.ItemFamilies;

public sealed class ItemFamilyBackendContractTests
{
    [Fact]
    public async Task CreateValidator_RequiresParentAndConsistentExternalIdentity()
    {
        var validator = new CreateItemFamilyCommandValidator();
        var command = new CreateItemFamilyCommand(
            0, "", "", null, -1, true, "SAP_B1", null, null, null);

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Select(error => error.PropertyName).Should().Contain([
            "ItemGroupId", "Code", "Name", "SortOrder", "ExternalCode"]);
    }

    [Fact]
    public void DedicatedEndpoint_UsesOnlyDefinitionsInventoryRouteAndOwnPermissions()
    {
        var endpoint = Read(
            "src", "Backend", "NuanSystem.Api", "Endpoints", "Definitions", "Inventory", "ItemFamilies", "ItemFamilyEndpoints.cs");
        var catalog = Read(
            "src", "Backend", "NuanSystem.Api", "Endpoints", "InventoryCatalogEndpoints.cs");

        endpoint.Should().Contain("/api/definitions/inventory/item-families")
            .And.Contain("PermissionCodes.GeneralInventoryItemFamiliesRead")
            .And.Contain("PermissionCodes.GeneralInventoryItemFamiliesManage")
            .And.Contain("MapGet(\"/lookup\"")
            .And.Contain("MapGet(\"/{id:int}/history\"")
            .And.Contain("RequireFormOperation(FormKey, \"create\")")
            .And.Contain("RequireFormOperation(FormKey, \"update\")")
            .And.Contain("RequireFormOperation(FormKey, \"delete\")");
        catalog.Should().NotContain("MapGroup(\"/api/item-families\")");
    }

    [Fact]
    public void DtoAndSyncPayload_IncludeSortOrderButExcludeSpeculativeIsSystem()
    {
        var dto = Read(
            "src", "Backend", "NuanSystem.Application", "Features", "Definitions", "Inventory", "ItemFamilies", "Dtos", "ItemFamilyDtos.cs");

        dto.Should().Contain("int SortOrder")
            .And.NotContain("IsSystem");
    }

    [Fact]
    public void LegacyItemEditorClient_UsesNewLookupRouteWithoutMaintenanceListAccess()
    {
        var client = Read(
            "src", "Frontend", "NuanSystem.WinForms.Services", "GeneralInventory", "ItemFamilies", "ItemFamilyClient.cs");

        client.Should().Contain("/api/definitions/inventory/item-families")
            .And.Contain("$\"{BaseRoute}/lookup\"")
            .And.Contain("$\"{BaseRoute}/lookup?itemGroupId={itemGroupId}\"")
            .And.NotContain("\"/api/item-families");
    }

    [Fact]
    public void FamilyEditor_LoadsGroupsThroughLookupAndPreservesHistoricalSelection()
    {
        var viewModel = Read(
            "src", "Frontend", "NuanSystem.WinForms.ViewModels", "Definitions", "Inventory", "ItemFamilies", "ItemFamiliesViewModel.cs");

        viewModel.Should().Contain("itemGroupClient.GetLookupAsync")
            .And.Contain("selectedItemGroupCode")
            .And.NotContain("var groupsTask = itemGroupClient.GetAsync");
    }

    private static string Read(params string[] segments) => File.ReadAllText(PathInRoot(segments));

    private static string PathInRoot(params string[] segments)
    {
        var directory = FindRepositoryRoot(new DirectoryInfo(AppContext.BaseDirectory))
            ?? FindRepositoryRoot(new DirectoryInfo(Directory.GetCurrentDirectory()))
            ?? FindRepositoryRoot(new DirectoryInfo(Path.GetDirectoryName(SourceFilePath())!));

        directory.Should().NotBeNull();
        return Path.Combine([directory!.FullName, .. segments]);
    }

    private static DirectoryInfo? FindRepositoryRoot(DirectoryInfo? directory)
    {
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "src"))
                && File.Exists(Path.Combine(directory.FullName, "nuansystem.sln")))
                return directory;
            directory = directory.Parent;
        }

        return null;
    }

    private static string SourceFilePath([CallerFilePath] string path = "") => path;
}
