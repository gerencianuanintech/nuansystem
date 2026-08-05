using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.General.Countries;

namespace NuanSystem.WinForms.Forms.Definitions.General.Countries;

public sealed partial class CountryEditForm : BaseEditForm
{
    public CountryEditForm()
    {
        InitializeComponent();
        ConfigureForm();
    }

    public CountryEditForm(CountryItem item, bool copyMode = false)
        : this()
    {
        LoadCountry(item, copyMode);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveCountryRequest Request { get; private set; } = new(string.Empty, string.Empty, null, null, null, true);

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= Validator.RequireText(txtCode, "Ingrese el código.");
        isValid &= Validator.RequireText(txtName, "Ingrese el nombre.");
        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveCountryRequest(
            txtCode.Text.Trim(),
            txtName.Text.Trim(),
            NullIfWhiteSpace(txtIso2.Text),
            NullIfWhiteSpace(txtIso3.Text),
            NullIfWhiteSpace(txtPhonePrefix.Text),
            chkIsActive.Checked);
    }

    private void ConfigureForm()
    {
        Text = "Nuevo país";
        chkIsActive.Checked = true;
        btnSave.Click += (_, _) => Save();
    }

    private void LoadCountry(CountryItem item, bool copyMode)
    {
        Text = copyMode ? "Copiar país" : "Editar país";
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        txtIso2.Text = item.Iso2;
        txtIso3.Text = item.Iso3;
        txtPhonePrefix.Text = item.PhonePrefix;
        chkIsActive.Checked = item.IsActive;
    }

    private static string? NullIfWhiteSpace(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
