using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Accounting.ChartOfAccounts;
using NuanSystem.WinForms.Forms.BusinessPartners;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.ConfigurationCompanies;
using NuanSystem.WinForms.Forms.Documents.SecurityDocumentSeries;
using NuanSystem.WinForms.Forms.OperationalCatalogs;
using NuanSystem.WinForms.Forms.GeneralSupplier.ContactChannels;
using NuanSystem.WinForms.Forms.GeneralSupplier.ContactTypes;
using NuanSystem.WinForms.Forms.GeneralSupplier.EconomicActivities;
using NuanSystem.WinForms.Forms.GeneralSupplier.Catalogs;
using NuanSystem.WinForms.Forms.GeneralSupplier.SupplierClasses;
using NuanSystem.WinForms.Forms.GeneralSupplier.SupplierGroups;
using NuanSystem.WinForms.Forms.GeneralSupplier.SupplyMethods;
using NuanSystem.WinForms.Forms.GeneralSupplier.Zones;
using NuanSystem.WinForms.Forms.FinancialCatalogs.Banks;
using NuanSystem.WinForms.Forms.FinancialCatalogs.BankAccountTypes;
using NuanSystem.WinForms.Forms.FinancialCatalogs.Currencies;
using NuanSystem.WinForms.Forms.FinancialCatalogs.PriceLists;
using NuanSystem.WinForms.Forms.FinancialCatalogs.PurchasingAgents;
using NuanSystem.WinForms.Forms.FinancialCatalogs.AccountingPaymentMethods;
using NuanSystem.WinForms.Forms.FinancialCatalogs.ApprovalFlows;
using NuanSystem.WinForms.Forms.FinancialCatalogs.Branches;
using NuanSystem.WinForms.Forms.FinancialCatalogs.BusinessLines;
using NuanSystem.WinForms.Forms.FinancialCatalogs.CostCenters;
using NuanSystem.WinForms.Forms.FinancialCatalogs.Departments;
using NuanSystem.WinForms.Forms.FinancialCatalogs.PaymentDocumentTypes;
using NuanSystem.WinForms.Forms.FinancialCatalogs.PaymentPriorities;
using NuanSystem.WinForms.Forms.FinancialCatalogs.Projects;
using NuanSystem.WinForms.Forms.TaxCatalogs.RetentionTypes;
using NuanSystem.WinForms.Forms.TaxCatalogs.RetentionConcepts;
using NuanSystem.WinForms.Forms.TaxCatalogs.TaxRegimes;
using NuanSystem.WinForms.Forms.TaxCatalogs.TaxSupports;
using NuanSystem.WinForms.Forms.TaxCatalogs.TaxpayerTypes;
using NuanSystem.WinForms.Forms.Geography.Cities;
using NuanSystem.WinForms.Forms.Geography.Countries;
using NuanSystem.WinForms.Forms.Geography.Provinces;
using NuanSystem.WinForms.Forms.GeneralInventory.ItemFamilies;
using NuanSystem.WinForms.Forms.GeneralInventory.ItemGroups;
using NuanSystem.WinForms.Forms.InventoryItems;
using NuanSystem.WinForms.Forms.Purchasing.PurchaseOrders;
using NuanSystem.WinForms.Forms.Roles;
using NuanSystem.WinForms.Forms.Sap;
using NuanSystem.WinForms.Forms.SecurityOperations;
using NuanSystem.WinForms.Forms.SecurityMenus;
using NuanSystem.WinForms.Forms.SecurityForms;
using NuanSystem.WinForms.Forms.SecurityFields;
using NuanSystem.WinForms.Forms.SecurityAccess;
using NuanSystem.WinForms.Forms.SecurityRoles;
using NuanSystem.WinForms.Forms.ConfigurationSettings;
using NuanSystem.WinForms.Forms.SecurityUsers;
using NuanSystem.WinForms.Services.SecurityAccess.Models;
using NuanSystem.WinForms.ViewModels.Shell;

namespace NuanSystem.WinForms.Forms.Shell;

public sealed class MainForm : RibbonForm
{
    private static readonly Color AccentColor = BrandResources.Primary;
    private static readonly Color AccentHoverColor = BrandResources.PrimaryHover;
    private static readonly Color AppBackground = Color.FromArgb(245, 247, 250);
    private static readonly Color StatusBackground = Color.FromArgb(23, 32, 51);
    private static readonly Color CardHoverBack = Color.FromArgb(244, 253, 250);

    private readonly ShellViewModel viewModel;
    private readonly Func<ConfigurationCompaniesForm> configurationCompaniesFormFactory;
    private readonly Func<UsersForm> usersFormFactory;
    private readonly Func<SecurityRolesForm> rolesFormFactory;
    private readonly Func<OperationsForm> operationsFormFactory;
    private readonly Func<MenusForm> menusFormFactory;
    private readonly Func<FormsForm> formsFormFactory;
    private readonly Func<FieldsForm> fieldsFormFactory;
    private readonly Func<SecurityMaintenanceFormAccessForm> securityMaintenanceFormAccessFormFactory;
    private readonly Func<SecurityTransactionalFormAccessForm> securityTransactionalFormAccessFormFactory;
    private readonly Func<SecurityMaintenanceFieldAccessForm> securityMaintenanceFieldAccessFormFactory;
    private readonly Func<SecurityTransactionalFieldAccessForm> securityTransactionalFieldAccessFormFactory;
    private readonly Func<BusinessPartnersForm> customersFormFactory;
    private readonly Func<BusinessPartnersForm> suppliersFormFactory;
    private readonly Func<ChartOfAccountsForm> chartOfAccountsFormFactory;
    private readonly Func<SupplierGroupsForm> supplierGroupsFormFactory;
    private readonly Func<SupplierClassesForm> supplierClassesFormFactory;
    private readonly Func<EconomicActivitiesForm> economicActivitiesFormFactory;
    private readonly Func<ZonesForm> zonesFormFactory;
    private readonly Func<SupplyMethodsForm> supplyMethodsFormFactory;
    private readonly Func<ContactTypesForm> contactTypesFormFactory;
    private readonly Func<ContactChannelsForm> contactChannelsFormFactory;
    private readonly Func<BanksForm> banksFormFactory;
    private readonly Func<BankAccountTypesForm> bankAccountTypesFormFactory;
    private readonly Func<CurrenciesForm> currenciesFormFactory;
    private readonly Func<PriceListsForm> priceListsFormFactory;
    private readonly Func<PurchasingAgentsForm> purchasingAgentsFormFactory;
    private readonly Func<AccountingPaymentMethodsForm> accountingPaymentMethodsFormFactory;
    private readonly Func<PaymentPrioritiesForm> paymentPrioritiesFormFactory;
    private readonly Func<ApprovalFlowsForm> approvalFlowsFormFactory;
    private readonly Func<PaymentDocumentTypesForm> paymentDocumentTypesFormFactory;
    private readonly Func<BranchesForm> branchesFormFactory;
    private readonly Func<DepartmentsForm> departmentsFormFactory;
    private readonly Func<BusinessLinesForm> businessLinesFormFactory;
    private readonly Func<CostCentersForm> costCentersFormFactory;
    private readonly Func<ProjectsForm> projectsFormFactory;
    private readonly Func<TaxRegimesForm> taxRegimesFormFactory;
    private readonly Func<TaxpayerTypesForm> taxpayerTypesFormFactory;
    private readonly Func<RetentionTypesForm> retentionTypesFormFactory;
    private readonly Func<RetentionConceptsForm> retentionConceptsFormFactory;
    private readonly Func<TaxSupportsForm> taxSupportsFormFactory;
    private readonly Func<CountriesForm> countriesFormFactory;
    private readonly Func<ProvincesForm> provincesFormFactory;
    private readonly Func<CitiesForm> citiesFormFactory;
    private readonly Func<string, Form?> generalInventoryCatalogFormFactory;
    private readonly Func<ItemGroupsForm> itemGroupsFormFactory;
    private readonly Func<SecurityDocumentSeriesForm> securityDocumentSeriesFormFactory;
    private readonly Func<OperationalCatalogsForm> operationalCatalogsFormFactory;
    private readonly Func<ItemFamiliesForm> itemFamiliesFormFactory;
    private readonly Func<ItemsForm> itemsFormFactory;
    private readonly Func<PurchaseOrdersForm> purchaseOrdersFormFactory;
    private readonly Func<SapSyncLogForm> sapSyncLogFormFactory;
    private readonly Func<AuditLogsForm> auditLogsFormFactory;
    private readonly Func<SettingsForm> settingsFormFactory;

    private readonly AccordionControl navigationMenu = new();
    private readonly XtraTabControl tabControl = new();
    private readonly TextEdit searchEdit = new();
    private readonly Dictionary<string, XtraTabPage> openModuleTabs = new(StringComparer.OrdinalIgnoreCase);

    private RibbonControl ribbon = null!;
    private RibbonStatusBar statusBar = null!;
    private BarStaticItem sessionInfoItem = null!;
    private BarStaticItem moduleInfoItem = null!;
    private BarButtonItem homeButton = null!;
    private BarButtonItem refreshButton = null!;
    private BarButtonItem createButton = null!;
    private BarButtonItem copyButton = null!;
    private BarButtonItem editButton = null!;
    private BarButtonItem deleteButton = null!;
    private BarButtonItem consultButton = null!;
    private BarButtonItem historyButton = null!;
    private BarButtonItem columnsButton = null!;
    private BarButtonItem excelButton = null!;
    private BarButtonItem pdfButton = null!;
    private BarButtonItem jsonButton = null!;
    private BarButtonItem xmlButton = null!;
    private BarButtonItem reloadAccessButton = null!;
    private BarButtonItem logoutButton = null!;
    private RibbonPageGroup sessionRibbonGroup = null!;
    private XtraTabPage? homePage;

