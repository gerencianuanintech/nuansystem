using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Authentication;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Persistence.Connections;
using NuanSystem.Persistence.Options;
using NuanSystem.Persistence.Repositories;
using NuanSystem.Persistence.Repositories.FinancialCatalogs;
using NuanSystem.Persistence.Repositories.GeneralInventory;
using NuanSystem.Persistence.Repositories.GeneralSupplier;
using NuanSystem.Persistence.Repositories.Geography;
using NuanSystem.Persistence.Repositories.Purchasing;
using NuanSystem.Persistence.Repositories.TaxCatalogs;
using NuanSystem.Persistence.Security;
using NuanSystem.Persistence.Services;
using NuanSystem.Persistence.Tenancy;
using NuanSystem.Persistence.Transactions;

namespace NuanSystem.Persistence.DependencyInjection;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MasterDatabaseOptions>(options =>
        {
            options.DatabaseName = configuration[$"{MasterDatabaseOptions.SectionName}:DatabaseName"]
                ?? options.DatabaseName;
        });

        services.AddSingleton<IMasterDatabaseInitializer, SqlServerMasterDatabaseInitializer>();
        services.AddScoped<ICompanyContext, CompanyContext>();
        services.AddScoped<MasterConnectionFactory>();
        services.AddScoped<IMasterConnectionFactory>(provider => provider.GetRequiredService<MasterConnectionFactory>());
        services.AddScoped<ITenantConnectionFactory, TenantConnectionFactory>();
        services.AddScoped<ITransactionRunner, SqlTransactionRunner>();
        services.AddScoped<ITenantDatabaseInitializer, SqlServerTenantDatabaseInitializer>();
        services.AddScoped<ICompanyResolver, SqlServerCompanyResolver>();
        services.AddScoped<ITenantConnectionStringResolver, TenantConnectionStringResolver>();
        services.AddScoped<IAuthService, SqlServerAuthService>();
        services.AddScoped<IUserSecurityStateService, UserSecurityStateService>();
        services.AddScoped<IBusinessPartnerRepository, BusinessPartnerRepository>();
        services.AddScoped<IFinancialCatalogRepository, FinancialCatalogRepository>();
        services.AddScoped<IGeographyRepository, GeographyRepository>();
        services.AddScoped<IGeneralInventoryCatalogRepository, GeneralInventoryCatalogRepository>();
        services.AddScoped<IGeneralSupplierCatalogRepository, GeneralSupplierCatalogRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<ITaxCatalogRepository, TaxCatalogRepository>();
        services.AddScoped<ICompanyAdminRepository, CompanyAdminRepository>();
        services.AddScoped<IConfigurationCompanyRepository, ConfigurationCompanyRepository>();
        services.AddScoped<ICompanyConnectionTester, SqlServerCompanyConnectionTester>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IItemGroupRepository, ItemGroupRepository>();
        services.AddScoped<IItemFamilyRepository, ItemFamilyRepository>();
        services.AddScoped<IChartOfAccountRepository, ChartOfAccountRepository>();
        services.AddScoped<ISapCompanySettingsRepository, SapCompanySettingsRepository>();
        services.AddScoped<ISapSyncLogRepository, SapSyncLogRepository>();
        services.AddScoped<ICompanyParameterRepository, CompanyParameterRepository>();
        services.AddScoped<IConfigurationSettingRepository, ConfigurationSettingRepository>();
        services.AddScoped<IUserAdminRepository, UserAdminRepository>();
        services.AddScoped<IRoleAdminRepository, RoleAdminRepository>();
        services.AddScoped<ISecurityRoleRepository, SecurityRoleRepository>();
        services.AddScoped<ISecurityOperationRepository, SecurityOperationRepository>();
        services.AddScoped<ISecurityMenuRepository, SecurityMenuRepository>();
        services.AddScoped<ISecurityFormRepository, SecurityFormRepository>();
        services.AddScoped<ISecurityFieldRepository, SecurityFieldRepository>();
        services.AddScoped<ISecurityAccessRepository, SecurityAccessRepository>();
        services.AddScoped<IGridColumnSettingsRepository, GridColumnSettingsRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IInventoryAuditRepository, InventoryAuditRepository>();

        return services;
    }
}
