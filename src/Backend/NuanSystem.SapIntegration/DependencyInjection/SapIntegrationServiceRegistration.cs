using Microsoft.Extensions.DependencyInjection;
using NuanSystem.SapIntegration.Abstractions;
using NuanSystem.SapIntegration.Clients;
using NuanSystem.SapIntegration.Clients.DiApi;
using NuanSystem.SapIntegration.Clients.ServiceLayer;
using NuanSystem.SapIntegration.Documents;
using NuanSystem.SapIntegration.Hana;
using NuanSystem.SapIntegration.Suppliers;
using NuanSystem.SapIntegration.Warehouses;
using NuanSystem.SapIntegration.Items;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.SapIntegration.ServiceLayer;

namespace NuanSystem.SapIntegration.DependencyInjection;

public static class SapIntegrationServiceRegistration
{
    public static IServiceCollection AddSapIntegrationServices(this IServiceCollection services)
    {
        services.AddHttpClient("SapServiceLayer")
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                // SAP sessions are scoped explicitly per request to avoid cross-company cookie reuse.
                UseCookies = false
            });
        services.AddScoped<SapServiceLayerClient>();
        services.AddScoped<SapDiApiClient>();
        services.AddScoped<ISapClientFactory, SapClientFactory>();
        services.AddScoped<ISapHanaConnectionFactory, SapHanaConnectionFactory>();
        services.AddScoped<ISapHanaQueryClient, SapHanaQueryClient>();
        services.AddScoped<ISapSupplierReader, SapSupplierReader>();
        services.AddScoped<ISapWarehouseReader, SapServiceLayerWarehouseReader>();
        services.AddScoped<ISapItemReader, SapServiceLayerItemReader>();
        services.AddScoped<SapServiceLayerQueryClient>();
        services.AddScoped<ISapPurchaseOrderReader, SapServiceLayerPurchaseOrderReader>();
        services.AddScoped<ISapDocumentSender, SapDocumentSender>();

        return services;
    }
}
