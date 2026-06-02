using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.GeneralSupplier.Catalogs;
using NuanSystem.WinForms.Services.BusinessPartners.Models;
using NuanSystem.WinForms.Services.Geography;
using NuanSystem.WinForms.Services.Geography.Models;
using NuanSystem.WinForms.Services.Session;

namespace NuanSystem.WinForms.Forms.BusinessPartners;

public sealed partial class SupplierEditForm : BaseEditForm
{
    private static readonly RelatedMasterDescriptor CountriesMaintenance = new("countries", "Países", PermissionCodes.GeographyCountriesManage);
    private static readonly RelatedMasterDescriptor ProvincesMaintenance = new("provinces", "Provincias", PermissionCodes.GeographyProvincesManage);
    private static readonly RelatedMasterDescriptor CitiesMaintenance = new("cities", "Ciudades", PermissionCodes.GeographyCitiesManage);
    private static readonly RelatedMasterDescriptor BanksMaintenance = new("banks", "Bancos", PermissionCodes.FinancialCatalogsBanksManage);
    private static readonly RelatedMasterDescriptor BankAccountTypesMaintenance = new("bank-account-types", "Tipos de cuenta bancaria", PermissionCodes.FinancialCatalogsBankAccountTypesManage);
    private static readonly RelatedMasterDescriptor CurrenciesMaintenance = new("currencies", "Monedas", PermissionCodes.FinancialCatalogsCurrenciesManage);
    private static readonly RelatedMasterDescriptor PriceListsMaintenance = new("price-lists", "Listas de precios", PermissionCodes.FinancialCatalogsPriceListsManage);
    private static readonly RelatedMasterDescriptor PurchasingAgentsMaintenance = new("purchasing-agents", "Compradores", PermissionCodes.FinancialCatalogsPurchasingAgentsManage);
    private static readonly RelatedMasterDescriptor AccountingPaymentMethodsMaintenance = new("accounting-payment-methods", "Metodos de pago contable", PermissionCodes.FinancialCatalogsAccountingPaymentMethodsManage);
    private static readonly RelatedMasterDescriptor PaymentPrioritiesMaintenance = new("payment-priorities", "Prioridades de pago", PermissionCodes.FinancialCatalogsPaymentPrioritiesManage);
    private static readonly RelatedMasterDescriptor ApprovalFlowsMaintenance = new("approval-flows", "Flujos de aprobacion", PermissionCodes.FinancialCatalogsApprovalFlowsManage);
    private static readonly RelatedMasterDescriptor PaymentDocumentTypesMaintenance = new("payment-document-types", "Tipos de documento de pago", PermissionCodes.FinancialCatalogsPaymentDocumentTypesManage);
    private static readonly RelatedMasterDescriptor BranchesMaintenance = new("branches", "Sucursales", PermissionCodes.FinancialCatalogsBranchesManage);
    private static readonly RelatedMasterDescriptor DepartmentsMaintenance = new("departments", "Departamentos", PermissionCodes.FinancialCatalogsDepartmentsManage);
    private static readonly RelatedMasterDescriptor BusinessLinesMaintenance = new("business-lines", "Lineas de negocio", PermissionCodes.FinancialCatalogsBusinessLinesManage);
    private static readonly RelatedMasterDescriptor CostCentersMaintenance = new("cost-centers", "Centros de costo", PermissionCodes.FinancialCatalogsCostCentersManage);
    private static readonly RelatedMasterDescriptor ProjectsMaintenance = new("projects", "Proyectos", PermissionCodes.FinancialCatalogsProjectsManage);
    private static readonly RelatedMasterDescriptor TaxRegimesMaintenance = new("tax-regimes", "Regimenes tributarios", PermissionCodes.TaxRegimesManage);
    private static readonly RelatedMasterDescriptor TaxpayerTypesMaintenance = new("taxpayer-types", "Tipos de contribuyente", PermissionCodes.TaxpayerTypesManage);
    private static readonly RelatedMasterDescriptor RetentionTypesMaintenance = new("retention-types", "Tipos de retencion", PermissionCodes.RetentionTypesManage);
    private static readonly RelatedMasterDescriptor RetentionConceptsMaintenance = new("retention-concepts", "Conceptos de retencion", PermissionCodes.RetentionConceptsManage);
    private static readonly RelatedMasterDescriptor TaxSupportsMaintenance = new("tax-supports", "Sustentos tributarios", PermissionCodes.TaxSupportsManage);

    private BusinessPartnerLookups lookups;
    private readonly BusinessPartnerItem? partner;
    private readonly bool canCreateRelatedMasters;
    private readonly ApiSession? session;
    private readonly Func<string, Form?>? relatedMaintenanceFormFactory;
    private readonly Func<CancellationToken, Task<BusinessPartnerLookups>>? reloadLookupsAsync;
    private readonly IGeographyClient? geographyClient;
    private DataTable? contactTable;
    private DataTable? addressTable;
    private DataTable? bankTable;
    private DataTable? retentionTable;
    private DataTable? sapFieldMappingTable;
    private DataTable? attachmentTable;

    public SupplierEditForm()
        : this(null, CreateDesignLookups())
    {
    }

    public SupplierEditForm(
        BusinessPartnerItem? partner,
        BusinessPartnerLookups lookups,
        bool canCreateRelatedMasters = false,
        ApiSession? session = null,
        Func<string, Form?>? relatedMaintenanceFormFactory = null,
        Func<CancellationToken, Task<BusinessPartnerLookups>>? reloadLookupsAsync = null,
        IGeographyClient? geographyClient = null)
    {
        this.partner = partner;
        this.lookups = lookups;
        this.canCreateRelatedMasters = canCreateRelatedMasters;
        this.session = session;
        this.relatedMaintenanceFormFactory = relatedMaintenanceFormFactory;
        this.reloadLookupsAsync = reloadLookupsAsync;
        this.geographyClient = geographyClient;
        InitializeComponent();
        FormStyler.ApplyPanelInheritedBackColor(this);

        if (IsDesignerHosted())
        {
            return;
        }

        WireEvents();
        ConfigureRelatedMasterCreateButtons();
        BindLookups();
        BindContactLookups();
        BindAddressLookups();
        BindPurchaseLookups();
        BindBankLookups();
        BindAccountingLookups();
        BindRetentionLookups();
        BindSapLookups();
        BindNotesLookups();
        LoadPartner();
        LoadContactRows();
        LoadAddressRows();
        LoadPurchaseRows();
        LoadBankRows();
        LoadRetentionRows();
        LoadSapRows();
        LoadSapFieldMappingRows();
        LoadAttachmentRows();
        ResetAddressMapPreview();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveBusinessPartnerRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        return Validator.RequireText(txtSupplierCode, "Codigo es requerido.")
            & Validator.RequireText(txtSupplierName, "Razon social es requerida.")
            & Validator.RequireText(txtIdentificationNumber, "RUC / Cedula es requerido.")
            & RequireLookup(lueIdentificationType, "Tipo de identificacion es requerido.");
    }

    protected override void BuildRequest()
    {
        Request = new SaveBusinessPartnerRequest(
            txtSupplierCode.Text.Trim(),
            txtSupplierName.Text.Trim(),
            NullIfEmpty(txtSupplierCommercialName.Text),
            "Supplier",
            Convert.ToInt32(lueIdentificationType.EditValue),
            txtIdentificationNumber.Text.Trim(),
            ToNullableInt(lueSupplierGroup?.EditValue),
            ToNullableInt(lueSupplierClass?.EditValue),
            ToNullableInt(lueEconomicActivity?.EditValue),
            ToNullableInt(lueZone?.EditValue),
            ToNullableInt(lueSupplyMethod?.EditValue),
            null,
            null,
            null,
            NullIfEmpty(memReturnPolicy.Text),
            string.Equals(btnStatusToggle.Text, "Activo", StringComparison.OrdinalIgnoreCase),
            ToNullableInt(lueRetentionTaxpayerType?.EditValue),
            ToNullableInt(lueRetentionFiscalRegime?.EditValue),
            ToNullableInt(lueRetentionFiscalCountry?.EditValue),
            NullIfEmpty(ControlText(lueRetentionTaxpayerType)),
            IsYes(lueRetentionAccountingRequired),
            IsYes(lueRetentionAgentConfig),
            NullIfEmpty(ControlText(lueRetentionFiscalRegime)),
            NullIfEmpty(LookupCode(lueRetentionFiscalCountry) ?? ToCountryCode(ControlText(lueRetentionFiscalCountry))),
            NullIfEmpty(ControlText(lueProvince)),
            NullIfEmpty(ControlText(lueCity)),
            null,
            ToNullableInt(lueAccountingSupplierAccount?.EditValue),
            null,
            ToNullableInt(lueAccountingAdvanceAccount?.EditValue),
            ToNullableInt(lueAccountingRetentionPayableAccount?.EditValue),
            ToNullableInt(lueAccountingBranch?.EditValue),
            ToNullableInt(lueAccountingDepartment?.EditValue),
            ToNullableInt(lueAccountingBusinessLine?.EditValue),
            ToNullableInt(lueAccountingCostCenter?.EditValue),
            ToNullableInt(lueAccountingProject?.EditValue),
            null,
            ToNullableInt(lueAccountingDefaultExpenseAccount?.EditValue),
            ToNullableInt(lueAccountingDifferenceAccount?.EditValue),
            ToNullableInt(lueAccountingRoundingAccount?.EditValue),
            ToNullableInt(lueAccountingClearingAccount?.EditValue),
            ToNullableInt(lueAccountingDiscountAccount?.EditValue),
            IsToggleOn(chkAccountingBySupplier),
            IsToggleOn(chkAccountingRequiresProvision),
            IsToggleOn(chkAccountingAllowsAdvance),
            IsToggleOn(chkAccountingAllowsCompensation),
            IsToggleOn(chkAccountingAllowsPartialPayments),
            IsToggleOn(chkAccountingBlocked),
            IsToggleOn(chkAccountingUsesWithholdingBase),
            IsToggleOn(chkAccountingConciliationRequired),
            ToNullableInt(lueAccountingPaymentMethod?.EditValue),
            ToNullableInt(lueAccountingPaymentPriority?.EditValue),
            ToNullableInt(lueAccountingApprovalFlow?.EditValue),
            ToNullableInt(lueAccountingPaymentDocumentType?.EditValue),
            NullIfEmpty(ControlText(lueAccountingPaymentMethod)),
            NullIfEmpty(ControlText(lueAccountingPaymentPriority)),
            NullIfEmpty(ControlText(lueAccountingRequiredPaymentDay)),
            NullIfEmpty(ControlText(lueAccountingApprovalFlow)),
            NullIfEmpty(ControlText(lueAccountingPaymentDocumentType)),
            Convert.ToInt32(spnAccountingAveragePaymentDays?.Value ?? 0),
            spnAccountingPaymentTolerance?.Value ?? 0,
            ToNullableInt(luePurchasePaymentTerm?.EditValue),
            Convert.ToInt32(spnCreditDays.Value),
            spnCreditLimit.Value,
            Convert.ToInt32(spnDeliveryDays.Value),
            spnMinimumOrder.Value,
            IsToggleOn(tsAllowSales),
            NullIfEmpty(LookupCode(luePurchaseCurrency) ?? ToCurrencyCode(ControlText(luePurchaseCurrency))),
            NullIfEmpty(LookupCode(luePriceList) ?? ControlText(luePriceList)),
            null,
            NullIfEmpty(LookupCode(luePurchaseBuyer) ?? LookupCode(lueBuyer) ?? ControlText(luePurchaseBuyer) ?? ControlText(lueBuyer)),
            "Normal",
            NullIfEmpty(partner?.SapCardCode),
            "S",
            ToSapSyncStatus(ControlText(lueSapSyncStatus) ?? lblSapStatusValue?.Text),
            partner?.SapLastSyncAt,
            NullIfEmpty(partner?.SapLastError),
            IsYes(lueSapEnabled),
            NullIfEmpty(ControlText(lueSapMode)),
            NullIfEmpty(ControlText(lueSapCompany)),
            ToIntOrDefault(txtSapRetryCount?.Text),
            IsYes(lueSapSyncAsSupplier),
            IsYes(lueSapManualRetry),
            IsYes(lueSapRequiresApproval),
            BuildAddressRequests(),
            BuildContactRequests(),
            BuildBankAccountRequests(),
            BuildRetentionRequests(),
            BuildNotesRequest(),
            BuildSapFieldMappingRequests(),
            BuildAttachmentRequests());
    }

    private void WireEvents()
    {
        btnSave.Click += (_, _) => Save();
        btnContactAdd.Click += (_, _) => AddContactRow();
        btnContactUpdate.Click += (_, _) => UpdateContactRow();
        btnContactRemove.Click += (_, _) => RemoveFocusedRow(grvSupplierContacts);
        btnContactClear.Click += (_, _) => ClearContactInputs();
        btnAddressAdd.Click += (_, _) => AddAddressRow();
        btnAddressUpdate.Click += (_, _) => UpdateAddressRow();
        btnAddressRemove.Click += (_, _) => RemoveFocusedRow(grvSupplierAddresses);
        btnAddressClear.Click += (_, _) => ClearAddressInputs();
        btnValidateCoordinates.Click += async (_, _) => await ValidateCoordinatesAsync();
        btnClearCoordinates.Click += (_, _) => ClearCoordinates();
        lueCountry.EditValueChanged += (_, _) => RebindGeneralProvinces(clearSelection: true);
        lueProvince.EditValueChanged += (_, _) => RebindGeneralCities(clearSelection: true);
        lueSupplierAddressCountry.EditValueChanged += (_, _) => RebindAddressProvinces(clearSelection: true);
        lueSupplierAddressProvince.EditValueChanged += (_, _) => RebindAddressCities(clearSelection: true);
        lueSupplierAddressCity.EditValueChanged += (_, _) => ApplyPostalCodeFromSelectedCity();
        btnAddressClear2.Click += (_, _) => AddBankRow();
        btnAddressClear1.Click += (_, _) => UpdateBankRow();
        btnAddressClear0.Click += (_, _) => RemoveFocusedRow(grvBankAccounts);
        btnBankClear.Click += (_, _) => ClearBankInputs();
        btnAddressClear6.Click += (_, _) => AddRetentionRow();
        btnAddressClear5.Click += (_, _) => UpdateRetentionRow();
        btnAddressClear4.Click += (_, _) => RemoveFocusedRow(grvRetentionRules);
        btnAddressClear3.Click += (_, _) => ClearRetentionInputs();
        lueRetentionEntrySriCode.EditValueChanged += (_, _) => ApplyRetentionConceptMetadata();
    }

