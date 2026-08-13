using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Authentication;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Abstractions.Operations;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Sri;
using NuanSystem.Persistence.Connections;
using NuanSystem.Persistence.Options;
using NuanSystem.Persistence.Repositories;
using NuanSystem.Persistence.Repositories.Documents;
using NuanSystem.Persistence.Repositories.FinancialCatalogs;
using NuanSystem.Persistence.Repositories.GeneralInventory;
using NuanSystem.Persistence.Repositories.GeneralSupplier;
using NuanSystem.Persistence.Repositories.Definitions.General;
using NuanSystem.Persistence.Repositories.Definitions.Inventory;
using NuanSystem.Persistence.Repositories.OperationalCatalogs;
using NuanSystem.Persistence.Repositories.Purchasing;
using NuanSystem.Persistence.Repositories.SapSync;
using NuanSystem.Persistence.Repositories.Operations;
using NuanSystem.Persistence.Repositories.Sync;
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

        services.Configure<SqlConnectionPolicyOptions>(options =>
        {
            if (bool.TryParse(configuration[$"{SqlConnectionPolicyOptions.SectionName}:Encrypt"], out var encrypt))
            {
                options.Encrypt = encrypt;
            }

            if (bool.TryParse(configuration[$"{SqlConnectionPolicyOptions.SectionName}:TrustServerCertificate"], out var trustServerCertificate))
            {
                options.TrustServerCertificate = trustServerCertificate;
            }
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
        services.AddScoped<ICarrierRepository, CarrierRepository>();
        services.AddScoped<ISriDocumentQueueRepository, SriDocumentQueueRepository>();
        services.AddScoped<ISriTxtImportRepository, SriTxtImportRepository>();
        services.AddScoped<ISriWorkerCompanyRepository, SriWorkerRepository>();
        services.AddScoped<ISriWorkerQueueRepository, SriWorkerRepository>();
        services.AddScoped<IFinancialCatalogRepository, FinancialCatalogRepository>();
        services.AddScoped<IPriceListRepository, PriceListRepository>();
        services.AddScoped<IGeographyRepository, GeographyRepository>();
        services.AddScoped<IGeneralInventoryCatalogRepository, GeneralInventoryCatalogRepository>();
        services.AddScoped<IItemTypeRepository, ItemTypeRepository>();
        services.AddScoped<IGeneralSupplierCatalogRepository, GeneralSupplierCatalogRepository>();
        services.AddScoped<IOperationalCatalogRepository, OperationalCatalogRepository>();
        services.AddScoped<ISecurityDocumentSeriesRepository, SecurityDocumentSeriesRepository>();
        services.AddScoped<ISecurityDocumentNumberingService, SecurityDocumentNumberingService>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<ISapPurchaseOrderImportRepository, SapPurchaseOrderImportRepository>();
        services.AddScoped<ISapPaymentTermImportRepository, SapPaymentTermImportRepository>();
        services.AddScoped<ITaxCatalogRepository, TaxCatalogRepository>();
        services.AddScoped<ITaxRepository, TaxRepository>();
        services.AddScoped<ICompanyAdminRepository, CompanyAdminRepository>();
        services.AddScoped<IConfigurationCompanyRepository, ConfigurationCompanyRepository>();
        services.AddScoped<ITenantFeatureRepository, TenantFeatureRepository>();
        services.AddScoped<ITenantIntegrationRepository, TenantIntegrationRepository>();
        services.AddScoped<IEntityOwnershipRepository, EntityOwnershipRepository>();
        services.AddScoped<ICompanyConnectionTester, SqlServerCompanyConnectionTester>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IItemGroupRepository, ItemGroupRepository>();
        services.AddScoped<IItemFamilyRepository, NuanSystem.Persistence.Repositories.Definitions.Inventory.ItemFamilyRepository>();
        services.AddScoped<IItemBrandRepository, ItemBrandRepository>();
        services.AddScoped<IUnitMeasureRepository, UnitMeasureRepository>();
        services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
        services.AddScoped<IItemLineRepository, ItemLineRepository>();
        services.AddScoped<IChartOfAccountRepository, ChartOfAccountRepository>();
        services.AddScoped<ISapCompanySettingsRepository, SapCompanySettingsRepository>();
        services.AddScoped<ISapCatalogMappingRepository, SapCatalogMappingRepository>();
        services.AddScoped<ISapSyncLogRepository, SapSyncLogRepository>();
        services.AddScoped<ISapSyncCompanyRepository, SapSyncCompanyRepository>();
        services.AddScoped<ISapSyncProfileRepository, SapSyncProfileRepository>();
        services.AddScoped<ISapSyncScheduleRepository, SapSyncScheduleRepository>();
        services.AddScoped<ISapSyncExecutionRepository, SapSyncExecutionRepository>();
        services.AddScoped<ISapSyncSettingsRepository, SapSyncSettingsRepository>();
        services.AddScoped<ISapSyncOutboxRepository, SapSyncOutboxRepository>();
        services.AddScoped<ISapSyncInboxRepository, SapSyncInboxRepository>();
        services.AddScoped<ISapSyncTechnicalLogRepository, SapSyncTechnicalLogRepository>();
        services.AddScoped<ISapSyncWatermarkRepository, SapSyncWatermarkRepository>();
        services.AddScoped<ISapSyncLockRepository, SapSyncLockRepository>();
        services.AddScoped<IWorkerHeartbeatRepository, WorkerHeartbeatRepository>();
        services.AddScoped<ICompanyParameterRepository, CompanyParameterRepository>();
        services.AddScoped<IConfigurationSettingRepository, ConfigurationSettingRepository>();
        services.AddScoped<IUserCredentialRepository, SqlServerUserCredentialRepository>();
        services.AddScoped<IUserAdminRepository, UserAdminRepository>();
        services.AddScoped<ISecurityRoleRepository, SecurityRoleRepository>();
        services.AddScoped<ISecurityOperationRepository, SecurityOperationRepository>();
        services.AddScoped<ISecurityMenuRepository, SecurityMenuRepository>();
        services.AddScoped<ISecurityFormRepository, SecurityFormRepository>();
        services.AddScoped<ISecurityFieldRepository, SecurityFieldRepository>();
        services.AddScoped<ISecurityAccessRepository, SecurityAccessRepository>();
        services.AddScoped<ISecurityRoleFormAccessRepository, SecurityRoleFormAccessRepository>();
        services.AddScoped<ISecurityRoleFormFieldAccessRepository, SecurityRoleFormFieldAccessRepository>();
        services.AddScoped<ISecurityDocumentSeriesAccessRepository, SecurityDocumentSeriesAccessRepository>();
        services.AddScoped<IGridColumnSettingsRepository, GridColumnSettingsRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IInventoryAuditRepository, InventoryAuditRepository>();
        services.AddScoped<ISyncOutboxRepository, SyncOutboxRepository>();
        services.AddScoped<ILocalSyncOutboxRepository, LocalSyncOutboxRepository>();
        services.AddScoped<ISyncOutboxPromotionRepository, SyncOutboxPromotionRepository>();
        services.AddScoped<ISyncInboxRepository, SyncInboxRepository>();
        services.AddScoped<ISyncAuditRepository, SyncAuditRepository>();
        services.AddScoped<ISyncRuleEvaluator, SyncRuleEvaluator>();
        services.AddScoped<ISyncProfileRepository, SyncProfileRepository>();
        services.AddScoped<ISyncEntityDefinitionRepository, SyncEntityDefinitionRepository>();
        services.AddScoped<ISyncProfileExecutionRepository, SyncProfileExecutionRepository>();
        services.AddScoped<ISyncRoutingRepository, SyncRoutingRepository>();
        services.AddScoped<ISyncDistributionPolicyRepository, SyncDistributionPolicyRepository>();
        services.AddScoped<IReplicableEntityMetadataProvider, ReplicableEntityMetadataProvider>();
        services.AddScoped<ISyncFullEntitySource, CountryFullEntitySource>();
        services.AddScoped<ISyncFullEntitySource, ProvinceFullEntitySource>();
        services.AddScoped<ISyncFullEntitySource, CityFullEntitySource>();
        services.AddScoped<ISyncFullEntitySource, CurrencyFullEntitySource>();
        services.AddScoped<ISyncFullEntitySource, TaxFullEntitySource>();
        services.AddScoped<ISyncFullEntitySource, PriceListFullEntitySource>();
        services.AddScoped<ISyncFullEntitySource, PaymentTermFullEntitySource>();
        services.AddScoped<ISyncFullEntitySource, BusinessPartnerFullEntitySource>();
        services.AddScoped<ISyncFullEntitySource, CarrierFullEntitySource>();
        services.AddScoped<ISyncFullEntitySource, ItemGroupFullEntitySource>();
        services.AddScoped<ISyncFullEntitySource, ItemFamilyFullEntitySource>();
        services.AddScoped<ISyncFullEntitySource, ItemBrandFullEntitySource>();
        services.AddScoped<ISyncFullEntitySource, UnitMeasureFullEntitySource>();
        services.AddScoped<ISyncFullEntitySource, ProductTypeFullEntitySource>();
        services.AddScoped<ISyncFullEntitySource, ItemLineFullEntitySource>();
        services.AddScoped<ISyncFullEntitySource, ItemFullEntitySource>();
        services.AddScoped<ISyncFullEntitySource, WarehouseFullEntitySource>();
        services.AddScoped<IBusinessPartnerSyncApplyRepository, BusinessPartnerSyncApplyRepository>();
        services.AddScoped<IItemSyncApplyRepository, ItemSyncApplyRepository>();
        services.AddScoped<IWarehouseSyncApplyRepository, WarehouseSyncApplyRepository>();
        services.AddScoped<ICountrySyncApplyRepository, CountrySyncApplyRepository>();
        services.AddScoped<IProvinceSyncApplyRepository, ProvinceSyncApplyRepository>();
        services.AddScoped<ICitySyncApplyRepository, CitySyncApplyRepository>();
        services.AddScoped<ICurrencySyncApplyRepository, CurrencySyncApplyRepository>();
        services.AddScoped<IPriceListSyncApplyRepository, PriceListSyncApplyRepository>();
        services.AddScoped<ITaxSyncApplyRepository, TaxSyncApplyRepository>();
        services.AddScoped<IReferenceCatalogSyncApplyRepository, ReferenceCatalogSyncApplyRepository>();
        services.AddScoped<IPurchaseOrderRoutingRepository, PurchaseOrderRoutingRepository>();
        services.AddScoped<IPurchaseOrderSyncApplyRepository, PurchaseOrderSyncApplyRepository>();
        services.AddScoped<IItemGroupSyncApplyRepository, ItemGroupSyncApplyRepository>();
        services.AddScoped<IItemFamilySyncApplyRepository, ItemFamilySyncApplyRepository>();
        services.AddScoped<IItemBrandSyncApplyRepository, ItemBrandSyncApplyRepository>();
        services.AddScoped<IUnitMeasureSyncApplyRepository, UnitMeasureSyncApplyRepository>();
        services.AddScoped<IProductTypeSyncApplyRepository, ProductTypeSyncApplyRepository>();
        services.AddScoped<IItemLineSyncApplyRepository, ItemLineSyncApplyRepository>();
        services.AddScoped<ICarrierSyncApplyRepository, CarrierSyncApplyRepository>();

        return services;
    }
}
