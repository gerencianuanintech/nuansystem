using FluentAssertions;

namespace NuanSystem.Application.Tests.Architecture;

public sealed class BackendHardeningArchitectureTests
{
    [Fact]
    public void Purchase_order_handlers_are_split_into_focused_files()
    {
        var commandsDirectory = FindPath(
            "src", "Backend", "NuanSystem.Application", "Features", "Purchasing", "PurchaseOrders", "Commands");

        File.Exists(Path.Combine(commandsDirectory, "PurchaseOrderCommandHandlers.cs")).Should().BeFalse();
        Directory.GetFiles(commandsDirectory, "*Handler*.cs")
            .Should().OnlyContain(path => File.ReadAllLines(path).Length <= 350);
    }

    [Fact]
    public void Application_does_not_reference_database_or_transport_providers()
    {
        var applicationDirectory = FindPath("src", "Backend", "NuanSystem.Application");
        var source = ReadAllCSharp(applicationDirectory);

        source.Should().NotContain("Microsoft.Data.SqlClient");
        source.Should().NotContain("NuanSystem.SapIntegration");
    }

    [Fact]
    public void Api_does_not_reference_provider_specific_exceptions()
    {
        var apiDirectory = FindPath("src", "Backend", "NuanSystem.Api");
        var source = ReadAllCSharp(apiDirectory);

        source.Should().NotContain("SqlException");
        source.Should().NotContain("SapServiceLayerException");
    }

    [Fact]
    public void Exception_middleware_only_coordinates_the_pipeline()
    {
        var middleware = Path.Combine(
            FindPath("src", "Backend", "NuanSystem.Api", "Middleware"),
            "GlobalExceptionMiddleware.cs");
        var source = File.ReadAllText(middleware);

        File.ReadAllLines(middleware).Length.Should().BeLessThanOrEqualTo(70);
        source.Should().NotContain("CreateAuditErrorLogData");
        source.Should().NotContain("ClassifyException");
        source.Should().Contain("ExceptionResponseMapper");
        source.Should().Contain("ExceptionAuditWriter");
    }

    [Fact]
    public void Purchase_order_workflow_keeps_unavailable_export_guard_and_no_todo()
    {
        var commandsDirectory = FindPath(
            "src", "Backend", "NuanSystem.Application", "Features", "Purchasing", "PurchaseOrders", "Commands");
        var source = ReadAllCSharp(commandsDirectory);

        source.Should().Contain("SAP_EXPORT_NOT_AVAILABLE");
        source.Should().NotContain("TODO");
    }

    private static string ReadAllCSharp(string directory) => string.Join(
        Environment.NewLine,
        Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories).Select(File.ReadAllText));

    private static string FindPath(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine([directory.FullName, .. pathParts]);
            if (Directory.Exists(path)) return path;
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(Path.Combine(pathParts));
    }
}
