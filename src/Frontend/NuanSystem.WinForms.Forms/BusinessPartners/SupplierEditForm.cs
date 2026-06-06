using System.ComponentModel;
using System.IO;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.BusinessPartners.Models;
using NuanSystem.WinForms.Services.Geography;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.BusinessPartners.Suppliers;

namespace NuanSystem.WinForms.Forms.BusinessPartners;

public sealed partial class SupplierEditForm : BaseEditForm
{
    private BusinessPartnerLookups lookups;
    private readonly BusinessPartnerItem? partner;
    private readonly BindingList<SupplierContactViewModel> contacts = new();
    private readonly BindingList<SupplierAddressViewModel> addresses = new();
    private readonly BindingList<SupplierPurchaseHistoryViewModel> purchaseHistory = new();
    private readonly BindingList<SupplierBankAccountViewModel> bankAccounts = new();
    private readonly BindingList<SupplierWithholdingViewModel> withholdings = new();
    private readonly BindingList<SupplierAccountingAccountViewModel> accountingAccounts = new();
    private readonly BindingList<SupplierSapAuditViewModel> sapAudit = new();
    private readonly BindingList<SupplierAttachmentViewModel> attachments = new();

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

        InitializeComponent();

        if (IsDesignerHosted())
        {
            return;
        }

