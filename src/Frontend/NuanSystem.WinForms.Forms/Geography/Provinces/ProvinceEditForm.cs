using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Geography.Models;

namespace NuanSystem.WinForms.Forms.Geography.Provinces;

public sealed partial class ProvinceEditForm : BaseEditForm
{
    public ProvinceEditForm()
    {
        InitializeComponent();
        ConfigureForm();
    }

    public ProvinceEditForm(IReadOnlyCollection<GeographyLookupItem> countries)
        : this()
    {
        BindCountries(countries);
    }

    public ProvinceEditForm(IReadOnlyCollection<GeographyLookupItem> countries, ProvinceItem item, bool copyMode = false)
        : this(countries)
    {
        LoadProvince(item, copyMode);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveProvinceRequest Request { get; private set; } = new(0, string.Empty, string.Empty, true);

    protected override bool ValidateForm()
    {
        var isValid = true;
        if (lueCountry.EditValue is null)
        {
            Validator.SetError(lueCountry, "Seleccione el país.");
            isValid = false;
        }

        isValid &= Validator.RequireText(txtCode, "Ingrese el código.");
        isValid &= Validator.RequireText(txtName, "Ingrese el nombre.");
        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveProvinceRequest(
            Convert.ToInt32(lueCountry.EditValue),
            txtCode.Text.Trim(),
            txtName.Text.Trim(),
            chkIsActive.Checked);
    }

    private void ConfigureForm()
    {
        Text = "Nueva provincia";
        chkIsActive.Checked = true;
        btnSave.Click += (_, _) => Save();
    }

    private void BindCountries(IReadOnlyCollection<GeographyLookupItem> countries)
    {
        lueCountry.Properties.DataSource = countries;
        lueCountry.Properties.DisplayMember = nameof(GeographyLookupItem.Name);
        lueCountry.Properties.ValueMember = nameof(GeographyLookupItem.Id);
        lueCountry.Properties.Columns.Clear();
        lueCountry.Properties.Columns.Add(new LookUpColumnInfo(nameof(GeographyLookupItem.Code), "Código", 80));
        lueCountry.Properties.Columns.Add(new LookUpColumnInfo(nameof(GeographyLookupItem.Name), "Nombre", 180));
    }

    private void LoadProvince(ProvinceItem item, bool copyMode)
    {
        Text = copyMode ? "Copiar provincia" : "Editar provincia";
        lueCountry.EditValue = item.CountryId;
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        chkIsActive.Checked = item.IsActive;
    }
}
