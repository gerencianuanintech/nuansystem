using Microsoft.Extensions.DependencyInjection;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.SapIntegration.Abstractions;
using NuanSystem.SapIntegration.Clients;
using NuanSystem.SapIntegration.Clients.DiApi;
using NuanSystem.SapIntegration.Clients.ServiceLayer;
using NuanSystem.SapIntegration.Documents;

namespace NuanSystem.SapIntegration.DependencyInjection;

public static class SapIntegrationServiceRegistration
{
    public static IServiceCollection AddSapIntegrationServices(this IServiceCollection services)
    {
        services.AddHttpClient("SapServiceLayer");
        services.AddScoped<SapServiceLayerClient>();
        services.AddScoped<SapDiApiClient>();
        services.AddScoped<ISapClientFactory, SapClientFactory>();
        services.AddScoped<ISapDocumentSender, SapDocumentSender>();

        return services;
    }
}
