using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Geography.Models;

namespace NuanSystem.WinForms.Forms.Geography.Cities;

public sealed partial class CityEditForm : BaseEditForm
{
    public CityEditForm()
    {
        InitializeComponent();
        ConfigureForm();
    }

    public CityEditForm(IReadOnlyCollection<GeographyLookupItem> countries, IReadOnlyCollection<GeographyLookupItem> provinces)
        : this()
    {
        BindLookups(countries, provinces);
    }

    public CityEditForm(IReadOnlyCollection<GeographyLookupItem> countries, IReadOnlyCollection<GeographyLookupItem> provinces, CityItem item, bool copyMode = false)
        : this(countries, provinces)
    {
        LoadCity(item, copyMode);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveCityRequest Request { get; private set; } = new(0, 0, string.Empty, string.Empty, true);

    protected override bool ValidateForm()
    {
        var isValid = true;
        if (lueCountry.EditValue is null)
        {
            Validator.SetError(lueCountry, "Seleccione el país.");
            isValid = false;
        }

        if (lueProvince.EditValue is null)
        {
            Validator.SetError(lueProvince, "Seleccione la provincia.");
            isValid = false;
        }

        isValid &= Validator.RequireText(txtCode, "Ingrese el código.");
        isValid &= Validator.RequireText(txtName, "Ingrese el nombre.");
        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveCityRequest(
            Convert.ToInt32(lueCountry.EditValue),
            Convert.ToInt32(lueProvince.EditValue),
            txtCode.Text.Trim(),
            txtName.Text.Trim(),
            chkIsActive.Checked);
    }

    private void ConfigureForm()
    {
        Text = "Nueva ciudad";
        chkIsActive.Checked = true;
        btnSave.Click += (_, _) => Save();
    }

    private void BindLookups(IReadOnlyCollection<GeographyLookupItem> countries, IReadOnlyCollection<GeographyLookupItem> provinces)
    {
        BindLookup(lueCountry, countries);
        BindLookup(lueProvince, provinces);
    }

    private static void BindLookup(DevExpress.XtraEditors.LookUpEdit lookup, IReadOnlyCollection<GeographyLookupItem> items)
    {
        lookup.Properties.DataSource = items;
        lookup.Properties.DisplayMember = nameof(GeographyLookupItem.Name);
        lookup.Properties.ValueMember = nameof(GeographyLookupItem.Id);
        lookup.Properties.Columns.Clear();
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(GeographyLookupItem.Code), "Código", 80));
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(GeographyLookupItem.Name), "Nombre", 180));
    }

    private void LoadCity(CityItem item, bool copyMode)
    {
        Text = copyMode ? "Copiar ciudad" : "Editar ciudad";
        lueCountry.EditValue = item.CountryId;
        lueProvince.EditValue = item.ProvinceId;
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        chkIsActive.Checked = item.IsActive;
    }
}
