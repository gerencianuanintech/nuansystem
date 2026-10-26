using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapMappingApiContractTests
{
    [Fact]
    public void Mapping_endpoint_group_exposes_approved_routes_and_permissions()
    {
        var source = Read(
            "src", "Backend", "NuanSystem.Api", "Endpoints", "SapSyncMappingEndpoints.cs");

        source.Should().Contain("MappingRoute = \"/api/sap/mapping\"")
            .And.Contain("ProfileRoute = \"/api/sap/profiles\"")
            .And.Contain("/query-definitions")
            .And.Contain("/entities/{{profileEntityId:long}}/field-mappings")
            .And.Contain("/{{profileId:long}}/validate")
            .And.Contain("/entities/{{profileEntityId:long}}/preview")
            .And.Contain("PermissionCodes.SapSyncProfilesView")
            .And.Contain("PermissionCodes.SapSyncProfilesEdit")
            .And.Contain("PermissionCodes.SapSyncProfilesValidate")
            .And.Contain("cancellationToken");
    }

    [Fact]
    public void Preview_endpoint_is_configuration_only_and_does_not_execute_writes()
    {
        var source = Read(
            "src", "Backend", "NuanSystem.Api", "Endpoints", "SapSyncMappingEndpoints.cs");

        source.Should().Contain("PreviewSapEntityMappingQuery")
            .And.NotContain("Import")
            .And.NotContain("ExecuteSap")
            .And.NotContain("SapOutbox");
    }

    private static string Read(params string[] path)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
            directory = directory.Parent;
        return File.ReadAllText(Path.Combine([directory!.FullName, .. path]));
    }
}
