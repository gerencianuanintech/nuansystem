using System.ComponentModel;
using System.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.BusinessPartners.Models;

namespace NuanSystem.WinForms.Forms.BusinessPartners;

public sealed partial class CustomerEditForm : BaseEditForm
{
    private readonly BusinessPartnerLookups lookups;
    private readonly BusinessPartnerItem? partner;

    public CustomerEditForm()
        : this(null, CreateDesignLookups())
    {
    }

    public CustomerEditForm(BusinessPartnerItem? partner, BusinessPartnerLookups lookups)
    {
        this.partner = partner;
        this.lookups = lookups;
        InitializeComponent();
        FormStyler.ApplyPanelInheritedBackColor(this);
        WireEvents();
        BindLookups();
        LoadPartner();
        LoadDemoTables();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveBusinessPartnerRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        return Validator.RequireText(txtCustomerCode, "Codigo es requerido.")
            & Validator.RequireText(txtCustomerName, "Nombre es requerido.")
            & Validator.RequireText(txtIdentificationNumber, "Identificacion es requerida.")
            & Validator.EmailIfPresent(txtEmail, "Correo no tiene un formato valido.")
            & RequireLookup(lueIdentificationType, "Tipo de identificacion es requerido.");
    }

    protected override void BuildRequest()
    {
        Request = new SaveBusinessPartnerRequest(
            txtCustomerCode.Text.Trim(),
            txtCustomerName.Text.Trim(),
            NullIfEmpty(txtCustomerCommercialName.Text),
            "Customer",
            Convert.ToInt32(lueIdentificationType.EditValue),
            txtIdentificationNumber.Text.Trim(),
            null,
            null,
            null,
            null,
            null,
            NullIfEmpty(txtEmail.Text),
            NullIfEmpty(txtPhone.Text),
            null,
            NullIfEmpty(memObservations.Text),
            string.Equals(lueStatus.Text, "Activo", StringComparison.OrdinalIgnoreCase),
            null,
            null,
            null,
            NullIfEmpty(lueTaxpayerType.Text),
            tsAccountingRequired.IsOn,
            tsWithholdingAgent.IsOn || tsSubjectToWithholding.IsOn,
            NullIfEmpty(lueFiscalRegime.Text),
            NullIfEmpty(lueFiscalCountry.Text),
            NullIfEmpty(lueFiscalProvince.Text),
            NullIfEmpty(lueFiscalCity.Text),
            ToNullableInt(sluReceivableAccount.EditValue),
            null,
            ToNullableInt(sluCustomerAdvanceAccount.EditValue),
            null,
            ToNullableInt(sluIncomeWithholding.EditValue),
            null,
            null,
            null,
            null,
            null,
            NullIfEmpty(lueCostCenter.Text),
            null,
            null,
            null,
            null,
            null,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            0,
            0,
            ToNullableInt(luePaymentTerm.EditValue),
            0,
            spnCreditLimit.Value,
            0,
            0,
            false,
            null,
            NullIfEmpty(luePriceList.Text),
            NullIfEmpty(lueSalesPerson.Text),
            null,
            "Normal",
            NullIfEmpty(txtSapCardCode.Text),
            "C",
            NullIfEmpty(lueSapStatus.Text) ?? "Pending",
            null,
            null,
            false,
            null,
            null,
            0,
            false,
            false,
            false,
            Array.Empty<SaveBusinessPartnerAddressRequest>(),
            Array.Empty<SaveBusinessPartnerContactRequest>(),
            Array.Empty<SaveBusinessPartnerBankAccountRequest>(),
            Array.Empty<SaveBusinessPartnerRetentionSettingRequest>(),
            null,
            Array.Empty<SaveBusinessPartnerSapFieldMappingRequest>());
    }

    private void WireEvents()
    {
        btnSave.Click += (_, _) => Save();
    }

