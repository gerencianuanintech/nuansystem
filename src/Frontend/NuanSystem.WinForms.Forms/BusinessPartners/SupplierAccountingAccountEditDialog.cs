using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.WinForms.ViewModels.BusinessPartners.Suppliers;

namespace NuanSystem.WinForms.Forms.BusinessPartners;

public sealed partial class SupplierAccountingAccountEditDialog : XtraForm
{
    private readonly IReadOnlyDictionary<string, (string Code, string Name)> accountCatalog =
        new Dictionary<string, (string Code, string Name)>(StringComparer.OrdinalIgnoreCase)
        {
            ["421101 - Proveedores Nacionales"] = ("421101", "Proveedores Nacionales"),
            ["422101 - Anticipos a Proveedores"] = ("422101", "Anticipos a Proveedores"),
            ["601101 - Compras Nacionales"] = ("601101", "Compras Nacionales"),
            ["401701 - Retenciones IVA por Pagar"] = ("401701", "Retenciones IVA por Pagar"),
            ["401702 - Retenciones Renta por Pagar"] = ("401702", "Retenciones Renta por Pagar"),
            ["676101 - Diferencia de Cambio"] = ("676101", "Diferencia de Cambio"),
            ["421201 - Descuentos por Pronto Pago"] = ("421201", "Descuentos por Pronto Pago")
        };

    public SupplierAccountingAccountEditDialog()
        : this(null)
    {
    }

    internal SupplierAccountingAccountEditDialog(SupplierAccountingAccountViewModel? accountingAccount)
    {
        InitializeComponent();
        BindLookups();

        AccountingAccount = accountingAccount?.Clone() ?? new SupplierAccountingAccountViewModel
        {
            AccountType = "Cuenta por Pagar",
            AccountCode = "421101",
            AccountName = "Proveedores Nacionales",
            Dimension1 = "ADM",
            Dimension2 = "COM",
            Dimension3 = "LOG",
            IsDefault = true,
            IsActive = true,
            Notes = "Cuenta contable principal para facturas de proveedor nacional."
        };

        Text = accountingAccount is null ? "Nueva Cuenta Contable" : "Editar Cuenta Contable";
        LoadAccountingAccount();
        WireEvents();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal SupplierAccountingAccountViewModel AccountingAccount { get; private set; }

    private void WireEvents()
    {
        btnSaveAccountingAccount.Click += (_, _) => SaveAccountingAccount();
        btnCancelAccountingAccount.Click += (_, _) => Close();
    }

    private void BindLookups()
    {
        BindLookup(lueAccountingAccountType, "Cuenta por Pagar", "Anticipo Proveedor", "Gasto", "Diferencia de Cambio", "Retención IVA", "Retención Renta", "Pronto Pago", "Otros");
        BindLookup(slueAccountingAccount, accountCatalog.Keys.ToArray());
        BindLookup(lueAccountingDimension1, "ADM", "COM", "FIN", "LOG", "VACÍO");
        BindLookup(lueAccountingDimension2, "ADM", "COM", "FIN", "LOG", "VACÍO");
        BindLookup(lueAccountingDimension3, "ADM", "COM", "FIN", "LOG", "VACÍO");
        BindLookup(lueAccountingDimension4, "ADM", "COM", "FIN", "LOG", "VACÍO");
        BindLookup(lueAccountingDimension5, "ADM", "COM", "FIN", "LOG", "VACÍO");
    }

    private static void BindLookup(LookUpEdit lookup, params string[] values)
    {
        lookup.Properties.DataSource = values.Select(value => new SupplierTextOptionViewModel(value, value)).ToList();
        lookup.Properties.DisplayMember = nameof(SupplierTextOptionViewModel.Name);
        lookup.Properties.ValueMember = nameof(SupplierTextOptionViewModel.Code);
        lookup.Properties.Columns.Clear();
        lookup.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(SupplierTextOptionViewModel.Name), "Nombre", 240));
    }

    private void LoadAccountingAccount()
    {
        lueAccountingAccountType.EditValue = AccountingAccount.AccountType;
        slueAccountingAccount.EditValue = AccountingAccount.AccountCodeName;
        lueAccountingDimension1.EditValue = EmptyAsVacio(AccountingAccount.Dimension1);
        lueAccountingDimension2.EditValue = EmptyAsVacio(AccountingAccount.Dimension2);
        lueAccountingDimension3.EditValue = EmptyAsVacio(AccountingAccount.Dimension3);
        lueAccountingDimension4.EditValue = EmptyAsVacio(AccountingAccount.Dimension4);
        lueAccountingDimension5.EditValue = EmptyAsVacio(AccountingAccount.Dimension5);
        tglAccountingAccountDefault.IsOn = AccountingAccount.IsDefault;
        tglAccountingAccountActive.IsOn = AccountingAccount.IsActive;
        memAccountingAccountNotes.Text = AccountingAccount.Notes;
    }

    private void SaveAccountingAccount()
    {
        if (!ValidateAccountingAccount())
        {
            return;
        }

        var accountText = Convert.ToString(slueAccountingAccount.EditValue) ?? string.Empty;
        var account = accountCatalog.TryGetValue(accountText, out var catalogAccount)
            ? catalogAccount
            : SplitAccount(accountText);

        AccountingAccount.AccountType = Convert.ToString(lueAccountingAccountType.EditValue) ?? string.Empty;
        AccountingAccount.AccountCode = account.Code.Trim();
        AccountingAccount.AccountName = account.Name.Trim();
        AccountingAccount.Dimension1 = VacioAsEmpty(lueAccountingDimension1.EditValue);
        AccountingAccount.Dimension2 = VacioAsEmpty(lueAccountingDimension2.EditValue);
        AccountingAccount.Dimension3 = VacioAsEmpty(lueAccountingDimension3.EditValue);
        AccountingAccount.Dimension4 = VacioAsEmpty(lueAccountingDimension4.EditValue);
        AccountingAccount.Dimension5 = VacioAsEmpty(lueAccountingDimension5.EditValue);
        AccountingAccount.IsDefault = tglAccountingAccountDefault.IsOn;
        AccountingAccount.IsActive = tglAccountingAccountActive.IsOn;
        AccountingAccount.Notes = memAccountingAccountNotes.Text.Trim();

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateAccountingAccount()
    {
        if (lueAccountingAccountType.EditValue is null)
        {
            return ShowValidation("Tipo es requerido.", lueAccountingAccountType);
        }

        if (slueAccountingAccount.EditValue is null || string.IsNullOrWhiteSpace(Convert.ToString(slueAccountingAccount.EditValue)))
        {
            return ShowValidation("Cuenta Contable es requerida.", slueAccountingAccount);
        }

        if (tglAccountingAccountDefault.IsOn && lueAccountingAccountType.EditValue is null)
        {
            return ShowValidation("Tipo es requerido para marcar predeterminada.", lueAccountingAccountType);
        }

        return true;
    }

    private bool ShowValidation(string message, Control control)
    {
        XtraMessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        control.Focus();
        return false;
    }

    private static string EmptyAsVacio(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? "VACÍO" : value.Trim();
    }

    private static string VacioAsEmpty(object? value)
    {
        var text = Convert.ToString(value)?.Trim() ?? string.Empty;
        return string.Equals(text, "VACÍO", StringComparison.OrdinalIgnoreCase) ? string.Empty : text;
    }

    private static (string Code, string Name) SplitAccount(string accountText)
    {
        var parts = accountText.Split(" - ", 2, StringSplitOptions.TrimEntries);
        return parts.Length == 2 ? (parts[0], parts[1]) : (accountText.Trim(), string.Empty);
    }
}
