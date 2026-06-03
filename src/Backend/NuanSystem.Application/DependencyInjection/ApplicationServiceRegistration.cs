using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Common.Behaviors;
using NuanSystem.Application.Features.SapSync.Handlers;
using NuanSystem.Application.Features.SapSync.Services;
using System.Reflection;

namespace NuanSystem.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<ISapSupplierImportService, SapSupplierImportService>();
        services.AddScoped<ISapSyncOrchestrator, SapSyncOrchestrator>();
        services.AddScoped<ISapSyncJobRunner, SapSyncJobRunner>();
        services.AddScoped<ISapSyncLockService, SapSyncLockService>();
        services.AddScoped<ISapSyncWatermarkService, SapSyncWatermarkService>();
        services.AddScoped<ISapSyncLogService, SapSyncLogService>();
        services.AddScoped<ISapSyncRetryPolicy, SapSyncRetryPolicy>();
        services.AddScoped<IWorkerHeartbeatService, WorkerHeartbeatService>();
        services.AddScoped<ISapSyncEntityHandler, SapSupplierSyncHandler>();
        services.AddScoped<ISapSyncEntityHandler, SapItemSyncHandler>();
        services.AddScoped<ISapSyncEntityHandler, SapPurchaseOrderSyncHandler>();

        return services;
    }
}
