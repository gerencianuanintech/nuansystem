using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
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
using NuanSystem.SapIntegration.PaymentTerms;
using NuanSystem.SapIntegration.Countries;
using NuanSystem.SapIntegration.Provinces;
using NuanSystem.SapIntegration.Cities;

namespace NuanSystem.SapIntegration.DependencyInjection;

public static class SapIntegrationServiceRegistration
{
    public static IServiceCollection AddSapIntegrationServices(
        this IServiceCollection services,
        IConfiguration configuration,
        bool allowUnsafeServerCertificates = false)
    {
        var sectionName = SapServiceLayerTransportOptions.SectionName;
        var transport = new SapServiceLayerTransportOptions
        {
            HttpTimeoutSeconds = int.TryParse(configuration[$"{sectionName}:HttpTimeoutSeconds"], out var timeout)
                ? timeout
                : 100,
            IgnoreSslErrors = bool.TryParse(configuration[$"{sectionName}:IgnoreSslErrors"], out var ignoreSslErrors)
                && ignoreSslErrors
        };
        if (transport.IgnoreSslErrors && !allowUnsafeServerCertificates)
        {
            throw new InvalidOperationException(
                "ServiceLayer:IgnoreSslErrors solo puede activarse explícitamente en un entorno de desarrollo.");
        }

        services.AddHttpClient("SapServiceLayer")
            .ConfigureHttpClient(client =>
                client.Timeout = TimeSpan.FromSeconds(Math.Clamp(transport.HttpTimeoutSeconds, 5, 600)))
            .ConfigurePrimaryHttpMessageHandler(() =>
                SapServiceLayerHttpMessageHandlerFactory.Create(transport.IgnoreSslErrors));
        services.AddScoped<SapServiceLayerClient>();
        services.AddScoped<SapDiApiClient>();
        services.AddScoped<ISapClientFactory, SapClientFactory>();
        services.AddScoped<ISapHanaConnectionFactory, SapHanaConnectionFactory>();
        services.AddScoped<ISapHanaQueryClient, SapHanaQueryClient>();
        services.AddScoped<ISapSupplierReader, SapSupplierReader>();
        services.AddScoped<ISapWarehouseReader, SapServiceLayerWarehouseReader>();
        services.AddScoped<ISapCountryReader, SapServiceLayerCountryReader>();
        services.AddScoped<ISapProvinceReader, SapServiceLayerProvinceReader>();
        services.AddScoped<ISapCityReader, SapHanaCityReader>();
        services.AddScoped<ISapItemReader, SapServiceLayerItemReader>();
        services.AddScoped<SapServiceLayerQueryClient>();
        services.AddScoped<ISapPurchaseOrderReader, SapServiceLayerPurchaseOrderReader>();
        services.AddScoped<ISapDocumentSender, SapDocumentSender>();
        services.AddScoped<ISapPaymentTermReader, SapServiceLayerPaymentTermReader>();

        return services;
    }
}