    public MainForm()
    {
        viewModel = null!;
        configurationCompaniesFormFactory = null!;
        usersFormFactory = null!;
        rolesFormFactory = null!;
        operationsFormFactory = null!;
        menusFormFactory = null!;
        formsFormFactory = null!;
        fieldsFormFactory = null!;
        securityMaintenanceFormAccessFormFactory = null!;
        securityTransactionalFormAccessFormFactory = null!;
        securityMaintenanceFieldAccessFormFactory = null!;
        securityTransactionalFieldAccessFormFactory = null!;
        customersFormFactory = null!;
        suppliersFormFactory = null!;
        chartOfAccountsFormFactory = null!;
        supplierGroupsFormFactory = null!;
        supplierClassesFormFactory = null!;
        economicActivitiesFormFactory = null!;
        zonesFormFactory = null!;
        supplyMethodsFormFactory = null!;
        contactTypesFormFactory = null!;
        contactChannelsFormFactory = null!;
        banksFormFactory = null!;
        bankAccountTypesFormFactory = null!;
        currenciesFormFactory = null!;
        priceListsFormFactory = null!;
        purchasingAgentsFormFactory = null!;
        accountingPaymentMethodsFormFactory = null!;
        paymentPrioritiesFormFactory = null!;
        approvalFlowsFormFactory = null!;
        paymentDocumentTypesFormFactory = null!;
        branchesFormFactory = null!;
        departmentsFormFactory = null!;
        businessLinesFormFactory = null!;
        costCentersFormFactory = null!;
        projectsFormFactory = null!;
        taxRegimesFormFactory = null!;
        taxpayerTypesFormFactory = null!;
        retentionTypesFormFactory = null!;
        retentionConceptsFormFactory = null!;
        taxSupportsFormFactory = null!;
        countriesFormFactory = null!;
        provincesFormFactory = null!;
        citiesFormFactory = null!;
        generalInventoryCatalogFormFactory = null!;
        itemGroupsFormFactory = null!;
        securityDocumentSeriesFormFactory = null!;
        operationalCatalogsFormFactory = null!;
        itemFamiliesFormFactory = null!;
        itemsFormFactory = null!;
        purchaseOrdersFormFactory = null!;
        sapSyncLogFormFactory = null!;
        auditLogsFormFactory = null!;
        settingsFormFactory = null!;
        BuildLayout();
    }

    public MainForm(
        ShellViewModel viewModel,
        Func<ConfigurationCompaniesForm> configurationCompaniesFormFactory,
        Func<UsersForm> usersFormFactory,
        Func<SecurityRolesForm> rolesFormFactory,
        Func<OperationsForm> operationsFormFactory,
        Func<MenusForm> menusFormFactory,
        Func<FormsForm> formsFormFactory,
        Func<FieldsForm> fieldsFormFactory,
        Func<SecurityMaintenanceFormAccessForm> securityMaintenanceFormAccessFormFactory,
        Func<SecurityTransactionalFormAccessForm> securityTransactionalFormAccessFormFactory,
        Func<SecurityMaintenanceFieldAccessForm> securityMaintenanceFieldAccessFormFactory,
        Func<SecurityTransactionalFieldAccessForm> securityTransactionalFieldAccessFormFactory,
        Func<BusinessPartnersForm> customersFormFactory,
        Func<BusinessPartnersForm> suppliersFormFactory,
        Func<ChartOfAccountsForm> chartOfAccountsFormFactory,
        Func<SupplierGroupsForm> supplierGroupsFormFactory,
        Func<SupplierClassesForm> supplierClassesFormFactory,
        Func<EconomicActivitiesForm> economicActivitiesFormFactory,
        Func<ZonesForm> zonesFormFactory,
        Func<SupplyMethodsForm> supplyMethodsFormFactory,
        Func<ContactTypesForm> contactTypesFormFactory,
        Func<ContactChannelsForm> contactChannelsFormFactory,
        Func<BanksForm> banksFormFactory,
        Func<BankAccountTypesForm> bankAccountTypesFormFactory,
        Func<CurrenciesForm> currenciesFormFactory,
        Func<PriceListsForm> priceListsFormFactory,
        Func<PurchasingAgentsForm> purchasingAgentsFormFactory,
        Func<AccountingPaymentMethodsForm> accountingPaymentMethodsFormFactory,
        Func<PaymentPrioritiesForm> paymentPrioritiesFormFactory,
        Func<ApprovalFlowsForm> approvalFlowsFormFactory,
        Func<PaymentDocumentTypesForm> paymentDocumentTypesFormFactory,
        Func<BranchesForm> branchesFormFactory,
        Func<DepartmentsForm> departmentsFormFactory,
        Func<BusinessLinesForm> businessLinesFormFactory,
        Func<CostCentersForm> costCentersFormFactory,
        Func<ProjectsForm> projectsFormFactory,
        Func<TaxRegimesForm> taxRegimesFormFactory,
        Func<TaxpayerTypesForm> taxpayerTypesFormFactory,
        Func<RetentionTypesForm> retentionTypesFormFactory,
        Func<RetentionConceptsForm> retentionConceptsFormFactory,
        Func<TaxSupportsForm> taxSupportsFormFactory,
        Func<CountriesForm> countriesFormFactory,
        Func<ProvincesForm> provincesFormFactory,
        Func<CitiesForm> citiesFormFactory,
        Func<string, Form?> generalInventoryCatalogFormFactory,
        Func<ItemGroupsForm> itemGroupsFormFactory,
        Func<SecurityDocumentSeriesForm> securityDocumentSeriesFormFactory,
        Func<OperationalCatalogsForm> operationalCatalogsFormFactory,
        Func<ItemFamiliesForm> itemFamiliesFormFactory,
        Func<ItemsForm> itemsFormFactory,
        Func<PurchaseOrdersForm> purchaseOrdersFormFactory,
        Func<SapSyncLogForm> sapSyncLogFormFactory,
        Func<AuditLogsForm> auditLogsFormFactory,
        Func<SettingsForm> settingsFormFactory)
    {
        this.viewModel = viewModel;
        this.configurationCompaniesFormFactory = configurationCompaniesFormFactory;
        this.usersFormFactory = usersFormFactory;
        this.rolesFormFactory = rolesFormFactory;
        this.operationsFormFactory = operationsFormFactory;
        this.menusFormFactory = menusFormFactory;
        this.formsFormFactory = formsFormFactory;
        this.fieldsFormFactory = fieldsFormFactory;
        this.securityMaintenanceFormAccessFormFactory = securityMaintenanceFormAccessFormFactory;
        this.securityTransactionalFormAccessFormFactory = securityTransactionalFormAccessFormFactory;
        this.securityMaintenanceFieldAccessFormFactory = securityMaintenanceFieldAccessFormFactory;
        this.securityTransactionalFieldAccessFormFactory = securityTransactionalFieldAccessFormFactory;
        this.customersFormFactory = customersFormFactory;
        this.suppliersFormFactory = suppliersFormFactory;
        this.chartOfAccountsFormFactory = chartOfAccountsFormFactory;
        this.supplierGroupsFormFactory = supplierGroupsFormFactory;
        this.supplierClassesFormFactory = supplierClassesFormFactory;
        this.economicActivitiesFormFactory = economicActivitiesFormFactory;
        this.zonesFormFactory = zonesFormFactory;
        this.supplyMethodsFormFactory = supplyMethodsFormFactory;
        this.contactTypesFormFactory = contactTypesFormFactory;
        this.contactChannelsFormFactory = contactChannelsFormFactory;
        this.banksFormFactory = banksFormFactory;
        this.bankAccountTypesFormFactory = bankAccountTypesFormFactory;
        this.currenciesFormFactory = currenciesFormFactory;
        this.priceListsFormFactory = priceListsFormFactory;
        this.purchasingAgentsFormFactory = purchasingAgentsFormFactory;
        this.accountingPaymentMethodsFormFactory = accountingPaymentMethodsFormFactory;
        this.paymentPrioritiesFormFactory = paymentPrioritiesFormFactory;
        this.approvalFlowsFormFactory = approvalFlowsFormFactory;
        this.paymentDocumentTypesFormFactory = paymentDocumentTypesFormFactory;
        this.branchesFormFactory = branchesFormFactory;
        this.departmentsFormFactory = departmentsFormFactory;
        this.businessLinesFormFactory = businessLinesFormFactory;
        this.costCentersFormFactory = costCentersFormFactory;
        this.projectsFormFactory = projectsFormFactory;
        this.taxRegimesFormFactory = taxRegimesFormFactory;
        this.taxpayerTypesFormFactory = taxpayerTypesFormFactory;
        this.retentionTypesFormFactory = retentionTypesFormFactory;
        this.retentionConceptsFormFactory = retentionConceptsFormFactory;
        this.taxSupportsFormFactory = taxSupportsFormFactory;
        this.countriesFormFactory = countriesFormFactory;
        this.provincesFormFactory = provincesFormFactory;
        this.citiesFormFactory = citiesFormFactory;
        this.generalInventoryCatalogFormFactory = generalInventoryCatalogFormFactory;
        this.itemGroupsFormFactory = itemGroupsFormFactory;
        this.securityDocumentSeriesFormFactory = securityDocumentSeriesFormFactory;
        this.operationalCatalogsFormFactory = operationalCatalogsFormFactory;
        this.itemFamiliesFormFactory = itemFamiliesFormFactory;
        this.itemsFormFactory = itemsFormFactory;
        this.purchaseOrdersFormFactory = purchaseOrdersFormFactory;
        this.sapSyncLogFormFactory = sapSyncLogFormFactory;
        this.auditLogsFormFactory = auditLogsFormFactory;
        this.settingsFormFactory = settingsFormFactory;
        BuildLayout();
    }

