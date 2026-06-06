using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.WinForms.ViewModels.BusinessPartners.Suppliers;

namespace NuanSystem.WinForms.Forms.BusinessPartners;

public sealed partial class SupplierBankAccountEditDialog : XtraForm
{
    public SupplierBankAccountEditDialog()
        : this(null)
    {
    }

    internal SupplierBankAccountEditDialog(SupplierBankAccountViewModel? bankAccount)
    {
        InitializeComponent();
        BindLookups();

        BankAccount = bankAccount?.Clone() ?? new SupplierBankAccountViewModel
        {
            BankName = "BCP - Banco de Crédito del Perú",
            Branch = "San Isidro",
            AccountType = "Cuenta Corriente",
            AccountNumber = "193-2212345-0-72",
            Currency = "PEN - Sol Peruano",
            AccountHolder = "ACME S.A.C.",
            HolderIdentification = "RUC 20123456789",
            SwiftBic = "BCPLPEPL",
            CciIban = "00219300221234507217",
            Country = "Perú",
            NotificationEmail = "tesoreria@acme.com.pe",
            Notes = "Cuenta principal para pagos en moneda nacional.",
            IsDefault = true,
            IsActive = true
        };

        Text = bankAccount is null ? "Nueva Cuenta Bancaria" : "Editar Cuenta Bancaria";
        LoadBankAccount();
        WireEvents();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal SupplierBankAccountViewModel BankAccount { get; private set; }

    private void WireEvents()
    {
        btnSaveBankAccount.Click += (_, _) => SaveBankAccount();
        btnCancelBankAccount.Click += (_, _) => Close();
    }

    private void BindLookups()
    {
        BindLookup(lueBank, "BCP - Banco de Crédito del Perú", "BBVA Perú", "Interbank", "Scotiabank Perú");
        BindLookup(lueBankAccountType, "Cuenta Corriente", "Cuenta de Ahorros");
        BindLookup(lueBankCurrency, "PEN - Sol Peruano", "USD - Dólar Americano");
        BindLookup(lueBankCountry, "Perú", "Ecuador", "Colombia");
    }

    private static void BindLookup(LookUpEdit lookup, params string[] values)
    {
        lookup.Properties.DataSource = values.Select(value => new TextOption(value, value)).ToList();
        lookup.Properties.DisplayMember = nameof(TextOption.Name);
        lookup.Properties.ValueMember = nameof(TextOption.Code);
        lookup.Properties.Columns.Clear();
        lookup.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(TextOption.Name), "Nombre", 220));
    }

    private void LoadBankAccount()
    {
        lueBank.EditValue = BankAccount.BankName;
        txtBankBranch.Text = BankAccount.Branch;
        lueBankAccountType.EditValue = BankAccount.AccountType;
        txtBankAccountNumber.Text = BankAccount.AccountNumber;
        lueBankCurrency.EditValue = BankAccount.Currency;
        txtBankAccountHolder.Text = BankAccount.AccountHolder;
        txtBankHolderIdentification.Text = BankAccount.HolderIdentification;
        txtSwiftBic.Text = BankAccount.SwiftBic;
        txtCciIban.Text = BankAccount.CciIban;
        lueBankCountry.EditValue = BankAccount.Country;
        txtBankNotificationEmail.Text = BankAccount.NotificationEmail;
        memBankNotes.Text = BankAccount.Notes;
        tglBankDefault.IsOn = BankAccount.IsDefault;
        tglBankActive.IsOn = BankAccount.IsActive;
    }

    private void SaveBankAccount()
    {
        if (!ValidateBankAccount())
        {
            return;
        }

        BankAccount.BankName = Convert.ToString(lueBank.EditValue) ?? string.Empty;
        BankAccount.Branch = txtBankBranch.Text.Trim();
        BankAccount.AccountType = Convert.ToString(lueBankAccountType.EditValue) ?? string.Empty;
        BankAccount.AccountNumber = txtBankAccountNumber.Text.Trim();
        BankAccount.Currency = Convert.ToString(lueBankCurrency.EditValue) ?? string.Empty;
        BankAccount.AccountHolder = txtBankAccountHolder.Text.Trim();
        BankAccount.HolderIdentification = txtBankHolderIdentification.Text.Trim();
        BankAccount.SwiftBic = txtSwiftBic.Text.Replace(" ", string.Empty).Trim();
        BankAccount.CciIban = txtCciIban.Text.Replace(" ", string.Empty).Trim();
        BankAccount.Country = Convert.ToString(lueBankCountry.EditValue) ?? string.Empty;
        BankAccount.NotificationEmail = txtBankNotificationEmail.Text.Trim();
        BankAccount.Notes = memBankNotes.Text.Trim();
        BankAccount.IsDefault = tglBankDefault.IsOn;
        BankAccount.IsActive = tglBankActive.IsOn;

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateBankAccount()
    {
        if (lueBank.EditValue is null)
        {
            return ShowValidation("Banco es requerido.", lueBank);
        }

        if (lueBankAccountType.EditValue is null)
        {
            return ShowValidation("Tipo de Cuenta es requerido.", lueBankAccountType);
        }

        if (string.IsNullOrWhiteSpace(txtBankAccountNumber.Text))
        {
            return ShowValidation("N° de Cuenta es requerido.", txtBankAccountNumber);
        }

        if (lueBankCurrency.EditValue is null)
        {
            return ShowValidation("Moneda es requerida.", lueBankCurrency);
        }

        if (string.IsNullOrWhiteSpace(txtBankAccountHolder.Text))
        {
            return ShowValidation("Titular es requerido.", txtBankAccountHolder);
        }

        if (string.IsNullOrWhiteSpace(txtBankHolderIdentification.Text))
        {
            return ShowValidation("Identificación del Titular es requerida.", txtBankHolderIdentification);
        }

        if (lueBankCountry.EditValue is null)
        {
            return ShowValidation("País es requerido.", lueBankCountry);
        }

        var email = txtBankNotificationEmail.Text.Trim();
        if (email.Length > 0 && (!email.Contains('@') || !email.Contains('.')))
        {
            return ShowValidation("Ingrese un correo de notificación válido.", txtBankNotificationEmail);
        }

        return true;
    }

    private bool ShowValidation(string message, Control control)
    {
        XtraMessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        control.Focus();
        return false;
    }

    private sealed record TextOption(string Code, string Name);
}
