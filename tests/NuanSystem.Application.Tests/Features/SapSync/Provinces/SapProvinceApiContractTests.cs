using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync.Provinces;

public sealed class SapProvinceApiContractTests
{
    [Fact]
    public void Endpoints_AreSeparateThinAndProtected()
    {
        var endpoints = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "SapEndpoints.cs");

        endpoints.Should().Contain("/api/sap/provinces/preview")
            .And.Contain("new PreviewProvincesFromSapQuery()")
            .And.Contain("/api/sap/provinces/import")
            .And.Contain("new ImportProvincesFromSapCommand(auditUser.UserId, auditUser.UserName)")
            .And.Contain("PermissionCodes.SapRead")
            .And.Contain("PermissionCodes.SapManage");
    }

    [Fact]
    public void Wiring_RegistersReaderImportHandlerScheduledAndRetryContracts()
    {
        var application = Read("src", "Backend", "NuanSystem.Application", "DependencyInjection", "ApplicationServiceRegistration.cs");
        var integration = Read("src", "Backend", "NuanSystem.SapIntegration", "DependencyInjection", "SapIntegrationServiceRegistration.cs");

        application.Should().Contain("ISapProvinceImportService, SapProvinceImportService")
            .And.Contain("ISapSyncEntityHandler, SapProvinceSyncHandler")
            .And.Contain("ISapSyncScheduledExecutionProcessor, SapProvinceExecutionProcessor")
            .And.Contain("ISapSyncExecutionRetryProcessor, SapProvinceExecutionRetryProcessor")
            .And.Contain("AddScoped<SapProvinceRecordProcessor>()");
        integration.Should().Contain("ISapProvinceReader, SapServiceLayerProvinceReader");
    }

    [Fact]
    public void ApplicationVertical_HasNoSapFilterContract()
    {
        var root = Path.Combine(
            WorkspaceRoot(), "src", "Backend", "NuanSystem.Application", "Features", "SapSync", "Provinces");
        var source = string.Join("\n", Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories)
            .Select(File.ReadAllText));

        source.Should().NotContain("SapProvinceFilter")
            .And.NotContain("NameContains")
            .And.NotContain("ExactName");
    }

    private static string Read(params string[] parts) =>
        File.ReadAllText(Path.Combine(WorkspaceRoot(), Path.Combine(parts)));

    private static string WorkspaceRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Workspace root not found.");
    }
}