    private void BuildLayout()
    {
        UserLookAndFeel.Default.SetSkinStyle("Office 2019 White");
        Common.FormStyler.ApplyBase(this);

        Text = viewModel is null
            ? "NuanSystem"
            : $"NuanSystem - {viewModel.CompanyName}";
        Size = new Size(1220, 760);
        MinimumSize = new Size(1120, 720);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = AppBackground;
        ActiveGlowColor = AccentColor;
        InactiveGlowColor = AccentHoverColor;

        BuildRibbon();
        BuildAccordion();
        ConfigureTabs();
        BuildStatusBar();

        Controls.Add(tabControl);

        NavigationControl = navigationMenu;
        NavigationControlLayoutMode = RibbonFormNavigationControlLayoutMode.StretchToFormTitle;

        LoadNavigation();
        OpenHomePage();
        UpdateStatusBar("Inicio");
    }

    private void BuildRibbon()
    {
        ribbon = new RibbonControl
        {
            Name = "ribbonControl",
            RibbonStyle = RibbonControlStyle.Office365,
            ColorScheme = RibbonControlColorScheme.Teal,
            BackColor = Color.White,
            ForeColor = BrandResources.Text,
            ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False
        };
        ribbon.DrawGroupCaptions = DevExpress.Utils.DefaultBoolean.True;

        Controls.Add(ribbon);
        Ribbon = ribbon;

        var pageInicio = new RibbonPage("Inicio");
        var pageHerramientas = new RibbonPage("Herramientas");
        var pageAyuda = new RibbonPage("Ayuda");
        ribbon.Pages.AddRange(new RibbonPage[] { pageInicio, pageHerramientas, pageAyuda });

        var groupNavigation = new RibbonPageGroup("Navegacion");
        var groupActions = new RibbonPageGroup("Acciones");
        var groupSession = new RibbonPageGroup("Sesion");
        sessionRibbonGroup = groupSession;
        pageInicio.Groups.AddRange(new RibbonPageGroup[] { groupNavigation, groupActions, groupSession });

        homeButton = CreateRibbonButton("Inicio", "Home_32x32.svg", RibbonItemStyles.Large);
        ConfigureRibbonButtonHelp(homeButton, "Volver a la pantalla de inicio.", new RibbonShortcut(Keys.Control | Keys.I, "Ctrl + I"));
        homeButton.ItemClick += (_, _) => OpenHomePage();
        groupNavigation.ItemLinks.Add(homeButton);

        refreshButton = CreateRibbonButton("Actualizar", null, RibbonItemStyles.Large);
        ConfigureRibbonButtonHelp(refreshButton, "Actualizar la informacion del listado activo.", new RibbonShortcut(Keys.F5, "F5"));
        refreshButton.ItemClick += async (_, _) => await ExecuteActiveCrudActionAsync(form => form.ExecuteRefreshAsync());
        groupActions.ItemLinks.Add(refreshButton);

        createButton = CreateRibbonButton("Nuevo", null, RibbonItemStyles.Large);
        ConfigureRibbonButtonHelp(createButton, "Crear un nuevo registro.", new RibbonShortcut(Keys.Control | Keys.N, "Ctrl + N"));
        createButton.ItemClick += async (_, _) => await ExecuteActiveCrudActionAsync(form => form.ExecuteCreateAsync());
        groupActions.ItemLinks.Add(createButton);

        copyButton = CreateRibbonButton("Copiar", null, RibbonItemStyles.Large);
        ConfigureRibbonButtonHelp(copyButton, "Copiar el registro seleccionado para crear uno nuevo.", new RibbonShortcut(Keys.Control | Keys.Shift | Keys.C, "Ctrl + Shift + C"));
        copyButton.ItemClick += async (_, _) => await ExecuteActiveCrudActionAsync(form => form.ExecuteCopyAsync());
        groupActions.ItemLinks.Add(copyButton);

        editButton = CreateRibbonButton("Editar", null, RibbonItemStyles.Large);
        ConfigureRibbonButtonHelp(editButton, "Modificar el registro seleccionado.", new RibbonShortcut(Keys.Control | Keys.E, "Ctrl + E"));
        editButton.ItemClick += async (_, _) => await ExecuteActiveCrudActionAsync(form => form.ExecuteEditAsync());
        groupActions.ItemLinks.Add(editButton);

        consultButton = CreateRibbonButton("Consultar", null, RibbonItemStyles.Large);
        ConfigureRibbonButtonHelp(consultButton, "Consultar el registro seleccionado sin permitir cambios.", new RibbonShortcut(Keys.Control | Keys.Q, "Ctrl + Q"));
        consultButton.ItemClick += async (_, _) => await ExecuteActiveCrudActionAsync(form => form.ExecuteConsultAsync());
        groupActions.ItemLinks.Add(consultButton);

        historyButton = CreateRibbonButton("Historial", null, RibbonItemStyles.Large);
        ConfigureRibbonButtonHelp(historyButton, "Ver las modificaciones realizadas al registro seleccionado.", new RibbonShortcut(Keys.Control | Keys.H, "Ctrl + H"));
        historyButton.ItemClick += async (_, _) => await ExecuteActiveCrudActionAsync(form => form.ExecuteHistoryAsync());
        groupActions.ItemLinks.Add(historyButton);

        columnsButton = CreateRibbonButton("Columnas", null, RibbonItemStyles.Large);
        ConfigureRibbonButtonHelp(columnsButton, "Personalizar columnas del listado activo.", new RibbonShortcut(Keys.Control | Keys.Shift | Keys.L, "Ctrl + Shift + L"));
        columnsButton.ItemClick += async (_, _) => await ExecuteActiveCrudActionAsync(form => form.ExecuteCustomizeColumnsAsync());
        groupActions.ItemLinks.Add(columnsButton);

        deleteButton = CreateRibbonButton("Eliminar", null, RibbonItemStyles.Large);
        ConfigureRibbonButtonHelp(deleteButton, "Eliminar el registro seleccionado.", new RibbonShortcut(Keys.Delete, "Del"));
        deleteButton.ItemClick += async (_, _) => await ExecuteActiveCrudActionAsync(form => form.ExecuteDeleteAsync());
        groupActions.ItemLinks.Add(deleteButton);

        reloadAccessButton = CreateRibbonButton("Cargar accesos", "Refresh_32x32.svg", RibbonItemStyles.Large);
        ConfigureRibbonButtonHelp(reloadAccessButton, "Recargar menus y operaciones sin cerrar sesion.", new RibbonShortcut(Keys.Control | Keys.F5, "Ctrl + F5"));
        reloadAccessButton.ItemClick += async (_, _) => await ReloadAccessAsync();
        groupSession.ItemLinks.Add(reloadAccessButton);

        logoutButton = CreateRibbonButton("Cerrar sesion", "Close_32x32.svg", RibbonItemStyles.Large);
        ConfigureRibbonButtonHelp(logoutButton, "Cerrar la sesion actual.", new RibbonShortcut(Keys.Control | Keys.Shift | Keys.Q, "Ctrl + Shift + Q"));
        logoutButton.ItemClick += LogoutButton_ItemClick;
        groupSession.ItemLinks.Add(logoutButton);

        var exportGroup = new RibbonPageGroup("Exportar");
        pageHerramientas.Groups.Add(exportGroup);
        excelButton = CreateRibbonButton("Excel", "ExportToXLS_32x32.svg", RibbonItemStyles.Large);
        ConfigureRibbonButtonHelp(excelButton, "Exportar el listado activo a Excel.", null);
        excelButton.ItemClick += (_, _) => ExecuteActiveGridExportToExcel();
        exportGroup.ItemLinks.Add(excelButton);
        pdfButton = CreateRibbonButton("PDF", "ExportToPDF_32x32.svg", RibbonItemStyles.Large);
        ConfigureRibbonButtonHelp(pdfButton, "Exportar el listado activo a PDF.", null);
        pdfButton.ItemClick += (_, _) => ExecuteActiveGridExportToPdf();
        exportGroup.ItemLinks.Add(pdfButton);
        jsonButton = CreateRibbonButton("JSON", "ExportToXLS_32x32.svg", RibbonItemStyles.Large);
        ConfigureRibbonButtonHelp(jsonButton, "Exportar solo los datos visibles del listado a JSON.", null);
        jsonButton.ItemClick += (_, _) => ExecuteActiveGridExportToJson();
        exportGroup.ItemLinks.Add(jsonButton);
        xmlButton = CreateRibbonButton("XML", "ExportToXLS_32x32.svg", RibbonItemStyles.Large);
        ConfigureRibbonButtonHelp(xmlButton, "Exportar solo los datos visibles del listado a XML.", null);
        xmlButton.ItemClick += (_, _) => ExecuteActiveGridExportToXml();
        exportGroup.ItemLinks.Add(xmlButton);
        exportGroup.ItemLinks.Add(CreatePlaceholderButton("Imprimir", "Print_32x32.svg"));

        var helpGroup = new RibbonPageGroup("Informacion");
        pageAyuda.Groups.Add(helpGroup);
        helpGroup.ItemLinks.Add(CreatePlaceholderButton("Acerca de", "Info_32x32.svg"));

        UpdateRibbonActionState();
    }

