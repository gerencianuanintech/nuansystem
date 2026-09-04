using System.ComponentModel;
using System.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.BusinessPartners.Models;
using NuanSystem.WinForms.ViewModels.BusinessPartners.Suppliers;

namespace NuanSystem.WinForms.Forms.BusinessPartners;

public sealed partial class CustomerEditForm : BaseEditForm
{
    private readonly BusinessPartnerLookups lookups;
    private readonly BusinessPartnerItem? partner;
    private readonly BindingList<SupplierAddressViewModel> addresses = new();
    private readonly BindingList<SupplierContactViewModel> contacts = new();

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
        ConfigureChildDetailEditors();
        LoadPartner();
        LoadChildren();
        ApplyEditState();
        LoadSapTable();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveBusinessPartnerRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        return Validator.RequireText(txtCustomerName, "Nombre es requerido.")
            & Validator.RequireText(txtIdentificationNumber, "Identificacion es requerida.")
            & Validator.EmailIfPresent(txtEmail, "Correo no tiene un formato valido.")
            & RequireLookup(lueIdentificationType, "Tipo de identificacion es requerido.");
    }

    protected override void BuildRequest()
    {
        var proposed = new SaveBusinessPartnerRequest(
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
            LookupTextOrValue(lueTaxpayerType),
            tsAccountingRequired.IsOn,
            tsWithholdingAgent.IsOn || tsSubjectToWithholding.IsOn,
            LookupTextOrValue(lueFiscalRegime),
            LookupTextOrValue(lueFiscalCountry),
            LookupTextOrValue(lueFiscalProvince),
            LookupTextOrValue(lueFiscalCity),
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
            LookupTextOrValue(lueCostCenter),
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
            LookupTextOrValue(luePriceList),
            LookupTextOrValue(lueSalesPerson),
            null,
            tsCreditBlocked.IsOn ? "Blocked" : "Normal",
            NullIfEmpty(txtSapCardCode.Text),
            "C",
            LookupTextOrValue(lueSapStatus) ?? "Pending",
            null,
            null,
            false,
            null,
            null,
            0,
            false,
            false,
            false,
            SupplierBusinessPartnerMapper.ToAddressRequests(addresses, lookups),
            SupplierBusinessPartnerMapper.ToContactRequests(contacts),
            Array.Empty<SaveBusinessPartnerBankAccountRequest>(),
            Array.Empty<SaveBusinessPartnerRetentionSettingRequest>(),
            null,
            Array.Empty<SaveBusinessPartnerSapFieldMappingRequest>(),
            ExpectedRowVersion: partner is { Id: > 0 } ? partner.RowVersion : null);
        Request = SupplierBusinessPartnerMapper.ComposeCustomerRequest(
            proposed,
            partner,
            BusinessPartnerEditPolicy.From(lookups.EditPolicy));
    }

    private void WireEvents()
    {
        btnSave.Click += (_, _) => Save();
        btnAddAddress.Click += (_, _) => AddAddress();
        btnEditAddress.Click += (_, _) => EditSelectedAddress();
        btnDeleteAddress.Click += (_, _) => DeleteSelectedAddress();
        btnSetPrimaryAddress.Click += (_, _) => SetPrimaryAddress();
        grvCustomerAddresses.DoubleClick += (_, _) => EditSelectedAddress();
        grvCustomerAddresses.FocusedRowChanged += (_, _) => ShowSelectedAddress();
        btnAddContact.Click += (_, _) => AddContact();
        btnEditContact.Click += (_, _) => EditSelectedContact();
        btnDeleteContact.Click += (_, _) => DeleteSelectedContact();
        grvCustomerContactList.DoubleClick += (_, _) => EditSelectedContact();
        grvCustomerContactList.FocusedRowChanged += (_, _) => ShowSelectedContact();
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
        grdCustomerAddresses.DataSource = addresses;
        grdCustomerContactList.DataSource = contacts;
        grdCustomerContacts.DataSource = contacts;
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
        lueTaxpayerType.EditValue = partner.TaxpayerType;
        tsAccountingRequired.IsOn = partner.IsAccountingRequired;
        tsWithholdingAgent.IsOn = partner.AppliesRetention;
        tsSubjectToWithholding.IsOn = partner.AppliesRetention;
        lueFiscalRegime.EditValue = partner.FiscalRegime;
        lueFiscalCountry.EditValue = partner.CountryCode;
        lueFiscalProvince.EditValue = partner.Province;
        lueFiscalCity.EditValue = partner.City;
        luePaymentTerm.EditValue = partner.PaymentTermId;
        spnCreditLimit.Value = partner.CreditLimit;
        luePriceList.EditValue = partner.PriceListCode;
        lueSalesPerson.EditValue = partner.AssignedSellerCode;
        tsCreditBlocked.IsOn = string.Equals(partner.CreditStatus, "Blocked", StringComparison.OrdinalIgnoreCase);
        sluReceivableAccount.EditValue = partner.CustomerAccountId;
        sluCustomerAdvanceAccount.EditValue = partner.CustomerAdvanceAccountId;
        sluIncomeWithholding.EditValue = partner.RetentionAccountId;
        lueCostCenter.EditValue = partner.CostCenterCode;
        txtSapCardCode.Text = partner.SapCardCode;
        lueSapStatus.EditValue = partner.SapSyncStatus;
        lueStatus.EditValue = partner.IsActive ? "Activo" : "Inactivo";
    }

    private void ApplyEditState()
    {
        var editPolicy = BusinessPartnerEditPolicy.From(lookups.EditPolicy);
        var state = BusinessPartnerFormEditStatePolicy.Evaluate(partner, editPolicy);

        txtCustomerCode.Text = state.CodeText;
        txtCustomerCode.Properties.ReadOnly = true;
        txtCustomerCode.Properties.NullValuePrompt = state.CodeHint;
        txtCustomerCode.Properties.ShowNullValuePromptWhenFocused = true;
        lueIdentificationType.Properties.ReadOnly = !state.IdentificationEditable;
        txtIdentificationNumber.Properties.ReadOnly = !state.IdentificationEditable;
        btnSave.Enabled = state.CanSave;

        lblMasterSyncStatus.Text = state.SyncPresentation.Caption;
        ApplySyncBadge(lblMasterSyncStatus, state.SyncPresentation.BadgeKind);
        lblMasterSyncMessage.Text = state.SyncPresentation.Message;
        lblMasterSyncMessage.Visible = !string.IsNullOrWhiteSpace(state.SyncPresentation.Message);

        if (!editPolicy.IsSyncedBranch)
        {
            return;
        }

        SetReadOnly(grpGeneralInfo, true);
        SetReadOnly(grpClassification, true);
        foreach (var page in new[] { xtpGeneral, xtpFiscal, xtpCommercial, xtpAccounting, xtpSap })
        {
            page.PageEnabled = false;
        }

        lueIdentificationType.Properties.ReadOnly = !state.IdentificationEditable;
        txtIdentificationNumber.Properties.ReadOnly = !state.IdentificationEditable;
        txtCustomerName.Properties.ReadOnly = !state.NameEditable;
        txtCustomerCommercialName.Properties.ReadOnly = !state.CommercialNameEditable;
        txtPhone.Properties.ReadOnly = !state.PhoneEditable;
        txtEmail.Properties.ReadOnly = !state.EmailEditable;
        xtpAddresses.PageEnabled = state.AddressesEditable;
        xtpContacts.PageEnabled = state.ContactsEditable;
        SetAddressActionsEnabled(state.AddressesEditable && state.CanSave);
        SetContactActionsEnabled(state.ContactsEditable && state.CanSave);
    }

    private void ConfigureChildDetailEditors()
    {
        foreach (var editor in new BaseEdit[]
        {
            lueAddressType,
            memAddress,
            lueAddressCountry,
            lueAddressProvince,
            lueAddressCity,
            txtPostalCode,
            txtAddressReference,
            tsPrimaryAddress
        })
        {
            editor.Properties.ReadOnly = true;
        }
    }

    private void SetAddressActionsEnabled(bool enabled)
    {
        btnAddAddress.Enabled = enabled;
        btnEditAddress.Enabled = enabled;
        btnDeleteAddress.Enabled = enabled;
        btnSetPrimaryAddress.Enabled = enabled;
    }

    private void SetContactActionsEnabled(bool enabled)
    {
        btnAddContact.Enabled = enabled;
        btnEditContact.Enabled = enabled;
        btnDeleteContact.Enabled = enabled;
    }

    private static void SetReadOnly(Control parent, bool readOnly)
    {
        foreach (Control control in parent.Controls)
        {
            if (control is BaseEdit editor)
            {
                editor.Properties.ReadOnly = readOnly;
            }

            if (control.HasChildren)
            {
                SetReadOnly(control, readOnly);
            }
        }
    }

    private static void ApplySyncBadge(LabelControl label, string badgeKind)
    {
        (label.Appearance.BackColor, label.Appearance.ForeColor) = badgeKind switch
        {
            "Success" => (Color.FromArgb(220, 252, 231), Color.FromArgb(22, 101, 52)),
            "Warning" => (Color.FromArgb(254, 243, 199), Color.FromArgb(146, 64, 14)),
            _ => (Color.FromArgb(254, 226, 226), Color.FromArgb(153, 27, 27))
        };
        label.Appearance.Options.UseBackColor = true;
        label.Appearance.Options.UseForeColor = true;
    }

    private void LoadChildren()
    {
        addresses.Clear();
        foreach (var address in SupplierBusinessPartnerMapper.ToAddressViewModels(partner))
        {
            addresses.Add(address);
        }

        contacts.Clear();
        foreach (var contact in SupplierBusinessPartnerMapper.ToContactViewModels(partner, lookups))
        {
            contacts.Add(contact);
        }

        ShowSelectedAddress();
        ShowSelectedContact();
    }

    private void LoadSapTable()
    {
        grdCustomerSapLog.DataSource = CreateTable(
            ("FechaHora", typeof(DateTime)),
            ("Evento", typeof(string)),
            ("Descripcion", typeof(string)),
            ("Usuario", typeof(string)),
            ("Resultado", typeof(string)));
    }

    private void AddAddress()
    {
        using var dialog = new SupplierAddressEditDialog(CreateAddressCode());
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var address = dialog.Address.Clone();
        address.IsPrimary = addresses.Count == 0;
        addresses.Add(address);
        RefreshAddressData();
    }

    private void EditSelectedAddress()
    {
        var address = SelectedAddress();
        if (address is null)
        {
            ShowSelectionRequired("una direccion");
            return;
        }

        using var dialog = new SupplierAddressEditDialog(address);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var edited = SupplierBusinessPartnerMapper.ComposeCustomerAddressEditResult(
            address,
            dialog.Address);
        address.CopyFrom(edited);
        RefreshAddressData();
    }

    private void DeleteSelectedAddress()
    {
        var address = SelectedAddress();
        if (address is null)
        {
            ShowSelectionRequired("una direccion");
            return;
        }

        if (ConfirmDelete($"la direccion {address.Code}"))
        {
            addresses.Remove(address);
            RefreshAddressData();
        }
    }

    private void SetPrimaryAddress()
    {
        var address = SelectedAddress();
        if (address is null)
        {
            ShowSelectionRequired("una direccion");
            return;
        }

        foreach (var item in addresses)
        {
            item.IsPrimary = item.Id == address.Id;
        }

        RefreshAddressData();
    }

    private void AddContact()
    {
        using var dialog = new SupplierContactEditDialog(lookups.ContactTypes, lookups.ContactChannels);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var contact = dialog.Contact;
        ApplyPrimaryContact(contact);
        contacts.Add(contact);
        RefreshContactData();
    }

    private void EditSelectedContact()
    {
        var contact = SelectedContact();
        if (contact is null)
        {
            ShowSelectionRequired("un contacto");
            return;
        }

        using var dialog = new SupplierContactEditDialog(contact, lookups.ContactTypes, lookups.ContactChannels);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var edited = dialog.Contact;
        ApplyPrimaryContact(edited, contact.Id);
        contact.CopyFrom(edited);
        RefreshContactData();
    }

    private void DeleteSelectedContact()
    {
        var contact = SelectedContact();
        if (contact is null)
        {
            ShowSelectionRequired("un contacto");
            return;
        }

        if (ConfirmDelete($"el contacto {contact.FullName}"))
        {
            contacts.Remove(contact);
            RefreshContactData();
        }
    }

    private SupplierAddressViewModel? SelectedAddress() =>
        grvCustomerAddresses.GetFocusedRow() as SupplierAddressViewModel;

    private SupplierContactViewModel? SelectedContact() =>
        grvCustomerContactList.GetFocusedRow() as SupplierContactViewModel;

    private void ApplyPrimaryContact(SupplierContactViewModel contact, Guid? exceptId = null)
    {
        if (!contact.IsPrimary)
        {
            return;
        }

        foreach (var item in contacts.Where(item => !exceptId.HasValue || item.Id != exceptId.Value))
        {
            item.IsPrimary = false;
        }
    }

    private void RefreshAddressData()
    {
        grdCustomerAddresses.RefreshDataSource();
        grvCustomerAddresses.RefreshData();
        ShowSelectedAddress();
    }

    private void RefreshContactData()
    {
        grdCustomerContactList.RefreshDataSource();
        grdCustomerContacts.RefreshDataSource();
        grvCustomerContactList.RefreshData();
        ShowSelectedContact();
    }

    private void ShowSelectedAddress()
    {
        var address = SelectedAddress();
        lueAddressType.EditValue = address?.AddressType;
        memAddress.Text = address?.FullAddress ?? string.Empty;
        lueAddressCountry.EditValue = address?.Country;
        lueAddressProvince.EditValue = address?.Province;
        lueAddressCity.EditValue = address?.City;
        txtPostalCode.Text = address?.PostalCode ?? string.Empty;
        txtAddressReference.Text = address?.Reference ?? string.Empty;
        tsPrimaryAddress.IsOn = address?.IsPrimary == true;
    }

    private void ShowSelectedContact()
    {
        var detail = SupplierBusinessPartnerMapper.ToCustomerContactDetail(SelectedContact());
        txtContactName.Text = detail.Name;
        txtContactPosition.Text = detail.Position;
        txtContactPhone.Text = detail.Phone;
        txtContactMobile.Text = detail.Mobile;
        txtContactEmail.Text = detail.Email;
        tsPrimaryContact.IsOn = detail.IsPrimary;
        tsActiveContact.IsOn = detail.IsActive;
        memContactNotes.Text = detail.Notes;
    }

    private string CreateAddressCode()
    {
        var next = addresses.Count + 1;
        string code;
        do
        {
            code = $"DIR-{next++:000}";
        }
        while (addresses.Any(item => string.Equals(item.Code, code, StringComparison.OrdinalIgnoreCase)));

        return code;
    }

    private void ShowSelectionRequired(string item) =>
        XtraMessageBox.Show(this, $"Seleccione {item}.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

    private bool ConfirmDelete(string item) =>
        XtraMessageBox.Show(this, $"Desea eliminar {item}?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        == DialogResult.Yes;

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

    private static string? LookupTextOrValue(BaseEdit editor)
    {
        return NullIfEmpty(editor.Text) ?? NullIfEmpty(Convert.ToString(editor.EditValue));
    }

    private static SaveBusinessPartnerRequest EmptyRequest()
    {
        return new SaveBusinessPartnerRequest(
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
            SapFieldMappings: Array.Empty<SaveBusinessPartnerSapFieldMappingRequest>(),
            ExpectedRowVersion: null);
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