    private void BindLookups()
    {
        BindIdentificationTypes();
        BindPaymentTerms();
        BindAccountLookup(sluReceivableAccount, grvReceivableAccountLookup);
        BindAccountLookup(sluCustomerAdvanceAccount, grvCustomerAdvanceAccountLookup);
        BindAccountLookup(sluDiscountAccount, grvDiscountAccountLookup);
        BindAccountLookup(sluInterestAccount, grvInterestAccountLookup);
        BindAccountLookup(sluIncomeWithholding, grvIncomeWithholdingLookup);
        BindAccountLookup(sluVatWithholding, grvVatWithholdingLookup);
        AddItems(lueStatus, "Activo", "Inactivo");
        AddItems(lueCustomerType, "Empresa", "Persona natural", "Gobierno");
        AddSearchItems(lueCustomerGroup, grvCustomerGroupLookup, "Mayoristas", "Minoristas", "Distribuidores");
        AddSearchItems(lueSalesPerson, grvSalesPersonLookup, "Maria Fernanda Lopez", "Carlos Perez", "Ana Rivera");
        AddItems(luePriceList, "Lista Mayorista", "Lista Minorista", "Lista Especial");
        AddItems(lueCurrency, "Soles (PEN)", "USD - Dolar estadounidense", "Moneda local");
        AddItems(lueChannel, "Distribuidores", "Retail", "Online");
        AddItems(lueZone, "Lima Metropolitana", "Norte", "Sur");
        AddItems(lueRiskLevel, "Bajo", "Medio", "Alto");
        AddItems(lueTaxpayerType, "Empresa", "Sociedad", "Persona natural");
        AddItems(lueFiscalRegime, "Regimen General", "Regimen Especial");
        AddItems(lueFiscalCountry, "Peru", "Ecuador", "Colombia");
        AddItems(lueFiscalProvince, "Lima", "Pichincha", "Guayas");
        AddItems(lueFiscalCity, "San Isidro", "Quito", "Guayaquil");
        AddItems(lueRentType, "Renta de Cuarta Categoria", "Bienes", "Servicios");
        AddItems(lueEmissionType, "Electronica", "Fisica");
        AddItems(luePrintFormat, "Factura - Estandar", "Formato resumido");
        AddItems(lueSegment, "Consumo masivo", "Institucional", "Distribucion");
        AddItems(lueInternalClassification, "A", "B", "C");
        AddItems(lueSapGroup, "MAY - Mayoristas", "MIN - Minoristas");
        AddItems(lueSapPaymentTerm, "30 - 30 Dias", "60 - 60 Dias");
        AddItems(lueSapCurrency, "PEN - Soles", "USD - Dolares");
        AddItems(lueSapStatus, "Pending", "Synced", "Error");
        AddItems(lueAccountingCurrency, "USD - Dolar estadounidense", "PEN - Soles");
        AddItems(lueValidationStatus, "Valido", "Pendiente", "Observado");
    }

    private void LoadPartner()
    {
        lueStatus.EditValue = "Activo";
        lueCustomerType.EditValue = "Empresa";
        lueSapStatus.EditValue = "Pending";
        tsAllowSales.IsOn = true;
        tsTaxExempt.IsOn = false;
        tsStrategicCustomer.IsOn = false;

        if (partner is null)
        {
            return;
        }

        txtCustomerCode.Text = partner.Code;
        txtCustomerName.Text = partner.Name;
        txtCustomerCommercialName.Text = partner.CommercialName;
        lueIdentificationType.EditValue = partner.IdentificationTypeId;
        txtIdentificationNumber.Text = partner.IdentificationNumber;
        txtPhone.Text = partner.Phone;
        txtEmail.Text = partner.Email;
        memObservations.Text = partner.Remarks;
        luePaymentTerm.EditValue = partner.PaymentTermId;
        spnCreditLimit.Value = partner.CreditLimit;
        lueSalesPerson.EditValue = partner.AssignedSellerCode;
        sluReceivableAccount.EditValue = partner.CustomerAccountId;
        txtSapCardCode.Text = partner.SapCardCode;
        lueSapStatus.EditValue = partner.SapSyncStatus;
        lueStatus.EditValue = partner.IsActive ? "Activo" : "Inactivo";
    }

    private void LoadDemoTables()
    {
        grdCustomerContacts.DataSource = CreateTable(
            ("Nombre", typeof(string)),
            ("Cargo", typeof(string)),
            ("Telefono", typeof(string)),
            ("Correo", typeof(string)),
            ("DireccionPrincipal", typeof(string)),
            ("EsPrincipal", typeof(bool)));

        grdCustomerAddresses.DataSource = CreateTable(
            ("TipoDireccion", typeof(string)),
            ("Direccion", typeof(string)),
            ("Ciudad", typeof(string)),
            ("Provincia", typeof(string)),
            ("Principal", typeof(bool)));

        grdCustomerSapLog.DataSource = CreateTable(
            ("FechaHora", typeof(DateTime)),
            ("Evento", typeof(string)),
            ("Descripcion", typeof(string)),
            ("Usuario", typeof(string)),
            ("Resultado", typeof(string)));
    }

    private void BindIdentificationTypes()
    {
        lueIdentificationType.Properties.DataSource = lookups.IdentificationTypes.ToList();
        lueIdentificationType.Properties.DisplayMember = nameof(BusinessPartnerIdentificationTypeLookup.Name);
        lueIdentificationType.Properties.ValueMember = nameof(BusinessPartnerIdentificationTypeLookup.Id);
        lueIdentificationType.Properties.Columns.Clear();
        lueIdentificationType.Properties.Columns.Add(new LookUpColumnInfo(nameof(BusinessPartnerIdentificationTypeLookup.Code), "Codigo", 80));
        lueIdentificationType.Properties.Columns.Add(new LookUpColumnInfo(nameof(BusinessPartnerIdentificationTypeLookup.Name), "Nombre", 180));
    }

