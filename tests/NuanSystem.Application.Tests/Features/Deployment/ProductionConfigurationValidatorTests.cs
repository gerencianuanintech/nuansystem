using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using NuanSystem.Api.Options;

namespace NuanSystem.Application.Tests.Features.Deployment;

public sealed class ProductionConfigurationValidatorTests
{
    [Fact]
    public void PublishProjects_ExplicitlyExcludeLocalAppsettings()
    {
        var root = FindRoot();
        foreach (var project in new[]
        {
            Path.Combine(root,"src","Backend","NuanSystem.Api","NuanSystem.Api.csproj"),
            Path.Combine(root,"src","Backend","NuanSystem.MasterBranchSyncWorker","NuanSystem.MasterBranchSyncWorker.csproj")
        })
        {
            var xml = File.ReadAllText(project);
            Assert.Contains("appsettings.Local.json", xml, StringComparison.Ordinal);
            Assert.Contains("CopyToPublishDirectory=\"Never\"", xml, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Validate_DoesNotRequireProductionSecrets_InDevelopment()
    {
        var environment = Substitute.For<IHostEnvironment>();
        environment.EnvironmentName.Returns(Environments.Development);
        var configuration = new ConfigurationBuilder().Build();

        ProductionConfigurationValidator.Validate(configuration, environment);
    }

    [Fact]
    public void Validate_RejectsMissingSecrets_InProduction()
    {
        var environment = Substitute.For<IHostEnvironment>();
        environment.EnvironmentName.Returns(Environments.Production);
        var configuration = new ConfigurationBuilder().Build();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            ProductionConfigurationValidator.Validate(configuration, environment));

        Assert.Contains("ConnectionStrings:SqlServerAdmin", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Validate_AcceptsExternalizedProductionConfiguration()
    {
        var environment = Substitute.For<IHostEnvironment>();
        environment.EnvironmentName.Returns(Environments.Production);
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:SqlServerAdmin"] = "Server=db;Database=master;User Id=app;Password=external-secret;Encrypt=True",
            ["Security:EncryptionKey"] = "external-encryption-key",
            ["Jwt:SigningKey"] = "external-jwt-signing-key-with-at-least-32-characters",
            ["AllowedHosts"] = "api.example.internal",
            ["DatabaseInitialization:InitializeMasterOnStartup"] = "false"
        }).Build();

        ProductionConfigurationValidator.Validate(configuration, environment);
    }

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while(directory is not null && !File.Exists(Path.Combine(directory.FullName,"nuansystem.sln"))) directory=directory.Parent;
        return directory?.FullName ?? throw new InvalidOperationException("Root no encontrado.");
    }
}
