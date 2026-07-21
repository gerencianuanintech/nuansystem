using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Accounting.ChartOfAccounts;
using NuanSystem.WinForms.Forms.Auth;
using NuanSystem.WinForms.Forms.BusinessPartners;
using NuanSystem.WinForms.Forms.Carriers;
using NuanSystem.WinForms.Forms.ConfigurationCompanies;
using NuanSystem.WinForms.Forms.Documents.SecurityDocumentSeries;
using NuanSystem.WinForms.Forms.OperationalCatalogs;
using NuanSystem.WinForms.Forms.InventoryItems;
using NuanSystem.WinForms.Forms.Purchasing.PurchaseOrders;
using NuanSystem.WinForms.Forms.Sap;
using NuanSystem.WinForms.Forms.Sync;
using NuanSystem.WinForms.Forms.SriDocuments;
using NuanSystem.WinForms.Forms.Sync.Configuration;
using NuanSystem.WinForms.Forms.Sync.EntityDefinitions;
using NuanSystem.WinForms.Forms.Security.Operations;
using NuanSystem.WinForms.Forms.Security.Menus;
using NuanSystem.WinForms.Forms.Security.Forms;
using NuanSystem.WinForms.Forms.Security.Fields;
using NuanSystem.WinForms.Forms.Security.Access;
using NuanSystem.WinForms.Forms.ConfigurationSettings;
using NuanSystem.WinForms.Forms.Shell;
using NuanSystem.WinForms.Forms.Security.Users;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.GeneralSupplier.ContactChannels;
using NuanSystem.WinForms.Forms.GeneralSupplier.ContactTypes;
using NuanSystem.WinForms.Forms.GeneralSupplier.EconomicActivities;
using NuanSystem.WinForms.Forms.GeneralSupplier.SupplierClasses;
using NuanSystem.WinForms.Forms.GeneralSupplier.SupplierGroups;
using NuanSystem.WinForms.Forms.GeneralSupplier.SupplyMethods;
using NuanSystem.WinForms.Forms.GeneralSupplier.Zones;
using NuanSystem.WinForms.Forms.FinancialCatalogs.Banks;
using NuanSystem.WinForms.Forms.FinancialCatalogs.BankAccountTypes;
using NuanSystem.WinForms.Forms.FinancialCatalogs.AccountingPaymentMethods;
using NuanSystem.WinForms.Forms.FinancialCatalogs.ApprovalFlows;
using NuanSystem.WinForms.Forms.FinancialCatalogs.Branches;
using NuanSystem.WinForms.Forms.FinancialCatalogs.BusinessLines;
using NuanSystem.WinForms.Forms.FinancialCatalogs.Catalogs;
using NuanSystem.WinForms.Forms.FinancialCatalogs.CostCenters;
using NuanSystem.WinForms.Forms.FinancialCatalogs.Currencies;
using NuanSystem.WinForms.Forms.FinancialCatalogs.Departments;
using NuanSystem.WinForms.Forms.FinancialCatalogs.PaymentDocumentTypes;
using NuanSystem.WinForms.Forms.FinancialCatalogs.PaymentPriorities;
using NuanSystem.WinForms.Forms.FinancialCatalogs.PriceLists;
using NuanSystem.WinForms.Forms.FinancialCatalogs.Projects;
using NuanSystem.WinForms.Forms.FinancialCatalogs.PurchasingAgents;
using NuanSystem.WinForms.Forms.TaxCatalogs.Catalogs;
using NuanSystem.WinForms.Forms.TaxCatalogs.RetentionConcepts;
using NuanSystem.WinForms.Forms.TaxCatalogs.RetentionTypes;
using NuanSystem.WinForms.Forms.TaxCatalogs.TaxRegimes;
using NuanSystem.WinForms.Forms.TaxCatalogs.TaxSupports;
using NuanSystem.WinForms.Forms.TaxCatalogs.TaxpayerTypes;
using NuanSystem.WinForms.Forms.Geography.Cities;
using NuanSystem.WinForms.Forms.Geography.Countries;
using NuanSystem.WinForms.Forms.Geography.Provinces;
using NuanSystem.WinForms.Forms.GeneralInventory.AttachmentCategories;
using NuanSystem.WinForms.Forms.GeneralInventory.AttachmentDocumentTypes;
using NuanSystem.WinForms.Forms.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Forms.GeneralInventory.ItemBrands;
using NuanSystem.WinForms.Forms.GeneralInventory.ItemFamilies;
using NuanSystem.WinForms.Forms.GeneralInventory.ItemGroups;
using NuanSystem.WinForms.Forms.GeneralInventory.ItemLines;
using NuanSystem.WinForms.Forms.GeneralInventory.ItemSubgroups;
using NuanSystem.WinForms.Forms.GeneralInventory.ItemTypes;
using NuanSystem.WinForms.Forms.GeneralInventory.ProductTypes;
using NuanSystem.WinForms.Forms.GeneralInventory.ReplenishmentMethods;
using NuanSystem.WinForms.Forms.GeneralInventory.SalesChannels;
using NuanSystem.WinForms.Forms.GeneralInventory.StorageConditions;
using NuanSystem.WinForms.Forms.GeneralInventory.StorageZones;
using NuanSystem.WinForms.Forms.GeneralInventory.UnitMeasures;
using NuanSystem.WinForms.Forms.GeneralInventory.VariantAttributes;
using NuanSystem.WinForms.Forms.GeneralInventory.WarehouseLocations;
using NuanSystem.WinForms.Forms.GeneralInventory.Warehouses;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts;
using NuanSystem.WinForms.Services.Authentication;
using NuanSystem.WinForms.Services.Companies;
using NuanSystem.WinForms.Services.ConfigurationCompanies;
using NuanSystem.WinForms.Services.ConfigurationSettings;
using NuanSystem.WinForms.Services.Configuration;
using NuanSystem.WinForms.Services.SriDocuments;
using NuanSystem.WinForms.ViewModels.SriDocuments;
using NuanSystem.WinForms.Services.Documents.SecurityDocumentSeries;
using NuanSystem.WinForms.Services.OperationalCatalogs;
using NuanSystem.WinForms.Services.BusinessPartners;
using NuanSystem.WinForms.Services.Carriers;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.FinancialCatalogs.Catalogs;
using NuanSystem.WinForms.Services.TaxCatalogs.Catalogs;
using NuanSystem.WinForms.Services.Geography;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralSupplier.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies;
using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups;
using NuanSystem.WinForms.Services.GeneralInventory.Warehouses;
using NuanSystem.WinForms.Services.InventoryItems;
using NuanSystem.WinForms.Services.Purchasing.PurchaseOrders;
using NuanSystem.WinForms.Services.Sap;
using NuanSystem.WinForms.Services.Sync;
using NuanSystem.WinForms.Services.Sync.EntityDefinitions;
using NuanSystem.WinForms.Services.Security.Operations;
using NuanSystem.WinForms.Services.Security.Menus;
using NuanSystem.WinForms.Services.Security.Forms;
using NuanSystem.WinForms.Services.Security.Fields;
using NuanSystem.WinForms.Services.Security.Access;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.Settings;
using NuanSystem.WinForms.Services.Security.Users;
using NuanSystem.WinForms.ViewModels.Audit;
using NuanSystem.WinForms.ViewModels.Accounting.ChartOfAccounts;
using NuanSystem.WinForms.ViewModels.Auth;
using NuanSystem.WinForms.ViewModels.BusinessPartners;
using NuanSystem.WinForms.ViewModels.Carriers;
using NuanSystem.WinForms.ViewModels.Companies;
using NuanSystem.WinForms.ViewModels.ConfigurationCompanies;
using NuanSystem.WinForms.ViewModels.ConfigurationSettings;
using NuanSystem.WinForms.ViewModels.Documents.SecurityDocumentSeries;
using NuanSystem.WinForms.ViewModels.OperationalCatalogs;
using NuanSystem.WinForms.ViewModels.FinancialCatalogs.Catalogs;
using NuanSystem.WinForms.ViewModels.TaxCatalogs.Catalogs;
using NuanSystem.WinForms.ViewModels.Geography;
using NuanSystem.WinForms.ViewModels.GeneralInventory.Catalogs;
using NuanSystem.WinForms.ViewModels.GeneralSupplier.Catalogs;
using NuanSystem.WinForms.ViewModels.GeneralInventory.ItemFamilies;
using NuanSystem.WinForms.ViewModels.GeneralInventory.ItemGroups;
using NuanSystem.WinForms.ViewModels.GeneralInventory.Warehouses;
using NuanSystem.WinForms.ViewModels.InventoryItems;
using NuanSystem.WinForms.ViewModels.Purchasing.PurchaseOrders;
using NuanSystem.WinForms.ViewModels.Sap;
using NuanSystem.WinForms.ViewModels.Sync;
using NuanSystem.WinForms.ViewModels.Sync.EntityDefinitions;
using NuanSystem.WinForms.ViewModels.Security.Operations;
using NuanSystem.WinForms.ViewModels.Security.Menus;
using NuanSystem.WinForms.ViewModels.Security.Forms;
using NuanSystem.WinForms.ViewModels.Security.Fields;
using NuanSystem.WinForms.ViewModels.Security.Access;
using NuanSystem.WinForms.ViewModels.Settings;
using NuanSystem.WinForms.ViewModels.Shell;
using NuanSystem.WinForms.ViewModels.Security.Users;
using RoleMaintenanceClient = NuanSystem.WinForms.Services.Security.Roles.RoleClient;
using RoleMaintenanceForm = NuanSystem.WinForms.Forms.Security.Roles.RolesForm;
using RoleMaintenanceViewModel = NuanSystem.WinForms.ViewModels.Security.Roles.RolesViewModel;