    private BarButtonItem CreatePlaceholderButton(string caption, string imageName)
    {
        var button = CreateRibbonButton(caption, imageName, RibbonItemStyles.Large);
        ConfigureRibbonButtonHelp(button, $"Accion '{caption}' preparada para el modulo activo.", null);
        button.ItemClick += (_, _) =>
        {
            XtraMessageBox.Show(this, $"Accion '{caption}' preparada para el modulo activo.", caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        };
        return button;
    }

    private BarButtonItem CreateRibbonButton(string caption, string? imageName, RibbonItemStyles style)
    {
        var button = new BarButtonItem(ribbon.Manager, caption)
        {
            Caption = caption,
            RibbonStyle = style
        };

        ApplyRibbonButtonImage(button, imageName);

        ribbon.Items.Add(button);
        return button;
    }

    private static void ConfigureRibbonButtonHelp(BarButtonItem button, string description, RibbonShortcut? shortcut)
    {
        button.Hint = shortcut is null
            ? description
            : $"{description} Acceso rapido: {shortcut.Text}";
        button.ItemShortcut = shortcut is null
            ? BarShortcut.Empty
            : new BarShortcut(shortcut.Keys);

        var superToolTip = new SuperToolTip();
        var titleItem = new ToolTipTitleItem
        {
            Text = button.Caption
        };
        var descriptionItem = new ToolTipItem
        {
            Text = shortcut is null
                ? description
                : $"{description}\r\nAcceso rapido: {shortcut.Text}"
        };

        superToolTip.Items.Add(titleItem);
        superToolTip.Items.Add(descriptionItem);
        button.SuperTip = superToolTip;
    }

    private static void ApplyRibbonButtonImage(BarButtonItem button, string? imageName)
    {
        button.ImageOptions.SvgImage = null;

        var image = LoadSystemSvgImage(imageName);
        if (image is null)
        {
            return;
        }

        button.ImageOptions.SvgImage = image;
        button.ImageOptions.SvgImageSize = new Size(32, 32);
    }

    private static void ApplyRibbonButtonDatabaseImage(BarButtonItem button, string? imageName)
    {
        button.ImageOptions.SvgImage = null;

        var image = LoadDatabaseSvgImage(imageName);
        if (image is null)
        {
            return;
        }

        button.ImageOptions.SvgImage = image;
        button.ImageOptions.SvgImageSize = new Size(32, 32);
    }

    private static SvgImage? LoadDatabaseSvgImage(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var normalizedName = name.Trim().Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        var candidatePaths = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", normalizedName),
            Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "Ribbon", normalizedName),
            Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "Operaciones", normalizedName),
            Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "Accordion", normalizedName),
            Path.Combine(AppContext.BaseDirectory, "Assets", "Icons", "Formularios", normalizedName)
        };

        var iconPath = candidatePaths.FirstOrDefault(File.Exists);
        if (iconPath is null)
        {
            return null;
        }

        return SvgImage.FromFile(iconPath);
    }

    private static SvgImage? LoadSystemSvgImage(string? name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? null
            : LoadDatabaseSvgImage(NormalizeIconName(name));
    }

    private static string NormalizeIconName(string name)
    {
        return name.Trim().Replace('\\', '/') switch
        {
            "Add_32x32.svg" => "nuevo_32.svg",
            "Add_16x16.svg" => "nuevo_16.svg",
            "Edit_32x32.svg" => "editar_32.svg",
            "Edit_16x16.svg" => "editar_16.svg",
            "Delete_32x32.svg" => "eliminar_32.svg",
            "Delete_16x16.svg" => "eliminar_16.svg",
            "Refresh_32x32.svg" => "actualizar_32.svg",
            "Refresh_16x16.svg" => "actualizar_16.svg",
            "Find_32x32.svg" => "buscar_32.svg",
            "Find_16x16.svg" => "buscar_16.svg",
            "History_32x32.svg" => "historia_32.svg",
            "History_16x16.svg" => "historia_16.svg",
            "ExportToXLS_32x32.svg" => "exportar_32.svg",
            "ExportToXLS_16x16.svg" => "exportar_16.svg",
            "ExportToPDF_32x32.svg" => "exportar_32.svg",
            "ExportToPDF_16x16.svg" => "exportar_16.svg",
            "Print_32x32.svg" => "imprimir_32.svg",
            "Print_16x16.svg" => "imprimir_16.svg",
            "Info_32x32.svg" => "auditoria_32.svg",
            "Info_16x16.svg" => "auditoria_16.svg",
            "Close_32x32.svg" => "cancelar_32.svg",
            "Close_16x16.svg" => "cancelar_16.svg",
            "Home_32x32.svg" => "dashboard_32.svg",
            "Home_16x16.svg" => "dashboard_16.svg",
            var value => value
        };
    }

    private void BuildAccordion()
    {
        navigationMenu.Name = "accordionControl";
        navigationMenu.Dock = DockStyle.Left;
        navigationMenu.Width = 230;
        navigationMenu.ScrollBarMode = ScrollBarMode.Touch;
        navigationMenu.ViewType = AccordionControlViewType.HamburgerMenu;
        navigationMenu.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        navigationMenu.Appearance.AccordionControl.BackColor = Color.White;
        navigationMenu.Appearance.AccordionControl.Options.UseBackColor = true;
        navigationMenu.Appearance.Group.Normal.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        navigationMenu.Appearance.Group.Normal.ForeColor = BrandResources.Text;
        navigationMenu.Appearance.Group.Normal.Options.UseFont = true;
        navigationMenu.Appearance.Group.Normal.Options.UseForeColor = true;
        navigationMenu.Appearance.Item.Normal.Font = new Font("Segoe UI", 9.5F);
        navigationMenu.Appearance.Item.Normal.ForeColor = BrandResources.MutedText;
        navigationMenu.Appearance.Item.Normal.Options.UseFont = true;
        navigationMenu.Appearance.Item.Normal.Options.UseForeColor = true;
        navigationMenu.Appearance.Item.Hovered.BackColor = BrandResources.PrimarySoft;
        navigationMenu.Appearance.Item.Hovered.ForeColor = BrandResources.Text;
        navigationMenu.Appearance.Item.Hovered.Options.UseBackColor = true;
        navigationMenu.Appearance.Item.Hovered.Options.UseForeColor = true;
        navigationMenu.Appearance.Item.Pressed.BackColor = BrandResources.PrimarySoft;
        navigationMenu.Appearance.Item.Pressed.ForeColor = BrandResources.Text;
        navigationMenu.Appearance.Item.Pressed.Options.UseBackColor = true;
        navigationMenu.Appearance.Item.Pressed.Options.UseForeColor = true;
        navigationMenu.ElementClick += NavigationMenu_ElementClick;
        Controls.Add(navigationMenu);
    }

    private void ConfigureTabs()
    {
        tabControl.Dock = DockStyle.Fill;
        tabControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        tabControl.HeaderLocation = TabHeaderLocation.Top;
        tabControl.ClosePageButtonShowMode = ClosePageButtonShowMode.InActiveTabPageHeader;
        tabControl.AppearancePage.Header.Font = new Font("Segoe UI", 9F);
        tabControl.AppearancePage.Header.Options.UseFont = true;
        tabControl.CloseButtonClick += TabControl_CloseButtonClick;
        tabControl.SelectedPageChanged += (_, e) =>
        {
            UpdateStatusBar(e.Page?.Text ?? "Inicio");
            UpdateRibbonActionState();
        };
    }

    private void BuildStatusBar()
    {
        statusBar = new RibbonStatusBar
        {
            Ribbon = ribbon,
            BackColor = StatusBackground,
            ForeColor = Color.White
        };

        Controls.Add(statusBar);
        StatusBar = statusBar;

        sessionInfoItem = new BarStaticItem
        {
            Caption = string.Empty,
            ItemAppearance = { Normal = { ForeColor = Color.White } }
        };

        moduleInfoItem = new BarStaticItem
        {
            Caption = string.Empty,
            Alignment = BarItemLinkAlignment.Right,
            ItemAppearance = { Normal = { ForeColor = Color.White } }
        };

        ribbon.StatusBar.ItemLinks.Add(sessionInfoItem);
        ribbon.StatusBar.ItemLinks.Add(moduleInfoItem);
    }

    private void LoadNavigation(string? filter = null)
    {
        navigationMenu.Elements.Clear();

        var headerItem = new AccordionControlElement(ElementStyle.Item)
        {
            Text = viewModel is null ? "NuanSystem" : viewModel.CompanyName,
            Name = "headerItem",
            Enabled = false
        };
        headerItem.Appearance.Normal.ForeColor = BrandResources.Text;
        headerItem.Appearance.Normal.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        headerItem.Appearance.Normal.Options.UseForeColor = true;
        headerItem.Appearance.Normal.Options.UseFont = true;
        navigationMenu.Elements.Add(headerItem);

        var searchElement = new AccordionContentContainer
        {
            Height = 46,
            Padding = new Padding(8, 6, 8, 6)
        };
        searchEdit.Dock = DockStyle.Fill;
        searchEdit.Properties.NullValuePrompt = "Buscar modulo...";
        searchEdit.TextChanged -= SearchEdit_TextChanged;
        searchEdit.TextChanged += SearchEdit_TextChanged;
        searchElement.Controls.Add(searchEdit);
        navigationMenu.Elements.Add(new AccordionControlElement
        {
            Style = ElementStyle.Item,
            ContentContainer = searchElement,
            Text = string.Empty
        });

        if (viewModel is null || !viewModel.HasModules)
        {
            navigationMenu.Elements.Add(new AccordionControlElement(ElementStyle.Item)
            {
                Text = "Sin modulos disponibles",
                Enabled = false
            });
            return;
        }

        var normalizedFilter = filter?.Trim();
        var modules = string.IsNullOrWhiteSpace(normalizedFilter)
            ? viewModel.Modules
            : viewModel.Modules
                .Where(module =>
                    module.Title.Contains(normalizedFilter, StringComparison.OrdinalIgnoreCase) ||
                    module.Category.Contains(normalizedFilter, StringComparison.OrdinalIgnoreCase) ||
                    module.Description.Contains(normalizedFilter, StringComparison.OrdinalIgnoreCase))
                .ToArray();

        if (string.IsNullOrWhiteSpace(normalizedFilter) && viewModel.NavigationMenus.Count > 0)
        {
            LoadNavigationTree(viewModel.NavigationMenus);
            return;
        }

        foreach (var group in modules.GroupBy(module => module.Category))
        {
            var groupElement = new AccordionControlElement(ElementStyle.Group)
            {
                Text = group.Key.ToUpperInvariant(),
                Name = $"grp{group.Key}",
                Expanded = false
            };
            ApplyNavigationGroupStyle(groupElement);

            foreach (var module in group)
            {
                var item = new AccordionControlElement(ElementStyle.Item)
                {
                    Text = module.Title,
                    Name = module.Key,
                    Tag = module
                };
                ApplyNavigationItemStyle(item);
                groupElement.Elements.Add(item);
            }

            navigationMenu.Elements.Add(groupElement);
        }
    }

    private void LoadNavigationTree(IReadOnlyCollection<NavigationMenuItem> menus)
    {
        const int rootMenuKey = 0;
        var byParent = menus
            .Where(menu => menu.IsVisible && menu.IsActive)
            .GroupBy(menu => menu.ParentId ?? rootMenuKey)
            .ToDictionary(group => group.Key, group => group.OrderBy(menu => menu.DisplayOrder).ThenBy(menu => menu.Name).ToArray());

        if (!byParent.TryGetValue(rootMenuKey, out var rootMenus))
        {
            return;
        }

        foreach (var rootMenu in rootMenus)
        {
            navigationMenu.Elements.Add(CreateNavigationElement(rootMenu, byParent, isRoot: true));
        }
    }

    private AccordionControlElement CreateNavigationElement(
        NavigationMenuItem menu,
        IReadOnlyDictionary<int, NavigationMenuItem[]> byParent,
        bool isRoot)
    {
        var hasChildren = byParent.TryGetValue(menu.Id, out var children) && children.Length > 0;
        var module = ResolveModule(menu);
        var element = new AccordionControlElement(hasChildren ? ElementStyle.Group : ElementStyle.Item)
        {
            Text = menu.Name,
            Name = $"menu{menu.Id}",
            Tag = module,
            Expanded = false
        };
        if (isRoot)
        {
            ApplyNavigationGroupStyle(element);
        }
        else
        {
            ApplyNavigationItemStyle(element);
        }
        ApplyAccordionElementImage(element, menu.MenuType == 1 ? menu.IconLarge : menu.IconSmall, menu.MenuType == 1 ? 32 : 16);

        if (!hasChildren && module is null)
        {
            element.Enabled = false;
        }

        if (hasChildren)
        {
            foreach (var child in children!)
            {
                element.Elements.Add(CreateNavigationElement(child, byParent, isRoot: false));
            }
        }

        if (isRoot)
        {
            element.Text = menu.Name;
        }

        return element;
    }

    private static void ApplyNavigationGroupStyle(AccordionControlElement element)
    {
        element.Appearance.Normal.ForeColor = BrandResources.Text;
        element.Appearance.Normal.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        element.Appearance.Normal.Options.UseForeColor = true;
        element.Appearance.Normal.Options.UseFont = true;
        element.Appearance.Hovered.BackColor = BrandResources.PrimarySoft;
        element.Appearance.Hovered.ForeColor = BrandResources.Text;
        element.Appearance.Hovered.Options.UseBackColor = true;
        element.Appearance.Hovered.Options.UseForeColor = true;
    }

    private static void ApplyNavigationItemStyle(AccordionControlElement element)
    {
        element.Appearance.Normal.ForeColor = BrandResources.MutedText;
        element.Appearance.Normal.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        element.Appearance.Normal.Options.UseForeColor = true;
        element.Appearance.Normal.Options.UseFont = true;
        element.Appearance.Hovered.BackColor = BrandResources.PrimarySoft;
        element.Appearance.Hovered.ForeColor = BrandResources.Text;
        element.Appearance.Hovered.Options.UseBackColor = true;
        element.Appearance.Hovered.Options.UseForeColor = true;
        element.Appearance.Pressed.BackColor = BrandResources.PrimarySoft;
        element.Appearance.Pressed.ForeColor = BrandResources.Text;
        element.Appearance.Pressed.Options.UseBackColor = true;
        element.Appearance.Pressed.Options.UseForeColor = true;
    }

    private static void ApplyAccordionElementImage(AccordionControlElement element, string? imageName, int imageSize)
    {
        var image = LoadDatabaseSvgImage(imageName);
        if (image is null)
        {
            return;
        }

        element.ImageOptions.SvgImage = image;
        element.ImageOptions.SvgImageSize = new Size(imageSize, imageSize);
    }

    private ShellModuleItem? ResolveModule(NavigationMenuItem menu)
    {
        if (string.IsNullOrWhiteSpace(menu.FormKey))
        {
            return null;
        }

        return viewModel.Modules.FirstOrDefault(module => string.Equals(module.Key, menu.FormKey, StringComparison.OrdinalIgnoreCase))
            ?? new ShellModuleItem(menu.Name, menu.Name, menu.Description ?? menu.Name, menu.FormKey, Array.Empty<string>());
    }

    private void SearchEdit_TextChanged(object? sender, EventArgs e)
    {
        LoadNavigation(searchEdit.Text);
    }

    private void OpenHomePage()
    {
        if (homePage is not null)
        {
            tabControl.SelectedTabPage = homePage;
            UpdateStatusBar("Inicio");
            UpdateRibbonActionState();
            return;
        }

        homePage = new XtraTabPage
        {
            Text = "Inicio"
        };

        var homePanel = new PanelControl
        {
            Dock = DockStyle.Fill,
            BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder,
            Padding = new Padding(28)
        };
        homePanel.Appearance.BackColor = AppBackground;
        homePanel.Appearance.Options.UseBackColor = true;

        var title = new LabelControl
        {
            Text = "Panel de trabajo",
            Dock = DockStyle.Top,
            Height = 38
        };
        title.Appearance.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
        title.Appearance.ForeColor = BrandResources.Text;
        title.Appearance.Options.UseFont = true;
        title.Appearance.Options.UseForeColor = true;

        var subtitle = new LabelControl
        {
            Text = "Accesos rapidos segun tus permisos y empresa activa.",
            Dock = DockStyle.Top,
            Height = 30
        };
        subtitle.Appearance.ForeColor = BrandResources.MutedText;
        subtitle.Appearance.Options.UseForeColor = true;

        var modulesPanel = CreateHomeModulesPanel();

        homePanel.Controls.Add(modulesPanel);
        homePanel.Controls.Add(subtitle);
        homePanel.Controls.Add(title);
        homePage.Controls.Add(homePanel);
        tabControl.TabPages.Add(homePage);
        tabControl.SelectedTabPage = homePage;
        UpdateRibbonActionState();
    }

    private Control CreateHomeModulesPanel()
    {
        var scroll = new XtraScrollableControl
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 22, 0, 0)
        };
        scroll.Appearance.BackColor = AppBackground;
        scroll.Appearance.Options.UseBackColor = true;

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = AppBackground,
            Padding = new Padding(0),
            Margin = new Padding(0),
            WrapContents = true
        };

        if (viewModel is null || !viewModel.HasModules)
        {
            flow.Controls.Add(CreateInfoCard("Sin modulos", "El usuario actual no tiene permisos disponibles."));
        }
        else
        {
            foreach (var module in viewModel.Modules.OrderBy(module => ModuleSortOrder(module.Category)).ThenBy(module => module.Title))
            {
                flow.Controls.Add(CreateModuleCard(module));
            }
        }

        scroll.Controls.Add(flow);
        return scroll;
    }

    private static int ModuleSortOrder(string category)
    {
        return category switch
        {
            "Administracion" => 10,
            "Seguridad" => 20,
            "Catalogos" => 30,
            "Ventas" => 40,
            "SAP" => 50,
            "Sistema" => 60,
            _ => 100
        };
    }

    private Control CreateInfoCard(string title, string description)
    {
        return CreateDashboardCard(title, description, "-", null);
    }

    private Control CreateModuleCard(ShellModuleItem module)
    {
        return CreateDashboardCard(module.Title, module.Description, ResolveModuleInitials(module.Title), () => OpenModule(module));
    }

    private static string ResolveModuleInitials(string title)
    {
        var parts = title
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Take(2)
            .Select(part => char.ToUpperInvariant(part[0]).ToString())
            .ToArray();

        return parts.Length == 0 ? "M" : string.Concat(parts);
    }

    private static Control CreateDashboardCard(string title, string description, string initials, Action? onClick)
    {
        var card = new PanelControl
        {
            Size = new Size(238, 112),
            Margin = new Padding(0, 0, 14, 14),
            BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple
        };
        card.Appearance.BackColor = Color.White;
        card.Appearance.BorderColor = BrandResources.Border;
        card.Appearance.Options.UseBackColor = true;
        card.Appearance.Options.UseBorderColor = true;
        card.Cursor = onClick is null ? Cursors.Default : Cursors.Hand;

        var accent = new PanelControl
        {
            Location = new Point(14, 14),
            Size = new Size(36, 36),
            BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder
        };
        accent.Appearance.BackColor = BrandResources.PrimarySoft;
        accent.Appearance.Options.UseBackColor = true;

        var icon = new LabelControl
        {
            Text = initials,
            AutoSizeMode = LabelAutoSizeMode.None,
            Dock = DockStyle.Fill,
            Appearance =
            {
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = AccentColor,
                TextOptions = { HAlignment = HorzAlignment.Center, VAlignment = VertAlignment.Center }
            }
        };
        icon.Appearance.Options.UseFont = true;
        icon.Appearance.Options.UseForeColor = true;
        accent.Controls.Add(icon);

        var titleLabel = new LabelControl
        {
            Text = title,
            Location = new Point(62, 15),
            Size = new Size(158, 20),
            AutoSizeMode = LabelAutoSizeMode.None
        };
        titleLabel.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        titleLabel.Appearance.ForeColor = BrandResources.Text;
        titleLabel.Appearance.Options.UseFont = true;
        titleLabel.Appearance.Options.UseForeColor = true;

        var descriptionLabel = new LabelControl
        {
            Text = description,
            Location = new Point(62, 39),
            Size = new Size(158, 56),
            AutoSizeMode = LabelAutoSizeMode.Vertical
        };
        descriptionLabel.Appearance.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
        descriptionLabel.Appearance.ForeColor = BrandResources.MutedText;
        descriptionLabel.Appearance.Options.UseFont = true;
        descriptionLabel.Appearance.Options.UseForeColor = true;

        card.Controls.Add(accent);
        card.Controls.Add(titleLabel);
        card.Controls.Add(descriptionLabel);

        if (onClick is not null)
        {
            AttachCardClick(card, onClick);
        }

        return card;
    }

    private static void AttachCardClick(Control control, Action onClick)
    {
        control.Click += (_, _) => onClick();
        control.MouseEnter += (_, _) => SetCardBackColor(control, CardHoverBack);
        control.MouseLeave += (_, _) => SetCardBackColor(control, Color.White);

        foreach (Control child in control.Controls)
        {
            child.Cursor = Cursors.Hand;
            AttachCardClick(child, onClick);
        }
    }

    private static void SetCardBackColor(Control control, Color color)
    {
        var card = control is PanelControl panel
            ? panel
            : control.Parent as PanelControl;

        if (card is null)
        {
            return;
        }

        card.Appearance.BackColor = color;
        card.Appearance.Options.UseBackColor = true;
    }

    private void NavigationMenu_ElementClick(object? sender, ElementClickEventArgs e)
    {
        if (e.Element.Tag is ShellModuleItem module)
        {
            OpenModule(module);
        }
    }

    private void OpenModule(ShellModuleItem module)
    {
        if (openModuleTabs.TryGetValue(module.Key, out var existingPage))
        {
            tabControl.SelectedTabPage = existingPage;
            UpdateStatusBar(module.Title);
            return;
        }

        var form = CreateModuleForm(module);
        if (form is null)
        {
            XtraMessageBox.Show(this, $"Modulo '{module.Title}' queda preparado para la siguiente fase.", module.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var page = new XtraTabPage
        {
            Text = module.Title,
            Tag = form
        };

        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        form.Text = module.Title;
        if (form is BaseCrudListForm crudForm)
        {
            crudForm.ActionStateChanged += ActiveCrudForm_ActionStateChanged;
            _ = ApplyOperationAccessAsync(module, crudForm);
        }

        page.Controls.Add(form);
        openModuleTabs[module.Key] = page;
        tabControl.TabPages.Add(page);
        tabControl.SelectedTabPage = page;
        form.Show();
        UpdateStatusBar(module.Title);
        UpdateRibbonActionState();
    }

    private async Task ApplyOperationAccessAsync(ShellModuleItem module, BaseCrudListForm crudForm)
    {
        if (viewModel is null)
        {
            return;
        }

        var operations = await viewModel.GetFormOperationsAsync(module.Key);
        var allowedOperations = operations
            .Where(operation => operation.IsAllowed)
            .SelectMany(operation => new[] { operation.ActionKey, operation.Code, operation.Name })
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!)
            .ToArray();

        crudForm.ConfigureCrudOperationAccess(allowedOperations);
        ApplyOperationImages(operations);
        UpdateRibbonActionState();
    }

    private void ApplyOperationImages(IReadOnlyCollection<FormOperationAccessItem> operations)
    {
        var buttonOperations = new[]
        {
            ResolveOperationButton(refreshButton, operations, "refresh", "actualizar", "reload"),
            ResolveOperationButton(createButton, operations, "create", "crear", "new", "nuevo", "post"),
            ResolveOperationButton(copyButton, operations, "copy", "copiar", "duplicate", "duplicar"),
            ResolveOperationButton(editButton, operations, "update", "editar", "edit", "modificar", "put", "patch"),
            ResolveOperationButton(consultButton, operations, "consult", "consultar", "read", "buscar", "view"),
            ResolveOperationButton(historyButton, operations, "history", "historia", "historial"),
            ResolveOperationButton(columnsButton, operations, "customizecolumns", "customize-columns", "columns", "columnas", "personalizarcolumnas", "configurarcolumnas"),
            ResolveOperationButton(deleteButton, operations, "delete", "eliminar", "remove"),
            ResolveOperationButton(excelButton, operations, "exportexcel", "export-excel", "excel"),
            ResolveOperationButton(pdfButton, operations, "exportpdf", "export-pdf", "pdf"),
            ResolveOperationButton(jsonButton, operations, "exportjson", "export-json", "json"),
            ResolveOperationButton(xmlButton, operations, "exportxml", "export-xml", "xml")
        };

        foreach (var buttonOperation in buttonOperations.Where(item => item.Operation is null))
        {
            ApplyRibbonButtonDatabaseImage(buttonOperation.Button, null);
            RemoveRibbonButtonLinks(buttonOperation.Button);
        }

        foreach (var buttonOperation in buttonOperations
            .Where(item => item.Operation is not null)
            .OrderBy(item => item.Operation!.DisplayOrder)
            .ThenBy(item => item.Operation!.Name))
        {
            ApplyOperationButtonSettings(buttonOperation.Button, buttonOperation.Operation!);
        }
    }

    private static OperationButton ResolveOperationButton(BarButtonItem button, IReadOnlyCollection<FormOperationAccessItem> operations, params string[] keys)
    {
        return new OperationButton(button, ResolveOperation(operations, keys));
    }

    private void ApplyOperationButtonSettings(BarButtonItem button, FormOperationAccessItem operation)
    {
        if (!string.IsNullOrWhiteSpace(operation.Name))
        {
            button.Caption = operation.Name;
        }

        ApplyRibbonButtonDatabaseImage(button, operation.IconLarge ?? operation.IconSmall);
        ConfigureRibbonButtonHelp(button, ResolveOperationDescription(operation), ResolveOperationShortcut(operation));
        MoveRibbonButton(button, operation.RibbonPageName, operation.RibbonGroupName);
    }

    private static string ResolveOperationDescription(FormOperationAccessItem operation)
    {
        if (!string.IsNullOrWhiteSpace(operation.Description))
        {
            return operation.Description.Trim();
        }

        return !string.IsNullOrWhiteSpace(operation.Name)
            ? $"Ejecutar la operacion {operation.Name.Trim()}."
            : "Ejecutar la operacion seleccionada.";
    }

    private static RibbonShortcut? ResolveOperationShortcut(FormOperationAccessItem operation)
    {
        if (MatchesOperationKey(operation.ActionKey, "refresh") ||
            MatchesOperationKey(operation.Code, "refresh") ||
            MatchesOperationKey(operation.Name, "actualizar") ||
            MatchesOperationKey(operation.Name, "reload"))
        {
            return new RibbonShortcut(Keys.F5, "F5");
        }

        if (MatchesOperationKey(operation.ActionKey, "create") ||
            MatchesOperationKey(operation.ActionKey, "post") ||
            MatchesOperationKey(operation.Code, "new") ||
            MatchesOperationKey(operation.Name, "nuevo"))
        {
            return new RibbonShortcut(Keys.Control | Keys.N, "Ctrl + N");
        }

        if (MatchesOperationKey(operation.ActionKey, "copy") ||
            MatchesOperationKey(operation.Code, "copy") ||
            MatchesOperationKey(operation.Name, "copiar"))
        {
            return new RibbonShortcut(Keys.Control | Keys.Shift | Keys.C, "Ctrl + Shift + C");
        }

        if (MatchesOperationKey(operation.ActionKey, "update") ||
            MatchesOperationKey(operation.ActionKey, "put") ||
            MatchesOperationKey(operation.ActionKey, "patch") ||
            MatchesOperationKey(operation.Code, "edit") ||
            MatchesOperationKey(operation.Name, "editar"))
        {
            return new RibbonShortcut(Keys.Control | Keys.E, "Ctrl + E");
        }

        if (MatchesOperationKey(operation.ActionKey, "consult") ||
            MatchesOperationKey(operation.ActionKey, "read") ||
            MatchesOperationKey(operation.Code, "view") ||
            MatchesOperationKey(operation.Name, "consultar"))
        {
            return new RibbonShortcut(Keys.Control | Keys.Q, "Ctrl + Q");
        }

        if (MatchesOperationKey(operation.ActionKey, "history") ||
            MatchesOperationKey(operation.Code, "history") ||
            MatchesOperationKey(operation.Name, "historial"))
        {
            return new RibbonShortcut(Keys.Control | Keys.H, "Ctrl + H");
        }

        if (MatchesOperationKey(operation.ActionKey, "customizecolumns") ||
            MatchesOperationKey(operation.ActionKey, "customize-columns") ||
            MatchesOperationKey(operation.Code, "customizecolumns") ||
            MatchesOperationKey(operation.Name, "columnas"))
        {
            return new RibbonShortcut(Keys.Control | Keys.Shift | Keys.L, "Ctrl + Shift + L");
        }

        if (MatchesOperationKey(operation.ActionKey, "delete") ||
            MatchesOperationKey(operation.Code, "delete") ||
            MatchesOperationKey(operation.Name, "eliminar"))
        {
            return new RibbonShortcut(Keys.Delete, "Del");
        }

        if (MatchesOperationKey(operation.ActionKey, "export-excel") ||
            MatchesOperationKey(operation.Code, "export-excel") ||
            MatchesOperationKey(operation.Name, "excel"))
        {
            return new RibbonShortcut(Keys.Control | Keys.Shift | Keys.E, "Ctrl + Shift + E");
        }

        if (MatchesOperationKey(operation.ActionKey, "export-pdf") ||
            MatchesOperationKey(operation.Code, "export-pdf") ||
            MatchesOperationKey(operation.Name, "pdf"))
        {
            return new RibbonShortcut(Keys.Control | Keys.Shift | Keys.P, "Ctrl + Shift + P");
        }

        return null;
    }

    private static FormOperationAccessItem? ResolveOperation(IReadOnlyCollection<FormOperationAccessItem> operations, params string[] keys)
    {
        return operations.FirstOrDefault(operation =>
            keys.Any(key =>
                MatchesOperationKey(operation.ActionKey, key) ||
                MatchesOperationKey(operation.Code, key) ||
                MatchesOperationKey(operation.Name, key)));
    }

    private void MoveRibbonButton(BarButtonItem button, string? pageName, string? groupName)
    {
        var targetPage = GetOrCreateRibbonPage(string.IsNullOrWhiteSpace(pageName) ? "Inicio" : pageName.Trim());
        var targetGroup = GetOrCreateRibbonGroup(targetPage, string.IsNullOrWhiteSpace(groupName) ? "Acciones" : groupName.Trim());

        RemoveRibbonButtonLinks(button);
        targetGroup.ItemLinks.Add(button);
        MoveSessionGroupToEnd(targetPage);
    }

    private RibbonPage GetOrCreateRibbonPage(string pageName)
    {
        var page = ribbon.Pages.Cast<RibbonPage>().FirstOrDefault(page => string.Equals(page.Text, pageName, StringComparison.OrdinalIgnoreCase));
        if (page is not null)
        {
            return page;
        }

        page = new RibbonPage(pageName);
        ribbon.Pages.Add(page);
        return page;
    }

    private static RibbonPageGroup GetOrCreateRibbonGroup(RibbonPage page, string groupName)
    {
        var group = page.Groups.Cast<RibbonPageGroup>().FirstOrDefault(group => string.Equals(group.Text, groupName, StringComparison.OrdinalIgnoreCase));
        if (group is not null)
        {
            return group;
        }

        group = new RibbonPageGroup(groupName);
        page.Groups.Add(group);
        return group;
    }

    private void RemoveRibbonButtonLinks(BarButtonItem button)
    {
        foreach (var page in ribbon.Pages.Cast<RibbonPage>())
        {
            foreach (var group in page.Groups.Cast<RibbonPageGroup>())
            {
                var links = group.ItemLinks.Cast<BarItemLink>()
                    .Where(link => ReferenceEquals(link.Item, button))
                    .ToArray();

                foreach (var link in links)
                {
                    group.ItemLinks.Remove(link);
                }
            }
        }
    }

    private void MoveSessionGroupToEnd(RibbonPage page)
    {
        if (sessionRibbonGroup is null || !page.Groups.Contains(sessionRibbonGroup))
        {
            return;
        }

        page.Groups.Remove(sessionRibbonGroup);
        page.Groups.Add(sessionRibbonGroup);
    }

    private static bool MatchesOperationKey(string? value, string key)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalizedValue = value.Replace("ACTION.", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase);
        var normalizedKey = key.Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase);

        return normalizedValue.Equals(normalizedKey, StringComparison.OrdinalIgnoreCase);
    }

    private sealed record OperationButton(BarButtonItem Button, FormOperationAccessItem? Operation);

    private sealed record RibbonShortcut(Keys Keys, string Text);

    private async Task ReloadAccessAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        try
        {
            await viewModel.LoadNavigationAsync();
            LoadNavigation(searchEdit.Text);

            var activeModuleKey = openModuleTabs
                .FirstOrDefault(pair => pair.Value == tabControl.SelectedTabPage)
                .Key;

            if (!string.IsNullOrWhiteSpace(activeModuleKey)
                && tabControl.SelectedTabPage?.Tag is BaseCrudListForm crudForm
                && viewModel.Modules.FirstOrDefault(module => string.Equals(module.Key, activeModuleKey, StringComparison.OrdinalIgnoreCase)) is { } module)
            {
                await ApplyOperationAccessAsync(module, crudForm);
            }

            UpdateRibbonActionState();
            XtraMessageBox.Show(this, "Accesos actualizados correctamente.", "Cargar accesos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception exception)
        {
            UiExceptionHandler.ShowError(this, "Cargar accesos", exception);
        }
    }

    private Form? CreateModuleForm(ShellModuleItem module)
    {
        if (viewModel is null)
        {
            return null;
        }

        return module.Key switch
        {
            "configuration-companies" => configurationCompaniesFormFactory(),
            "users" => usersFormFactory(),
            "roles" => rolesFormFactory(),
            "security-roles" => rolesFormFactory(),
            "security-operations" => operationsFormFactory(),
            "security-menus" => menusFormFactory(),
            "security-forms" => formsFormFactory(),
            "security-fields" => fieldsFormFactory(),
            "security-access" => securityMaintenanceFormAccessFormFactory(),
            "security-form-access-transactional" => securityTransactionalFormAccessFormFactory(),
            "security-field-access-maintenance" => securityMaintenanceFieldAccessFormFactory(),
            "security-field-access-transactional" => securityTransactionalFieldAccessFormFactory(),
            "customers" => customersFormFactory(),
            "suppliers" => suppliersFormFactory(),
            "chart-of-accounts" => chartOfAccountsFormFactory(),
            "supplier-groups" => supplierGroupsFormFactory(),
            "supplier-classes" => supplierClassesFormFactory(),
            "economic-activities" => economicActivitiesFormFactory(),
            "supplier-zones" => zonesFormFactory(),
            "supply-methods" => supplyMethodsFormFactory(),
            "supplier-contact-types" => contactTypesFormFactory(),
            "supplier-contact-channels" => contactChannelsFormFactory(),
            "banks" => banksFormFactory(),
            "bank-account-types" => bankAccountTypesFormFactory(),
            "currencies" => currenciesFormFactory(),
            "price-lists" => priceListsFormFactory(),
            "purchasing-agents" => purchasingAgentsFormFactory(),
            "accounting-payment-methods" => accountingPaymentMethodsFormFactory(),
            "payment-priorities" => paymentPrioritiesFormFactory(),
            "approval-flows" => approvalFlowsFormFactory(),
            "payment-document-types" => paymentDocumentTypesFormFactory(),
            "branches" => branchesFormFactory(),
            "departments" => departmentsFormFactory(),
            "business-lines" => businessLinesFormFactory(),
            "cost-centers" => costCentersFormFactory(),
            "projects" => projectsFormFactory(),
            "tax-regimes" => taxRegimesFormFactory(),
            "taxpayer-types" => taxpayerTypesFormFactory(),
            "retention-types" => retentionTypesFormFactory(),
            "retention-concepts" => retentionConceptsFormFactory(),
            "tax-supports" => taxSupportsFormFactory(),
            "countries" => countriesFormFactory(),
            "provinces" => provincesFormFactory(),
            "cities" => citiesFormFactory(),
            "inventory-unit-measures" => generalInventoryCatalogFormFactory(module.Key),
            "inventory-warehouses" => generalInventoryCatalogFormFactory(module.Key),
            "inventory-item-brands" => generalInventoryCatalogFormFactory(module.Key),
            "inventory-item-types" => generalInventoryCatalogFormFactory(module.Key),
            "inventory-product-types" => generalInventoryCatalogFormFactory(module.Key),
            "inventory-item-lines" => generalInventoryCatalogFormFactory(module.Key),
            "inventory-item-subgroups" => generalInventoryCatalogFormFactory(module.Key),
            "inventory-sales-channels" => generalInventoryCatalogFormFactory(module.Key),
            "inventory-warehouse-locations" => generalInventoryCatalogFormFactory(module.Key),
            "inventory-storage-zones" => generalInventoryCatalogFormFactory(module.Key),
            "inventory-storage-conditions" => generalInventoryCatalogFormFactory(module.Key),
            "inventory-replenishment-methods" => generalInventoryCatalogFormFactory(module.Key),
            "inventory-variant-attributes" => generalInventoryCatalogFormFactory(module.Key),
            "inventory-attachment-document-types" => generalInventoryCatalogFormFactory(module.Key),
            "inventory-attachment-categories" => generalInventoryCatalogFormFactory(module.Key),
            "item-groups" => itemGroupsFormFactory(),
            "security-document-series" => securityDocumentSeriesFormFactory(),
            "operational-catalogs" => operationalCatalogsFormFactory(),
            "item-families" => itemFamiliesFormFactory(),
            "items" => itemsFormFactory(),
            "purchase-orders" => purchaseOrdersFormFactory(),
            "sap" => sapSyncLogFormFactory(),
            "audit" => auditLogsFormFactory(),
            "configuration-settings" => settingsFormFactory(),
            "settings" => settingsFormFactory(),
            _ => null
        };
    }

    private async Task ExecuteActiveCrudActionAsync(Func<BaseCrudListForm, Task> action)
    {
        if (tabControl.SelectedTabPage?.Tag is BaseCrudListForm form)
        {
            await action(form);
            UpdateRibbonActionState();
        }
    }

    private void ExecuteActiveGridExportToExcel()
    {
        if (tabControl.SelectedTabPage?.Tag is BaseGridCrudListForm form)
        {
            form.ExportVisibleColumnsToExcel(
                viewModel?.UserName ?? Environment.UserName,
                viewModel?.CompanyName ?? "NUAN SYSTEM",
                viewModel?.CompanyLogoImage);
            return;
        }

        XtraMessageBox.Show(this, "No hay un listado activo para exportar.", "Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ExecuteActiveGridExportToPdf()
    {
        if (tabControl.SelectedTabPage?.Tag is BaseGridCrudListForm form)
        {
            form.ExportVisibleColumnsToPdf(
                viewModel?.UserName ?? Environment.UserName,
                viewModel?.CompanyName ?? "NUAN SYSTEM",
                viewModel?.CompanyLogoImage);
            return;
        }

        XtraMessageBox.Show(this, "No hay un listado activo para exportar.", "PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ExecuteActiveGridExportToJson()
    {
        if (tabControl.SelectedTabPage?.Tag is BaseGridCrudListForm form)
        {
            form.ExportVisibleColumnsToJson();
            return;
        }

        XtraMessageBox.Show(this, "No hay un listado activo para exportar.", "JSON", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ExecuteActiveGridExportToXml()
    {
        if (tabControl.SelectedTabPage?.Tag is BaseGridCrudListForm form)
        {
            form.ExportVisibleColumnsToXml();
            return;
        }

        XtraMessageBox.Show(this, "No hay un listado activo para exportar.", "XML", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void UpdateRibbonActionState()
    {
        if (refreshButton is null || createButton is null || copyButton is null || editButton is null || deleteButton is null || consultButton is null || historyButton is null || columnsButton is null || excelButton is null || pdfButton is null || jsonButton is null || xmlButton is null)
        {
            return;
        }

        var activeCrudForm = tabControl.SelectedTabPage?.Tag as BaseCrudListForm;
        var hasActiveCrudForm = activeCrudForm is not null;

        reloadAccessButton.Visibility = BarItemVisibility.Always;
        reloadAccessButton.Enabled = true;

        refreshButton.Visibility = hasActiveCrudForm ? BarItemVisibility.Always : BarItemVisibility.Never;
        createButton.Visibility = hasActiveCrudForm ? BarItemVisibility.Always : BarItemVisibility.Never;
        copyButton.Visibility = hasActiveCrudForm ? BarItemVisibility.Always : BarItemVisibility.Never;
        editButton.Visibility = hasActiveCrudForm ? BarItemVisibility.Always : BarItemVisibility.Never;
        consultButton.Visibility = hasActiveCrudForm ? BarItemVisibility.Always : BarItemVisibility.Never;
        historyButton.Visibility = hasActiveCrudForm ? BarItemVisibility.Always : BarItemVisibility.Never;
        columnsButton.Visibility = hasActiveCrudForm ? BarItemVisibility.Always : BarItemVisibility.Never;
        deleteButton.Visibility = hasActiveCrudForm ? BarItemVisibility.Always : BarItemVisibility.Never;
        excelButton.Visibility = hasActiveCrudForm ? BarItemVisibility.Always : BarItemVisibility.Never;
        pdfButton.Visibility = hasActiveCrudForm ? BarItemVisibility.Always : BarItemVisibility.Never;
        jsonButton.Visibility = hasActiveCrudForm ? BarItemVisibility.Always : BarItemVisibility.Never;
        xmlButton.Visibility = hasActiveCrudForm ? BarItemVisibility.Always : BarItemVisibility.Never;

        refreshButton.Enabled = activeCrudForm?.CanRefresh ?? false;
        createButton.Enabled = activeCrudForm?.CanCreate ?? false;
        copyButton.Enabled = activeCrudForm?.CanCopy ?? false;
        editButton.Enabled = activeCrudForm?.CanUpdate ?? false;
        consultButton.Enabled = activeCrudForm?.CanConsult ?? false;
        historyButton.Enabled = activeCrudForm?.CanHistory ?? false;
        columnsButton.Enabled = activeCrudForm?.CanCustomizeColumns ?? false;
        deleteButton.Enabled = activeCrudForm?.CanDelete ?? false;
        excelButton.Enabled = activeCrudForm?.CanExportExcel ?? false;
        pdfButton.Enabled = activeCrudForm?.CanExportPdf ?? false;
        jsonButton.Enabled = activeCrudForm?.CanExportJson ?? false;
        xmlButton.Enabled = activeCrudForm?.CanExportXml ?? false;

        if (ribbon.Pages.Cast<RibbonPage>().FirstOrDefault(page => string.Equals(page.Text, "Inicio", StringComparison.OrdinalIgnoreCase)) is { } homeRibbonPage)
        {
            MoveSessionGroupToEnd(homeRibbonPage);
        }
    }

    private void TabControl_CloseButtonClick(object? sender, EventArgs e)
    {
        if (tabControl.SelectedTabPage is null || tabControl.SelectedTabPage == homePage)
        {
            return;
        }

        var page = tabControl.SelectedTabPage;
        var moduleKey = openModuleTabs.FirstOrDefault(pair => pair.Value == page).Key;
        if (!string.IsNullOrWhiteSpace(moduleKey))
        {
            openModuleTabs.Remove(moduleKey);
        }

        if (page.Tag is Form form)
        {
            if (form is BaseCrudListForm crudForm)
            {
                crudForm.ActionStateChanged -= ActiveCrudForm_ActionStateChanged;
            }

            form.Dispose();
        }

        tabControl.TabPages.Remove(page);
        page.Dispose();
        OpenHomePage();
    }

    private void UpdateStatusBar(string activeModule)
    {
        if (sessionInfoItem is null || moduleInfoItem is null)
        {
            return;
        }

        sessionInfoItem.Caption = viewModel is null
            ? "Usuario"
            : $"  {viewModel.UserDisplayName} ({viewModel.UserName})  |  {viewModel.CompanyName}";
        moduleInfoItem.Caption = $"Modulo activo: {activeModule}  |  Abiertos: {openModuleTabs.Count}";
    }

    private void ActiveCrudForm_ActionStateChanged(object? sender, EventArgs e)
    {
        if (ReferenceEquals(tabControl.SelectedTabPage?.Tag, sender))
        {
            UpdateRibbonActionState();
        }
    }

    private void LogoutButton_ItemClick(object? sender, ItemClickEventArgs e)
    {
        var confirmation = XtraMessageBox.Show(
            this,
            "Desea cerrar la sesion actual?",
            "Cerrar sesion",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirmation != DialogResult.Yes)
        {
            return;
        }

        viewModel?.Logout();
        DialogResult = DialogResult.Retry;
        Close();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        foreach (var form in openModuleTabs.Values.Select(page => page.Tag).OfType<Form>())
        {
            if (form is BaseCrudListForm crudForm)
            {
                crudForm.ActionStateChanged -= ActiveCrudForm_ActionStateChanged;
            }

            form.Dispose();
        }

        base.OnFormClosed(e);
    }
}