    private void ConfigureRelatedMasterCreateButtons()
    {
        ConfigureRelatedMasterCreateButton(lueSupplierGroup, "Crear grupo de proveedor", GeneralSupplierCatalogDescriptors.SupplierGroups);
        ConfigureRelatedMasterCreateButton(lueSupplierClass, "Crear clase de proveedor", GeneralSupplierCatalogDescriptors.SupplierClasses);
        ConfigureRelatedMasterCreateButton(lueEconomicActivity, "Crear actividad economica", GeneralSupplierCatalogDescriptors.EconomicActivities);
        ConfigureRelatedMasterCreateButton(lueZone, "Crear zona", GeneralSupplierCatalogDescriptors.Zones);
        ConfigureRelatedMasterCreateButton(lueCountry, "Crear pais", CountriesMaintenance);
        ConfigureRelatedMasterCreateButton(lueProvince, "Crear provincia", ProvincesMaintenance);
        ConfigureRelatedMasterCreateButton(lueCity, "Crear ciudad", CitiesMaintenance);
        ConfigureRelatedMasterCreateButton(lueSupplierAddressCountry, "Crear pais", CountriesMaintenance);
        ConfigureRelatedMasterCreateButton(lueSupplierAddressProvince, "Crear provincia", ProvincesMaintenance);
        ConfigureRelatedMasterCreateButton(lueSupplierAddressCity, "Crear ciudad", CitiesMaintenance);
        ConfigureRelatedMasterCreateButton(luePriceList, "Crear lista de precios", PriceListsMaintenance);
        ConfigureRelatedMasterCreateButton(lueBuyer, "Crear comprador", PurchasingAgentsMaintenance);
        ConfigureRelatedMasterCreateButton(luePurchasePaymentTerm, "Crear condicion de pago");
        ConfigureRelatedMasterCreateButton(luePurchaseCurrency, "Crear moneda", CurrenciesMaintenance);
        ConfigureRelatedMasterCreateButton(luePurchaseBuyer, "Crear comprador", PurchasingAgentsMaintenance);
        ConfigureRelatedMasterCreateButton(lueBankName, "Crear banco", BanksMaintenance);
        ConfigureRelatedMasterCreateButton(lueBankAccountType, "Crear tipo de cuenta bancaria", BankAccountTypesMaintenance);
        ConfigureRelatedMasterCreateButton(lueBankCountry, "Crear pais", CountriesMaintenance);
        ConfigureRelatedMasterCreateButton(lueBankCity, "Crear ciudad", CitiesMaintenance);
        ConfigureRelatedMasterCreateButton(lueBankCurrency, "Crear moneda", CurrenciesMaintenance);
        ConfigureRelatedMasterCreateButton(lueAccountingSupplierAccount, "Crear cuenta contable");
        ConfigureRelatedMasterCreateButton(lueAccountingAdvanceAccount, "Crear cuenta contable");
        ConfigureRelatedMasterCreateButton(lueAccountingDefaultExpenseAccount, "Crear cuenta contable");
        ConfigureRelatedMasterCreateButton(lueAccountingDifferenceAccount, "Crear cuenta contable");
        ConfigureRelatedMasterCreateButton(lueAccountingRoundingAccount, "Crear cuenta contable");
        ConfigureRelatedMasterCreateButton(lueAccountingClearingAccount, "Crear cuenta contable");
        ConfigureRelatedMasterCreateButton(lueAccountingDiscountAccount, "Crear cuenta contable");
        ConfigureRelatedMasterCreateButton(lueAccountingRetentionPayableAccount, "Crear cuenta contable");
        ConfigureRelatedMasterCreateButton(lueAccountingPaymentMethod, "Crear metodo de pago", AccountingPaymentMethodsMaintenance);
        ConfigureRelatedMasterCreateButton(lueAccountingPaymentPriority, "Crear prioridad de pago", PaymentPrioritiesMaintenance);
        ConfigureRelatedMasterCreateButton(lueAccountingApprovalFlow, "Crear flujo de aprobacion", ApprovalFlowsMaintenance);
        ConfigureRelatedMasterCreateButton(lueAccountingPaymentDocumentType, "Crear tipo de documento", PaymentDocumentTypesMaintenance);
        ConfigureRelatedMasterCreateButton(lueAccountingBranch, "Crear sucursal", BranchesMaintenance);
        ConfigureRelatedMasterCreateButton(lueAccountingDepartment, "Crear departamento", DepartmentsMaintenance);
        ConfigureRelatedMasterCreateButton(lueAccountingBusinessLine, "Crear linea de negocio", BusinessLinesMaintenance);
        ConfigureRelatedMasterCreateButton(lueAccountingCostCenter, "Crear centro de costo", CostCentersMaintenance);
        ConfigureRelatedMasterCreateButton(lueAccountingProject, "Crear proyecto", ProjectsMaintenance);
        ConfigureRelatedMasterCreateButton(lueRetentionFiscalCountry, "Crear pais fiscal", CountriesMaintenance);
        ConfigureRelatedMasterCreateButton(lueRetentionFiscalRegime, "Crear regimen tributario", TaxRegimesMaintenance);
        ConfigureRelatedMasterCreateButton(lueRetentionTaxpayerType, "Crear tipo de contribuyente", TaxpayerTypesMaintenance);
        ConfigureRelatedMasterCreateButton(lueRetentionEntryType, "Crear tipo de retencion", RetentionTypesMaintenance);
        ConfigureRelatedMasterCreateButton(lueRetentionEntrySriCode, "Crear concepto de retencion", RetentionConceptsMaintenance);
        ConfigureRelatedMasterCreateButton(lueRetentionEntrySupport, "Crear sustento tributario", TaxSupportsMaintenance);
        ConfigureRelatedMasterCreateButton(lueRetentionEntryAccount, "Crear cuenta contable");
        ConfigureRelatedMasterCreateButton(lueSapCompany, "Crear empresa SAP");
        ConfigureRelatedMasterCreateButton(lueSupplyMethod, "Crear forma de abastecimiento", GeneralSupplierCatalogDescriptors.SupplyMethods);
        ConfigureRelatedMasterCreateButton(lueSupplierContactType, "Crear tipo de contacto", GeneralSupplierCatalogDescriptors.ContactTypes);
        ConfigureRelatedMasterCreateButton(lueSupplierContactChannel, "Crear canal de contacto", GeneralSupplierCatalogDescriptors.ContactChannels);
    }

    private void ConfigureRelatedMasterCreateButton(SearchLookUpEdit? lookup, string tooltip, GeneralSupplierCatalogDescriptor? descriptor = null)
    {
        if (lookup is null)
        {
            return;
        }

        var button = new EditorButton(ButtonPredefines.Plus)
        {
            ToolTip = tooltip,
            Enabled = CanCreateRelatedMaster(descriptor)
        };

        lookup.Properties.Buttons.Add(button);
        lookup.Properties.ButtonClick += async (_, e) =>
        {
            if (!ReferenceEquals(e.Button, button))
            {
                return;
            }

            await OpenRelatedMaintenanceAsync(descriptor, tooltip);
        };
    }

    private void ConfigureRelatedMasterCreateButton(SearchLookUpEdit? lookup, string tooltip, RelatedMasterDescriptor descriptor)
    {
        if (lookup is null)
        {
            return;
        }

        var button = new EditorButton(ButtonPredefines.Plus)
        {
            ToolTip = tooltip,
            Enabled = CanCreateRelatedMaster(descriptor)
        };

        lookup.Properties.Buttons.Add(button);
        lookup.Properties.ButtonClick += async (_, e) =>
        {
            if (!ReferenceEquals(e.Button, button))
            {
                return;
            }

            await OpenRelatedMaintenanceAsync(descriptor, tooltip, lookup);
        };
    }

    private void ConfigureRelatedMasterCreateButton(LookUpEdit? lookup, string tooltip, GeneralSupplierCatalogDescriptor? descriptor = null)
    {
        if (lookup is null)
        {
            return;
        }

        var button = new EditorButton(ButtonPredefines.Plus)
        {
            ToolTip = tooltip,
            Enabled = CanCreateRelatedMaster(descriptor)
        };

        lookup.Properties.Buttons.Add(button);
        lookup.Properties.ButtonClick += async (_, e) =>
        {
            if (!ReferenceEquals(e.Button, button))
            {
                return;
            }

            await OpenRelatedMaintenanceAsync(descriptor, tooltip);
        };
    }

    private void ConfigureRelatedMasterCreateButton(LookUpEdit? lookup, string tooltip, RelatedMasterDescriptor descriptor)
    {
        if (lookup is null)
        {
            return;
        }

        var button = new EditorButton(ButtonPredefines.Plus)
        {
            ToolTip = tooltip,
            Enabled = CanCreateRelatedMaster(descriptor)
        };

        lookup.Properties.Buttons.Add(button);
        lookup.Properties.ButtonClick += async (_, e) =>
        {
            if (!ReferenceEquals(e.Button, button))
            {
                return;
            }

            await OpenRelatedMaintenanceAsync(descriptor, tooltip, lookup);
        };
    }

    private bool CanCreateRelatedMaster(GeneralSupplierCatalogDescriptor? descriptor)
    {
        return descriptor is null
            ? canCreateRelatedMasters
            : session?.HasPermission(descriptor.Permissions.Create) == true;
    }

    private bool CanCreateRelatedMaster(RelatedMasterDescriptor descriptor)
    {
        return session?.HasPermission(descriptor.CreatePermission) == true;
    }

    private async Task OpenRelatedMaintenanceAsync(GeneralSupplierCatalogDescriptor? descriptor, string fallbackName)
    {
        if (descriptor is not null && relatedMaintenanceFormFactory is not null)
        {
            var beforeIds = GetSupplierLookupOptions(descriptor, lookups)
                .Select(option => option.Id)
                .ToHashSet();

            try
            {
                using var form = relatedMaintenanceFormFactory(descriptor.FormKey);
                if (form is not null)
                {
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.ShowDialog(this);
                    await RefreshRelatedLookupAsync(descriptor, beforeIds);
                    return;
                }
            }
            catch (Exception exception)
            {
                UiExceptionHandler.ShowError(this, descriptor.Title, exception);
                return;
            }
        }

        ShowRelatedMasterMessage(descriptor?.Title ?? fallbackName);
    }