    private void BindPaymentTerms()
    {
        luePaymentTerm.Properties.DataSource = lookups.PaymentTerms.ToList();
        luePaymentTerm.Properties.DisplayMember = nameof(BusinessPartnerPaymentTermLookup.Name);
        luePaymentTerm.Properties.ValueMember = nameof(BusinessPartnerPaymentTermLookup.Id);
        luePaymentTerm.Properties.Columns.Clear();
        luePaymentTerm.Properties.Columns.Add(new LookUpColumnInfo(nameof(BusinessPartnerPaymentTermLookup.Code), "Codigo", 80));
        luePaymentTerm.Properties.Columns.Add(new LookUpColumnInfo(nameof(BusinessPartnerPaymentTermLookup.Name), "Nombre", 180));
    }

    private void BindAccountLookup(SearchLookUpEdit lookup, GridView? view)
    {
        view ??= lookup.Properties.PopupView as GridView ?? new GridView();
        lookup.Properties.DataSource = lookups.Accounts.ToList();
        lookup.Properties.DisplayMember = nameof(BusinessPartnerLookupOption.Name);
        lookup.Properties.ValueMember = nameof(BusinessPartnerLookupOption.Id);
        lookup.Properties.PopupView = view;
        view.Columns.Clear();
        view.Columns.AddVisible(nameof(BusinessPartnerLookupOption.Code), "Codigo").Width = 110;
        view.Columns.AddVisible(nameof(BusinessPartnerLookupOption.Name), "Nombre").Width = 220;
    }

    private static void ConfigureLookupColumn(GridView view, string fieldName, string? caption = null, int visibleIndex = 0, int width = 100, bool visible = true)
    {
        if (view.Columns[fieldName] is not { } column)
        {
            return;
        }

        column.Visible = visible;
        if (!visible)
        {
            return;
        }

        column.Caption = caption ?? fieldName;
        column.VisibleIndex = visibleIndex;
        column.Width = width;
    }

    private static void AddItems(LookUpEdit lookup, params string[] items)
    {
        lookup.Properties.DataSource = items.Select(item => new LookupText(item)).ToList();
        lookup.Properties.DisplayMember = nameof(LookupText.Name);
        lookup.Properties.ValueMember = nameof(LookupText.Name);
        lookup.Properties.Columns.Clear();
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(LookupText.Name), "Nombre", 180));
    }

    private static void AddSearchItems(SearchLookUpEdit lookup, GridView view, params string[] items)
    {
        var source = items.Select((item, index) => new LookupSearchText(index + 1, item, item)).ToList();
        lookup.Properties.DataSource = source;
        lookup.Properties.DisplayMember = nameof(LookupSearchText.Name);
        lookup.Properties.ValueMember = nameof(LookupSearchText.Code);
        lookup.Properties.PopupView = view;
        view.Columns.Clear();
        view.Columns.AddVisible(nameof(LookupSearchText.Code), "Codigo").Width = 110;
        view.Columns.AddVisible(nameof(LookupSearchText.Name), "Nombre").Width = 220;
    }

    private static DataTable CreateTable(params (string Name, Type Type)[] columns)
    {
        var table = new DataTable();
        foreach (var column in columns)
        {
            table.Columns.Add(column.Name, column.Type);
        }

        return table;
    }

    private static int? ToNullableInt(object? value)
    {
        return value is null || value == DBNull.Value ? null : Convert.ToInt32(value);
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
            PartnerType: "Customer",
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
            SapCardType: "C",
            SapSyncStatus: "Pending",
            SapLastSyncAt: null,
            SapLastError: null,
            SapEnabled: false,
            SapMode: null,
            SapCompanyCode: null,
            SapRetryCount: 0,
            SyncAsSupplier: false,
            AllowManualSapRetry: false,
            RequiresApprovalBeforeSapSync: false,
            Addresses: Array.Empty<SaveBusinessPartnerAddressRequest>(),
            Contacts: Array.Empty<SaveBusinessPartnerContactRequest>(),
            BankAccounts: Array.Empty<SaveBusinessPartnerBankAccountRequest>(),
            RetentionSettings: Array.Empty<SaveBusinessPartnerRetentionSettingRequest>(),
            Notes: null,
            SapFieldMappings: Array.Empty<SaveBusinessPartnerSapFieldMappingRequest>());
    }

    private static BusinessPartnerLookups CreateDesignLookups()
    {
        return new BusinessPartnerLookups(
            new[] { new BusinessPartnerIdentificationTypeLookup(1, "RUC", "RUC", "PE") },
            new[] { new BusinessPartnerPaymentTermLookup(1, "30D", "30 Dias", 30, true) },
            new[] { new BusinessPartnerLookupOption(1, "1201.01", "Cuentas por cobrar clientes") },
            new[] { new BusinessPartnerCodeNameLookup("Customer", "Cliente") },
            new[] { new BusinessPartnerCodeNameLookup("Active", "Activo") },
            new[] { new BusinessPartnerCodeNameLookup("Pending", "Pendiente") },
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerGeoLookupOption>(),
            Array.Empty<BusinessPartnerGeoLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerRetentionConceptLookup>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>(),
            Array.Empty<BusinessPartnerLookupOption>());
    }

    private sealed record LookupText(string Name);

    private sealed record LookupSearchText(int Id, string Code, string Name);
}
