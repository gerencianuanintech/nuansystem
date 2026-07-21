using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Abstractions.Common;
using NuanSystem.Application.Common;
using NuanSystem.Application.Common.Behaviors;
using NuanSystem.Application.Features.SapSync.Handlers;
using NuanSystem.Application.Features.SapSync.Services;
using NuanSystem.Application.Features.SriDocuments.Services;
using NuanSystem.Application.Features.Sync.Configuration.Services;
using NuanSystem.Application.Features.Sync.Execution.Services;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Services;
using NuanSystem.Application.Features.Sync.Services;
using NuanSystem.Application.Features.TenantConfiguration.Services;
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
        services.AddScoped<ISapWarehouseImportService, SapWarehouseImportService>();
        services.AddScoped<ISapItemImportService, SapItemImportService>();
        services.AddScoped<ISapPurchaseOrderImportService, SapPurchaseOrderImportService>();
        services.AddScoped<ISapPaymentTermImportService, SapPaymentTermImportService>();
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
        services.AddScoped<ISapSyncEntityHandler, SapPaymentTermSyncHandler>();
        services.AddScoped<ITenantFeatureService, TenantFeatureService>();
        services.AddScoped<ITenantIntegrationService, TenantIntegrationService>();
        services.AddScoped<ISriDocumentQueuePolicy, SriDocumentQueuePolicy>();
        services.AddScoped<IEntityOwnershipService, EntityOwnershipService>();
        services.AddSingleton<ISystemClock, SystemClock>();
        services.AddScoped<ISyncEventPublisher, SyncEventPublisher>();
        services.AddScoped<ISyncEventPayloadFactory, SyncEventPayloadFactory>();
        services.AddScoped<ISyncRoutingService, SyncRoutingService>();
        services.AddSingleton<ISyncDistributionPolicyEvaluator, SyncDistributionPolicyEvaluator>();
        services.AddScoped<ISyncProfileValidationService, SyncProfileValidationService>();
        services.AddScoped<ISyncEntityCatalogService, SyncEntityCatalogService>();
        services.AddScoped<ISyncProfileExecutionService, SyncProfileExecutionService>();
        services.AddScoped<IPurchaseOrderRoutingService, PurchaseOrderRoutingService>();
        services.AddScoped<ISyncScheduleCalculator, SyncScheduleCalculator>();

        return services;
    }
}
