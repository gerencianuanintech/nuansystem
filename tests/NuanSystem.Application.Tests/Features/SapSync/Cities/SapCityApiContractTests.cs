using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.SapSync.Cities;

public sealed class SapCityApiContractTests
{
    [Fact]
    public void Endpoints_ExposeSettingsPreviewAndImportWithPermissions()
    {
        var source = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "SapEndpoints.cs");
        source.Should().Contain("/api/sap/settings/cities-query")
            .And.Contain("new GetSapCityQuerySettingsQuery()")
            .And.Contain("UpdateSapCityQuerySettingsCommand")
            .And.Contain("/api/sap/cities/preview")
            .And.Contain("new PreviewCitiesFromSapQuery()")
            .And.Contain("/api/sap/cities/import")
            .And.Contain("new ImportCitiesFromSapCommand(auditUser.UserId, auditUser.UserName)")
            .And.Contain("PermissionCodes.SapRead")
            .And.Contain("PermissionCodes.SapManage");
    }

    [Fact]
    public void DependencyInjection_RegistersCompleteCityLifecycle()
    {
        var application = Read("src", "Backend", "NuanSystem.Application", "DependencyInjection", "ApplicationServiceRegistration.cs");
        var integration = Read("src", "Backend", "NuanSystem.SapIntegration", "DependencyInjection", "SapIntegrationServiceRegistration.cs");
        application.Should().Contain("ISapCityImportService, SapCityImportService")
            .And.Contain("ISapSyncEntityHandler, SapCitySyncHandler")
            .And.Contain("ISapSyncScheduledExecutionProcessor, SapCityExecutionProcessor")
            .And.Contain("ISapSyncExecutionRetryProcessor, SapCityExecutionRetryProcessor")
            .And.Contain("AddScoped<SapCityRecordProcessor>()");
        integration.Should().Contain("ISapCityReader, SapHanaCityReader");
    }

    [Fact]
    public void CityVertical_HasNoFilterContract()
    {
        var root = Path.Combine(WorkspaceRoot(), "src", "Backend", "NuanSystem.Application", "Features", "SapSync", "Cities");
        var source = string.Join("\n", Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories).Select(File.ReadAllText));
        source.Should().NotContain("CityFilter").And.NotContain("NameContains").And.NotContain("ExactName");
    }

    private static string Read(params string[] parts) => File.ReadAllText(Path.Combine(WorkspaceRoot(), Path.Combine(parts)));
    private static string WorkspaceRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "NuanSystem.sln"))) directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Workspace root not found.");
    }
}