        BindLookups();
        LoadPartner();
        LoadContacts();
        LoadAddresses();
        LoadPurchaseSettings();
        LoadPurchaseHistory();
        LoadBankAccounts();
        LoadWithholdingSettings();
        LoadWithholdings();
        LoadAccountingSettings();
        LoadAccountingAccounts();
        LoadSapSyncData();
        LoadSapAudit();
        LoadAttachments();
        WireContactEvents();
        WireAddressEvents();
        WireBankAccountEvents();
        WireWithholdingEvents();
        WireAccountingAccountEvents();
        WireAttachmentEvents();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveBusinessPartnerRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        return Validator.RequireText(txtSupplierCode, "Código es requerido.")
            & Validator.RequireText(txtBusinessName, "Razón social es requerida.")
            & Validator.RequireText(txtDocumentNumber, "RUC / Identificación es requerido.")
            & RequireLookup(lueDocumentType, "Tipo de documento es requerido.");
    }

    protected override void BuildRequest()
    {
        Request = EmptyRequest() with
        {
            Code = txtSupplierCode.Text.Trim(),
            Name = txtBusinessName.Text.Trim(),
            CommercialName = NullIfEmpty(txtTradeName.Text),
            IdentificationTypeId = ToInt(lueDocumentType.EditValue),
            IdentificationNumber = txtDocumentNumber.Text.Trim(),
            SupplierGroupId = ToNullableInt(lueSupplierCategory.EditValue),
            Email = NullIfEmpty(txtEmail.Text),
            Phone = NullIfEmpty(txtPhone.Text),
            Remarks = NullIfEmpty(memShortObservation.Text),
            IsActive = tglSupplierActive.IsOn,
            PreferredCurrencyCode = LookupCode(lueCurrency)
        };
    }

    private void BindLookups()
    {
        BindLookup(lueDocumentType, lookups.IdentificationTypes.Select(x => new BusinessPartnerLookupOption(x.Id, x.Code, x.Name)).ToList());
        BindLookup(luePersonType, "Jurídica", "Natural");
        BindLookup(lueSupplierType, "Bienes", "Servicios", "Bienes y Servicios");
        BindLookup(lueCurrency, lookups.Currencies);
        BindLookup(lueSupplierCategory, lookups.SupplierGroups);
        BindLookup(lueCountry, lookups.Countries);
        BindLookup(lueInternalClassification, "PROV. NACIONALES", "PROV. EXTRANJEROS", "PROV. SERVICIOS");
        BindLookup(lueSupplierSegment, "A - Proveedores Estratégicos", "B - Proveedores Regulares", "C - Proveedores Eventuales");
        grdContacts.DataSource = contacts;
        grdAddresses.DataSource = addresses;
        grdPurchaseHistory.DataSource = purchaseHistory;
        grdBankAccounts.DataSource = bankAccounts;
        grdWithholdings.DataSource = withholdings;
        grdAccountingAccounts.DataSource = accountingAccounts;
        grdSapAudit.DataSource = sapAudit;
        grdAttachments.DataSource = attachments;
    }

    private void LoadPartner()
    {
        Text = "Mantenimiento de Proveedores";
        txtSupplierCode.Text = partner?.Code ?? "P001";
        txtBusinessName.Text = partner?.Name ?? "ACME S.A.C.";
        txtTradeName.Text = partner?.CommercialName ?? "ACME";
        txtDocumentNumber.Text = partner?.IdentificationNumber ?? "20123456789";
        txtMainContact.Text = partner?.Contacts?.FirstOrDefault(x => x.IsPrimary)?.Name ?? "Carlos Alberto Ramírez Flores";
        txtPhone.Text = partner?.Phone ?? "(01) 123-4567";
        txtEmail.Text = partner?.Email ?? "ventas@acme.com.pe";
        memShortObservation.Text = partner?.Remarks ?? "Proveedor de repuestos y suministros industriales.";
        tglSupplierActive.IsOn = partner?.IsActive ?? true;
        txtProvinceCity.Text = string.IsNullOrWhiteSpace(partner?.Province) && string.IsNullOrWhiteSpace(partner?.City)
            ? "Lima / Lima"
            : $"{partner?.Province} / {partner?.City}".Trim(' ', '/');
        txtWebsite.Text = partner?.Website ?? "www.acme.com.pe";
        dteRegistrationDate.EditValue = partner?.CreatedAt == default ? new DateTime(2022, 3, 15) : partner?.CreatedAt;
        spnCreditLimit.Value = partner?.CreditLimit > 0 ? partner.CreditLimit : 50000m;
        spnPaymentTermDays.Value = partner?.CreditDays > 0 ? partner.CreditDays : 30m;
        tglActiveForPurchases.IsOn = true;
        tglSubjectToWithholding.IsOn = partner?.AppliesRetention ?? true;
        tglHandlesCredit.IsOn = true;
        tglBlocked.IsOn = false;
        memGeneralComments.Text = "Proveedor estratégico para compras recurrentes de repuestos, suministros industriales y materiales operativos.";

        SetEditValue(lueDocumentType, partner?.IdentificationTypeId ?? lookups.IdentificationTypes.FirstOrDefault()?.Id);
        SetEditValue(lueSupplierCategory, partner?.SupplierGroupId ?? lookups.SupplierGroups.FirstOrDefault()?.Id);
        SetEditValue(lueCurrency, LookupValueByCode(lookups.Currencies, partner?.PreferredCurrencyCode) ?? lookups.Currencies.FirstOrDefault()?.Id);
        SetEditValue(lueCountry, LookupValueByCode(lookups.Countries, partner?.CountryCode) ?? lookups.Countries.FirstOrDefault()?.Id);
        luePersonType.EditValue = "Jurídica";
        lueSupplierType.EditValue = "Bienes";
        lueInternalClassification.EditValue = "PROV. NACIONALES";
        lueSupplierSegment.EditValue = "A - Proveedores Estratégicos";
    }

    private void LoadContacts()
    {
        contacts.Clear();

        if (partner?.Contacts?.Count > 0)
        {
            foreach (var contact in partner.Contacts)
            {
                var names = SplitContactName(contact.Name);
                contacts.Add(new SupplierContactViewModel
                {
                    FirstName = names.FirstName,
                    LastName = names.LastName,
                    Position = contact.Position ?? string.Empty,
                    Department = contact.Department ?? string.Empty,
                    Phone = contact.Phone ?? string.Empty,
                    Mobile = contact.Mobile ?? string.Empty,
                    Email = contact.Email ?? string.Empty,
                    IsPrimary = contact.IsPrimary,
                    IsActive = contact.IsActive,
                    Notes = contact.Notes ?? string.Empty
                });
            }

            return;
        }

        contacts.Add(new SupplierContactViewModel { FirstName = "Carlos Alberto", LastName = "Ramírez Flores", Position = "Gerente Comercial", Department = "Comercial", Phone = "(01) 123-4567", Email = "ventas@acme.com.pe", IsPrimary = true, IsActive = true });
        contacts.Add(new SupplierContactViewModel { FirstName = "María Fernanda", LastName = "López Díaz", Position = "Jefa de Ventas", Department = "Ventas", Phone = "(01) 123-4570", Email = "mlopez@acme.com.pe", IsActive = true });
        contacts.Add(new SupplierContactViewModel { FirstName = "José Luis", LastName = "Torres Silva", Position = "Ejecutivo de Cuentas", Department = "Ventas", Phone = "(01) 123-4571", Email = "jtorres@acme.com.pe", IsActive = true });
        contacts.Add(new SupplierContactViewModel { FirstName = "Ana Lucía", LastName = "Vega Paredes", Position = "Coordinadora de Logística", Department = "Logística", Phone = "(01) 123-4572", Email = "avega@acme.com.pe", IsActive = true });
        contacts.Add(new SupplierContactViewModel { FirstName = "Luis Enrique", LastName = "Mendoza Ramos", Position = "Jefe de Compras", Department = "Compras", Phone = "(01) 123-4573", Email = "lmendoza@acme.com.pe", IsActive = true });
    }

    private void WireContactEvents()
    {
        btnAddContact.Click += (_, _) => AddContact();
        btnEditContact.Click += (_, _) => EditSelectedContact();
        btnDeleteContact.Click += (_, _) => DeleteSelectedContact();
        btnSetDefaultContact.Click += (_, _) => SetDefaultContact();
        gvContacts.DoubleClick += (_, _) => EditSelectedContact();
    }

    private void LoadAddresses()
    {
        addresses.Clear();

        if (partner?.Addresses?.Count > 0)
        {
            var index = 1;
            foreach (var address in partner.Addresses)
            {
                addresses.Add(new SupplierAddressViewModel
                {
                    AddressType = address.AddressType,
                    Code = $"DIR-{index:000}",
                    MainStreet = address.Line1,
                    SecondaryStreet = address.Line2 ?? string.Empty,
                    Province = address.Province ?? string.Empty,
                    City = address.City ?? string.Empty,
                    Country = address.CountryCode ?? string.Empty,
                    PostalCode = address.PostalCode ?? string.Empty,
                    Latitude = address.Latitude,
                    Longitude = address.Longitude,
                    Reference = address.Line2 ?? string.Empty,
                    IsPrimary = address.IsPrimary,
                    IsDefaultDelivery = address.IsPrimary,
                    IsActive = address.IsActive
                });
                index++;
            }

            return;
        }

        addresses.Add(new SupplierAddressViewModel { AddressType = "Entrega", Code = "DIR-001", AddressName = "Almacén Norte", MainStreet = "Av. De las Américas", AddressNumber = "450", Reference = "Junto al centro logístico", Neighborhood = "Parque Industrial", Province = "Lima", City = "San Martín de Porres", Country = "Perú", PostalCode = "15108", Latitude = -12.0464m, Longitude = -77.0428m, IsDefaultDelivery = true, IsPrimary = true, IsActive = true, Notes = "Dirección principal de entrega para recepción de mercadería." });
        addresses.Add(new SupplierAddressViewModel { AddressType = "Facturación", Code = "DIR-002", AddressName = "Oficina administrativa", MainStreet = "Av. Javier Prado Este", AddressNumber = "1200", Reference = "Oficina administrativa", Province = "Lima", City = "San Isidro", Country = "Perú", IsDefaultBilling = true, IsActive = true });
        addresses.Add(new SupplierAddressViewModel { AddressType = "Entrega", Code = "DIR-003", AddressName = "Almacén secundario", MainStreet = "Calle Los Pinos", AddressNumber = "785", Reference = "Almacén secundario", Province = "Lima", City = "Ate", Country = "Perú", IsActive = true });
    }

    private void WireAddressEvents()
    {
        btnAddAddress.Click += (_, _) => AddAddress();
        btnEditAddress.Click += (_, _) => EditSelectedAddress();
        btnDeleteAddress.Click += (_, _) => DeleteSelectedAddress();
        btnDuplicateAddress.Click += (_, _) => DuplicateSelectedAddress();
        btnSetDefaultAddress.Click += (_, _) => SetDefaultAddress();
        gvAddresses.DoubleClick += (_, _) => EditSelectedAddress();
    }

    private void LoadPurchaseSettings()
    {
        BindLookup(luePurchasePaymentCondition, "Crédito 30 días", "Contado", "Crédito 15 días");
        BindLookup(luePurchasePriceList, "Lista Compra Nacional", "Lista Compra Importación", "LP-COMP-ACME-01");
        BindLookup(lueIncoterm, "EXW", "CIP - Carriage and Insurance Paid To", "FOB", "CIF");
        BindLookup(lueAssignedBuyer, "Juan Pérez", "CP01 - Juan Carlos Pérez");
        BindLookup(luePreferredWarehouse, "Bodega Principal", "B01 - Almacén Principal");

        luePurchasePaymentCondition.EditValue = "Crédito 30 días";
        luePurchasePriceList.EditValue = "Lista Compra Nacional";
        spnDeliveryTermDays.Value = 7m;
        lueIncoterm.EditValue = "EXW";
        spnCommercialDiscountPercent.Value = 5m;
        lueAssignedBuyer.EditValue = "Juan Pérez";
        luePreferredWarehouse.EditValue = "Bodega Principal";
        spnAverageDeliveryDays.Value = 6m;
        spnMinimumOrderAmount.Value = 500m;
        spnMinimumOrderQuantity.Value = 1m;
        spnLeadTimeDays.Value = 5m;
        spnDeliveryToleranceDays.Value = 2m;
        tglRequiresPurchaseOrder.IsOn = true;
        tglSubjectToEvaluation.IsOn = true;
        tglActiveForImport.IsOn = false;
        tglAllowsUrgentPurchases.IsOn = true;
        lblPurchasesLast12MonthsValue.Text = "125,000.00 PEN";
        lblAveragePurchaseValue.Text = "8,950.00 PEN";
        lblAverageDelivery12MonthsValue.Text = "6";
        lblPurchaseOrdersLast12MonthsValue.Text = "14";
    }

    private void LoadPurchaseHistory()
    {
        purchaseHistory.Clear();
        purchaseHistory.Add(new SupplierPurchaseHistoryViewModel { PurchaseDate = new DateTime(2024, 5, 10), DocumentNumber = "OC-000145", Amount = 8500m, Currency = "PEN", AverageDeliveryDays = 5 });
        purchaseHistory.Add(new SupplierPurchaseHistoryViewModel { PurchaseDate = new DateTime(2024, 4, 22), DocumentNumber = "OC-000132", Amount = 12300m, Currency = "PEN", AverageDeliveryDays = 6 });
        purchaseHistory.Add(new SupplierPurchaseHistoryViewModel { PurchaseDate = new DateTime(2024, 4, 5), DocumentNumber = "OC-000119", Amount = 4750m, Currency = "PEN", AverageDeliveryDays = 7 });
        purchaseHistory.Add(new SupplierPurchaseHistoryViewModel { PurchaseDate = new DateTime(2024, 3, 18), DocumentNumber = "OC-000101", Amount = 15900m, Currency = "PEN", AverageDeliveryDays = 6 });
        purchaseHistory.Add(new SupplierPurchaseHistoryViewModel { PurchaseDate = new DateTime(2024, 2, 27), DocumentNumber = "OC-000087", Amount = 6200m, Currency = "PEN", AverageDeliveryDays = 8 });
        purchaseHistory.Add(new SupplierPurchaseHistoryViewModel { PurchaseDate = new DateTime(2024, 2, 12), DocumentNumber = "OC-000073", Amount = 9450m, Currency = "PEN", AverageDeliveryDays = 5 });
    }

    private void LoadBankAccounts()
    {
        bankAccounts.Clear();
        bankAccounts.Add(new SupplierBankAccountViewModel { BankName = "BCP - Banco de Crédito del Perú", Branch = "San Isidro", AccountType = "Cuenta Corriente", AccountNumber = "193-2212345-0-72", Currency = "PEN", AccountHolder = "ACME S.A.C.", HolderIdentification = "RUC 20123456789", SwiftBic = "BCPLPEPL", CciIban = "00219300221234507217", Country = "Perú", NotificationEmail = "tesoreria@acme.com.pe", Notes = "Cuenta principal para pagos en moneda nacional.", IsDefault = true, IsActive = true });
        bankAccounts.Add(new SupplierBankAccountViewModel { BankName = "BBVA Perú", Branch = "Miraflores", AccountType = "Cuenta de Ahorros", AccountNumber = "0011-0325-01-02012345", Currency = "USD", AccountHolder = "ACME S.A.C.", HolderIdentification = "RUC 20123456789", SwiftBic = "BCONPEPL", CciIban = "01132500010201234559", Country = "Perú", NotificationEmail = "tesoreria@acme.com.pe", IsActive = true });
        bankAccounts.Add(new SupplierBankAccountViewModel { BankName = "Interbank", Branch = "San Borja", AccountType = "Cuenta Corriente", AccountNumber = "200-3004005001", Currency = "PEN", AccountHolder = "ACME S.A.C.", HolderIdentification = "RUC 20123456789", SwiftBic = "BINPPEPL", CciIban = "00320000300400500188", Country = "Perú", NotificationEmail = "tesoreria@acme.com.pe", IsActive = true });
        lblBankAccountsTotal.Text = $"Total de registros: {bankAccounts.Count}";
    }

    private void WireBankAccountEvents()
    {
        btnAddBankAccount.Click += (_, _) => AddBankAccount();
        btnEditBankAccount.Click += (_, _) => EditSelectedBankAccount();
        btnDeleteBankAccount.Click += (_, _) => DeleteSelectedBankAccount();
        btnSetDefaultBankAccount.Click += (_, _) => SetDefaultBankAccount();
        gvBankAccounts.DoubleClick += (_, _) => EditSelectedBankAccount();
    }

    private void AddBankAccount()
    {
        using var dialog = new SupplierBankAccountEditDialog();
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var bankAccount = dialog.BankAccount;
        ApplyBankAccountDefault(bankAccount);
        bankAccounts.Add(bankAccount);
        RefreshBankAccounts();
    }

    private void EditSelectedBankAccount()
    {
        var bankAccount = SelectedBankAccount();
        if (bankAccount is null)
        {
            XtraMessageBox.Show(this, "Seleccione una cuenta bancaria para editar.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new SupplierBankAccountEditDialog(bankAccount);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var editedBankAccount = dialog.BankAccount;
        ApplyBankAccountDefault(editedBankAccount, bankAccount.Id);
        bankAccount.CopyFrom(editedBankAccount);
        RefreshBankAccounts();
    }

    private void DeleteSelectedBankAccount()
    {
        var bankAccount = SelectedBankAccount();
        if (bankAccount is null)
        {
            XtraMessageBox.Show(this, "Seleccione una cuenta bancaria para eliminar.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var result = XtraMessageBox.Show(
            this,
            $"¿Desea eliminar la cuenta bancaria {bankAccount.AccountNumber}?",
            Text,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            bankAccounts.Remove(bankAccount);
            RefreshBankAccounts();
        }
    }

    private void SetDefaultBankAccount()
    {
        var bankAccount = SelectedBankAccount();
        if (bankAccount is null)
        {
            XtraMessageBox.Show(this, "Seleccione una cuenta bancaria para marcar como predeterminada.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        foreach (var item in bankAccounts)
        {
            item.IsDefault = item.Id == bankAccount.Id;
        }

        RefreshBankAccounts();
    }

    private SupplierBankAccountViewModel? SelectedBankAccount()
    {
        return gvBankAccounts.GetFocusedRow() as SupplierBankAccountViewModel;
    }

    private void ApplyBankAccountDefault(SupplierBankAccountViewModel bankAccount, Guid? currentId = null)
    {
        if (!bankAccount.IsDefault)
        {
            return;
        }

        foreach (var item in bankAccounts)
        {
            if (currentId.HasValue && item.Id == currentId.Value)
            {
                continue;
            }

            item.IsDefault = false;
        }
    }

    private void RefreshBankAccounts()
    {
        grdBankAccounts.RefreshDataSource();
        gvBankAccounts.RefreshData();
        lblBankAccountsTotal.Text = $"Total de registros: {bankAccounts.Count}";
    }

    private void LoadWithholdingSettings()
    {
        BindLookup(lueGeneralWithholdingType, "Renta e IVA", "Renta", "IVA", "Especial");

        tglWithholdingAgent.IsOn = true;
        lueGeneralWithholdingType.EditValue = "Renta e IVA";
        txtWithholdingResolutionNumber.Text = "NAC-DGERCGC24-000001";
        tglWithholdsVat.IsOn = true;
        tglWithholdsIncomeTax.IsOn = true;
        tglIssuesElectronicReceipts.IsOn = true;
        tglSubjectToPerception.IsOn = false;
    }

    private void LoadWithholdings()
    {
        withholdings.Clear();
        withholdings.Add(new SupplierWithholdingViewModel { Document = "RUC 20123456789", Type = "Renta", IncomeTaxWithholdingPercent = 1.75m, VatWithholdingPercent = 30m, TaxSupport = "Compra de bienes", FiscalRegime = "Régimen General", IsRequiredAccounting = true, ValidityFrom = new DateTime(2024, 1, 1), ValidityTo = new DateTime(2024, 12, 31), IsDefault = true, IsActive = true, Notes = "Configuración de retención aplicable para compras nacionales." });
        withholdings.Add(new SupplierWithholdingViewModel { Document = "Certificado IVA 2024", Type = "IVA", VatWithholdingPercent = 30m, TaxSupport = "Compra de bienes", FiscalRegime = "Régimen General", IsRequiredAccounting = true, ValidityFrom = new DateTime(2024, 1, 1), ValidityTo = new DateTime(2024, 12, 31), IsActive = true });
        withholdings.Add(new SupplierWithholdingViewModel { Document = "Régimen Especial", Type = "Especial", IncomeTaxWithholdingPercent = 1m, TaxSupport = "Servicio", FiscalRegime = "Régimen Especial", ValidityFrom = new DateTime(2024, 3, 1), ValidityTo = new DateTime(2024, 12, 31), IsActive = true });
        RefreshWithholdings();
    }

    private void WireWithholdingEvents()
    {
        btnAddWithholding.Click += (_, _) => AddWithholding();
        btnEditWithholding.Click += (_, _) => EditSelectedWithholding();
        btnDeleteWithholding.Click += (_, _) => DeleteSelectedWithholding();
        btnSetDefaultWithholding.Click += (_, _) => SetDefaultWithholding();
        gvWithholdings.DoubleClick += (_, _) => EditSelectedWithholding();
    }

    private void AddWithholding()
    {
        using var dialog = new SupplierWithholdingEditDialog();
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var withholding = dialog.Withholding;
        ApplyWithholdingDefault(withholding);
        withholdings.Add(withholding);
        RefreshWithholdings();
    }

    private void EditSelectedWithholding()
    {
        var withholding = SelectedWithholding();
        if (withholding is null)
        {
            XtraMessageBox.Show(this, "Seleccione una retención para editar.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new SupplierWithholdingEditDialog(withholding);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var editedWithholding = dialog.Withholding;
        ApplyWithholdingDefault(editedWithholding, withholding.Id);
        withholding.CopyFrom(editedWithholding);
        RefreshWithholdings();
    }

    private void DeleteSelectedWithholding()
    {
        var withholding = SelectedWithholding();
        if (withholding is null)
        {
            XtraMessageBox.Show(this, "Seleccione una retención para eliminar.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var result = XtraMessageBox.Show(
            this,
            $"¿Desea eliminar la retención {withholding.Document}?",
            Text,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            withholdings.Remove(withholding);
            RefreshWithholdings();
        }
    }

    private void SetDefaultWithholding()
    {
        var withholding = SelectedWithholding();
        if (withholding is null)
        {
            XtraMessageBox.Show(this, "Seleccione una retención para marcar como predeterminada.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        foreach (var item in withholdings)
        {
            item.IsDefault = item.Id == withholding.Id;
        }

        RefreshWithholdings();
    }

    private SupplierWithholdingViewModel? SelectedWithholding()
    {
        return gvWithholdings.GetFocusedRow() as SupplierWithholdingViewModel;
    }

    private void ApplyWithholdingDefault(SupplierWithholdingViewModel withholding, Guid? currentId = null)
    {
        if (!withholding.IsDefault)
        {
            return;
        }

        foreach (var item in withholdings)
        {
            if (currentId.HasValue && item.Id == currentId.Value)
            {
                continue;
            }

            item.IsDefault = false;
        }
    }

    private void RefreshWithholdings()
    {
        grdWithholdings.RefreshDataSource();
        gvWithholdings.RefreshData();
    }

    private void LoadAccountingSettings()
    {
        BindLookup(lueDefaultProject, "PRY-OPERACIONES", "PRY001 - Operación General");
        BindLookup(lueFiscalCondition, "Régimen General", "Habido", "No Habido");
        BindLookup(lueThirdPartyType, "Proveedor Nacional", "Proveedor Extranjero");

        lueDefaultProject.EditValue = "PRY-OPERACIONES";
        lueFiscalCondition.EditValue = "Régimen General";
        lueThirdPartyType.EditValue = "Proveedor Nacional";
        tglAutomaticAccounting.IsOn = true;
        tglRequiresReconciliation.IsOn = true;
        tglHandlesAdvances.IsOn = true;
        tglAccountingBlocked.IsOn = false;
    }

    private void LoadAccountingAccounts()
    {
        accountingAccounts.Clear();
        accountingAccounts.Add(new SupplierAccountingAccountViewModel { AccountType = "Cuenta por Pagar", AccountCode = "421101", AccountName = "Proveedores Nacionales", Dimension1 = "ADM", Dimension2 = "COM", Dimension3 = "LOG", IsDefault = true, IsActive = true, Notes = "Cuenta contable principal para facturas de proveedor nacional." });
        accountingAccounts.Add(new SupplierAccountingAccountViewModel { AccountType = "Anticipo Proveedor", AccountCode = "422101", AccountName = "Anticipos a Proveedores", Dimension1 = "ADM", Dimension2 = "FIN", IsActive = true });
        accountingAccounts.Add(new SupplierAccountingAccountViewModel { AccountType = "Gasto", AccountCode = "601101", AccountName = "Compras Nacionales", Dimension1 = "COM", Dimension2 = "LOG", IsActive = true });
        accountingAccounts.Add(new SupplierAccountingAccountViewModel { AccountType = "Retención IVA", AccountCode = "401701", AccountName = "Retenciones IVA por Pagar", Dimension1 = "FIN", IsActive = true });
        accountingAccounts.Add(new SupplierAccountingAccountViewModel { AccountType = "Retención Renta", AccountCode = "401702", AccountName = "Retenciones Renta por Pagar", Dimension1 = "FIN", IsActive = true });
        RefreshAccountingAccounts();
    }

    private void WireAccountingAccountEvents()
    {
        btnAddAccountingAccount.Click += (_, _) => AddAccountingAccount();
        btnEditAccountingAccount.Click += (_, _) => EditSelectedAccountingAccount();
        btnDeleteAccountingAccount.Click += (_, _) => DeleteSelectedAccountingAccount();
        btnSetDefaultAccountingAccount.Click += (_, _) => SetDefaultAccountingAccount();
        gvAccountingAccounts.DoubleClick += (_, _) => EditSelectedAccountingAccount();
    }

    private void AddAccountingAccount()
    {
        using var dialog = new SupplierAccountingAccountEditDialog();
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var accountingAccount = dialog.AccountingAccount;
        ApplyAccountingAccountDefault(accountingAccount);
        accountingAccounts.Add(accountingAccount);
        RefreshAccountingAccounts();
    }

    private void EditSelectedAccountingAccount()
    {
        var accountingAccount = SelectedAccountingAccount();
        if (accountingAccount is null)
        {
            XtraMessageBox.Show(this, "Seleccione una cuenta contable para editar.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new SupplierAccountingAccountEditDialog(accountingAccount);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var editedAccountingAccount = dialog.AccountingAccount;
        ApplyAccountingAccountDefault(editedAccountingAccount, accountingAccount.Id);
        accountingAccount.CopyFrom(editedAccountingAccount);
        RefreshAccountingAccounts();
    }

    private void DeleteSelectedAccountingAccount()
    {
        var accountingAccount = SelectedAccountingAccount();
        if (accountingAccount is null)
        {
            XtraMessageBox.Show(this, "Seleccione una cuenta contable para eliminar.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var result = XtraMessageBox.Show(
            this,
            $"¿Desea eliminar la cuenta contable {accountingAccount.AccountCodeName}?",
            Text,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            accountingAccounts.Remove(accountingAccount);
            RefreshAccountingAccounts();
        }
    }

    private void SetDefaultAccountingAccount()
    {
        var accountingAccount = SelectedAccountingAccount();
        if (accountingAccount is null)
        {
            XtraMessageBox.Show(this, "Seleccione una cuenta contable para marcar como predeterminada.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        accountingAccount.IsDefault = true;
        ApplyAccountingAccountDefault(accountingAccount, accountingAccount.Id);
        RefreshAccountingAccounts();
    }

    private SupplierAccountingAccountViewModel? SelectedAccountingAccount()
    {
        return gvAccountingAccounts.GetFocusedRow() as SupplierAccountingAccountViewModel;
    }

    private void ApplyAccountingAccountDefault(SupplierAccountingAccountViewModel accountingAccount, Guid? currentId = null)
    {
        if (!accountingAccount.IsDefault)
        {
            return;
        }

        foreach (var item in accountingAccounts)
        {
            if (currentId.HasValue && item.Id == currentId.Value)
            {
                continue;
            }

            if (string.Equals(item.AccountType, accountingAccount.AccountType, StringComparison.OrdinalIgnoreCase))
            {
                item.IsDefault = false;
            }
        }
    }

    private void RefreshAccountingAccounts()
    {
        grdAccountingAccounts.RefreshDataSource();
        gvAccountingAccounts.RefreshData();
    }

    private void LoadSapSyncData()
    {
        tglSapSynchronized.IsOn = true;
        tglSapIntegrationValid.IsOn = true;
        tglSapErrorBlocked.IsOn = false;
        tglSapAutoUpdate.IsOn = true;
        txtSapLastSync.Text = "15/03/2024 09:18";
        txtSapLastSyncUser.Text = "ADMIN";
        txtSapDataOrigin.Text = "NuanSystem";
        txtSapIntegrationStatus.Text = "Sincronizado";
    }

    private void LoadSapAudit()
    {
        sapAudit.Clear();
        sapAudit.Add(new SupplierSapAuditViewModel(new DateTime(2024, 3, 15, 9, 18, 0), "Sincronización", "Éxito", "ADMIN", "Proveedor sincronizado correctamente."));
        sapAudit.Add(new SupplierSapAuditViewModel(new DateTime(2024, 3, 12, 16, 41, 0), "Actualización", "Éxito", "ADMIN", "Actualización de datos básica."));
        sapAudit.Add(new SupplierSapAuditViewModel(new DateTime(2024, 3, 5, 10, 22, 0), "Creación", "Éxito", "ADMIN", "Proveedor creado en SAP Business One."));
        sapAudit.Add(new SupplierSapAuditViewModel(new DateTime(2024, 3, 5, 10, 20, 0), "Validación", "Éxito", "ADMIN", "Validación previa a la creación satisfactoria."));
        sapAudit.Add(new SupplierSapAuditViewModel(new DateTime(2024, 2, 28, 11, 5, 0), "Sincronización", "Error", "ADMIN", "Error de comunicación con SAP. Reintento programado."));
        grdSapAudit.RefreshDataSource();
        gvSapAudit.RefreshData();
    }

    private void LoadAttachments()
    {
        memSupplierObservations.Text = "Proveedor estratégico para compras recurrentes de repuestos, suministros industriales y materiales operativos. Mantiene buen historial de entrega y condiciones comerciales preferenciales. Revisar periódicamente documentación tributaria y vigencia de certificados.";
        attachments.Clear();
        attachments.Add(new SupplierAttachmentViewModel { DocumentType = "RUC", FileName = "ruc_acme.pdf", UploadDate = new DateTime(2024, 3, 15, 9, 15, 0), User = "ADMIN", FileSize = "245 KB", Status = "Vigente", FilePath = @"\\servidor\documentos\proveedores\ACME\ruc_acme.pdf", Category = "Tributario", ExpirationDate = new DateTime(2024, 12, 31), Description = "Documento tributario del proveedor para validación administrativa." });
        attachments.Add(new SupplierAttachmentViewModel { DocumentType = "Certificado Bancario", FileName = "certificado_bcp.pdf", UploadDate = new DateTime(2024, 3, 12, 10, 0, 0), User = "ADMIN", FileSize = "180 KB", Status = "Vigente", FilePath = @"\\servidor\documentos\proveedores\ACME\certificado_bcp.pdf", Category = "Bancario", ExpirationDate = new DateTime(2024, 12, 31), Description = "Certificado bancario vigente para validación de pagos." });
        attachments.Add(new SupplierAttachmentViewModel { DocumentType = "Contrato", FileName = "contrato_suministro_2024.pdf", UploadDate = new DateTime(2024, 3, 5, 14, 20, 0), User = "ADMIN", FileSize = "1.2 MB", Status = "Vigente", FilePath = @"\\servidor\documentos\proveedores\ACME\contrato_suministro_2024.pdf", Category = "Legal", ExpirationDate = new DateTime(2024, 12, 31), Description = "Contrato de suministro vigente para compras recurrentes." });
        attachments.Add(new SupplierAttachmentViewModel { DocumentType = "Certificado Retención", FileName = "certificado_retencion_2024.pdf", UploadDate = new DateTime(2024, 3, 1, 11, 10, 0), User = "ADMIN", FileSize = "320 KB", Status = "Vigente", FilePath = @"\\servidor\documentos\proveedores\ACME\certificado_retencion_2024.pdf", Category = "Tributario", ExpirationDate = new DateTime(2024, 12, 31), Description = "Certificado de retención vigente para control tributario." });
        RefreshAttachments();
        UpdateSelectedAttachmentInfo();
    }

    private void WireAttachmentEvents()
    {
        btnAttachDocument.Click += (_, _) => AttachDocument();
        btnDownloadDocument.Click += (_, _) => DownloadDocument();
        btnViewDocument.Click += (_, _) => ViewDocument();
        btnDeleteDocument.Click += (_, _) => DeleteDocument();
        gvAttachments.FocusedRowChanged += (_, _) => UpdateSelectedAttachmentInfo();
    }

    private void AttachDocument()
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Adjuntar documento",
            Filter = "Documentos|*.pdf;*.doc;*.docx;*.xls;*.xlsx;*.png;*.jpg;*.jpeg|Todos los archivos|*.*",
            Multiselect = false
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var fileInfo = new FileInfo(dialog.FileName);
        var attachment = new SupplierAttachmentViewModel
        {
            DocumentType = "Anexo",
            FileName = fileInfo.Name,
            UploadDate = DateTime.Now,
            User = "ADMIN",
            FileSize = FormatFileSize(fileInfo.Exists ? fileInfo.Length : 0),
            Status = "Pendiente",
            FilePath = dialog.FileName,
            Category = "Pendiente de clasificación",
            Description = "Documento adjuntado localmente, pendiente de integración documental."
        };

        attachments.Add(attachment);
        RefreshAttachments();
        gvAttachments.FocusedRowHandle = attachments.Count - 1;
        UpdateSelectedAttachmentInfo(attachment);
    }

    private void DownloadDocument()
    {
        if (SelectedAttachment() is null)
        {
            XtraMessageBox.Show(this, "Seleccione un documento para descargar.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        XtraMessageBox.Show(this, "Funcionalidad de descarga pendiente de integración documental.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ViewDocument()
    {
        var attachment = SelectedAttachment();
        if (attachment is null)
        {
            XtraMessageBox.Show(this, "Seleccione un documento para ver.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        UpdateSelectedAttachmentInfo(attachment);
    }

    private void DeleteDocument()
    {
        var attachment = SelectedAttachment();
        if (attachment is null)
        {
            XtraMessageBox.Show(this, "Seleccione un documento para eliminar.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var result = XtraMessageBox.Show(this, "¿Desea eliminar el documento seleccionado?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes)
        {
            return;
        }

        attachments.Remove(attachment);
        RefreshAttachments();
        UpdateSelectedAttachmentInfo();
    }

    private SupplierAttachmentViewModel? SelectedAttachment()
    {
        return gvAttachments.GetFocusedRow() as SupplierAttachmentViewModel;
    }

    private void RefreshAttachments()
    {
        grdAttachments.RefreshDataSource();
        gvAttachments.RefreshData();
    }

    private void UpdateSelectedAttachmentInfo()
    {
        UpdateSelectedAttachmentInfo(SelectedAttachment() ?? attachments.FirstOrDefault());
    }

    private void UpdateSelectedAttachmentInfo(SupplierAttachmentViewModel? attachment)
    {
        txtAttachmentPath.Text = attachment?.FilePath ?? string.Empty;
        txtAttachmentCategory.Text = attachment?.Category ?? string.Empty;
        txtAttachmentExpirationDate.Text = attachment?.ExpirationDate?.ToString("dd/MM/yyyy") ?? string.Empty;
        memAttachmentDescription.Text = attachment?.Description ?? string.Empty;
        
    }

    private static string FormatFileSize(long bytes)
    {
        if (bytes <= 0)
        {
            return "0 KB";
        }

        if (bytes >= 1024 * 1024)
        {
            return $"{bytes / 1024d / 1024d:0.#} MB";
        }

        return $"{Math.Max(1, bytes / 1024)} KB";
    }

    private void AddAddress()
    {
        using var dialog = new SupplierAddressEditDialog(CreateNewAddressCode());
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var address = dialog.Address;
        ApplyAddressDefaults(address);
        addresses.Add(address);
        grdAddresses.RefreshDataSource();
    }

    private void EditSelectedAddress()
    {
        var address = SelectedAddress();
        if (address is null)
        {
            XtraMessageBox.Show(this, "Seleccione una dirección para editar.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new SupplierAddressEditDialog(address);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var editedAddress = dialog.Address;
        ApplyAddressDefaults(editedAddress, address.Id);
        address.CopyFrom(editedAddress);
        gvAddresses.RefreshData();
    }

    private void DeleteSelectedAddress()
    {
        var address = SelectedAddress();
        if (address is null)
        {
            XtraMessageBox.Show(this, "Seleccione una dirección para eliminar.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var result = XtraMessageBox.Show(
            this,
            $"¿Desea eliminar la dirección {address.Code}?",
            Text,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            addresses.Remove(address);
            grdAddresses.RefreshDataSource();
        }
    }

    private void DuplicateSelectedAddress()
    {
        var address = SelectedAddress();
        if (address is null)
        {
            XtraMessageBox.Show(this, "Seleccione una dirección para duplicar.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var copy = address.Clone();
        copy.Id = Guid.NewGuid();
        copy.Code = CreateCopyAddressCode();
        copy.IsPrimary = false;
        copy.IsDefaultBilling = false;
        copy.IsDefaultDelivery = false;
        addresses.Add(copy);
        grdAddresses.RefreshDataSource();
    }

    private void SetDefaultAddress()
    {
        var address = SelectedAddress();
        if (address is null)
        {
            XtraMessageBox.Show(this, "Seleccione una dirección para marcar como predeterminada.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        foreach (var item in addresses)
        {
            item.IsPrimary = item.Id == address.Id;
        }

        gvAddresses.RefreshData();
    }

    private SupplierAddressViewModel? SelectedAddress()
    {
        return gvAddresses.GetFocusedRow() as SupplierAddressViewModel;
    }

    private void ApplyAddressDefaults(SupplierAddressViewModel address, Guid? currentId = null)
    {
        address.IsPrimary = address.IsDefaultDelivery || address.IsDefaultBilling || address.IsPrimary;
        if (!address.IsPrimary)
        {
            return;
        }

        foreach (var item in addresses)
        {
            if (currentId.HasValue && item.Id == currentId.Value)
            {
                continue;
            }

            item.IsPrimary = false;
        }
    }

    private string CreateNewAddressCode()
    {
        var next = addresses.Count + 1;
        string code;
        do
        {
            code = $"DIR-{next:000}";
            next++;
        }
        while (addresses.Any(x => string.Equals(x.Code, code, StringComparison.OrdinalIgnoreCase)));

        return code;
    }

    private string CreateCopyAddressCode()
    {
        var next = 1;
        string code;
        do
        {
            code = $"DIR-COPIA-{next:000}";
            next++;
        }
        while (addresses.Any(x => string.Equals(x.Code, code, StringComparison.OrdinalIgnoreCase)));

        return code;
    }

    private void AddContact()
    {
        using var dialog = new SupplierContactEditDialog();
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var contact = dialog.Contact;
        if (contact.IsPrimary)
        {
            ClearPrimaryContacts();
        }

        contacts.Add(contact);
        grdContacts.RefreshDataSource();
    }

    private void EditSelectedContact()
    {
        var contact = SelectedContact();
        if (contact is null)
        {
            XtraMessageBox.Show(this, "Seleccione un contacto para editar.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new SupplierContactEditDialog(contact);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        var editedContact = dialog.Contact;
        if (editedContact.IsPrimary)
        {
            ClearPrimaryContacts(contact.Id);
        }

        contact.CopyFrom(editedContact);
        gvContacts.RefreshData();
    }

    private void DeleteSelectedContact()
    {
        var contact = SelectedContact();
        if (contact is null)
        {
            XtraMessageBox.Show(this, "Seleccione un contacto para eliminar.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var result = XtraMessageBox.Show(
            this,
            $"¿Desea eliminar el contacto {contact.FullName}?",
            Text,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            contacts.Remove(contact);
            grdContacts.RefreshDataSource();
        }
    }

    private void SetDefaultContact()
    {
        var contact = SelectedContact();
        if (contact is null)
        {
            XtraMessageBox.Show(this, "Seleccione un contacto para marcar como predeterminado.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        foreach (var item in contacts)
        {
            item.IsPrimary = item.Id == contact.Id;
        }

        gvContacts.RefreshData();
    }

    private SupplierContactViewModel? SelectedContact()
    {
        return gvContacts.GetFocusedRow() as SupplierContactViewModel;
    }

    private void ClearPrimaryContacts(Guid? exceptId = null)
    {
        foreach (var contact in contacts)
        {
            if (exceptId.HasValue && contact.Id == exceptId.Value)
            {
                continue;
            }

            contact.IsPrimary = false;
        }
    }

    private static (string FirstName, string LastName) SplitContactName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return (string.Empty, string.Empty);
        }

        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length <= 2)
        {
            return (fullName.Trim(), string.Empty);
        }

        return (string.Join(' ', parts.Take(2)), string.Join(' ', parts.Skip(2)));
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

    private static void BindLookup(LookUpEdit lookup, IReadOnlyCollection<BusinessPartnerLookupOption> options)
    {
        lookup.Properties.DataSource = options.Where(x => x.IsActive).ToList();
        lookup.Properties.DisplayMember = nameof(BusinessPartnerLookupOption.Name);
        lookup.Properties.ValueMember = nameof(BusinessPartnerLookupOption.Id);
        lookup.Properties.Columns.Clear();
        lookup.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(BusinessPartnerLookupOption.Code), "Código", 80));
        lookup.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(BusinessPartnerLookupOption.Name), "Nombre", 180));
    }

    private static void BindLookup(LookUpEdit lookup, params string[] values)
    {
        lookup.Properties.DataSource = values.Select(value => new SupplierTextOptionViewModel(value, value)).ToList();
        lookup.Properties.DisplayMember = nameof(SupplierTextOptionViewModel.Name);
        lookup.Properties.ValueMember = nameof(SupplierTextOptionViewModel.Code);
        lookup.Properties.Columns.Clear();
        lookup.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(SupplierTextOptionViewModel.Name), "Nombre", 180));
    }

    private static void SetEditValue(BaseEdit control, object? value)
    {
        if (value is not null)
        {
            control.EditValue = value;
        }
    }

    private static int ToInt(object? value)
    {
        return ToNullableInt(value) ?? 0;
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

        return int.TryParse(Convert.ToString(value), out var parsed) ? parsed : null;
    }

    private static object? LookupValueByCode(IReadOnlyCollection<BusinessPartnerLookupOption> options, string? code)
    {
        return options.FirstOrDefault(x => string.Equals(x.Code, code, StringComparison.OrdinalIgnoreCase))?.Id;
    }

    private static string? LookupCode(LookUpEdit lookup)
    {
        if (lookup.Properties.DataSource is not IEnumerable<BusinessPartnerLookupOption> options)
        {
            return null;
        }

        var selectedId = ToNullableInt(lookup.EditValue);
        return options.FirstOrDefault(x => selectedId.HasValue && x.Id == selectedId.Value)?.Code;
    }

    private static string? NullIfEmpty(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private bool IsDesignerHosted()
    {
        return DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;
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
            new[] { new BusinessPartnerIdentificationTypeLookup(1, "RUC", "RUC", "PE") },
            new[] { new BusinessPartnerPaymentTermLookup(1, "CRED", "Crédito", 30, true) },
            new[] { new BusinessPartnerLookupOption(1, "42.01.01", "Proveedores Nacionales") },
            new[] { new BusinessPartnerCodeNameLookup("Supplier", "Proveedor") },
            new[] { new BusinessPartnerCodeNameLookup("Active", "Activo") },
            new[] { new BusinessPartnerCodeNameLookup("Pending", "Pendiente") },
            new[] { new BusinessPartnerLookupOption(1, "SUM", "Suministros Industriales") },
            new[] { new BusinessPartnerLookupOption(1, "A", "Proveedores Estratégicos") },
            new[] { new BusinessPartnerLookupOption(1, "COM", "Comercio al por mayor") },
            new[] { new BusinessPartnerLookupOption(1, "LIM", "Lima") },
            new[] { new BusinessPartnerLookupOption(1, "LOCAL", "Compra local") },
            new[] { new BusinessPartnerLookupOption(1, "COM", "Comercial") },
            new[] { new BusinessPartnerLookupOption(1, "EMAIL", "Correo electrónico") },
            new[] { new BusinessPartnerLookupOption(1, "PE", "Perú") },
            new[] { new BusinessPartnerGeoLookupOption(1, "LIM", "Lima", true, 1) },
            new[] { new BusinessPartnerGeoLookupOption(1, "LIM", "Lima", true, 1, 1, "15001") },
            new[] { new BusinessPartnerLookupOption(1, "BCP", "BCP - Banco de Crédito del Perú") },
            new[] { new BusinessPartnerLookupOption(1, "CORRIENTE", "Cuenta Corriente") },
            new[] { new BusinessPartnerLookupOption(1, "PEN", "PEN - Sol Peruano"), new BusinessPartnerLookupOption(2, "USD", "USD - Dólar Americano") },
            new[] { new BusinessPartnerLookupOption(1, "LP-COMP", "Lista compra estándar") },
            new[] { new BusinessPartnerLookupOption(1, "CP01", "Juan Carlos Pérez") },
            new[] { new BusinessPartnerLookupOption(1, "GENERAL", "Régimen General") },
            new[] { new BusinessPartnerLookupOption(1, "JUR", "Jurídica") },
            new[] { new BusinessPartnerLookupOption(1, "RENTA3", "Renta de 3ra Categoría") },
            new[] { new BusinessPartnerRetentionConceptLookup(1, "RET", "Certificado de Agente de Retención", true, "RET", 3m, true, true, 1) },
            new[] { new BusinessPartnerLookupOption(1, "LEGAL", "Documentos Legales") },
            new[] { new BusinessPartnerLookupOption(1, "TRANSFER", "Transferencia bancaria") },
            new[] { new BusinessPartnerLookupOption(1, "NORMAL", "Normal") },
            new[] { new BusinessPartnerLookupOption(1, "APROB", "Aprobación estándar") },
            new[] { new BusinessPartnerLookupOption(1, "PAYMENT", "Egreso proveedor") },
            new[] { new BusinessPartnerLookupOption(1, "01", "Matriz") },
            new[] { new BusinessPartnerLookupOption(1, "ADM", "Administración") },
            new[] { new BusinessPartnerLookupOption(1, "COM", "Comercialización") },
            new[] { new BusinessPartnerLookupOption(1, "CC-ADM-001", "Administración general") },
            new[] { new BusinessPartnerLookupOption(1, "PRY001", "Operación General") });
    }
}
