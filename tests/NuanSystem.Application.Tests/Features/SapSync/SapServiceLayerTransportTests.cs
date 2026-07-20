using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NuanSystem.SapIntegration.DependencyInjection;
using NuanSystem.SapIntegration.ServiceLayer;

namespace NuanSystem.Application.Tests.Features.SapSync;

public sealed class SapServiceLayerTransportTests
{
    [Fact]
    public void AddSapIntegrationServices_RejectsUnsafeCertificatesWithoutExplicitDevelopmentApproval()
    {
        var configuration = Configuration(new Dictionary<string, string?>
        {
            ["ServiceLayer:IgnoreSslErrors"] = "true"
        });
        var services = new ServiceCollection();

        var action = () => services.AddSapIntegrationServices(
            configuration,
            allowUnsafeServerCertificates: false);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*solo puede activarse*desarrollo*");
    }

    [Fact]
    public void HandlerFactory_WiresDangerousValidatorOnlyWhenExplicitlyEnabled()
    {
        using var strict = SapServiceLayerHttpMessageHandlerFactory.Create(ignoreSslErrors: false);
        using var development = SapServiceLayerHttpMessageHandlerFactory.Create(ignoreSslErrors: true);

        strict.UseCookies.Should().BeFalse();
        strict.ServerCertificateCustomValidationCallback.Should().BeNull();
        development.UseCookies.Should().BeFalse();
        development.ServerCertificateCustomValidationCallback.Should().BeSameAs(
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator);
    }

    [Fact]
    public void AddSapIntegrationServices_AllowsUnsafeCertificatesOnlyWithExplicitDevelopmentApproval()
    {
        var configuration = Configuration(new Dictionary<string, string?>
        {
            ["ServiceLayer:IgnoreSslErrors"] = "true",
            ["ServiceLayer:HttpTimeoutSeconds"] = "45"
        });
        var services = new ServiceCollection();

        var action = () => services.AddSapIntegrationServices(
            configuration,
            allowUnsafeServerCertificates: true);

        action.Should().NotThrow();
    }

    private static IConfiguration Configuration(IReadOnlyDictionary<string, string?> values) =>
        new ConfigurationBuilder().AddInMemoryCollection(values).Build();
}
