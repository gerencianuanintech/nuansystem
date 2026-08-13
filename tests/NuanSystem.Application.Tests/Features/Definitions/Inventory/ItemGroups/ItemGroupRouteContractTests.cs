using FluentAssertions;
using System.Runtime.CompilerServices;

namespace NuanSystem.Application.Tests.Features.Definitions.Inventory.ItemGroups;

public sealed class ItemGroupRouteContractTests
{
    private const string ExpectedRoute = "/api/definitions/inventory/item-groups";

    [Fact]
    public void Endpoint_UsesDefinitionsInventoryRouteExclusively()
    {
        var endpoint = Read(
            "src", "Backend", "NuanSystem.Api", "Endpoints", "Definitions", "Inventory", "ItemGroups", "ItemGroupEndpoints.cs");

        endpoint.Should().Contain($"private const string BaseRoute = \"{ExpectedRoute}\"")
            .And.Contain("app.MapGroup(BaseRoute)")
            .And.NotContain("app.MapGroup(\"/api/item-groups\")");
    }

    [Fact]
    public void Lookup_AllowsItemAndFamilyConsumersWithoutGrantingMaintenanceActions()
    {
        var endpoint = Read(
            "src", "Backend", "NuanSystem.Api", "Endpoints", "Definitions", "Inventory", "ItemGroups", "ItemGroupEndpoints.cs");

        endpoint.Should().Contain("PermissionCodes.ItemsRead")
            .And.Contain("PermissionCodes.GeneralInventoryItemFamiliesRead")
            .And.Contain("PermissionCodes.GeneralInventoryItemFamiliesManage")
            .And.Contain("MapGet(\"/lookup\"");
    }

    [Theory]
    [InlineData("Definitions", "Inventory", "ItemGroups")]
    [InlineData("GeneralInventory", "ItemGroups")]
    public void WinFormsClient_UsesSameDefinitionsInventoryRoute(params string[] clientSegments)
    {
        var segments = new[] { "src", "Frontend", "NuanSystem.WinForms.Services" }
            .Concat(clientSegments)
            .Append("ItemGroupClient.cs")
            .ToArray();

        var client = Read(segments);

        client.Should().Contain($"private const string BaseRoute = \"{ExpectedRoute}\"")
            .And.NotContain("\"/api/item-groups");
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
