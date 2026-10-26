using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapIdentityPolicyApiContractTests
{
    [Fact]
    public void Exposes_read_and_save_routes_with_existing_sap_permissions()
    {
        var source = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "SapIdentityPolicyEndpoints.cs");
        source.Should().Contain("/api/sap/identity-policy")
            .And.Contain("PermissionCodes.SapSyncProfilesView")
            .And.Contain("PermissionCodes.SapSyncProfilesEdit")
            .And.Contain("cancellationToken");
    }

    private static string Read(params string[] path)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln"))) directory = directory.Parent;
        return File.ReadAllText(Path.Combine([directory!.FullName, .. path]));
    }
}