    private async Task OpenRelatedMaintenanceAsync(RelatedMasterDescriptor descriptor, string fallbackName, BaseEdit sourceControl)
    {
        if (relatedMaintenanceFormFactory is null)
        {
            ShowRelatedMasterMessage(descriptor.Title);
            return;
        }

        var beforeIds = GetLookupOptions(descriptor.FormKey, lookups)
            .Select(option => option.Id)
            .ToHashSet();

        try
        {
            using var form = relatedMaintenanceFormFactory(descriptor.FormKey);
            if (form is null)
            {
                ShowRelatedMasterMessage(descriptor.Title);
                return;
            }

            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);
            await RefreshRelatedLookupAsync(descriptor, beforeIds, sourceControl);
        }
        catch (Exception exception)
        {
            UiExceptionHandler.ShowError(this, fallbackName, exception);
        }
    }

    private async Task RefreshRelatedLookupAsync(GeneralSupplierCatalogDescriptor descriptor, HashSet<int> beforeIds)
    {
        if (reloadLookupsAsync is null)
        {
            return;
        }

        var currentValue = GetRelatedLookupControl(descriptor)?.EditValue;
        lookups = await reloadLookupsAsync(CancellationToken.None);
        var options = GetSupplierLookupOptions(descriptor, lookups);
        BindRelatedLookup(descriptor, options);

        var newOption = options.FirstOrDefault(option => !beforeIds.Contains(option.Id));
        SetEditValue(GetRelatedLookupControl(descriptor), newOption?.Id ?? currentValue);
    }

    private BaseEdit? GetRelatedLookupControl(GeneralSupplierCatalogDescriptor descriptor)
    {
        return descriptor.FormKey switch
        {
            "supplier-groups" => lueSupplierGroup,
            "supplier-classes" => lueSupplierClass,
            "economic-activities" => lueEconomicActivity,
            "supplier-zones" => lueZone,
            "supply-methods" => lueSupplyMethod,
            "supplier-contact-types" => lueSupplierContactType,
            "supplier-contact-channels" => lueSupplierContactChannel,
            _ => null
        };
    }

    private void BindRelatedLookup(GeneralSupplierCatalogDescriptor descriptor, IReadOnlyCollection<BusinessPartnerLookupOption> options)
    {
        switch (descriptor.FormKey)
        {
            case "supplier-groups":
                BindLookup(lueSupplierGroup, options, "Proveedores Nacionales", "Importadores", "Servicios");
                break;
            case "supplier-classes":
                BindLookup(lueSupplierClass, options, "Materiales e Insumos", "Servicios", "Transporte");
                break;
            case "economic-activities":
                BindLookup(lueEconomicActivity, options, "Comercio al por mayor", "Servicios profesionales", "Manufactura");
                break;
            case "supplier-zones":
                BindLookup(lueZone, options, "Zona 1 - Sierra", "Zona 2 - Costa", "Zona 3 - Austro");
                break;
            case "supply-methods":
                BindLookup(lueSupplyMethod, options, "Compra local", "Importacion", "Servicio recurrente");
                break;
            case "supplier-contact-types":
                BindLookup(lueSupplierContactType, options, "Comercial", "Administrativo", "Financiero", "Operativo");
                break;
            case "supplier-contact-channels":
                BindLookup(lueSupplierContactChannel, options, "Correo", "Telefono", "Movil", "WhatsApp");
                break;
        }
    }

    private async Task RefreshRelatedLookupAsync(RelatedMasterDescriptor descriptor, HashSet<int> beforeIds, BaseEdit sourceControl)
    {
        if (reloadLookupsAsync is null)
        {
            return;
        }

        var currentValues = GetRelatedLookupControls(descriptor.FormKey)
            .Where(control => control is not null)
            .ToDictionary(control => control!, control => control!.EditValue);

        lookups = await reloadLookupsAsync(CancellationToken.None);
        BindRelatedLookup(descriptor.FormKey);

        foreach (var pair in currentValues)
        {
            pair.Key.EditValue = pair.Value;
        }

        var options = GetLookupOptions(descriptor.FormKey, lookups);
        var newOption = options.FirstOrDefault(option => !beforeIds.Contains(option.Id));
        if (newOption is not null)
        {
            sourceControl.EditValue = newOption.Id;
        }
    }

    private void BindRelatedLookup(string formKey)
    {
        switch (formKey)
        {
            case "countries":
                BindLookup(lueCountry, lookups.Countries, "Ecuador", "Peru", "Colombia");
                BindLookup(lueSupplierAddressCountry, lookups.Countries, "Ecuador", "Peru", "Colombia");
                BindLookup(lueBankCountry, lookups.Countries, "Ecuador", "Peru", "Colombia");
                BindLookup(lueRetentionFiscalCountry, lookups.Countries, "Ecuador", "Peru", "Colombia");
                break;
            case "provinces":
                RebindGeneralProvinces(clearSelection: false);
                RebindAddressProvinces(clearSelection: false);
                break;
            case "cities":
                RebindGeneralCities(clearSelection: false);
                RebindAddressCities(clearSelection: false);
                BindLookup(lueBankCity, lookups.Cities, "Quito", "Guayaquil", "Cuenca");
                break;
            case "banks":
                BindLookup(lueBankName, lookups.Banks, "Banco Pichincha", "Banco de Guayaquil", "Banco del Pacifico");
                break;
            case "bank-account-types":
                BindLookup(lueBankAccountType, lookups.BankAccountTypes, "Corriente", "Ahorros");
                break;
            case "currencies":
                BindLookup(luePurchaseCurrency, lookups.Currencies, "USD - Dolar Americano", "Moneda local");
                BindLookup(lueBankCurrency, lookups.Currencies, "USD - Dolar Americano", "Moneda local");
                break;
            case "price-lists":
                BindLookup(luePriceList, lookups.PriceLists, "Lista de precios 1", "Lista de precios 2");
                break;
            case "purchasing-agents":
                BindLookup(lueBuyer, lookups.PurchasingAgents, "Ana Lucia Perez", "Maria Fernandez Ortiz", "Carlos Perez");
                BindLookup(luePurchaseBuyer, lookups.PurchasingAgents, "Maria Fernandez Ortiz", "Ana Lucia Perez", "Carlos Perez");
                break;
            case "tax-regimes":
                BindLookup(lueRetentionFiscalRegime, lookups.TaxRegimes, "Regimen general", "Regimen especial");
                break;
            case "taxpayer-types":
                BindLookup(lueRetentionTaxpayerType, lookups.TaxpayerTypes, "Sociedad", "Persona natural");
                break;
            case "retention-types":
                BindLookup(lueRetentionEntryType, lookups.RetentionTypes, "Retencion Fuente", "Retencion IVA");
                break;
            case "retention-concepts":
                BindLookup(lueRetentionEntrySriCode, lookups.RetentionConcepts, "312", "723", "724");
                break;
            case "tax-supports":
                BindLookup(lueRetentionEntrySupport, lookups.TaxSupports, "Factura", "Liquidacion de compra", "Nota de credito");
                break;
            case "branches":
                BindLookup(lueAccountingBranch, lookups.Branches, "Matriz", "Sucursal principal");
                break;
            case "departments":
                BindLookup(lueAccountingDepartment, lookups.Departments, "Administracion", "Compras");
                break;
            case "business-lines":
                BindLookup(lueAccountingBusinessLine, lookups.BusinessLines, "Comercializacion", "Servicios");
                break;
            case "cost-centers":
                BindLookup(lueAccountingCostCenter, lookups.CostCenters, "Administracion general", "Compras");
                break;
            case "projects":
                BindLookup(lueAccountingProject, lookups.Projects, "Sin Proyecto");
                break;
        }
    }

    private IEnumerable<BaseEdit?> GetRelatedLookupControls(string formKey)
    {
        return formKey switch
        {
            "countries" => new BaseEdit?[] { lueCountry, lueSupplierAddressCountry, lueBankCountry, lueRetentionFiscalCountry },
            "provinces" => new BaseEdit?[] { lueProvince, lueSupplierAddressProvince },
            "cities" => new BaseEdit?[] { lueCity, lueSupplierAddressCity, lueBankCity },
            "banks" => new BaseEdit?[] { lueBankName },
            "bank-account-types" => new BaseEdit?[] { lueBankAccountType },
            "currencies" => new BaseEdit?[] { luePurchaseCurrency, lueBankCurrency },
            "price-lists" => new BaseEdit?[] { luePriceList },
            "purchasing-agents" => new BaseEdit?[] { lueBuyer, luePurchaseBuyer },
            "accounting-payment-methods" => new BaseEdit?[] { lueAccountingPaymentMethod },
            "payment-priorities" => new BaseEdit?[] { lueAccountingPaymentPriority },
            "approval-flows" => new BaseEdit?[] { lueAccountingApprovalFlow },
            "payment-document-types" => new BaseEdit?[] { lueAccountingPaymentDocumentType },
            "branches" => new BaseEdit?[] { lueAccountingBranch },
            "departments" => new BaseEdit?[] { lueAccountingDepartment },
            "business-lines" => new BaseEdit?[] { lueAccountingBusinessLine },
            "cost-centers" => new BaseEdit?[] { lueAccountingCostCenter },
            "projects" => new BaseEdit?[] { lueAccountingProject },
            "tax-regimes" => new BaseEdit?[] { lueRetentionFiscalRegime },
            "taxpayer-types" => new BaseEdit?[] { lueRetentionTaxpayerType },
            "retention-types" => new BaseEdit?[] { lueRetentionEntryType },
            "retention-concepts" => new BaseEdit?[] { lueRetentionEntrySriCode },
            "tax-supports" => new BaseEdit?[] { lueRetentionEntrySupport },
            _ => Array.Empty<BaseEdit?>()
        };
    }

    private static IReadOnlyCollection<BusinessPartnerLookupOption> GetLookupOptions(string formKey, BusinessPartnerLookups source)
    {
        return formKey switch
        {
            "countries" => source.Countries,
            "provinces" => source.Provinces.Select(ToLookupOption).ToArray(),
            "cities" => source.Cities.Select(ToLookupOption).ToArray(),
            "banks" => source.Banks,
            "bank-account-types" => source.BankAccountTypes,
            "currencies" => source.Currencies,
            "price-lists" => source.PriceLists,
            "purchasing-agents" => source.PurchasingAgents,
            "accounting-payment-methods" => source.AccountingPaymentMethods,
            "payment-priorities" => source.PaymentPriorities,
            "approval-flows" => source.ApprovalFlows,
            "payment-document-types" => source.PaymentDocumentTypes,
            "branches" => source.Branches,
            "departments" => source.Departments,
            "business-lines" => source.BusinessLines,
            "cost-centers" => source.CostCenters,
            "projects" => source.Projects,
            "tax-regimes" => source.TaxRegimes,
            "taxpayer-types" => source.TaxpayerTypes,
            "retention-types" => source.RetentionTypes,
            "retention-concepts" => source.RetentionConcepts.Select(ToLookupOption).ToArray(),
            "tax-supports" => source.TaxSupports,
            _ => Array.Empty<BusinessPartnerLookupOption>()
        };
    }

    private static IReadOnlyCollection<BusinessPartnerLookupOption> GetSupplierLookupOptions(
        GeneralSupplierCatalogDescriptor descriptor,
        BusinessPartnerLookups source)
    {
        return descriptor.FormKey switch
        {
            "supplier-groups" => source.SupplierGroups,
            "supplier-classes" => source.SupplierClasses,
            "economic-activities" => source.EconomicActivities,
            "supplier-zones" => source.Zones,
            "supply-methods" => source.SupplyMethods,
            "supplier-contact-types" => source.ContactTypes,
            "supplier-contact-channels" => source.ContactChannels,
            _ => Array.Empty<BusinessPartnerLookupOption>()
        };
    }

    private sealed record RelatedMasterDescriptor(string FormKey, string Title, string CreatePermission);

    private void ShowRelatedMasterMessage(string maintenanceName)
    {
        XtraMessageBox.Show(
            this,
            $"Abra el mantenimiento \"{maintenanceName}\" desde el menu aprobado para crear este dato auxiliar. Luego vuelva a cargar los datos del proveedor.",
            "NuanSystem",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void BindLookups()
    {
        lueIdentificationType.Properties.DataSource = lookups.IdentificationTypes.ToList();
        lueIdentificationType.Properties.DisplayMember = nameof(BusinessPartnerIdentificationTypeLookup.Name);
        lueIdentificationType.Properties.ValueMember = nameof(BusinessPartnerIdentificationTypeLookup.Id);
        if (lueIdentificationType.Properties.PopupView is GridView identificationTypeView)
        {
            identificationTypeView.Columns.Clear();
            identificationTypeView.Columns.AddVisible(nameof(BusinessPartnerIdentificationTypeLookup.Code), "Codigo");
            identificationTypeView.Columns.AddVisible(nameof(BusinessPartnerIdentificationTypeLookup.Name), "Nombre");
        }

        BindLookup(lueSupplierGroup, lookups.SupplierGroups, "Proveedores Nacionales", "Importadores", "Servicios");
        BindLookup(lueSupplierClass, lookups.SupplierClasses, "Materiales e Insumos", "Servicios", "Transporte");
        BindLookup(lueEconomicActivity, lookups.EconomicActivities, "Comercio al por mayor", "Servicios profesionales", "Manufactura");
        BindLookup(lueZone, lookups.Zones, "Zona 1 - Sierra", "Zona 2 - Costa", "Zona 3 - Austro");
        BindLookup(lueCountry, lookups.Countries, "Ecuador", "Peru", "Colombia");
        RebindGeneralProvinces(clearSelection: false);
        RebindGeneralCities(clearSelection: false);
        BindLookup(luePriceList, lookups.PriceLists, "Lista de precios 1", "Lista de precios 2");
        BindLookup(lueBuyer, lookups.PurchasingAgents, "Ana Lucia Perez", "Maria Fernandez Ortiz", "Carlos Perez");
        AddItems(lueChannel, "Mayorista", "Retail", "Online");
        BindLookup(lueSupplyMethod, lookups.SupplyMethods, "Compra local", "Importacion", "Servicio recurrente");
    }

    private void BindContactLookups()
    {
        AddItems(lueSupplierContactPosition, "Jefe de Ventas", "Asistente Comercial", "Cobranzas");
        AddItems(lueSupplierContactPrincipal, "Si", "No");
        AddItems(lueSupplierContactStatus, "Activo", "Inactivo");
        BindLookup(lueSupplierContactType, lookups.ContactTypes, "Comercial", "Administrativo", "Financiero", "Operativo");
        AddItems(lueSupplierContactDepartment, "Ventas", "Compras", "Cobranzas", "Logistica");
        BindLookup(lueSupplierContactChannel, lookups.ContactChannels, "Correo", "Telefono", "Movil", "WhatsApp");
        AddItems(lueSupplierContactLanguage, "Español", "Ingles");
        AddItems(lueSupplierContactNotifications, "Si", "No");
    }

    private void LoadContactRows()
    {
        contactTable = CreateContactTable();
        if (partner is not null)
        {
            foreach (var contact in partner.Contacts)
            {
                contactTable.Rows.Add(
                    contact.ContactTypeId,
                    contact.ContactChannelId,
                    contact.Name,
                    contact.Position ?? string.Empty,
                    contact.Department ?? string.Empty,
                    contact.Phone ?? string.Empty,
                    contact.Extension ?? string.Empty,
                    contact.Mobile ?? string.Empty,
                    contact.Email ?? string.Empty,
                    contact.Language ?? string.Empty,
                    contact.ReceivesNotifications,
                    contact.IsPrimary,
                    contact.IsActive,
                    contact.Notes ?? string.Empty);
            }
        }

        grdSupplierContacts.DataSource = contactTable;
    }

    private void BindAddressLookups()
    {
        AddItems(lueSupplierAddressType, "Fiscal", "Entrega", "Cobranza");
        BindLookup(lueSupplierAddressCountry, lookups.Countries, "Ecuador", "Peru", "Colombia");
        RebindAddressProvinces(clearSelection: false);
        RebindAddressCities(clearSelection: false);
        AddItems(lueSupplierAddressPrimary, "Si", "No");
        AddItems(lueSupplierAddressStatus, "Activo", "Inactivo");
    }

    private void LoadAddressRows()
    {
        addressTable = CreateAddressTable();
        if (partner is not null)
        {
            foreach (var address in partner.Addresses)
            {
                addressTable.Rows.Add(
                    address.CountryId ?? (object)DBNull.Value,
                    address.ProvinceId ?? (object)DBNull.Value,
                    address.CityId ?? (object)DBNull.Value,
                    address.AddressType,
                    address.Line1,
                    address.CountryCode ?? string.Empty,
                    address.Province ?? string.Empty,
                    address.City ?? string.Empty,
                    address.PostalCode ?? string.Empty,
                    address.IsPrimary,
                    address.IsActive,
                    address.Line2 ?? string.Empty,
                    address.Latitude,
                    address.Longitude);
            }
        }

        grdSupplierAddresses.DataSource = addressTable;
    }

    private static DataTable CreateContactTable()
    {
        var table = new DataTable();
        table.Columns.Add("ContactTypeId", typeof(int));
        table.Columns.Add("ContactChannelId", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Position", typeof(string));
        table.Columns.Add("Department", typeof(string));
        table.Columns.Add("Phone", typeof(string));
        table.Columns.Add("Extension", typeof(string));
        table.Columns.Add("Mobile", typeof(string));
        table.Columns.Add("Email", typeof(string));
        table.Columns.Add("Language", typeof(string));
        table.Columns.Add("ReceivesNotifications", typeof(bool));
        table.Columns.Add("IsPrimary", typeof(bool));
        table.Columns.Add("IsActive", typeof(bool));
        table.Columns.Add("Notes", typeof(string));
        return table;
    }

    private static DataTable CreateAddressTable()
    {
        var table = new DataTable();
        table.Columns.Add("CountryId", typeof(int));
        table.Columns.Add("ProvinceId", typeof(int));
        table.Columns.Add("CityId", typeof(int));
        table.Columns.Add("AddressType", typeof(string));
        table.Columns.Add("Line1", typeof(string));
        table.Columns.Add("Country", typeof(string));
        table.Columns.Add("Province", typeof(string));
        table.Columns.Add("City", typeof(string));
        table.Columns.Add("PostalCode", typeof(string));
        table.Columns.Add("IsPrimary", typeof(bool));
        table.Columns.Add("IsActive", typeof(bool));
        table.Columns.Add("Line2", typeof(string));
        table.Columns.Add("Latitude", typeof(decimal));
        table.Columns.Add("Longitude", typeof(decimal));
        return table;
    }

    private IReadOnlyCollection<SaveBusinessPartnerContactRequest> BuildContactRequests()
    {
        EnsureCurrentContactInGrid();

        return (contactTable ?? CreateContactTable())
            .Rows
            .Cast<DataRow>()
            .Select(row => new SaveBusinessPartnerContactRequest(
                RowInt(row, "ContactTypeId"),
                RowInt(row, "ContactChannelId"),
                RowText(row, "Name") ?? string.Empty,
                RowText(row, "Position"),
                RowText(row, "Department"),
                RowText(row, "Phone"),
                RowText(row, "Extension"),
                RowText(row, "Mobile"),
                RowText(row, "Email"),
                RowText(row, "Language"),
                RowBool(row, "ReceivesNotifications"),
                RowBool(row, "IsPrimary"),
                RowBool(row, "IsActive"),
                RowText(row, "Notes")))
            .Where(contact => !string.IsNullOrWhiteSpace(contact.Name))
            .ToList();
    }

    private IReadOnlyCollection<SaveBusinessPartnerAddressRequest> BuildAddressRequests()
    {
        EnsureCurrentAddressInGrid();

        return (addressTable ?? CreateAddressTable())
            .Rows
            .Cast<DataRow>()
            .Select(row => new SaveBusinessPartnerAddressRequest(
                RowInt(row, "CountryId"),
                RowInt(row, "ProvinceId"),
                RowInt(row, "CityId"),
                NormalizeAddressType(RowText(row, "AddressType")),
                RowText(row, "Line1") ?? string.Empty,
                RowText(row, "Line2"),
                ToCountryCode(RowText(row, "Country")),
                RowText(row, "Province"),
                RowText(row, "City"),
                RowText(row, "PostalCode"),
                RowDecimal(row, "Latitude"),
                RowDecimal(row, "Longitude"),
                RowBool(row, "IsPrimary"),
                RowBool(row, "IsActive")))
            .Where(address => !string.IsNullOrWhiteSpace(address.Line1))
            .ToList();
    }

    private void EnsureCurrentContactInGrid()
    {
        if (string.IsNullOrWhiteSpace(txtSupplierContactName?.Text))
        {
            return;
        }

        if ((contactTable ?? CreateContactTable()).Rows.Cast<DataRow>().Any(row =>
            string.Equals(RowText(row, "Name"), txtSupplierContactName.Text.Trim(), StringComparison.OrdinalIgnoreCase)
            && string.Equals(RowText(row, "Email"), NullIfEmpty(txtSupplierContactEmail?.Text), StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        AddContactRow();
    }

    private void EnsureCurrentAddressInGrid()
    {
        if (string.IsNullOrWhiteSpace(txtSupplierAddressLine1?.Text))
        {
            return;
        }

        if ((addressTable ?? CreateAddressTable()).Rows.Cast<DataRow>().Any(row =>
            string.Equals(RowText(row, "Line1"), txtSupplierAddressLine1.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        AddAddressRow();
    }

    private void AddContactRow()
    {
        contactTable ??= CreateContactTable();
        if (string.IsNullOrWhiteSpace(txtSupplierContactName?.Text))
        {
            return;
        }

        contactTable.Rows.Add(
            ToNullableInt(lueSupplierContactType?.EditValue) ?? (object)DBNull.Value,
            ToNullableInt(lueSupplierContactChannel?.EditValue) ?? (object)DBNull.Value,
            txtSupplierContactName.Text.Trim(),
            NullIfEmpty(ControlText(lueSupplierContactPosition)) ?? string.Empty,
            NullIfEmpty(ControlText(lueSupplierContactDepartment)) ?? string.Empty,
            NullIfEmpty(txtSupplierContactPhone?.Text) ?? string.Empty,
            NullIfEmpty(txtSupplierContactExtension?.Text) ?? string.Empty,
            NullIfEmpty(txtSupplierContactMobile?.Text) ?? string.Empty,
            NullIfEmpty(txtSupplierContactEmail?.Text) ?? string.Empty,
            NullIfEmpty(ControlText(lueSupplierContactLanguage)) ?? string.Empty,
            IsYes(lueSupplierContactNotifications),
            IsYes(lueSupplierContactPrincipal),
            !string.Equals(ControlText(lueSupplierContactStatus), "Inactivo", StringComparison.OrdinalIgnoreCase),
            NullIfEmpty(memSupplierContactNotes?.Text) ?? string.Empty);

        grdSupplierContacts.DataSource = contactTable;
    }

    private void UpdateContactRow()
    {
        var row = grvSupplierContacts.GetFocusedDataRow();
        if (row is null || string.IsNullOrWhiteSpace(txtSupplierContactName?.Text))
        {
            return;
        }

        row["Name"] = txtSupplierContactName.Text.Trim();
        row["ContactTypeId"] = ToNullableInt(lueSupplierContactType?.EditValue) ?? (object)DBNull.Value;
        row["ContactChannelId"] = ToNullableInt(lueSupplierContactChannel?.EditValue) ?? (object)DBNull.Value;
        row["Position"] = NullIfEmpty(ControlText(lueSupplierContactPosition)) ?? string.Empty;
        row["Department"] = NullIfEmpty(ControlText(lueSupplierContactDepartment)) ?? string.Empty;
        row["Phone"] = NullIfEmpty(txtSupplierContactPhone?.Text) ?? string.Empty;
        row["Extension"] = NullIfEmpty(txtSupplierContactExtension?.Text) ?? string.Empty;
        row["Mobile"] = NullIfEmpty(txtSupplierContactMobile?.Text) ?? string.Empty;
        row["Email"] = NullIfEmpty(txtSupplierContactEmail?.Text) ?? string.Empty;
        row["Language"] = NullIfEmpty(ControlText(lueSupplierContactLanguage)) ?? string.Empty;
        row["ReceivesNotifications"] = IsYes(lueSupplierContactNotifications);
        row["IsPrimary"] = IsYes(lueSupplierContactPrincipal);
        row["IsActive"] = !string.Equals(ControlText(lueSupplierContactStatus), "Inactivo", StringComparison.OrdinalIgnoreCase);
        row["Notes"] = NullIfEmpty(memSupplierContactNotes?.Text) ?? string.Empty;
    }

    private void ClearContactInputs()
    {
        if (txtSupplierContactName is not null) txtSupplierContactName.Text = string.Empty;
        if (lueSupplierContactType is not null) lueSupplierContactType.EditValue = null;
        if (lueSupplierContactChannel is not null) lueSupplierContactChannel.EditValue = null;
        if (lueSupplierContactPosition is not null) lueSupplierContactPosition.EditValue = null;
        if (lueSupplierContactDepartment is not null) lueSupplierContactDepartment.EditValue = null;
        if (txtSupplierContactPhone is not null) txtSupplierContactPhone.Text = string.Empty;
        if (txtSupplierContactExtension is not null) txtSupplierContactExtension.Text = string.Empty;
        if (txtSupplierContactMobile is not null) txtSupplierContactMobile.Text = string.Empty;
        if (txtSupplierContactEmail is not null) txtSupplierContactEmail.Text = string.Empty;
        if (lueSupplierContactLanguage is not null) lueSupplierContactLanguage.EditValue = null;
        if (lueSupplierContactNotifications is not null) lueSupplierContactNotifications.EditValue = "No";
        if (lueSupplierContactPrincipal is not null) lueSupplierContactPrincipal.EditValue = "No";
        if (lueSupplierContactStatus is not null) lueSupplierContactStatus.EditValue = "Activo";
        if (memSupplierContactNotes is not null) memSupplierContactNotes.Text = string.Empty;
    }

    private void AddAddressRow()
    {
        addressTable ??= CreateAddressTable();
        if (string.IsNullOrWhiteSpace(txtSupplierAddressLine1?.Text))
        {
            return;
        }

        addressTable.Rows.Add(
            ToNullableInt(lueSupplierAddressCountry?.EditValue) ?? (object)DBNull.Value,
            ToNullableInt(lueSupplierAddressProvince?.EditValue) ?? (object)DBNull.Value,
            ToNullableInt(lueSupplierAddressCity?.EditValue) ?? (object)DBNull.Value,
            NullIfEmpty(ControlText(lueSupplierAddressType)) ?? "Fiscal",
            txtSupplierAddressLine1.Text.Trim(),
            NullIfEmpty(ControlText(lueSupplierAddressCountry)) ?? string.Empty,
            NullIfEmpty(ControlText(lueSupplierAddressProvince)) ?? string.Empty,
            NullIfEmpty(ControlText(lueSupplierAddressCity)) ?? string.Empty,
            NullIfEmpty(txtSupplierAddressPostal?.Text) ?? string.Empty,
            IsYes(lueSupplierAddressPrimary),
            !string.Equals(ControlText(lueSupplierAddressStatus), "Inactivo", StringComparison.OrdinalIgnoreCase),
            NullIfEmpty(txtSupplierAddressLine2?.Text) ?? string.Empty,
            CoordinateValue(spnSupplierLatitude),
            CoordinateValue(spnSupplierLongitude));

        grdSupplierAddresses.DataSource = addressTable;
    }

    private void UpdateAddressRow()
    {
        var row = grvSupplierAddresses.GetFocusedDataRow();
        if (row is null || string.IsNullOrWhiteSpace(txtSupplierAddressLine1?.Text))
        {
            return;
        }

        row["AddressType"] = NullIfEmpty(ControlText(lueSupplierAddressType)) ?? "Fiscal";
        row["Line1"] = txtSupplierAddressLine1.Text.Trim();
        row["CountryId"] = ToNullableInt(lueSupplierAddressCountry?.EditValue) ?? (object)DBNull.Value;
        row["ProvinceId"] = ToNullableInt(lueSupplierAddressProvince?.EditValue) ?? (object)DBNull.Value;
        row["CityId"] = ToNullableInt(lueSupplierAddressCity?.EditValue) ?? (object)DBNull.Value;
        row["Country"] = NullIfEmpty(ControlText(lueSupplierAddressCountry)) ?? string.Empty;
        row["Province"] = NullIfEmpty(ControlText(lueSupplierAddressProvince)) ?? string.Empty;
        row["City"] = NullIfEmpty(ControlText(lueSupplierAddressCity)) ?? string.Empty;
        row["PostalCode"] = NullIfEmpty(txtSupplierAddressPostal?.Text) ?? string.Empty;
        row["IsPrimary"] = IsYes(lueSupplierAddressPrimary);
        row["IsActive"] = !string.Equals(ControlText(lueSupplierAddressStatus), "Inactivo", StringComparison.OrdinalIgnoreCase);
        row["Line2"] = NullIfEmpty(txtSupplierAddressLine2?.Text) ?? string.Empty;
        row["Latitude"] = CoordinateValue(spnSupplierLatitude);
        row["Longitude"] = CoordinateValue(spnSupplierLongitude);
    }

    private void ClearAddressInputs()
    {
        if (lueSupplierAddressType is not null) lueSupplierAddressType.EditValue = null;
        if (txtSupplierAddressLine1 is not null) txtSupplierAddressLine1.Text = string.Empty;
        if (txtSupplierAddressLine2 is not null) txtSupplierAddressLine2.Text = string.Empty;
        if (txtSupplierAddressReference is not null) txtSupplierAddressReference.Text = string.Empty;
        if (lueSupplierAddressCountry is not null) lueSupplierAddressCountry.EditValue = null;
        if (lueSupplierAddressProvince is not null) lueSupplierAddressProvince.EditValue = null;
        if (lueSupplierAddressCity is not null) lueSupplierAddressCity.EditValue = null;
        if (txtSupplierAddressPostal is not null) txtSupplierAddressPostal.Text = string.Empty;
        if (lueSupplierAddressPrimary is not null) lueSupplierAddressPrimary.EditValue = "No";
        if (lueSupplierAddressStatus is not null) lueSupplierAddressStatus.EditValue = "Activo";
        if (spnSupplierLatitude is not null) spnSupplierLatitude.Value = 0;
        if (spnSupplierLongitude is not null) spnSupplierLongitude.Value = 0;
    }

    private async Task ValidateCoordinatesAsync()
    {
        var latitude = spnSupplierLatitude?.Value ?? 0;
        var longitude = spnSupplierLongitude?.Value ?? 0;

        if (latitude == 0 && longitude == 0)
        {
            XtraMessageBox.Show(this, "Ingrese latitud y longitud antes de validar.", "Coordenadas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (latitude < -90 || latitude > 90)
        {
            XtraMessageBox.Show(this, "La latitud debe estar entre -90 y 90.", "Coordenadas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            spnSupplierLatitude?.Focus();
            return;
        }

        if (longitude < -180 || longitude > 180)
        {
            XtraMessageBox.Show(this, "La longitud debe estar entre -180 y 180.", "Coordenadas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            spnSupplierLongitude?.Focus();
            return;
        }

        if (!await TryApplyReverseGeocodeAsync(latitude, longitude))
        {
            return;
        }

        ApplyPostalCodeFromSelectedCity();

        if (ToNullableInt(lueSupplierAddressCountry?.EditValue) is null)
        {
            XtraMessageBox.Show(this, "Seleccione el pais de la direccion para validar la geolocalizacion.", "Coordenadas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            lueSupplierAddressCountry?.Focus();
            return;
        }

        if (ToNullableInt(lueSupplierAddressProvince?.EditValue) is null)
        {
            XtraMessageBox.Show(this, "Seleccione la provincia de la direccion para validar la geolocalizacion.", "Coordenadas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            lueSupplierAddressProvince?.Focus();
            return;
        }

        if (ToNullableInt(lueSupplierAddressCity?.EditValue) is null)
        {
            XtraMessageBox.Show(this, "Seleccione la ciudad de la direccion para validar la geolocalizacion.", "Coordenadas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            lueSupplierAddressCity?.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtSupplierAddressPostal?.Text))
        {
            XtraMessageBox.Show(this, "Ingrese el codigo postal de la direccion para validar la geolocalizacion.", "Coordenadas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtSupplierAddressPostal?.Focus();
            return;
        }

        if (lblAddressMapPlaceholder is not null)
        {
            lblAddressMapPlaceholder.Text =
                $"Coordenadas validas{Environment.NewLine}" +
                $"Pais: {ControlText(lueSupplierAddressCountry)}{Environment.NewLine}" +
                $"Provincia: {ControlText(lueSupplierAddressProvince)}{Environment.NewLine}" +
                $"Ciudad: {ControlText(lueSupplierAddressCity)}{Environment.NewLine}" +
                $"Codigo postal: {txtSupplierAddressPostal?.Text}{Environment.NewLine}" +
                $"Latitud: {latitude:n6}{Environment.NewLine}" +
                $"Longitud: {longitude:n6}";
        }

        await ShowAddressMapAsync(latitude, longitude);

        XtraMessageBox.Show(this, "Coordenadas validas.", "Coordenadas", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private async Task<bool> TryApplyReverseGeocodeAsync(decimal latitude, decimal longitude)
    {
        if (geographyClient is null)
        {
            return true;
        }

        try
        {
            SetCoordinateButtonsEnabled(false);
            var result = await geographyClient.ReverseGeocodeAsync(latitude, longitude);
            ApplyReverseGeocodeResult(result);
            return true;
        }
        catch (Exception exception)
        {
            UiExceptionHandler.ShowError(this, "Coordenadas", exception);
            return false;
        }
        finally
        {
            SetCoordinateButtonsEnabled(true);
        }
    }

    private void ApplyReverseGeocodeResult(ReverseGeocodeResult result)
    {
        var country = FindCountry(result.CountryCode, result.Country);
        if (country is not null && lueSupplierAddressCountry is not null)
        {
            lueSupplierAddressCountry.EditValue = country.Id;
        }

        var countryId = ToNullableInt(lueSupplierAddressCountry?.EditValue);
        var province = FindProvince(countryId, result.Province);
        if (province is not null && lueSupplierAddressProvince is not null)
        {
            lueSupplierAddressProvince.EditValue = province.Id;
        }

        var provinceId = ToNullableInt(lueSupplierAddressProvince?.EditValue);
        var city = FindCity(countryId, provinceId, result.City);
        if (city is not null && lueSupplierAddressCity is not null)
        {
            lueSupplierAddressCity.EditValue = city.Id;
        }

        if (txtSupplierAddressPostal is not null && !string.IsNullOrWhiteSpace(result.PostalCode))
        {
            txtSupplierAddressPostal.Text = result.PostalCode;
        }

        if (txtSupplierAddressLine1 is not null && !string.IsNullOrWhiteSpace(result.FormattedAddress))
        {
            txtSupplierAddressLine1.Text = result.FormattedAddress;
        }

        if (txtSupplierAddressReference is not null && !string.IsNullOrWhiteSpace(result.FormattedAddress))
        {
            txtSupplierAddressReference.Text = result.FormattedAddress;
        }
    }

    private void SetCoordinateButtonsEnabled(bool enabled)
    {
        if (btnValidateCoordinates is not null)
        {
            btnValidateCoordinates.Enabled = enabled;
        }

        if (btnClearCoordinates is not null)
        {
            btnClearCoordinates.Enabled = enabled;
        }
    }

    private void ClearCoordinates()
    {
        if (spnSupplierLatitude is not null) spnSupplierLatitude.Value = 0;
        if (spnSupplierLongitude is not null) spnSupplierLongitude.Value = 0;
        ResetAddressMapPreview();
    }

    private async Task ShowAddressMapAsync(decimal latitude, decimal longitude)
    {
        if (picAddressMap is null)
        {
            return;
        }

        if (geographyClient is null)
        {
            picAddressMap.Visible = false;
            if (lblAddressMapPlaceholder is not null)
            {
                lblAddressMapPlaceholder.Text =
                    $"Ubicacion validada{Environment.NewLine}" +
                    $"Latitud: {latitude:n6}{Environment.NewLine}" +
                    $"Longitud: {longitude:n6}{Environment.NewLine}" +
                    "Vista previa pendiente de servicio de mapas.";
                lblAddressMapPlaceholder.Visible = true;
            }
            return;
        }

        try
        {
            SetCoordinateButtonsEnabled(false);
            var mapResult = await geographyClient.GetStaticMapAsync(latitude, longitude);
            if (string.IsNullOrWhiteSpace(mapResult.ImageBase64))
            {
                ResetAddressMapPreview();
                return;
            }

            var bytes = Convert.FromBase64String(mapResult.ImageBase64);
            using var stream = new MemoryStream(bytes);
            using var loadedImage = Image.FromStream(stream);
            var previousImage = picAddressMap.Image;
            picAddressMap.Image = new Bitmap(loadedImage);
            previousImage?.Dispose();
            picAddressMap.Visible = true;

            if (lblAddressMapPlaceholder is not null)
            {
                lblAddressMapPlaceholder.Visible = false;
            }
        }
        catch (Exception exception)
        {
            ResetAddressMapPreview();
            UiExceptionHandler.ShowError(this, "Mapa", exception);
        }
        finally
        {
            SetCoordinateButtonsEnabled(true);
        }
    }

    private void ResetAddressMapPreview()
    {
        if (picAddressMap is not null)
        {
            var previousImage = picAddressMap.Image;
            picAddressMap.Image = null;
            previousImage?.Dispose();
            picAddressMap.Visible = false;
        }

        if (lblAddressMapPlaceholder is not null)
        {
            lblAddressMapPlaceholder.Visible = true;
            lblAddressMapPlaceholder.Text = "Vista previa de mapa pendiente de integracion";
        }
    }

    private void RebindGeneralProvinces(bool clearSelection)
    {
        BindLookup(lueProvince, FilterProvinces(ToNullableInt(lueCountry?.EditValue)), "Pichincha", "Guayas", "Azuay");
        if (clearSelection)
        {
            lueProvince.EditValue = null;
        }

        RebindGeneralCities(clearSelection);
    }

    private void RebindGeneralCities(bool clearSelection)
    {
        BindLookup(lueCity, FilterCities(ToNullableInt(lueCountry?.EditValue), ToNullableInt(lueProvince?.EditValue)), "Quito", "Guayaquil", "Cuenca");
        if (clearSelection)
        {
            lueCity.EditValue = null;
        }
    }

    private void RebindAddressProvinces(bool clearSelection)
    {
        BindLookup(lueSupplierAddressProvince, FilterProvinces(ToNullableInt(lueSupplierAddressCountry?.EditValue)), "Pichincha", "Guayas", "Azuay");
        if (clearSelection)
        {
            lueSupplierAddressProvince.EditValue = null;
            txtSupplierAddressPostal.Text = string.Empty;
        }

        RebindAddressCities(clearSelection);
    }

    private void RebindAddressCities(bool clearSelection)
    {
        BindLookup(lueSupplierAddressCity, FilterCities(ToNullableInt(lueSupplierAddressCountry?.EditValue), ToNullableInt(lueSupplierAddressProvince?.EditValue)), "Quito", "Guayaquil", "Cuenca");
        if (clearSelection)
        {
            lueSupplierAddressCity.EditValue = null;
            txtSupplierAddressPostal.Text = string.Empty;
        }
    }

    private IReadOnlyCollection<BusinessPartnerGeoLookupOption> FilterProvinces(int? countryId)
    {
        return lookups.Provinces
            .Where(option => option.IsActive)
            .Where(option => !countryId.HasValue || option.CountryId == countryId.Value)
            .ToList();
    }

    private IReadOnlyCollection<BusinessPartnerGeoLookupOption> FilterCities(int? countryId, int? provinceId)
    {
        return lookups.Cities
            .Where(option => option.IsActive)
            .Where(option => !countryId.HasValue || option.CountryId == countryId.Value)
            .Where(option => !provinceId.HasValue || option.ProvinceId == provinceId.Value)
            .ToList();
    }

    private BusinessPartnerLookupOption? FindCountry(string? countryCode, string? countryName)
    {
        return lookups.Countries
            .Where(option => option.IsActive)
            .FirstOrDefault(option =>
                TextMatches(option.Code, countryCode)
                || TextMatches(option.Name, countryName)
                || TextMatches(option.Code, countryName));
    }

    private BusinessPartnerGeoLookupOption? FindProvince(int? countryId, string? provinceName)
    {
        if (string.IsNullOrWhiteSpace(provinceName))
        {
            return null;
        }

        return lookups.Provinces
            .Where(option => option.IsActive)
            .Where(option => !countryId.HasValue || option.CountryId == countryId.Value)
            .FirstOrDefault(option => TextMatches(option.Name, provinceName) || TextMatches(option.Code, provinceName));
    }

    private BusinessPartnerGeoLookupOption? FindCity(int? countryId, int? provinceId, string? cityName)
    {
        if (string.IsNullOrWhiteSpace(cityName))
        {
            return null;
        }

        return lookups.Cities
            .Where(option => option.IsActive)
            .Where(option => !countryId.HasValue || option.CountryId == countryId.Value)
            .Where(option => !provinceId.HasValue || option.ProvinceId == provinceId.Value)
            .FirstOrDefault(option => TextMatches(option.Name, cityName) || TextMatches(option.Code, cityName));
    }

    private void ApplyPostalCodeFromSelectedCity()
    {
        if (txtSupplierAddressPostal is null || !string.IsNullOrWhiteSpace(txtSupplierAddressPostal.Text))
        {
            return;
        }

        var city = SelectedGeoOption(lueSupplierAddressCity);
        if (!string.IsNullOrWhiteSpace(city?.PostalCode))
        {
            txtSupplierAddressPostal.Text = city.PostalCode;
        }
    }

    private static bool TextMatches(string? left, string? right)
    {
        var leftValue = NullIfEmpty(left);
        var rightValue = NullIfEmpty(right);
        if (leftValue is null || rightValue is null)
        {
            return false;
        }

        return CultureInfo.InvariantCulture.CompareInfo.Compare(
            leftValue,
            rightValue,
            CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) == 0;
    }

    private static void RemoveFocusedRow(GridView? view)
    {
        if (view is null || view.FocusedRowHandle < 0)
        {
            return;
        }

        view.DeleteRow(view.FocusedRowHandle);
    }

    private static string? RowText(DataRow row, string columnName)
    {
        if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
        {
            return null;
        }

        return NullIfEmpty(Convert.ToString(row[columnName]));
    }

    private static bool RowBool(DataRow row, string columnName)
    {
        if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
        {
            return false;
        }

        return Convert.ToBoolean(row[columnName]);
    }

    private static int? RowInt(DataRow row, string columnName)
    {
        if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
        {
            return null;
        }

        return int.TryParse(Convert.ToString(row[columnName]), out var value) ? value : null;
    }

    private static long? RowLong(DataRow row, string columnName)
    {
        if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
        {
            return null;
        }

        return long.TryParse(Convert.ToString(row[columnName]), out var value) ? value : null;
    }

    private static decimal? RowDecimal(DataRow row, string columnName)
    {
        if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
        {
            return null;
        }

        var value = Convert.ToDecimal(row[columnName]);
        return value == 0 ? null : value;
    }

    private static decimal? CoordinateValue(SpinEdit? spinEdit)
    {
        if (spinEdit is null || spinEdit.Value == 0)
        {
            return null;
        }

        return spinEdit.Value;
    }

    private static string NormalizeAddressType(string? value)
    {
        return value?.Trim().ToUpperInvariant() switch
        {
            "FISCAL" => "Billing",
            "ENTREGA" => "Shipping",
            "COBRANZA" => "Other",
            "BILLING" => "Billing",
            "SHIPPING" => "Shipping",
            "OTHER" => "Other",
            _ => "Main"
        };
    }

    private static string? ToCountryCode(string? value)
    {
        return value?.Trim().ToUpperInvariant() switch
        {
            "ECUADOR" => "EC",
            "PERU" => "PE",
            "COLOMBIA" => "CO",
            { Length: <= 3 } code => code,
            _ => null
        };
    }

    private static string? LookupCode(BaseEdit? lookup)
    {
        var selectedValue = lookup?.EditValue;
        if (selectedValue is null || selectedValue == DBNull.Value)
        {
            return null;
        }

        var options = GetLookupOptions(lookup!).ToList();
        if (options.Count == 0)
        {
            return NullIfEmpty(Convert.ToString(selectedValue));
        }

        var selectedText = Convert.ToString(selectedValue);
        var selectedId = ToNullableInt(selectedValue);
        var selectedOption = options.FirstOrDefault(option => selectedId.HasValue && option.Id == selectedId.Value)
            ?? options.FirstOrDefault(option => string.Equals(option.Code, selectedText, StringComparison.OrdinalIgnoreCase))
            ?? options.FirstOrDefault(option => string.Equals(option.Name, selectedText, StringComparison.OrdinalIgnoreCase));

        return selectedOption?.Code;
    }

    private static object? LookupValueByCodeOrName(
        IReadOnlyCollection<BusinessPartnerLookupOption> options,
        string? value,
        string? fallbackValue = null)
    {
        var candidate = NullIfEmpty(value) ?? NullIfEmpty(fallbackValue);
        if (candidate is null || options.Count == 0)
        {
            return candidate;
        }

        var option = options.FirstOrDefault(item => string.Equals(item.Code, candidate, StringComparison.OrdinalIgnoreCase))
            ?? options.FirstOrDefault(item => string.Equals(item.Name, candidate, StringComparison.OrdinalIgnoreCase));

        return option is null ? candidate : option.Id;
    }

    private static object? LookupValueByCodeOrName(
        IReadOnlyCollection<BusinessPartnerGeoLookupOption> options,
        string? value,
        string? fallbackValue = null)
    {
        var candidate = NullIfEmpty(value) ?? NullIfEmpty(fallbackValue);
        if (candidate is null || options.Count == 0)
        {
            return candidate;
        }

        var option = options.FirstOrDefault(item => string.Equals(item.Code, candidate, StringComparison.OrdinalIgnoreCase))
            ?? options.FirstOrDefault(item => string.Equals(item.Name, candidate, StringComparison.OrdinalIgnoreCase));

        return option is null ? candidate : option.Id;
    }

    private static object? LookupValueByCodeOrName(
        IReadOnlyCollection<BusinessPartnerRetentionConceptLookup> options,
        string? value,
        string? fallbackValue = null)
    {
        var candidate = NullIfEmpty(value) ?? NullIfEmpty(fallbackValue);
        if (candidate is null || options.Count == 0)
        {
            return candidate;
        }

        var option = options.FirstOrDefault(item => string.Equals(item.Code, candidate, StringComparison.OrdinalIgnoreCase))
            ?? options.FirstOrDefault(item => string.Equals(item.SriCode, candidate, StringComparison.OrdinalIgnoreCase))
            ?? options.FirstOrDefault(item => string.Equals(item.Name, candidate, StringComparison.OrdinalIgnoreCase));

        return option is null ? candidate : option.Id;
    }

    private static BusinessPartnerLookupOption ToLookupOption(BusinessPartnerRetentionConceptLookup concept)
    {
        return new BusinessPartnerLookupOption(concept.Id, concept.Code, concept.Name, concept.IsActive);
    }

    private static BusinessPartnerLookupOption ToLookupOption(BusinessPartnerGeoLookupOption option)
    {
        return new BusinessPartnerLookupOption(option.Id, option.Code, option.Name, option.IsActive);
    }

    private static IEnumerable<BusinessPartnerLookupOption> GetLookupOptions(BaseEdit lookup)
    {
        var dataSource = lookup switch
        {
            LookUpEdit edit => edit.Properties.DataSource,
            SearchLookUpEdit edit => edit.Properties.DataSource,
            _ => null
        };

        if (dataSource is not IEnumerable enumerable)
        {
            return Enumerable.Empty<BusinessPartnerLookupOption>();
        }

        var values = enumerable.Cast<object>().ToList();
        if (values.Count == 0)
        {
            return Enumerable.Empty<BusinessPartnerLookupOption>();
        }

        if (values.OfType<BusinessPartnerLookupOption>().Any())
        {
            return values.OfType<BusinessPartnerLookupOption>();
        }

        if (values.OfType<BusinessPartnerGeoLookupOption>().Any())
        {
            return values.OfType<BusinessPartnerGeoLookupOption>().Select(ToLookupOption);
        }

        return values.OfType<BusinessPartnerRetentionConceptLookup>().Select(ToLookupOption);
    }

    private static string? ToCurrencyCode(string? value)
    {
        return value?.Trim().ToUpperInvariant() switch
        {
            null or "" => null,
            var text when text.StartsWith("USD", StringComparison.OrdinalIgnoreCase) => "USD",
            var text when text.StartsWith("EUR", StringComparison.OrdinalIgnoreCase) => "EUR",
            { Length: <= 3 } code => code,
            _ => null
        };
    }

    private static string ToSapSyncStatus(string? value)
    {
        return value?.Trim().ToUpperInvariant() switch
        {
            "SINCRONIZADO" or "SYNCED" => "Synced",
            "ERROR" => "Error",
            "DESHABILITADO" or "DISABLED" or "PENDIENTE" or "PENDING" or null or "" => "Pending",
            _ => "Pending"
        };
    }

    private static string ToSapSyncDisplay(string? value)
    {
        return value?.Trim().ToUpperInvariant() switch
        {
            "SYNCED" or "SINCRONIZADO" => "Sincronizado",
            "ERROR" => "Error",
            "DISABLED" or "DESHABILITADO" => "Deshabilitado",
            _ => "Pendiente"
        };
    }

    private void BindPurchaseLookups()
    {
        AddItemsIfExists(luePurchasePaymentTerm, "Credito 30 dias", "Credito 15 dias", "Contado");
        BindLookup(luePurchaseCurrency, lookups.Currencies, "USD - Dolar Americano", "Moneda local");
        BindLookup(luePurchaseBuyer, lookups.PurchasingAgents, "Maria Fernandez Ortiz", "Ana Lucia Perez", "Carlos Perez");

        if (luePurchasePaymentTerm is not null) luePurchasePaymentTerm.EditValue = "Credito 30 dias";
        if (luePurchaseCurrency is not null) luePurchaseCurrency.EditValue = LookupValueByCodeOrName(lookups.Currencies, "USD", "USD - Dolar Americano");
        if (luePurchaseBuyer is not null) luePurchaseBuyer.EditValue = LookupValueByCodeOrName(lookups.PurchasingAgents, null, "Maria Fernandez Ortiz");
    }

    private void LoadPurchaseRows()
    {
        var documents = new DataTable();
        documents.Columns.Add("Date", typeof(string));
        documents.Columns.Add("DocumentType", typeof(string));
        documents.Columns.Add("Number", typeof(string));
        documents.Columns.Add("Status", typeof(string));
        documents.Columns.Add("Total", typeof(decimal));
        documents.Columns.Add("Currency", typeof(string));
        documents.Columns.Add("SapStatus", typeof(string));

        documents.Rows.Add("15/05/2026", "OC", "OC-000894", "Recibido", 8750.00m, "USD", "Sincronizado");
        documents.Rows.Add("28/04/2026", "OC", "OC-000876", "Recibido", 12300.50m, "USD", "Sincronizado");
        documents.Rows.Add("10/04/2026", "OC", "OC-000856", "Recibido", 15875.20m, "USD", "Pendiente");
        documents.Rows.Add("25/03/2026", "OC", "OC-000821", "Cerrado", 9450.10m, "USD", "Sincronizado");

        var products = new DataTable();
        products.Columns.Add("Code", typeof(string));
        products.Columns.Add("Name", typeof(string));
        products.Columns.Add("Unit", typeof(string));
        products.Columns.Add("LastPrice", typeof(decimal));
        products.Columns.Add("Currency", typeof(string));
        products.Columns.Add("LastPurchaseDate", typeof(string));

        products.Rows.Add("INS-001", "Insumo industrial A", "UND", 12.75m, "USD", "15/05/2026");
        products.Rows.Add("MAT-014", "Material de empaque", "CAJ", 8.40m, "USD", "28/04/2026");
        products.Rows.Add("REP-032", "Repuesto operativo", "UND", 45.90m, "USD", "10/04/2026");

        grdPurchaseDocuments.DataSource = documents;
        grdPurchaseProducts.DataSource = products;
    }

    private void BindBankLookups()
    {
        BindLookup(lueBankName, lookups.Banks, "Banco Pichincha", "Banco de Guayaquil", "Banco del Pacifico");
        BindLookup(lueBankAccountType, lookups.BankAccountTypes, "Corriente", "Ahorros");
        BindLookup(lueBankCurrency, lookups.Currencies, "USD - Dolar Americano", "Moneda local");
        AddItemsIfExists(lueBankPrimary, "Si", "No");
        AddItemsIfExists(lueBankStatus, "Activo", "Inactivo");
        BindLookup(lueBankCountry, lookups.Countries, "Ecuador", "Peru", "Colombia");
        BindLookup(lueBankCity, lookups.Cities, "Quito", "Guayaquil", "Cuenca");
    }

    private void LoadBankRows()
    {
        if (grdBankAccounts is null)
        {
            return;
        }

        bankTable = CreateBankAccountTable();
        if (partner is not null)
        {
            foreach (var account in partner.BankAccounts)
            {
                bankTable.Rows.Add(
                    account.BankId ?? (object)DBNull.Value,
                    account.BankAccountTypeId ?? (object)DBNull.Value,
                    account.BankName ?? string.Empty,
                    account.AccountType ?? string.Empty,
                    account.AccountNumber,
                    account.HolderName ?? string.Empty,
                    account.HolderIdentification ?? string.Empty,
                    account.CurrencyCode ?? string.Empty,
                    account.IsPrimary,
                    account.IsActive,
                    account.SwiftCode ?? string.Empty,
                    account.AbaRoutingCode ?? string.Empty,
                    account.Iban ?? string.Empty,
                    account.BankCountry ?? string.Empty,
                    account.BankCity ?? string.Empty,
                    account.Notes ?? string.Empty);
            }
        }

        grdBankAccounts.DataSource = bankTable;
    }

    private static DataTable CreateBankAccountTable()
    {
        var table = new DataTable();
        table.Columns.Add("BankId", typeof(int));
        table.Columns.Add("BankAccountTypeId", typeof(int));
        table.Columns.Add("BankName", typeof(string));
        table.Columns.Add("AccountType", typeof(string));
        table.Columns.Add("AccountNumber", typeof(string));
        table.Columns.Add("Holder", typeof(string));
        table.Columns.Add("Identification", typeof(string));
        table.Columns.Add("Currency", typeof(string));
        table.Columns.Add("IsPrimary", typeof(bool));
        table.Columns.Add("IsActive", typeof(bool));
        table.Columns.Add("SwiftCode", typeof(string));
        table.Columns.Add("AbaRoutingCode", typeof(string));
        table.Columns.Add("Iban", typeof(string));
        table.Columns.Add("BankCountry", typeof(string));
        table.Columns.Add("BankCity", typeof(string));
        table.Columns.Add("Notes", typeof(string));
        return table;
    }

    private IReadOnlyCollection<SaveBusinessPartnerBankAccountRequest> BuildBankAccountRequests()
    {
        return (bankTable ?? CreateBankAccountTable())
            .Rows
            .Cast<DataRow>()
            .Select(row => new SaveBusinessPartnerBankAccountRequest(
                RowInt(row, "BankId"),
                RowInt(row, "BankAccountTypeId"),
                RowText(row, "BankName"),
                RowText(row, "AccountType"),
                RowText(row, "AccountNumber") ?? string.Empty,
                RowText(row, "Holder"),
                RowText(row, "Identification"),
                ToCurrencyCode(RowText(row, "Currency")),
                RowText(row, "SwiftCode"),
                RowText(row, "AbaRoutingCode"),
                RowText(row, "Iban"),
                RowText(row, "BankCountry"),
                RowText(row, "BankCity"),
                RowText(row, "Notes"),
                RowBool(row, "IsPrimary"),
                RowBool(row, "IsActive")))
            .Where(account => !string.IsNullOrWhiteSpace(account.AccountNumber))
            .ToList();
    }

    private void AddBankRow()
    {
        bankTable ??= CreateBankAccountTable();
        if (string.IsNullOrWhiteSpace(txtBankAccountNumber?.Text))
        {
            return;
        }

        bankTable.Rows.Add(
            ToNullableInt(lueBankName?.EditValue) ?? (object)DBNull.Value,
            ToNullableInt(lueBankAccountType?.EditValue) ?? (object)DBNull.Value,
            NullIfEmpty(ControlText(lueBankName)) ?? string.Empty,
            NullIfEmpty(ControlText(lueBankAccountType)) ?? string.Empty,
            txtBankAccountNumber.Text.Trim(),
            NullIfEmpty(txtBankHolder?.Text) ?? string.Empty,
            NullIfEmpty(txtBankHolderIdentification?.Text) ?? string.Empty,
            LookupCode(lueBankCurrency) ?? ToCurrencyCode(ControlText(lueBankCurrency)) ?? string.Empty,
            IsYes(lueBankPrimary),
            !string.Equals(ControlText(lueBankStatus), "Inactivo", StringComparison.OrdinalIgnoreCase),
            NullIfEmpty(txtBankSwift?.Text) ?? string.Empty,
            NullIfEmpty(txtBankAba?.Text) ?? string.Empty,
            NullIfEmpty(txtBankIban?.Text) ?? string.Empty,
            NullIfEmpty(ControlText(lueBankCountry)) ?? string.Empty,
            NullIfEmpty(ControlText(lueBankCity)) ?? string.Empty,
            NullIfEmpty(memBankNotes?.Text) ?? string.Empty);

        grdBankAccounts.DataSource = bankTable;
    }

    private void UpdateBankRow()
    {
        var row = grvBankAccounts.GetFocusedDataRow();
        if (row is null || string.IsNullOrWhiteSpace(txtBankAccountNumber?.Text))
        {
            return;
        }

        row["BankId"] = ToNullableInt(lueBankName?.EditValue) ?? (object)DBNull.Value;
        row["BankAccountTypeId"] = ToNullableInt(lueBankAccountType?.EditValue) ?? (object)DBNull.Value;
        row["BankName"] = NullIfEmpty(ControlText(lueBankName)) ?? string.Empty;
        row["AccountType"] = NullIfEmpty(ControlText(lueBankAccountType)) ?? string.Empty;
        row["AccountNumber"] = txtBankAccountNumber.Text.Trim();
        row["Holder"] = NullIfEmpty(txtBankHolder?.Text) ?? string.Empty;
        row["Identification"] = NullIfEmpty(txtBankHolderIdentification?.Text) ?? string.Empty;
        row["Currency"] = LookupCode(lueBankCurrency) ?? ToCurrencyCode(ControlText(lueBankCurrency)) ?? string.Empty;
        row["IsPrimary"] = IsYes(lueBankPrimary);
        row["IsActive"] = !string.Equals(ControlText(lueBankStatus), "Inactivo", StringComparison.OrdinalIgnoreCase);
        row["SwiftCode"] = NullIfEmpty(txtBankSwift?.Text) ?? string.Empty;
        row["AbaRoutingCode"] = NullIfEmpty(txtBankAba?.Text) ?? string.Empty;
        row["Iban"] = NullIfEmpty(txtBankIban?.Text) ?? string.Empty;
        row["BankCountry"] = NullIfEmpty(ControlText(lueBankCountry)) ?? string.Empty;
        row["BankCity"] = NullIfEmpty(ControlText(lueBankCity)) ?? string.Empty;
        row["Notes"] = NullIfEmpty(memBankNotes?.Text) ?? string.Empty;
    }

    private void ClearBankInputs()
    {
        if (lueBankName is not null) lueBankName.EditValue = null;
        if (lueBankAccountType is not null) lueBankAccountType.EditValue = null;
        if (lueBankCurrency is not null) lueBankCurrency.EditValue = null;
        if (lueBankPrimary is not null) lueBankPrimary.EditValue = "No";
        if (lueBankStatus is not null) lueBankStatus.EditValue = "Activo";
        if (lueBankCountry is not null) lueBankCountry.EditValue = null;
        if (lueBankCity is not null) lueBankCity.EditValue = null;
        if (txtBankHolder is not null) txtBankHolder.Text = string.Empty;
        if (txtBankHolderIdentification is not null) txtBankHolderIdentification.Text = string.Empty;
        if (txtBankAccountNumber is not null) txtBankAccountNumber.Text = string.Empty;
        if (txtBankSwift is not null) txtBankSwift.Text = string.Empty;
        if (txtBankAba is not null) txtBankAba.Text = string.Empty;
        if (txtBankIban is not null) txtBankIban.Text = string.Empty;
        if (memBankNotes is not null) memBankNotes.Text = string.Empty;
    }

    private void BindAccountingLookups()
    {
        BindAccountLookup(lueAccountingSupplierAccount);
        BindAccountLookup(lueAccountingAdvanceAccount);
        BindAccountLookup(lueAccountingDefaultExpenseAccount);
        BindAccountLookup(lueAccountingDifferenceAccount);
        BindAccountLookup(lueAccountingRoundingAccount);
        BindAccountLookup(lueAccountingClearingAccount);
        BindAccountLookup(lueAccountingDiscountAccount);
        BindAccountLookup(lueAccountingRetentionPayableAccount);
        BindLookup(lueAccountingPaymentMethod, lookups.AccountingPaymentMethods, "Transferencia bancaria", "Cheque", "Efectivo proveedor");
        BindLookup(lueAccountingPaymentPriority, lookups.PaymentPriorities, "Normal", "Alta", "Retenida");
        AddItemsIfExists(lueAccountingRequiredPaymentDay, "Viernes", "Lunes", "Miercoles");
        BindLookup(lueAccountingPaymentDocumentType, lookups.PaymentDocumentTypes, "Egreso proveedor", "Nota de debito", "Liquidacion");
        BindLookup(lueAccountingApprovalFlow, lookups.ApprovalFlows, "Pago > 5,000 requiere aprobacion", "Siempre requiere aprobacion", "Sin aprobacion");
        BindLookup(lueAccountingBranch, lookups.Branches, "Matriz", "Sucursal principal");
        BindLookup(lueAccountingDepartment, lookups.Departments, "Administracion", "Compras");
        BindLookup(lueAccountingBusinessLine, lookups.BusinessLines, "Comercializacion", "Servicios");
        BindLookup(lueAccountingCostCenter, lookups.CostCenters, "Administracion general", "Compras");
        BindLookup(lueAccountingProject, lookups.Projects, "Sin Proyecto");
        if (lueAccountingPaymentMethod is not null) lueAccountingPaymentMethod.EditValue = LookupValueByCodeOrName(lookups.AccountingPaymentMethods, null, "Transferencia bancaria") ?? "Transferencia bancaria";
        if (lueAccountingPaymentPriority is not null) lueAccountingPaymentPriority.EditValue = LookupValueByCodeOrName(lookups.PaymentPriorities, null, "Normal") ?? "Normal";
        if (lueAccountingRequiredPaymentDay is not null) lueAccountingRequiredPaymentDay.EditValue = "Viernes";
        if (lueAccountingPaymentDocumentType is not null) lueAccountingPaymentDocumentType.EditValue = LookupValueByCodeOrName(lookups.PaymentDocumentTypes, null, "Egreso proveedor") ?? "Egreso proveedor";
        if (lueAccountingApprovalFlow is not null) lueAccountingApprovalFlow.EditValue = LookupValueByCodeOrName(lookups.ApprovalFlows, null, "Pago > 5,000 requiere aprobacion") ?? "Pago > 5,000 requiere aprobacion";
        if (lueAccountingBranch is not null) lueAccountingBranch.EditValue = LookupValueByCodeOrName(lookups.Branches, "01", "Matriz");
        if (lueAccountingDepartment is not null) lueAccountingDepartment.EditValue = LookupValueByCodeOrName(lookups.Departments, "ADM", "Administracion");
        if (lueAccountingBusinessLine is not null) lueAccountingBusinessLine.EditValue = LookupValueByCodeOrName(lookups.BusinessLines, "COM", "Comercializacion");
        if (lueAccountingCostCenter is not null) lueAccountingCostCenter.EditValue = LookupValueByCodeOrName(lookups.CostCenters, "CC-ADM-001", "Administracion general");
        if (lueAccountingProject is not null) lueAccountingProject.EditValue = LookupValueByCodeOrName(lookups.Projects, "SINPROY", "Sin Proyecto");
        if (spnAccountingAveragePaymentDays is not null) spnAccountingAveragePaymentDays.Value = 30;
        if (spnAccountingPaymentTolerance is not null) spnAccountingPaymentTolerance.Value = 0;
    }

    private void ApplyRetentionConceptMetadata()
    {
        var concept = SelectedRetentionConcept();
        if (concept is null)
        {
            return;
        }

        if (lueRetentionEntryType is not null && concept.RetentionTypeId.HasValue)
        {
            lueRetentionEntryType.EditValue = concept.RetentionTypeId.Value;
        }

        if (spnRetentionEntryPercent is not null)
        {
            spnRetentionEntryPercent.Value = concept.Percent;
        }

        if (lueRetentionEntryAppliesIva is not null)
        {
            lueRetentionEntryAppliesIva.EditValue = concept.AppliesIva ? "Si" : "No";
        }

        if (lueRetentionEntryAppliesIncome is not null)
        {
            lueRetentionEntryAppliesIncome.EditValue = concept.AppliesIncome ? "Si" : "No";
        }
    }

    private BusinessPartnerRetentionConceptLookup? SelectedRetentionConcept()
    {
        var selectedValue = lueRetentionEntrySriCode?.EditValue;
        if (selectedValue is null || selectedValue == DBNull.Value)
        {
            return null;
        }

        var selectedId = ToNullableInt(selectedValue);
        var selectedText = Convert.ToString(selectedValue);
        return lookups.RetentionConcepts.FirstOrDefault(concept => selectedId.HasValue && concept.Id == selectedId.Value)
            ?? lookups.RetentionConcepts.FirstOrDefault(concept => string.Equals(concept.Code, selectedText, StringComparison.OrdinalIgnoreCase))
            ?? lookups.RetentionConcepts.FirstOrDefault(concept => string.Equals(concept.SriCode, selectedText, StringComparison.OrdinalIgnoreCase))
            ?? lookups.RetentionConcepts.FirstOrDefault(concept => string.Equals(concept.Name, selectedText, StringComparison.OrdinalIgnoreCase));
    }

    private void BindRetentionLookups()
    {
        AddItemsIfExists(lueRetentionAccountingRequired, "Si", "No");
        AddItemsIfExists(lueRetentionAgentConfig, "Si", "No");
        BindLookup(lueRetentionFiscalRegime, lookups.TaxRegimes, "Regimen general", "Regimen especial");
        AddItemsIfExists(lueRetentionSpecialTaxpayer, "No", "Si");
        BindLookup(lueRetentionTaxpayerType, lookups.TaxpayerTypes, "Sociedad", "Persona natural");
        BindLookup(lueRetentionFiscalCountry, lookups.Countries, "Ecuador", "Peru", "Colombia");
        BindLookup(lueRetentionEntryType, lookups.RetentionTypes, "Retencion Fuente", "Retencion IVA");
        BindLookup(lueRetentionEntrySriCode, lookups.RetentionConcepts, "312", "723", "724");
        BindAccountLookup(lueRetentionEntryAccount);
        BindLookup(lueRetentionEntrySupport, lookups.TaxSupports, "Factura", "Liquidacion de compra", "Nota de credito");
        AddItemsIfExists(lueRetentionEntryAppliesIva, "Si", "No");
        AddItemsIfExists(lueRetentionEntryAppliesIncome, "Si", "No");
        AddItemsIfExists(lueRetentionEntryCurrent, "Si", "No");

        if (lueRetentionAccountingRequired is not null) lueRetentionAccountingRequired.EditValue = "Si";
        if (lueRetentionAgentConfig is not null) lueRetentionAgentConfig.EditValue = "Si";
        if (lueRetentionFiscalRegime is not null) lueRetentionFiscalRegime.EditValue = LookupValueByCodeOrName(lookups.TaxRegimes, "GENERAL", "Regimen general") ?? "Regimen general";
        if (lueRetentionSpecialTaxpayer is not null) lueRetentionSpecialTaxpayer.EditValue = "No";
        if (lueRetentionTaxpayerType is not null) lueRetentionTaxpayerType.EditValue = LookupValueByCodeOrName(lookups.TaxpayerTypes, "SOCIEDAD", "Sociedad") ?? "Sociedad";
        if (lueRetentionFiscalCountry is not null) lueRetentionFiscalCountry.EditValue = LookupValueByCodeOrName(lookups.Countries, null, "Ecuador");
        if (lueRetentionEntryType is not null) lueRetentionEntryType.EditValue = LookupValueByCodeOrName(lookups.RetentionTypes, "FUENTE", "Retencion Fuente") ?? "Retencion Fuente";
        if (lueRetentionEntrySriCode is not null) lueRetentionEntrySriCode.EditValue = LookupValueByCodeOrName(lookups.RetentionConcepts, "312", "312") ?? "312";
        ApplyRetentionConceptMetadata();
        if (spnRetentionEntryPercent is not null && spnRetentionEntryPercent.Value == 0) spnRetentionEntryPercent.Value = 1.75m;
        if (lueRetentionEntrySupport is not null) lueRetentionEntrySupport.EditValue = LookupValueByCodeOrName(lookups.TaxSupports, "FACTURA", "Factura") ?? "Factura";
        if (lueRetentionEntryAppliesIva is not null) lueRetentionEntryAppliesIva.EditValue = "Si";
        if (lueRetentionEntryAppliesIncome is not null) lueRetentionEntryAppliesIncome.EditValue = "Si";
        if (lueRetentionEntryCurrent is not null) lueRetentionEntryCurrent.EditValue = "Si";
    }

    private void LoadRetentionRows()
    {
        if (grdRetentionRules is null)
        {
            return;
        }

        retentionTable = CreateRetentionTable();
        foreach (var item in partner?.RetentionSettings ?? [])
        {
            retentionTable.Rows.Add(
                item.RetentionTypeId ?? (object)DBNull.Value,
                item.RetentionConceptId ?? (object)DBNull.Value,
                item.TaxSupportId ?? (object)DBNull.Value,
                item.SriCode ?? string.Empty,
                item.RetentionType ?? string.Empty,
                GetRetentionKind(item.AppliesIva, item.AppliesIncome),
                item.Percent,
                string.Empty,
                item.IsCurrent,
                item.EntryAccountId,
                item.TaxSupport ?? string.Empty,
                item.AppliesIva,
                item.AppliesIncome,
                item.Notes ?? string.Empty);
        }

        grdRetentionRules.DataSource = retentionTable;
    }

    private static DataTable CreateRetentionTable()
    {
        var table = new DataTable();
        table.Columns.Add("RetentionTypeId", typeof(int));
        table.Columns.Add("RetentionConceptId", typeof(int));
        table.Columns.Add("TaxSupportId", typeof(int));
        table.Columns.Add("Code", typeof(string));
        table.Columns.Add("Concept", typeof(string));
        table.Columns.Add("Type", typeof(string));
        table.Columns.Add("Percent", typeof(decimal));
        table.Columns.Add("ValidFrom", typeof(string));
        table.Columns.Add("IsActive", typeof(bool));
        table.Columns.Add("EntryAccountId", typeof(int));
        table.Columns.Add("TaxSupport", typeof(string));
        table.Columns.Add("AppliesIva", typeof(bool));
        table.Columns.Add("AppliesIncome", typeof(bool));
        table.Columns.Add("Notes", typeof(string));
        return table;
    }

    private IReadOnlyCollection<SaveBusinessPartnerRetentionSettingRequest> BuildRetentionRequests()
    {
        var table = retentionTable;
        if (table is null)
        {
            return Array.Empty<SaveBusinessPartnerRetentionSettingRequest>();
        }

        return table.AsEnumerable()
            .Where(row => !string.IsNullOrWhiteSpace(row.Field<string>("Code")) || !string.IsNullOrWhiteSpace(row.Field<string>("Concept")))
            .Select(row => new SaveBusinessPartnerRetentionSettingRequest(
                RowInt(row, "RetentionTypeId"),
                RowInt(row, "RetentionConceptId"),
                RowInt(row, "TaxSupportId"),
                NullIfEmpty(row.Field<string>("Concept")),
                NullIfEmpty(row.Field<string>("Code")),
                row.Field<decimal>("Percent"),
                row.Field<int?>("EntryAccountId"),
                NullIfEmpty(row.Field<string>("TaxSupport")),
                row.Field<bool>("AppliesIva"),
                row.Field<bool>("AppliesIncome"),
                row.Field<bool>("IsActive"),
                NullIfEmpty(row.Field<string>("Notes"))))
            .ToArray();
    }

    private void AddRetentionRow()
    {
        retentionTable ??= CreateRetentionTable();
        AddRetentionValues(retentionTable.NewRow());
    }

    private void UpdateRetentionRow()
    {
        if (retentionTable is null || grvRetentionRules?.GetFocusedDataRow() is not { } row)
        {
            return;
        }

        SetRetentionValues(row);
    }

    private void AddRetentionValues(DataRow row)
    {
        SetRetentionValues(row);
        retentionTable?.Rows.Add(row);
    }

    private void SetRetentionValues(DataRow row)
    {
        var appliesIva = IsYes(lueRetentionEntryAppliesIva);
        var appliesIncome = IsYes(lueRetentionEntryAppliesIncome);
        row["RetentionTypeId"] = ToNullableInt(lueRetentionEntryType?.EditValue) ?? (object)DBNull.Value;
        row["RetentionConceptId"] = ToNullableInt(lueRetentionEntrySriCode?.EditValue) ?? (object)DBNull.Value;
        row["TaxSupportId"] = ToNullableInt(lueRetentionEntrySupport?.EditValue) ?? (object)DBNull.Value;
        row["Code"] = NullIfEmpty(ControlText(lueRetentionEntrySriCode)) ?? string.Empty;
        row["Concept"] = NullIfEmpty(ControlText(lueRetentionEntryType)) ?? string.Empty;
        row["Type"] = GetRetentionKind(appliesIva, appliesIncome);
        row["Percent"] = spnRetentionEntryPercent?.Value ?? 0m;
        row["ValidFrom"] = string.Empty;
        row["IsActive"] = IsYes(lueRetentionEntryCurrent);
        row["EntryAccountId"] = ToNullableInt(lueRetentionEntryAccount?.EditValue) ?? (object)DBNull.Value;
        row["TaxSupport"] = NullIfEmpty(ControlText(lueRetentionEntrySupport)) ?? string.Empty;
        row["AppliesIva"] = appliesIva;
        row["AppliesIncome"] = appliesIncome;
        row["Notes"] = string.Empty;
    }

    private void ClearRetentionInputs()
    {
        if (lueRetentionEntryType is not null) lueRetentionEntryType.EditValue = null;
        if (lueRetentionEntrySriCode is not null) lueRetentionEntrySriCode.EditValue = null;
        if (lueRetentionEntryAccount is not null) lueRetentionEntryAccount.EditValue = null;
        if (lueRetentionEntrySupport is not null) lueRetentionEntrySupport.EditValue = null;
        if (lueRetentionEntryAppliesIva is not null) lueRetentionEntryAppliesIva.EditValue = "No";
        if (lueRetentionEntryAppliesIncome is not null) lueRetentionEntryAppliesIncome.EditValue = "No";
        if (lueRetentionEntryCurrent is not null) lueRetentionEntryCurrent.EditValue = "Si";
        if (spnRetentionEntryPercent is not null) spnRetentionEntryPercent.Value = 0m;
    }

    private static string GetRetentionKind(bool appliesIva, bool appliesIncome)
    {
        return appliesIva && appliesIncome ? "IVA/Renta" : appliesIva ? "IVA" : appliesIncome ? "Renta" : "General";
    }

    private void BindSapLookups()
    {
        AddItemsIfExists(lueSapSyncStatus, "Sincronizado", "Pendiente", "Error", "Deshabilitado");
        AddItemsIfExists(lueSapEnabled, "Si", "No");
        AddItemsIfExists(lueSapMode, "Service Layer", "Conector backend alterno", "Sin SAP");
        AddItemsIfExists(lueSapCompany, "NuanSystem_PROD", "NuanSystem_TEST");
        AddItemsIfExists(lueSapSyncAsSupplier, "Si", "No");
        AddItemsIfExists(lueSapManualRetry, "Si", "No");
        AddItemsIfExists(lueSapRequiresApproval, "Si", "No");
        AddItemsIfExists(lueSapMapRequired, "Si", "No");
        AddItemsIfExists(lueSapMapEnabled, "Si", "No");

        if (lueSapSyncStatus is not null) lueSapSyncStatus.EditValue = "Sincronizado";
        if (lueSapEnabled is not null) lueSapEnabled.EditValue = "Si";
        if (lueSapMode is not null) lueSapMode.EditValue = "Service Layer";
        if (lueSapCompany is not null) lueSapCompany.EditValue = "NuanSystem_PROD";
        if (lueSapSyncAsSupplier is not null) lueSapSyncAsSupplier.EditValue = "Si";
        if (lueSapManualRetry is not null) lueSapManualRetry.EditValue = "Si";
        if (lueSapRequiresApproval is not null) lueSapRequiresApproval.EditValue = "No";
        if (txtSapLastSync is not null) txtSapLastSync.Text = "20/05/2026 16:41";
        if (txtSapLastError is not null) txtSapLastError.Text = "Sin errores pendientes";
        if (txtSapRetryCount is not null) txtSapRetryCount.Text = "0";
        if (txtSapMapSystemField is not null) txtSapMapSystemField.Text = "Name";
        if (txtSapMapSapField is not null) txtSapMapSapField.Text = "CardName";
        if (txtSapMapDescription is not null) txtSapMapDescription.Text = "Razon social sincronizada con SAP.";
        if (lueSapMapRequired is not null) lueSapMapRequired.EditValue = "Si";
        if (lueSapMapEnabled is not null) lueSapMapEnabled.EditValue = "Si";
    }

    private void LoadSapRows()
    {
        if (grdSapSyncHistory is null)
        {
            return;
        }

        var syncHistory = new DataTable();
        syncHistory.Columns.Add("Date", typeof(string));
        syncHistory.Columns.Add("Operation", typeof(string));
        syncHistory.Columns.Add("Status", typeof(string));
        syncHistory.Columns.Add("SapDocEntry", typeof(string));
        syncHistory.Columns.Add("SapDocNum", typeof(string));
        syncHistory.Columns.Add("RetryCount", typeof(int));
        syncHistory.Columns.Add("Message", typeof(string));

        syncHistory.Rows.Add("20/05/2026 16:41", "BusinessPartner.Update", "Sincronizado", "125407", "P000045", 0, "Proveedor actualizado correctamente.");
        syncHistory.Rows.Add("15/03/2026 09:18", "BusinessPartner.Create", "Sincronizado", "125407", "P000045", 0, "Proveedor creado en SAP.");

        grdSapSyncHistory.DataSource = syncHistory;
    }

    private void LoadSapFieldMappingRows()
    {
        if (grdSapFieldMapping is null)
        {
            return;
        }

        sapFieldMappingTable = new DataTable();
        sapFieldMappingTable.Columns.Add("SystemField", typeof(string));
        sapFieldMappingTable.Columns.Add("SapField", typeof(string));
        sapFieldMappingTable.Columns.Add("Description", typeof(string));
        sapFieldMappingTable.Columns.Add("Required", typeof(bool));
        sapFieldMappingTable.Columns.Add("Enabled", typeof(bool));

        if (partner is not null)
        {
            foreach (var mapping in partner.SapFieldMappings)
            {
                sapFieldMappingTable.Rows.Add(
                    mapping.SystemField,
                    mapping.SapField,
                    mapping.Description ?? string.Empty,
                    mapping.IsRequired,
                    mapping.IsEnabled);
            }
        }

        grdSapFieldMapping.DataSource = sapFieldMappingTable;
    }

    private IReadOnlyCollection<SaveBusinessPartnerSapFieldMappingRequest> BuildSapFieldMappingRequests()
    {
        return (sapFieldMappingTable ?? new DataTable())
            .Rows
            .Cast<DataRow>()
            .Select(row => new SaveBusinessPartnerSapFieldMappingRequest(
                RowText(row, "SystemField") ?? string.Empty,
                RowText(row, "SapField") ?? string.Empty,
                RowText(row, "Description"),
                RowBool(row, "Required"),
                RowBool(row, "Enabled")))
            .Where(mapping => !string.IsNullOrWhiteSpace(mapping.SystemField) && !string.IsNullOrWhiteSpace(mapping.SapField))
            .ToList();
    }

    private void AddSapFieldMappingRow()
    {
        if (sapFieldMappingTable is null)
        {
            return;
        }

        var systemField = NullIfEmpty(txtSapMapSystemField?.Text);
        var sapField = NullIfEmpty(txtSapMapSapField?.Text);
        if (systemField is null || sapField is null)
        {
            return;
        }

        sapFieldMappingTable.Rows.Add(
            systemField,
            sapField,
            NullIfEmpty(txtSapMapDescription?.Text) ?? string.Empty,
            string.Equals(lueSapMapRequired?.Text, "Si", StringComparison.OrdinalIgnoreCase),
            string.Equals(lueSapMapEnabled?.Text, "Si", StringComparison.OrdinalIgnoreCase));
    }

    private void UpdateSapFieldMappingRow()
    {
        if (grvSapFieldMapping is null || grvSapFieldMapping.FocusedRowHandle < 0)
        {
            return;
        }

        grvSapFieldMapping.SetFocusedRowCellValue(colSapMapSystemField, NullIfEmpty(txtSapMapSystemField?.Text));
        grvSapFieldMapping.SetFocusedRowCellValue(colSapMapSapField, NullIfEmpty(txtSapMapSapField?.Text));
        grvSapFieldMapping.SetFocusedRowCellValue(colSapMapDescription, NullIfEmpty(txtSapMapDescription?.Text) ?? string.Empty);
        grvSapFieldMapping.SetFocusedRowCellValue(colSapMapRequired, string.Equals(lueSapMapRequired?.Text, "Si", StringComparison.OrdinalIgnoreCase));
        grvSapFieldMapping.SetFocusedRowCellValue(colSapMapEnabled, string.Equals(lueSapMapEnabled?.Text, "Si", StringComparison.OrdinalIgnoreCase));
    }

    private void ClearSapFieldMappingInputs()
    {
        if (txtSapMapSystemField is not null) txtSapMapSystemField.Text = string.Empty;
        if (txtSapMapSapField is not null) txtSapMapSapField.Text = string.Empty;
        if (txtSapMapDescription is not null) txtSapMapDescription.Text = string.Empty;
        if (lueSapMapRequired is not null) lueSapMapRequired.EditValue = "No";
        if (lueSapMapEnabled is not null) lueSapMapEnabled.EditValue = "Si";
    }

    private void BindNotesLookups()
    {

    }

    private SaveBusinessPartnerNotesRequest? BuildNotesRequest()
    {
        var internalNotes = NullIfEmpty(memSupplierInternalNotes?.Text);
        var purchasingNotes = NullIfEmpty(memSupplierPurchasingNotes?.Text);
        var paymentNotes = NullIfEmpty(memSupplierPaymentNotes?.Text);
        var operationalAlert = NullIfEmpty(txtSupplierOperationalAlert?.Text);

        return internalNotes is null && purchasingNotes is null && paymentNotes is null && operationalAlert is null
            ? null
            : new SaveBusinessPartnerNotesRequest(internalNotes, purchasingNotes, paymentNotes, operationalAlert);
    }

    private void LoadAttachmentRows()
    {
        if (grdSupplierAttachments is null)
        {
            return;
        }

        attachmentTable = CreateAttachmentTable();
        foreach (var attachment in partner?.Attachments ?? [])
        {
            attachmentTable.Rows.Add(
                attachment.AttachmentType ?? string.Empty,
                attachment.FileName,
                attachment.Description ?? string.Empty,
                attachment.UploadedAt?.ToString("dd/MM/yyyy") ?? string.Empty,
                attachment.UploadedBy ?? string.Empty,
                attachment.IsActive ? "Activo" : "Inactivo",
                attachment.ReferencePath ?? string.Empty,
                attachment.FileSize,
                attachment.IsActive);
        }

        grdSupplierAttachments.DataSource = attachmentTable;
    }

    private static DataTable CreateAttachmentTable()
    {
        var table = new DataTable();
        table.Columns.Add("Type", typeof(string));
        table.Columns.Add("FileName", typeof(string));
        table.Columns.Add("Description", typeof(string));
        table.Columns.Add("Date", typeof(string));
        table.Columns.Add("User", typeof(string));
        table.Columns.Add("Status", typeof(string));
        table.Columns.Add("ReferencePath", typeof(string));
        table.Columns.Add("FileSize", typeof(long));
        table.Columns.Add("IsActive", typeof(bool));
        return table;
    }

    private IReadOnlyCollection<SaveBusinessPartnerAttachmentRequest> BuildAttachmentRequests()
    {
        return (attachmentTable ?? CreateAttachmentTable())
            .Rows
            .Cast<DataRow>()
            .Select(row => new SaveBusinessPartnerAttachmentRequest(
                RowText(row, "Type"),
                RowText(row, "FileName") ?? string.Empty,
                RowText(row, "Description"),
                RowText(row, "ReferencePath"),
                RowLong(row, "FileSize"),
                row.Table.Columns.Contains("IsActive")
                    ? RowBool(row, "IsActive")
                    : !string.Equals(RowText(row, "Status"), "Inactivo", StringComparison.OrdinalIgnoreCase)))
            .Where(attachment => !string.IsNullOrWhiteSpace(attachment.FileName))
            .ToList();
    }

    private void LoadPartner()
    {
        SetEditValue(lueIdentificationType, lookups.IdentificationTypes.FirstOrDefault()?.Id);
        SetEditValue(lueSupplierGroup, partner?.SupplierGroupId ?? (object)"Proveedores Nacionales");
        SetEditValue(lueSupplierClass, partner?.SupplierClassId ?? (object)"Materiales e Insumos");
        SetEditValue(lueEconomicActivity, partner?.EconomicActivityId ?? (object)"Comercio al por mayor");
        SetEditValue(lueZone, partner?.ZoneId ?? (object)"Zona 1 - Sierra");
        SetEditValue(lueCountry, LookupValueByCodeOrName(lookups.Countries, partner?.CountryCode, "Ecuador"));
        SetEditValue(lueProvince, LookupValueByCodeOrName(lookups.Provinces, partner?.Province, "Pichincha"));
        SetEditValue(lueCity, LookupValueByCodeOrName(lookups.Cities, partner?.City, "Quito"));
        SetEditValue(luePriceList, LookupValueByCodeOrName(lookups.PriceLists, partner?.PriceListCode, "Lista de precios 1"));
        SetEditValue(lueBuyer, LookupValueByCodeOrName(lookups.PurchasingAgents, partner?.AssignedBuyerCode, "Ana Lucia Perez"));
        SetEditValue(lueChannel, "Mayorista");
        SetEditValue(lueSupplyMethod, partner?.SupplyMethodId ?? (object)"Compra local");
        SetEditValue(lueRetentionAccountingRequired, "Si");
        SetEditValue(lueRetentionAgentConfig, "Si");
        SetEditValue(lueRetentionFiscalRegime, "Regimen general");
        SetEditValue(lueRetentionSpecialTaxpayer, "No");
        SetEditValue(lueRetentionTaxpayerType, partner?.TaxpayerTypeId ?? LookupValueByCodeOrName(lookups.TaxpayerTypes, partner?.TaxpayerType, "Sociedad"));
        SetEditValue(lueRetentionFiscalRegime, partner?.TaxRegimeId ?? LookupValueByCodeOrName(lookups.TaxRegimes, partner?.FiscalRegime, "Regimen general"));
        SetEditValue(lueRetentionFiscalCountry, partner?.FiscalCountryId ?? LookupValueByCodeOrName(lookups.Countries, partner?.CountryCode, "Ecuador"));
        SetSpinValue(spnCreditDays, 30);
        SetSpinValue(spnDeliveryDays, 7);
        SetSpinValue(spnMinimumOrder, 500);
        SetSpinValue(spnCreditLimit, 45000);
        SetText(memReturnPolicy, "Se aceptan devoluciones dentro de los 7 dias habiles por defectos de fabrica.");
        SetText(lblSapStatusValue, "Sincronizado");
        SetText(lblOpenOrdersValue, "5");
        SetText(lblPayableBalanceValue, "12,475.60");
        SetText(lblLastPurchaseValue, "15/05/2026");
        SetText(lblPurchases12mValue, "128,450.75");
        SetText(lblRetentionsValue, "3");
        if (tsAllowSales is not null)
        {
            tsAllowSales.IsOn = false;
        }

        if (partner is null)
        {
            return;
        }

        SetText(txtSupplierCode, partner.Code);
        SetText(txtSupplierName, partner.Name);
        SetText(txtSupplierCommercialName, partner.CommercialName);
        SetEditValue(lueIdentificationType, partner.IdentificationTypeId);
        SetText(txtIdentificationNumber, partner.IdentificationNumber);
        SetEditValue(luePurchasePaymentTerm, partner.PaymentTermId);
        SetSpinValue(spnCreditDays, partner.CreditDays);
        SetSpinValue(spnCreditLimit, partner.CreditLimit);
        SetSpinValue(spnDeliveryDays, partner.DeliveryDays);
        SetSpinValue(spnMinimumOrder, partner.MinimumOrderAmount);
        SetEditValue(luePurchaseCurrency, LookupValueByCodeOrName(lookups.Currencies, partner.PreferredCurrencyCode, "USD - Dolar Americano"));
        if (tsAllowSales is not null)
        {
            tsAllowSales.IsOn = partner.AllowsBackorder;
        }
        SetEditValue(lueBuyer, LookupValueByCodeOrName(lookups.PurchasingAgents, partner.AssignedBuyerCode));
        SetEditValue(luePurchaseBuyer, LookupValueByCodeOrName(lookups.PurchasingAgents, partner.AssignedBuyerCode));
        SetEditValue(lueAccountingSupplierAccount, partner.SupplierAccountId);
        SetEditValue(lueAccountingAdvanceAccount, partner.SupplierAdvanceAccountId);
        SetEditValue(lueAccountingRetentionPayableAccount, partner.RetentionAccountId);
        SetEditValue(lueAccountingDefaultExpenseAccount, partner.DefaultExpenseAccountId);
        SetEditValue(lueAccountingDifferenceAccount, partner.DifferenceAccountId);
        SetEditValue(lueAccountingRoundingAccount, partner.RoundingAccountId);
        SetEditValue(lueAccountingClearingAccount, partner.ClearingAccountId);
        SetEditValue(lueAccountingDiscountAccount, partner.DiscountAccountId);
        SetEditValue(lueAccountingBranch, partner.BranchId);
        SetEditValue(lueAccountingDepartment, partner.DepartmentId);
        SetEditValue(lueAccountingBusinessLine, partner.BusinessLineId);
        SetEditValue(lueAccountingCostCenter, partner.CostCenterId);
        SetEditValue(lueAccountingProject, partner.ProjectId);
        SetToggleValue(chkAccountingBySupplier, partner.AccountingBySupplier);
        SetToggleValue(chkAccountingRequiresProvision, partner.RequiresProvision);
        SetToggleValue(chkAccountingAllowsAdvance, partner.AllowsAdvance);
        SetToggleValue(chkAccountingAllowsCompensation, partner.AllowsCompensation);
        SetToggleValue(chkAccountingAllowsPartialPayments, partner.AllowsPartialPayments);
        SetToggleValue(chkAccountingBlocked, partner.IsPaymentBlocked);
        SetToggleValue(chkAccountingUsesWithholdingBase, partner.UsesWithholdingBase);
        SetToggleValue(chkAccountingConciliationRequired, partner.ConciliationRequired);
        SetEditValue(lueAccountingPaymentMethod, partner.AccountingPaymentMethodId.HasValue ? partner.AccountingPaymentMethodId.Value : partner.AccountingPaymentMethod);
        SetEditValue(lueAccountingPaymentPriority, partner.PaymentPriorityId.HasValue ? partner.PaymentPriorityId.Value : partner.PaymentPriority);
        SetEditValue(lueAccountingRequiredPaymentDay, partner.RequiredPaymentDay);
        SetEditValue(lueAccountingApprovalFlow, partner.ApprovalFlowId.HasValue ? partner.ApprovalFlowId.Value : partner.ApprovalFlow);
        SetEditValue(lueAccountingPaymentDocumentType, partner.PaymentDocumentTypeId.HasValue ? partner.PaymentDocumentTypeId.Value : partner.PaymentDocumentType);
        SetSpinValue(spnAccountingAveragePaymentDays, partner.AveragePaymentDays);
        SetSpinValue(spnAccountingPaymentTolerance, partner.PaymentTolerancePercent);
        SetText(lblSapStatusValue, ToSapSyncDisplay(partner.SapSyncStatus));
        SetEditValue(lueSapSyncStatus, ToSapSyncDisplay(partner.SapSyncStatus));
        SetEditValue(lueSapEnabled, partner.SapEnabled ? "Si" : "No");
        SetEditValue(lueSapMode, partner.SapMode);
        SetEditValue(lueSapCompany, partner.SapCompanyCode);
        SetEditValue(lueSapSyncAsSupplier, partner.SyncAsSupplier ? "Si" : "No");
        SetEditValue(lueSapManualRetry, partner.AllowManualSapRetry ? "Si" : "No");
        SetEditValue(lueSapRequiresApproval, partner.RequiresApprovalBeforeSapSync ? "Si" : "No");
        SetText(txtSapRetryCount, partner.SapRetryCount.ToString());
        SetText(txtSapLastSync, partner.SapLastSyncAt?.ToString("dd/MM/yyyy HH:mm"));
        SetText(txtSapLastError, partner.SapLastError);
        SetText(memReturnPolicy, partner.Remarks);
        SetText(memSupplierInternalNotes, partner.Notes?.InternalNotes);
        SetText(memSupplierPurchasingNotes, partner.Notes?.PurchasingNotes);
        SetText(memSupplierPaymentNotes, partner.Notes?.PaymentNotes);
        SetText(txtSupplierOperationalAlert, partner.Notes?.OperationalAlert);
    }

    private static void SetEditValue(BaseEdit? control, object? value)
    {
        if (control is not null)
        {
            control.EditValue = value;
        }
    }

    private static void SetSpinValue(SpinEdit? control, decimal value)
    {
        if (control is not null)
        {
            control.Value = value;
        }
    }

    private static void SetToggleValue(ToggleSwitch? control, bool value)
    {
        if (control is not null)
        {
            control.IsOn = value;
        }
    }

    private static void SetText(Control? control, string? value)
    {
        if (control is not null)
        {
            control.Text = value ?? string.Empty;
        }
    }

    private static string? ControlText(BaseEdit? control)
    {
        return control?.Text;
    }

    private static bool IsYes(BaseEdit? control)
    {
        return string.Equals(control?.Text, "Si", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsToggleOn(ToggleSwitch? toggle)
    {
        return toggle?.IsOn == true;
    }

    private static int ToIntOrDefault(string? value)
    {
        return int.TryParse(value, out var parsedValue) ? parsedValue : 0;
    }

    private static void AddItems(LookUpEdit? lookup, params string[] items)
    {
        if (lookup is null)
        {
            return;
        }

        lookup.Properties.DataSource = items.Select(item => new LookupText(item, item)).ToList();
        lookup.Properties.DisplayMember = nameof(LookupText.Name);
        lookup.Properties.ValueMember = nameof(LookupText.Name);
        lookup.Properties.Columns.Clear();
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(LookupText.Code), "Codigo", 80));
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(LookupText.Name), "Nombre", 180));
    }

    private static void AddItems(SearchLookUpEdit? lookup, params string[] items)
    {
        if (lookup is null)
        {
            return;
        }

        lookup.Properties.DataSource = items.Select(item => new LookupText(item, item)).ToList();
        lookup.Properties.DisplayMember = nameof(LookupText.Name);
        lookup.Properties.ValueMember = nameof(LookupText.Name);
        if (lookup.Properties.PopupView is GridView view)
        {
            view.Columns.Clear();
            view.Columns.AddVisible(nameof(LookupText.Code), "Codigo");
            view.Columns.AddVisible(nameof(LookupText.Name), "Nombre");
        }
    }

    private static void BindLookup(
        LookUpEdit? lookup,
        IReadOnlyCollection<BusinessPartnerLookupOption> options,
        params string[] fallbackItems)
    {
        if (lookup is null)
        {
            return;
        }

        if (options.Count == 0)
        {
            AddItems(lookup, fallbackItems);
            return;
        }

        lookup.Properties.DataSource = options.Where(option => option.IsActive).ToList();
        lookup.Properties.DisplayMember = nameof(BusinessPartnerLookupOption.Name);
        lookup.Properties.ValueMember = nameof(BusinessPartnerLookupOption.Id);
        lookup.Properties.Columns.Clear();
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(BusinessPartnerLookupOption.Code), "Codigo", 80));
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(BusinessPartnerLookupOption.Name), "Nombre", 180));
    }

    private static void BindLookup(
        LookUpEdit? lookup,
        IReadOnlyCollection<BusinessPartnerRetentionConceptLookup> options,
        params string[] fallbackItems)
    {
        if (lookup is null)
        {
            return;
        }

        if (options.Count == 0)
        {
            AddItems(lookup, fallbackItems);
            return;
        }

        lookup.Properties.DataSource = options.Where(option => option.IsActive).ToList();
        lookup.Properties.DisplayMember = nameof(BusinessPartnerRetentionConceptLookup.Code);
        lookup.Properties.ValueMember = nameof(BusinessPartnerRetentionConceptLookup.Id);
        lookup.Properties.Columns.Clear();
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(BusinessPartnerRetentionConceptLookup.Code), "Codigo", 80));
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(BusinessPartnerRetentionConceptLookup.SriCode), "Codigo SRI", 90));
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(BusinessPartnerRetentionConceptLookup.Name), "Concepto", 180));
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(BusinessPartnerRetentionConceptLookup.Percent), "%", 60));
    }

    private static void BindLookup(
        SearchLookUpEdit? lookup,
        IReadOnlyCollection<BusinessPartnerLookupOption> options,
        params string[] fallbackItems)
    {
        if (lookup is null)
        {
            return;
        }

        if (options.Count == 0)
        {
            AddItems(lookup, fallbackItems);
            return;
        }

        lookup.Properties.DataSource = options.Where(option => option.IsActive).ToList();
        lookup.Properties.DisplayMember = nameof(BusinessPartnerLookupOption.Name);
        lookup.Properties.ValueMember = nameof(BusinessPartnerLookupOption.Id);
        if (lookup.Properties.PopupView is GridView view)
        {
            view.Columns.Clear();
            view.Columns.AddVisible(nameof(BusinessPartnerLookupOption.Code), "Codigo");
            view.Columns.AddVisible(nameof(BusinessPartnerLookupOption.Name), "Nombre");
        }
    }

    private static void BindLookup(
        SearchLookUpEdit? lookup,
        IReadOnlyCollection<BusinessPartnerGeoLookupOption> options,
        params string[] fallbackItems)
    {
        if (lookup is null)
        {
            return;
        }

        if (options.Count == 0)
        {
            AddItems(lookup, fallbackItems);
            return;
        }

        lookup.Properties.DataSource = options.Where(option => option.IsActive).ToList();
        lookup.Properties.DisplayMember = nameof(BusinessPartnerGeoLookupOption.Name);
        lookup.Properties.ValueMember = nameof(BusinessPartnerGeoLookupOption.Id);
        if (lookup.Properties.PopupView is GridView view)
        {
            view.Columns.Clear();
            view.Columns.AddVisible(nameof(BusinessPartnerGeoLookupOption.Code), "Codigo");
            view.Columns.AddVisible(nameof(BusinessPartnerGeoLookupOption.Name), "Nombre");
        }
    }

    private static void BindLookup(
        SearchLookUpEdit? lookup,
        IReadOnlyCollection<BusinessPartnerRetentionConceptLookup> options,
        params string[] fallbackItems)
    {
        if (lookup is null)
        {
            return;
        }

        if (options.Count == 0)
        {
            AddItems(lookup, fallbackItems);
            return;
        }

        lookup.Properties.DataSource = options.Where(option => option.IsActive).ToList();
        lookup.Properties.DisplayMember = nameof(BusinessPartnerRetentionConceptLookup.Code);
        lookup.Properties.ValueMember = nameof(BusinessPartnerRetentionConceptLookup.Id);
        if (lookup.Properties.PopupView is GridView view)
        {
            view.Columns.Clear();
            view.Columns.AddVisible(nameof(BusinessPartnerRetentionConceptLookup.Code), "Codigo");
            view.Columns.AddVisible(nameof(BusinessPartnerRetentionConceptLookup.SriCode), "Codigo SRI");
            view.Columns.AddVisible(nameof(BusinessPartnerRetentionConceptLookup.Name), "Concepto");
            view.Columns.AddVisible(nameof(BusinessPartnerRetentionConceptLookup.Percent), "%");
        }
    }

    private void BindAccountLookup(SearchLookUpEdit? lookup)
    {
        BindLookup(lookup, lookups.Accounts);
    }

    private static BusinessPartnerGeoLookupOption? SelectedGeoOption(SearchLookUpEdit? lookup)
    {
        if (lookup?.EditValue is null || lookup.EditValue == DBNull.Value)
        {
            return null;
        }

        if (lookup.Properties.DataSource is not IEnumerable enumerable)
        {
            return null;
        }

        var selectedId = ToNullableInt(lookup.EditValue);
        var selectedText = Convert.ToString(lookup.EditValue);
        return enumerable
            .Cast<object>()
            .OfType<BusinessPartnerGeoLookupOption>()
            .FirstOrDefault(option => selectedId.HasValue && option.Id == selectedId.Value)
            ?? enumerable
                .Cast<object>()
                .OfType<BusinessPartnerGeoLookupOption>()
                .FirstOrDefault(option =>
                    string.Equals(option.Code, selectedText, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(option.Name, selectedText, StringComparison.OrdinalIgnoreCase));
    }

    private static void AddItemsIfExists(LookUpEdit? lookup, params string[] items)
    {
        AddItems(lookup, items);
    }

    private static void AddItemsIfExists(SearchLookUpEdit? lookup, params string[] items)
    {
        AddItems(lookup, items);
    }

    private bool IsDesignerHosted()
    {
        return DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;
    }

    private static int? ToNullableInt(object? value)
    {
        if (value is null || value == DBNull.Value)
        {
            return null;
        }

        if (value is int intValue)
        {
            return intValue;
        }

        return int.TryParse(Convert.ToString(value), out var parsedValue) ? parsedValue : null;
    }

    private bool RequireLookup(BaseEdit control, string message)
    {
        if (control.EditValue is null || control.EditValue == DBNull.Value)
        {
            Validator.SetError(control, message);
            return false;
        }

        return true;
    }

    private static string? NullIfEmpty(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static SaveBusinessPartnerRequest EmptyRequest()
    {
        return new SaveBusinessPartnerRequest(
            Code: string.Empty,
            Name: string.Empty,
            CommercialName: null,
            PartnerType: "Supplier",
            IdentificationTypeId: 0,
            IdentificationNumber: string.Empty,
            SupplierGroupId: null,
            SupplierClassId: null,
            EconomicActivityId: null,
            ZoneId: null,
            SupplyMethodId: null,
            Email: null,
            Phone: null,
            Website: null,
            Remarks: null,
            IsActive: true,
            TaxpayerTypeId: null,
            TaxRegimeId: null,
            FiscalCountryId: null,
            TaxpayerType: null,
            IsAccountingRequired: false,
            AppliesRetention: false,
            FiscalRegime: null,
            CountryCode: null,
            Province: null,
            City: null,
            CustomerAccountId: null,
            SupplierAccountId: null,
            CustomerAdvanceAccountId: null,
            SupplierAdvanceAccountId: null,
            RetentionAccountId: null,
            BranchId: null,
            DepartmentId: null,
            BusinessLineId: null,
            CostCenterId: null,
            ProjectId: null,
            CostCenterCode: null,
            DefaultExpenseAccountId: null,
            DifferenceAccountId: null,
            RoundingAccountId: null,
            ClearingAccountId: null,
            DiscountAccountId: null,
            AccountingBySupplier: false,
            RequiresProvision: false,
            AllowsAdvance: false,
            AllowsCompensation: false,
            AllowsPartialPayments: false,
            IsPaymentBlocked: false,
            UsesWithholdingBase: false,
            ConciliationRequired: false,
            AccountingPaymentMethodId: null,
            PaymentPriorityId: null,
            ApprovalFlowId: null,
            PaymentDocumentTypeId: null,
            AccountingPaymentMethod: null,
            PaymentPriority: null,
            RequiredPaymentDay: null,
            ApprovalFlow: null,
            PaymentDocumentType: null,
            AveragePaymentDays: 0,
            PaymentTolerancePercent: 0,
            PaymentTermId: null,
            CreditDays: 0,
            CreditLimit: 0,
            DeliveryDays: 0,
            MinimumOrderAmount: 0,
            AllowsBackorder: false,
            PreferredCurrencyCode: null,
            PriceListCode: null,
            AssignedSellerCode: null,
            AssignedBuyerCode: null,
            CreditStatus: "Normal",
            SapCardCode: null,
            SapCardType: "S",
            SapSyncStatus: "Pending",
            SapLastSyncAt: null,
            SapLastError: null,
            SapEnabled: false,
            SapMode: null,
            SapCompanyCode: null,
            SapRetryCount: 0,
            SyncAsSupplier: true,
            AllowManualSapRetry: false,
            RequiresApprovalBeforeSapSync: false,
            Addresses: Array.Empty<SaveBusinessPartnerAddressRequest>(),
            Contacts: Array.Empty<SaveBusinessPartnerContactRequest>(),
            BankAccounts: Array.Empty<SaveBusinessPartnerBankAccountRequest>(),
            RetentionSettings: Array.Empty<SaveBusinessPartnerRetentionSettingRequest>(),
            Notes: null,
            SapFieldMappings: Array.Empty<SaveBusinessPartnerSapFieldMappingRequest>(),
            Attachments: Array.Empty<SaveBusinessPartnerAttachmentRequest>());
    }

    private static BusinessPartnerLookups CreateDesignLookups()
    {
        return new BusinessPartnerLookups(
            new[] { new BusinessPartnerIdentificationTypeLookup(1, "RUC", "RUC", "EC") },
            new[] { new BusinessPartnerPaymentTermLookup(1, "30D", "Credito 30 dias", 30, true) },
            new[] { new BusinessPartnerLookupOption(1, "2101.01", "Cuentas por pagar proveedores") },
            new[] { new BusinessPartnerCodeNameLookup("Supplier", "Proveedor") },
            new[] { new BusinessPartnerCodeNameLookup("Active", "Activo") },
            new[] { new BusinessPartnerCodeNameLookup("Synced", "Sincronizado") },
            new[] { new BusinessPartnerLookupOption(1, "NAC", "Proveedores Nacionales") },
            new[] { new BusinessPartnerLookupOption(1, "MAT", "Materiales e Insumos") },
            new[] { new BusinessPartnerLookupOption(1, "COM", "Comercio al por mayor") },
            new[] { new BusinessPartnerLookupOption(1, "SIE", "Zona 1 - Sierra") },
            new[] { new BusinessPartnerLookupOption(1, "LOC", "Compra local") },
            new[] { new BusinessPartnerLookupOption(1, "COM", "Comercial") },
            new[] { new BusinessPartnerLookupOption(1, "EMAIL", "Correo electronico") },
            new[] { new BusinessPartnerLookupOption(1, "EC", "Ecuador") },
            new[] { new BusinessPartnerGeoLookupOption(1, "PIC", "Pichincha", true, 1) },
            new[] { new BusinessPartnerGeoLookupOption(1, "UIO", "Quito", true, 1, 1, "170135") },
            new[] { new BusinessPartnerLookupOption(1, "PICHINCHA", "Banco Pichincha") },
            new[] { new BusinessPartnerLookupOption(1, "CORRIENTE", "Corriente") },
            new[] { new BusinessPartnerLookupOption(1, "USD", "USD - Dolar Americano") },
            new[] { new BusinessPartnerLookupOption(1, "LP1", "Lista de precios 1") },
            new[] { new BusinessPartnerLookupOption(1, "MFORTIZ", "Maria Fernandez Ortiz") },
            new[] { new BusinessPartnerLookupOption(1, "GENERAL", "Regimen general") },
            new[] { new BusinessPartnerLookupOption(1, "SOCIEDAD", "Sociedad") },
            new[] { new BusinessPartnerLookupOption(1, "FUENTE", "Retencion Fuente") },
            new[] { new BusinessPartnerRetentionConceptLookup(1, "312", "Retencion Fuente 1.75%", true, "312", 1.75m, false, true, 1) },
            new[] { new BusinessPartnerLookupOption(1, "FACTURA", "Factura") },
            new[] { new BusinessPartnerLookupOption(1, "TRANSFER", "Transferencia bancaria") },
            new[] { new BusinessPartnerLookupOption(1, "NORMAL", "Normal") },
            new[] { new BusinessPartnerLookupOption(1, "GT5000", "Pagos > 5.000 requieren aprobacion") },
            new[] { new BusinessPartnerLookupOption(1, "PAYMENT", "Egreso proveedor") },
            new[] { new BusinessPartnerLookupOption(1, "01", "Matriz") },
            new[] { new BusinessPartnerLookupOption(1, "ADM", "Administracion") },
            new[] { new BusinessPartnerLookupOption(1, "COM", "Comercializacion") },
            new[] { new BusinessPartnerLookupOption(1, "CC-ADM-001", "Administracion general") },
            new[] { new BusinessPartnerLookupOption(1, "SINPROY", "Sin Proyecto") });
    }

    private sealed record LookupText(string Code, string Name);
}