namespace NuanSystem.WinForms;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            using var composition = BuildComposition();
            composition.ConfigureGlobalErrorHandling();

            while (true)
            {
                composition.ClearSession();
                using var loginForm = composition.CreateLoginForm();
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                using var mainForm = composition.CreateMainForm();
                var mainResult = mainForm.ShowDialog();
                if (mainResult != DialogResult.Retry)
                {
                    return;
                }
            }
        }
        catch (Exception exception)
        {
            TryWriteStartupError(exception);
            MessageBox.Show(
                "No fue posible iniciar NuanSystem. Revise la configuracion de la URL de API y el log de arranque.",
                "NuanSystem",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private static FrontendComposition BuildComposition()
    {
        var baseUrl = NormalizeApiBaseUrl(Environment.GetEnvironmentVariable("NUANSYSTEM_API_URL"));
        var options = new ApiClientOptions
        {
            BaseUrl = baseUrl
        };

        return new FrontendComposition(options);
    }

    private static string NormalizeApiBaseUrl(string? configuredBaseUrl)
    {
        if (string.IsNullOrWhiteSpace(configuredBaseUrl))
        {
            return "https://localhost:7293";
        }

        return Uri.TryCreate(configuredBaseUrl, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            ? uri.ToString().TrimEnd('/')
            : "https://localhost:7293";
    }

    private static void TryWriteStartupError(Exception exception)
    {
        try
        {
            var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logDirectory);
            var logPath = Path.Combine(logDirectory, $"winforms-startup-{DateTime.UtcNow:yyyyMMdd}.log");
            File.AppendAllText(
                logPath,
                $"[{DateTime.UtcNow:O}] {exception}{Environment.NewLine}{Environment.NewLine}");
        }
        catch
        {
            // Startup logging must never prevent the user-facing error message.
        }
    }
}

internal sealed class FrontendComposition : IDisposable
{
    private readonly HttpClient httpClient;
    private readonly ApiSession session = new();
    private readonly NuanApiClient apiClient;
    private readonly AuthenticationClient authenticationClient;
    private readonly CompanyClient companyClient;
    private readonly ConfigurationCompanyClient configurationCompanyClient;
    private readonly ConfigurationSettingClient configurationSettingClient;
    private readonly BusinessPartnerClient businessPartnerClient;
    private readonly CarrierClient carrierClient;
    private readonly ChartOfAccountClient chartOfAccountClient;
    private readonly FinancialCatalogClient financialCatalogClient;
    private readonly TaxCatalogClient taxCatalogClient;
    private readonly GeographyClient geographyClient;
    private readonly GeneralSupplierCatalogClient generalSupplierCatalogClient;
    private readonly GeneralInventoryCatalogClient generalInventoryCatalogClient;
    private readonly ItemGroupClient itemGroupClient;
    private readonly WarehouseClient warehouseClient;
    private readonly SecurityDocumentSeriesClient securityDocumentSeriesClient;
    private readonly OperationalCatalogClient operationalCatalogClient;
    private readonly ItemFamilyClient itemFamilyClient;
    private readonly ItemClient itemClient;
    private readonly PurchaseOrderClient purchaseOrderClient;
    private readonly SapClient sapClient;
    private readonly SyncMonitorClient syncMonitorClient;
    private readonly SriDocumentMonitorClient sriDocumentMonitorClient;
    private readonly SyncConfigurationClient syncConfigurationClient;
    private readonly SyncEntityDefinitionClient syncEntityDefinitionClient;
    private readonly AuditClient auditClient;
    private readonly SettingsClient settingsClient;
    private readonly UserClient userClient;
    private readonly RoleMaintenanceClient roleMaintenanceClient;
    private readonly OperationClient securityOperationClient;
    private readonly MenuClient securityMenuClient;
    private readonly FormClient securityFormClient;
    private readonly FieldClient securityFieldClient;
    private readonly SecurityAccessClient securityAccessClient;
    private readonly SecurityRoleFormAccessClient securityRoleFormAccessClient;
    private readonly SecurityTransactionalFormAccessClient securityTransactionalFormAccessClient;
    private readonly SecurityDocumentSeriesAccessClient securityDocumentSeriesAccessClient;
    private readonly SecurityRoleFormFieldAccessClient securityMaintenanceFieldAccessClient;
    private readonly SecurityRoleFormFieldAccessClient securityTransactionalFieldAccessClient;
    private readonly GridColumnSettingsClient gridColumnSettingsClient;

    public FrontendComposition(ApiClientOptions options)
    {
        httpClient = new HttpClient
        {
            BaseAddress = new Uri(options.BaseUrl),
            Timeout = options.Timeout
        };

        apiClient = new NuanApiClient(httpClient, session);
        authenticationClient = new AuthenticationClient(apiClient, session);
        companyClient = new CompanyClient(apiClient);
        configurationCompanyClient = new ConfigurationCompanyClient(apiClient);
        configurationSettingClient = new ConfigurationSettingClient(apiClient);
        businessPartnerClient = new BusinessPartnerClient(apiClient);
        carrierClient = new CarrierClient(apiClient);
        chartOfAccountClient = new ChartOfAccountClient(apiClient);
        financialCatalogClient = new FinancialCatalogClient(apiClient);
        taxCatalogClient = new TaxCatalogClient(apiClient);
        geographyClient = new GeographyClient(apiClient);
        generalSupplierCatalogClient = new GeneralSupplierCatalogClient(apiClient);
        generalInventoryCatalogClient = new GeneralInventoryCatalogClient(apiClient);
        itemGroupClient = new ItemGroupClient(apiClient);
        warehouseClient = new WarehouseClient(apiClient);
        securityDocumentSeriesClient = new SecurityDocumentSeriesClient(apiClient);
        operationalCatalogClient = new OperationalCatalogClient(apiClient);
        itemFamilyClient = new ItemFamilyClient(apiClient);
        itemClient = new ItemClient(apiClient);
        purchaseOrderClient = new PurchaseOrderClient(apiClient);
        sapClient = new SapClient(apiClient);
        syncMonitorClient = new SyncMonitorClient(apiClient);
        sriDocumentMonitorClient = new SriDocumentMonitorClient(apiClient);
        syncConfigurationClient = new SyncConfigurationClient(apiClient);
        syncEntityDefinitionClient = new SyncEntityDefinitionClient(apiClient);
        auditClient = new AuditClient(apiClient);
        settingsClient = new SettingsClient(apiClient);
        userClient = new UserClient(apiClient);
        roleMaintenanceClient = new RoleMaintenanceClient(apiClient);
        securityOperationClient = new OperationClient(apiClient);
        securityMenuClient = new MenuClient(apiClient);
        securityFormClient = new FormClient(apiClient);
        securityFieldClient = new FieldClient(apiClient);
        securityAccessClient = new SecurityAccessClient(apiClient);
        securityRoleFormAccessClient = new SecurityRoleFormAccessClient(apiClient);
        securityTransactionalFormAccessClient = new SecurityTransactionalFormAccessClient(apiClient);
        securityDocumentSeriesAccessClient = new SecurityDocumentSeriesAccessClient(apiClient);
        securityMaintenanceFieldAccessClient = new SecurityRoleFormFieldAccessClient(apiClient, "/api/security/maintenance-field-access");
        securityTransactionalFieldAccessClient = new SecurityRoleFormFieldAccessClient(apiClient, "/api/security/transactional-field-access");
        gridColumnSettingsClient = new GridColumnSettingsClient(apiClient);
    }

    public void ConfigureGlobalErrorHandling()
    {
        GlobalUiExceptionHandler.Configure(auditClient, session);
        GlobalUiExceptionHandler.RegisterApplicationHandlers();
    }

    public LoginForm CreateLoginForm()
    {
        return new LoginForm(
            new LoginViewModel(authenticationClient),
            new CompanySelectionViewModel(companyClient, session),
            apiClient);
    }

    public MainForm CreateMainForm()
    {
        var shellViewModel = new ShellViewModel(session, securityAccessClient);
        shellViewModel.LoadNavigationAsync().GetAwaiter().GetResult();

        return new MainForm(
            shellViewModel,
            CreateConfigurationCompaniesForm,
            CreateUsersForm,
            CreateRolesForm,
            CreateOperationsForm,
            CreateMenusForm,
            CreateFormsForm,
            CreateFieldsForm,
            CreateSecurityMaintenanceFormAccessForm,
            CreateSecurityTransactionalFormAccessForm,
            CreateSecurityMaintenanceFieldAccessForm,
            CreateSecurityTransactionalFieldAccessForm,
            CreateCustomersForm,
            CreateSuppliersForm,
            CreateCarriersForm,
            CreateChartOfAccountsForm,
            CreateSupplierGroupsForm,
            CreateSupplierClassesForm,
            CreateEconomicActivitiesForm,
            CreateZonesForm,
            CreateSupplyMethodsForm,
            CreateContactTypesForm,
            CreateContactChannelsForm,
            CreateBanksForm,
            CreateBankAccountTypesForm,
            CreateCurrenciesForm,
            CreatePriceListsForm,
            CreatePurchasingAgentsForm,
            CreateAccountingPaymentMethodsForm,
            CreatePaymentPrioritiesForm,
            CreateApprovalFlowsForm,
            CreatePaymentDocumentTypesForm,
            CreateBranchesForm,
            CreateDepartmentsForm,
            CreateBusinessLinesForm,
            CreateCostCentersForm,
            CreateProjectsForm,
            CreateTaxRegimesForm,
            CreateTaxpayerTypesForm,
            CreateRetentionTypesForm,
            CreateRetentionConceptsForm,
            CreateTaxSupportsForm,
            CreateCountriesForm,
            CreateProvincesForm,
            CreateCitiesForm,
            CreateGeneralInventoryCatalogForm,
            CreateItemGroupsForm,
            CreateSecurityDocumentSeriesForm,
            CreateOperationalCatalogsForm,
            CreateItemFamiliesForm,
            CreateItemsForm,
            CreatePurchaseOrdersForm,
            CreateSapSyncLogForm,
            CreateSyncMonitorForm,
            CreateSriDocumentMonitorForm,
            CreateSyncProfileListForm,
            CreateSyncEntityListForm,
            CreateSyncExecutionListForm,
            CreateAuditLogsForm,
            CreateSettingsForm);
    }

    public void ClearSession()
    {
        session.Clear();
    }

    public BusinessPartnersForm CreateCustomersForm()
    {
        return new BusinessPartnersForm(
            new BusinessPartnersViewModel(businessPartnerClient, "Customer", "customers"),
            session,
            auditClient,
            gridColumnSettingsClient,
            "Customer",
            "customers",
            "Clientes");
    }

    public BusinessPartnersForm CreateSuppliersForm()
    {
        return new BusinessPartnersForm(
            new BusinessPartnersViewModel(businessPartnerClient, "Supplier", "suppliers"),
            session,
            auditClient,
            gridColumnSettingsClient,
            "Supplier",
            "suppliers",
            "Proveedores",
            CreateGeneralSupplierMaintenanceForm,
            geographyClient);
    }

    public CarriersForm CreateCarriersForm()
    {
        return new CarriersForm(new CarriersViewModel(carrierClient), session, gridColumnSettingsClient);
    }

    private Form? CreateGeneralSupplierMaintenanceForm(string formKey)
    {
        return formKey switch
        {
            "supplier-groups" => CreateSupplierGroupsForm(),
            "supplier-classes" => CreateSupplierClassesForm(),
            "economic-activities" => CreateEconomicActivitiesForm(),
            "supplier-zones" => CreateZonesForm(),
            "supply-methods" => CreateSupplyMethodsForm(),
            "supplier-contact-types" => CreateContactTypesForm(),
            "supplier-contact-channels" => CreateContactChannelsForm(),
            "banks" => CreateBanksForm(),
            "bank-account-types" => CreateBankAccountTypesForm(),
            "currencies" => CreateCurrenciesForm(),
            "price-lists" => CreatePriceListsForm(),
            "purchasing-agents" => CreatePurchasingAgentsForm(),
            "accounting-payment-methods" => CreateAccountingPaymentMethodsForm(),
            "payment-priorities" => CreatePaymentPrioritiesForm(),
            "approval-flows" => CreateApprovalFlowsForm(),
            "payment-document-types" => CreatePaymentDocumentTypesForm(),
            "branches" => CreateBranchesForm(),
            "departments" => CreateDepartmentsForm(),
            "business-lines" => CreateBusinessLinesForm(),
            "cost-centers" => CreateCostCentersForm(),
            "projects" => CreateProjectsForm(),
            "tax-regimes" => CreateTaxRegimesForm(),
            "taxpayer-types" => CreateTaxpayerTypesForm(),
            "retention-types" => CreateRetentionTypesForm(),
            "retention-concepts" => CreateRetentionConceptsForm(),
            "tax-supports" => CreateTaxSupportsForm(),
            "countries" => CreateCountriesForm(),
            "provinces" => CreateProvincesForm(),
            "cities" => CreateCitiesForm(),
            _ => null
        };
    }

    public ChartOfAccountsForm CreateChartOfAccountsForm()
    {
        return new ChartOfAccountsForm(new ChartOfAccountsViewModel(chartOfAccountClient), session, gridColumnSettingsClient);
    }

    public SupplierGroupsForm CreateSupplierGroupsForm()
    {
        return new SupplierGroupsForm(
            new GeneralSupplierCatalogsViewModel(generalSupplierCatalogClient, GeneralSupplierCatalogDescriptors.SupplierGroups.Route),
            session,
            gridColumnSettingsClient);
    }

    public SupplierClassesForm CreateSupplierClassesForm()
    {
        return new SupplierClassesForm(
            new GeneralSupplierCatalogsViewModel(generalSupplierCatalogClient, GeneralSupplierCatalogDescriptors.SupplierClasses.Route),
            session,
            gridColumnSettingsClient);
    }

    public EconomicActivitiesForm CreateEconomicActivitiesForm()
    {
        return new EconomicActivitiesForm(
            new GeneralSupplierCatalogsViewModel(generalSupplierCatalogClient, GeneralSupplierCatalogDescriptors.EconomicActivities.Route),
            session,
            gridColumnSettingsClient);
    }

    public ZonesForm CreateZonesForm()
    {
        return new ZonesForm(
            new GeneralSupplierCatalogsViewModel(generalSupplierCatalogClient, GeneralSupplierCatalogDescriptors.Zones.Route),
            session,
            gridColumnSettingsClient);
    }

    public SupplyMethodsForm CreateSupplyMethodsForm()
    {
        return new SupplyMethodsForm(
            new GeneralSupplierCatalogsViewModel(generalSupplierCatalogClient, GeneralSupplierCatalogDescriptors.SupplyMethods.Route),
            session,
            gridColumnSettingsClient);
    }

    public ContactTypesForm CreateContactTypesForm()
    {
        return new ContactTypesForm(
            new GeneralSupplierCatalogsViewModel(generalSupplierCatalogClient, GeneralSupplierCatalogDescriptors.ContactTypes.Route),
            session,
            gridColumnSettingsClient);
    }

    public ContactChannelsForm CreateContactChannelsForm()
    {
        return new ContactChannelsForm(
            new GeneralSupplierCatalogsViewModel(generalSupplierCatalogClient, GeneralSupplierCatalogDescriptors.ContactChannels.Route),
            session,
            gridColumnSettingsClient);
    }

    public BanksForm CreateBanksForm()
    {
        return new BanksForm(
            new FinancialCatalogsViewModel(financialCatalogClient, FinancialCatalogDescriptors.Banks.Route),
            session,
            gridColumnSettingsClient);
    }

    public BankAccountTypesForm CreateBankAccountTypesForm()
    {
        return new BankAccountTypesForm(
            new FinancialCatalogsViewModel(financialCatalogClient, FinancialCatalogDescriptors.BankAccountTypes.Route),
            session,
            gridColumnSettingsClient);
    }

    public CurrenciesForm CreateCurrenciesForm()
    {
        return new CurrenciesForm(
            new FinancialCatalogsViewModel(financialCatalogClient, FinancialCatalogDescriptors.Currencies.Route),
            session,
            gridColumnSettingsClient);
    }

    public PriceListsForm CreatePriceListsForm()
    {
        return new PriceListsForm(
            new FinancialCatalogsViewModel(financialCatalogClient, FinancialCatalogDescriptors.PriceLists.Route),
            session,
            gridColumnSettingsClient);
    }

    public PurchasingAgentsForm CreatePurchasingAgentsForm()
    {
        return new PurchasingAgentsForm(
            new FinancialCatalogsViewModel(financialCatalogClient, FinancialCatalogDescriptors.PurchasingAgents.Route),
            session,
            gridColumnSettingsClient);
    }

    public AccountingPaymentMethodsForm CreateAccountingPaymentMethodsForm()
    {
        return new AccountingPaymentMethodsForm(
            new FinancialCatalogsViewModel(financialCatalogClient, FinancialCatalogDescriptors.AccountingPaymentMethods.Route),
            session,
            gridColumnSettingsClient);
    }

    public PaymentPrioritiesForm CreatePaymentPrioritiesForm()
    {
        return new PaymentPrioritiesForm(
            new FinancialCatalogsViewModel(financialCatalogClient, FinancialCatalogDescriptors.PaymentPriorities.Route),
            session,
            gridColumnSettingsClient);
    }

    public ApprovalFlowsForm CreateApprovalFlowsForm()
    {
        return new ApprovalFlowsForm(
            new FinancialCatalogsViewModel(financialCatalogClient, FinancialCatalogDescriptors.ApprovalFlows.Route),
            session,
            gridColumnSettingsClient);
    }

    public PaymentDocumentTypesForm CreatePaymentDocumentTypesForm()
    {
        return new PaymentDocumentTypesForm(
            new FinancialCatalogsViewModel(financialCatalogClient, FinancialCatalogDescriptors.PaymentDocumentTypes.Route),
            session,
            gridColumnSettingsClient);
    }

    public BranchesForm CreateBranchesForm()
    {
        return new BranchesForm(
            new FinancialCatalogsViewModel(financialCatalogClient, FinancialCatalogDescriptors.Branches.Route),
            session,
            gridColumnSettingsClient);
    }

    public DepartmentsForm CreateDepartmentsForm()
    {
        return new DepartmentsForm(
            new FinancialCatalogsViewModel(financialCatalogClient, FinancialCatalogDescriptors.Departments.Route),
            session,
            gridColumnSettingsClient);
    }

    public BusinessLinesForm CreateBusinessLinesForm()
    {
        return new BusinessLinesForm(
            new FinancialCatalogsViewModel(financialCatalogClient, FinancialCatalogDescriptors.BusinessLines.Route),
            session,
            gridColumnSettingsClient);
    }

    public CostCentersForm CreateCostCentersForm()
    {
        return new CostCentersForm(
            new FinancialCatalogsViewModel(financialCatalogClient, FinancialCatalogDescriptors.CostCenters.Route),
            session,
            gridColumnSettingsClient);
    }

    public ProjectsForm CreateProjectsForm()
    {
        return new ProjectsForm(
            new FinancialCatalogsViewModel(financialCatalogClient, FinancialCatalogDescriptors.Projects.Route),
            session,
            gridColumnSettingsClient);
    }

    public TaxRegimesForm CreateTaxRegimesForm()
    {
        return new TaxRegimesForm(
            new TaxCatalogsViewModel(taxCatalogClient, TaxCatalogDescriptors.TaxRegimes.Route),
            session,
            gridColumnSettingsClient);
    }

    public TaxpayerTypesForm CreateTaxpayerTypesForm()
    {
        return new TaxpayerTypesForm(
            new TaxCatalogsViewModel(taxCatalogClient, TaxCatalogDescriptors.TaxpayerTypes.Route),
            session,
            gridColumnSettingsClient);
    }

    public RetentionTypesForm CreateRetentionTypesForm()
    {
        return new RetentionTypesForm(
            new TaxCatalogsViewModel(taxCatalogClient, TaxCatalogDescriptors.RetentionTypes.Route),
            session,
            gridColumnSettingsClient);
    }

    public RetentionConceptsForm CreateRetentionConceptsForm()
    {
        return new RetentionConceptsForm(
            new RetentionConceptsViewModel(taxCatalogClient),
            session,
            gridColumnSettingsClient);
    }

    public TaxSupportsForm CreateTaxSupportsForm()
    {
        return new TaxSupportsForm(
            new TaxCatalogsViewModel(taxCatalogClient, TaxCatalogDescriptors.TaxSupports.Route),
            session,
            gridColumnSettingsClient);
    }

    public CountriesForm CreateCountriesForm()
    {
        return new CountriesForm(
            new CountriesViewModel(geographyClient),
            session,
            gridColumnSettingsClient);
    }

    public ProvincesForm CreateProvincesForm()
    {
        return new ProvincesForm(
            new ProvincesViewModel(geographyClient),
            session,
            gridColumnSettingsClient);
    }

    public CitiesForm CreateCitiesForm()
    {
        return new CitiesForm(
            new CitiesViewModel(geographyClient),
            session,
            gridColumnSettingsClient);
    }

    public Form? CreateGeneralInventoryCatalogForm(string formKey)
    {
        return formKey switch
        {
            "inventory-unit-measures" => CreateUnitMeasuresForm(),
            "inventory-warehouses" => CreateWarehousesForm(),
            "inventory-item-brands" => CreateItemBrandsForm(),
            "inventory-item-types" => CreateItemTypesForm(),
            "inventory-product-types" => CreateProductTypesForm(),
            "inventory-item-lines" => CreateItemLinesForm(),
            "inventory-item-subgroups" => CreateItemSubgroupsForm(),
            "inventory-sales-channels" => CreateSalesChannelsForm(),
            "inventory-warehouse-locations" => CreateWarehouseLocationsForm(),
            "inventory-storage-zones" => CreateStorageZonesForm(),
            "inventory-storage-conditions" => CreateStorageConditionsForm(),
            "inventory-replenishment-methods" => CreateReplenishmentMethodsForm(),
            "inventory-variant-attributes" => CreateVariantAttributesForm(),
            "inventory-attachment-document-types" => CreateAttachmentDocumentTypesForm(),
            "inventory-attachment-categories" => CreateAttachmentCategoriesForm(),
            _ => null
        };
    }

    public UnitMeasuresForm CreateUnitMeasuresForm()
    {
        return CreateCatalogForm<UnitMeasuresForm>(GeneralInventoryCatalogDescriptors.UnitMeasures);
    }

    public WarehousesForm CreateWarehousesForm()
    {
        return new WarehousesForm(new WarehousesViewModel(warehouseClient), session, gridColumnSettingsClient);
    }

    public ItemBrandsForm CreateItemBrandsForm()
    {
        return CreateCatalogForm<ItemBrandsForm>(GeneralInventoryCatalogDescriptors.ItemBrands);
    }

    public ItemTypesForm CreateItemTypesForm()
    {
        return CreateCatalogForm<ItemTypesForm>(GeneralInventoryCatalogDescriptors.ItemTypes);
    }

    public ProductTypesForm CreateProductTypesForm()
    {
        return CreateCatalogForm<ProductTypesForm>(GeneralInventoryCatalogDescriptors.ProductTypes);
    }

    public ItemLinesForm CreateItemLinesForm()
    {
        return CreateCatalogForm<ItemLinesForm>(GeneralInventoryCatalogDescriptors.ItemLines);
    }

    public ItemSubgroupsForm CreateItemSubgroupsForm()
    {
        return CreateCatalogForm<ItemSubgroupsForm>(GeneralInventoryCatalogDescriptors.ItemSubgroups);
    }

    public SalesChannelsForm CreateSalesChannelsForm()
    {
        return CreateCatalogForm<SalesChannelsForm>(GeneralInventoryCatalogDescriptors.SalesChannels);
    }

    public WarehouseLocationsForm CreateWarehouseLocationsForm()
    {
        return CreateCatalogForm<WarehouseLocationsForm>(GeneralInventoryCatalogDescriptors.WarehouseLocations);
    }

    public StorageZonesForm CreateStorageZonesForm()
    {
        return CreateCatalogForm<StorageZonesForm>(GeneralInventoryCatalogDescriptors.StorageZones);
    }

    public StorageConditionsForm CreateStorageConditionsForm()
    {
        return CreateCatalogForm<StorageConditionsForm>(GeneralInventoryCatalogDescriptors.StorageConditions);
    }

    public ReplenishmentMethodsForm CreateReplenishmentMethodsForm()
    {
        return CreateCatalogForm<ReplenishmentMethodsForm>(GeneralInventoryCatalogDescriptors.ReplenishmentMethods);
    }

    public VariantAttributesForm CreateVariantAttributesForm()
    {
        return CreateCatalogForm<VariantAttributesForm>(GeneralInventoryCatalogDescriptors.VariantAttributes);
    }

    public AttachmentDocumentTypesForm CreateAttachmentDocumentTypesForm()
    {
        return CreateCatalogForm<AttachmentDocumentTypesForm>(GeneralInventoryCatalogDescriptors.AttachmentDocumentTypes);
    }

    public AttachmentCategoriesForm CreateAttachmentCategoriesForm()
    {
        return CreateCatalogForm<AttachmentCategoriesForm>(GeneralInventoryCatalogDescriptors.AttachmentCategories);
    }

    private TForm CreateCatalogForm<TForm>(GeneralInventoryCatalogDescriptor descriptor)
        where TForm : Form
    {
        var viewModel = new GeneralInventoryCatalogsViewModel(generalInventoryCatalogClient, descriptor.Route);
        return (TForm)Activator.CreateInstance(typeof(TForm), viewModel, session, gridColumnSettingsClient)!;
    }

    public ItemGroupsForm CreateItemGroupsForm()
    {
        return new ItemGroupsForm(new ItemGroupsViewModel(itemGroupClient, chartOfAccountClient), session, auditClient, gridColumnSettingsClient);
    }

    public SecurityDocumentSeriesForm CreateSecurityDocumentSeriesForm()
    {
        return new SecurityDocumentSeriesForm(
            new SecurityDocumentSeriesViewModel(securityDocumentSeriesClient, operationalCatalogClient),
            session,
            auditClient,
            gridColumnSettingsClient);
    }

    public OperationalCatalogsForm CreateOperationalCatalogsForm()
    {
        return new OperationalCatalogsForm(
            new OperationalCatalogsViewModel(operationalCatalogClient),
            session,
            gridColumnSettingsClient);
    }

    public ItemFamiliesForm CreateItemFamiliesForm()
    {
        return new ItemFamiliesForm(new ItemFamiliesViewModel(itemFamilyClient, itemClient), session, auditClient, gridColumnSettingsClient);
    }

    public ConfigurationCompaniesForm CreateConfigurationCompaniesForm()
    {
        return new ConfigurationCompaniesForm(new ConfigurationCompaniesViewModel(configurationCompanyClient), session, auditClient, gridColumnSettingsClient);
    }

    public UsersForm CreateUsersForm()
    {
        return new UsersForm(new UsersViewModel(userClient, companyClient, roleMaintenanceClient, securityAccessClient), session, auditClient, gridColumnSettingsClient);
    }

    public RoleMaintenanceForm CreateRolesForm()
    {
        return new RoleMaintenanceForm(new RoleMaintenanceViewModel(roleMaintenanceClient), session, auditClient, gridColumnSettingsClient);
    }

    public OperationsForm CreateOperationsForm()
    {
        return new OperationsForm(new OperationsViewModel(securityOperationClient), session, auditClient, gridColumnSettingsClient);
    }

    public MenusForm CreateMenusForm()
    {
        return new MenusForm(new MenusViewModel(securityMenuClient, securityFormClient), session, auditClient, gridColumnSettingsClient);
    }

    public FormsForm CreateFormsForm()
    {
        return new FormsForm(new FormsViewModel(securityFormClient), session, auditClient, gridColumnSettingsClient);
    }

    public FieldsForm CreateFieldsForm()
    {
        return new FieldsForm(new FieldsViewModel(securityFieldClient, securityFormClient), session, auditClient, gridColumnSettingsClient);
    }

    public SecurityMaintenanceFormAccessForm CreateSecurityMaintenanceFormAccessForm()
    {
        return new SecurityMaintenanceFormAccessForm(
            new SecurityMaintenanceFormAccessViewModel(roleMaintenanceClient, securityRoleFormAccessClient));
    }

    public SecurityTransactionalFormAccessForm CreateSecurityTransactionalFormAccessForm()
    {
        return new SecurityTransactionalFormAccessForm(
            new SecurityTransactionalFormAccessViewModel(
                roleMaintenanceClient,
                securityTransactionalFormAccessClient,
                securityDocumentSeriesAccessClient));
    }

    public SecurityMaintenanceFieldAccessForm CreateSecurityMaintenanceFieldAccessForm()
    {
        return new SecurityMaintenanceFieldAccessForm(
            new SecurityFormFieldAccessViewModel(
                roleMaintenanceClient,
                securityRoleFormAccessClient,
                securityMaintenanceFieldAccessClient,
                1));
    }

    public SecurityTransactionalFieldAccessForm CreateSecurityTransactionalFieldAccessForm()
    {
        return new SecurityTransactionalFieldAccessForm(
            new SecurityTransactionalFieldAccessViewModel(
                roleMaintenanceClient,
                securityRoleFormAccessClient,
                securityDocumentSeriesAccessClient,
                securityTransactionalFieldAccessClient));
    }

    public ItemsForm CreateItemsForm()
    {
        return new ItemsForm(
            new ItemsViewModel(
                itemClient,
                itemGroupClient,
                itemFamilyClient,
                generalInventoryCatalogClient,
                chartOfAccountClient,
                securityAccessClient),
            session,
            auditClient,
            gridColumnSettingsClient,
            CreateGeneralInventoryCatalogForm);
    }

    public PurchaseOrdersForm CreatePurchaseOrdersForm()
    {
        return new PurchaseOrdersForm(
            new PurchaseOrdersViewModel(purchaseOrderClient),
            session,
            gridColumnSettingsClient);
    }

    public SapSyncLogForm CreateSapSyncLogForm()
    {
        return new SapSyncLogForm(new SapSyncLogViewModel(sapClient));
    }

    public SyncMonitorForm CreateSyncMonitorForm()
    {
        return new SyncMonitorForm(
            new SyncMonitorViewModel(syncMonitorClient),
            new SyncOutboxListViewModel(syncMonitorClient),
            new SyncOutboxDetailViewModel(syncMonitorClient, session),
            new SyncAuditViewModel(syncMonitorClient),
            session);
    }

    public SriDocumentMonitorForm CreateSriDocumentMonitorForm()
    {
        return new SriDocumentMonitorForm(
            new SriDocumentMonitorViewModel(
                sriDocumentMonitorClient,
                session.HasPermission(NuanSystem.Shared.Constants.PermissionCodes.SriDocumentsViewPayload),
                session.HasPermission(NuanSystem.Shared.Constants.PermissionCodes.SriDocumentsDownloadXml)),
            session);
    }

    public SyncProfileListForm CreateSyncProfileListForm()
    {
        return new SyncProfileListForm(
            new SyncProfilesViewModel(syncConfigurationClient),
            syncConfigurationClient,
            session,
            configurationCompanyClient,
            syncEntityDefinitionClient);
    }

    public SyncExecutionListForm CreateSyncExecutionListForm()
    {
        return new SyncExecutionListForm(
            new SyncExecutionsViewModel(syncConfigurationClient),
            new SyncProfileExecutionDetailViewModel(syncConfigurationClient),
            session);
    }

    public SyncEntityListForm CreateSyncEntityListForm()
    {
        return new SyncEntityListForm(
            new SyncEntityDefinitionsViewModel(syncEntityDefinitionClient),
            syncEntityDefinitionClient,
            session,
            gridColumnSettingsClient);
    }

    public AuditLogsForm CreateAuditLogsForm()
    {
        return new AuditLogsForm(new AuditLogsViewModel(auditClient));
    }

    public SettingsForm CreateSettingsForm()
    {
        return new SettingsForm(new ConfigurationSettingsViewModel(configurationSettingClient), session, auditClient, gridColumnSettingsClient);
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }
}

