using FluentAssertions;
using System.Runtime.CompilerServices;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.ItemTypes;

public sealed class ItemTypeBackendStructureTests
{
    [Fact]
    public void DedicatedEndpoint_PreservesRoutesPermissionsAndFormOperations()
    {
        var endpoint = Read(
            "src", "Backend", "NuanSystem.Api", "Endpoints", "Definitions", "Inventory", "ItemTypes", "ItemTypeEndpoints.cs");

        endpoint.Should().Contain("/api/general-inventory/item-types")
            .And.Contain("PermissionCodes.GeneralInventoryItemTypesRead")
            .And.Contain("PermissionCodes.GeneralInventoryItemTypesManage")
            .And.Contain("RequireFormOperation(FormKey, \"refresh\")")
            .And.Contain("RequireFormOperation(FormKey, \"consult\")")
            .And.Contain("RequireFormOperation(FormKey, \"create\")")
            .And.Contain("RequireFormOperation(FormKey, \"update\")")
            .And.Contain("RequireFormOperation(FormKey, \"delete\")");
        endpoint.Should().Contain("RequireFormOperation(FormKey, \"history\")");
    }

    [Fact]
    public void GenericCatalogRouting_NoLongerOwnsItemTypes()
    {
        Read("src", "Backend", "NuanSystem.Persistence", "Repositories", "GeneralInventory", "GeneralInventoryCatalogRepository.cs")
            .Should().NotContain("[\"item-types\"]");
        Read("src", "Backend", "NuanSystem.Api", "Endpoints", "InventoryCatalogEndpoints.cs")
            .Should().NotContain("\"item-types\",\r\n            PermissionCodes.GeneralInventoryItemTypesRead");
    }

    private static string Read(params string[] segments) => File.ReadAllText(PathInRoot(segments));

    private static string PathInRoot(params string[] segments)
    {
        var directory = FindRepositoryRoot(new DirectoryInfo(AppContext.BaseDirectory))
            ?? FindRepositoryRoot(new DirectoryInfo(Directory.GetCurrentDirectory()))
            ?? FindRepositoryRoot(new DirectoryInfo(Path.GetDirectoryName(SourceFilePath())!));

        directory.Should().NotBeNull("the repository root must be reachable from the test output or working directory");
        return Path.Combine([directory!.FullName, .. segments]);
    }

    private static DirectoryInfo? FindRepositoryRoot(DirectoryInfo? directory)
    {
        while (directory is not null)
        {
            if (Directory.Exists(Path.Combine(directory.FullName, "src"))
                && File.Exists(Path.Combine(directory.FullName, "nuansystem.sln")))
            {
                return directory;
            }

            directory = directory.Parent;
        }

        return null;
    }

    private static string SourceFilePath([CallerFilePath] string path = "") => path;
}
