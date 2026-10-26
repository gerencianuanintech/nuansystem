using Microsoft.Extensions.DependencyInjection;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.DependencyInjection;
using NuanSystem.Application.Features.SapSync.Constants;
using NuanSystem.Application.Features.SapSync.Profiles;
using NuanSystem.Application.Features.SapSync.Services;

namespace NuanSystem.Application.Tests.Architecture;

public sealed class PreSyncCapabilityBoundaryTests
{
    [Fact]
    public void Application_composition_does_not_register_a_productive_SAP_exporter()
    {
        var services = new ServiceCollection();

        services.AddApplicationServices();

        Assert.DoesNotContain(services, service => service.ServiceType == typeof(ISapSyncExporter));
    }

    [Fact]
    public void Purchase_order_export_is_not_advertised_without_an_executable_exporter()
    {
        var registry = new SapSyncProcessorRegistry(
            Array.Empty<ISapSyncImporter>(),
            Array.Empty<ISapSyncExporter>(),
            Array.Empty<ISapSyncScheduledExecutionProcessor>(),
            Array.Empty<ISapSyncExecutionRetryProcessor>());
        var configured = new SapSyncHandlerCapabilityDto(
            SapSyncEntityCode.PurchaseOrders,
            "Ordenes de compra",
            SupportsSapToErp: false,
            SupportsErpToSap: true,
            SupportsFull: true,
            SupportsIncremental: false,
            IsImplemented: true,
            IsActive: true);

        var executable = registry.Intersect(configured);

        Assert.Null(registry.FindExporter(SapSyncEntityCode.PurchaseOrders));
        Assert.False(executable.SupportsErpToSap);
        Assert.False(executable.IsImplemented);
    }
}
