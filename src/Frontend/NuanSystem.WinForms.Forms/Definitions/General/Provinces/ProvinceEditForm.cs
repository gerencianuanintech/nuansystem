using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Lookups;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.General.Common;
using NuanSystem.WinForms.Services.Definitions.General.Countries;
using NuanSystem.WinForms.Services.Definitions.General.Provinces;

namespace NuanSystem.WinForms.Forms.Definitions.General.Provinces;

public sealed partial class ProvinceEditForm : BaseEditForm
{
    private readonly List<GeographyLookupItem> countries;
    private readonly bool canCreateCountries;
    private readonly bool canUpdateCountries;
    private bool managingCountry;

    public ProvinceEditForm()
        : this(Array.Empty<GeographyLookupItem>())
    {
    }

    public ProvinceEditForm(
        IReadOnlyCollection<GeographyLookupItem> countries,
        bool canCreateCountries = false,
        bool canUpdateCountries = false,
        int? selectedCountryId = null)
    {
        this.countries = countries.ToList();
        this.canCreateCountries = canCreateCountries;
        this.canUpdateCountries = canUpdateCountries;
        InitializeComponent();
        ConfigureForm();
        BindCountries();
        lueCountry.EditValue = selectedCountryId;
    }

    public ProvinceEditForm(
        IReadOnlyCollection<GeographyLookupItem> countries,
        ProvinceItem item,
        bool copyMode = false,
        bool canCreateCountries = false,
        bool canUpdateCountries = false)
        : this(countries, canCreateCountries, canUpdateCountries, item.CountryId)
    {
        LoadProvince(item, copyMode);
    }

    public event Func<ProvinceEditForm, Task<CountryItem?>>? CreateCountryRequested;

    public event Func<ProvinceEditForm, int, Task<CountryItem?>>? EditCountryRequested;

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
        lueCountry.RefreshButtons();
        lueCountry.ClearButtonEnabled = false;
        lueCountry.CreateButtonEnabled = canCreateCountries;
        lueCountry.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueCountry.Properties.SearchMode = SearchMode.AutoSearch;
        lueCountry.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
        lueCountry.CreateButtonClick += CountryLookupCreateButtonClick;
        lueCountry.EditButtonClick += CountryLookupEditButtonClick;
        lueCountry.EditValueChanged += CountryLookupEditValueChanged;
        UpdateCountryEditButton();
    }

    private void BindCountries()
    {
        lueCountry.Properties.DataSource = countries;
        lueCountry.Properties.DisplayMember = nameof(GeographyLookupItem.Name);
        lueCountry.Properties.ValueMember = nameof(GeographyLookupItem.Id);
        lueCountry.Properties.Columns.Clear();
        lueCountry.Properties.Columns.Add(new LookUpColumnInfo(nameof(GeographyLookupItem.Code), "Código", 80));
        lueCountry.Properties.Columns.Add(new LookUpColumnInfo(nameof(GeographyLookupItem.Name), "Nombre", 180));
    }

    private async void CountryLookupCreateButtonClick(object? sender, EventArgs e)
    {
        await ManageCountryAsync(null);
    }

    private async void CountryLookupEditButtonClick(object? sender, EventArgs e)
    {
        if (lueCountry.EditValue is not null)
        {
            await ManageCountryAsync(Convert.ToInt32(lueCountry.EditValue));
        }
    }

    private void CountryLookupEditValueChanged(object? sender, EventArgs e)
    {
        UpdateCountryEditButton();
    }

    private async Task ManageCountryAsync(int? countryId)
    {
        var hasAccess = countryId.HasValue ? canUpdateCountries : canCreateCountries;
        if (managingCountry || !hasAccess)
        {
            return;
        }

        if ((!countryId.HasValue && CreateCountryRequested is null)
            || (countryId.HasValue && EditCountryRequested is null))
        {
            return;
        }

        managingCountry = true;
        lueCountry.Enabled = false;
        try
        {
            CountryItem? saved = null;
            await RunWithUiExceptionHandlingAsync(async () =>
            {
                saved = countryId.HasValue
                    ? await EditCountryRequested!(this, countryId.Value)
                    : await CreateCountryRequested!(this);
            });

            if (saved is not null)
            {
                UpsertCountry(saved);
                lueCountry.EditValue = saved.Id;
            }
        }
        finally
        {
            managingCountry = false;
            lueCountry.Enabled = true;
            UpdateCountryEditButton();
        }
    }

    private void UpsertCountry(CountryItem country)
    {
        var lookup = new GeographyLookupItem
        {
            Id = country.Id,
            Code = country.Code,
            Name = country.Name,
            IsActive = country.IsActive
        };
        var index = countries.FindIndex(item => item.Id == country.Id);
        if (index >= 0)
        {
            countries[index] = lookup;
        }
        else
        {
            countries.Add(lookup);
        }

        lueCountry.Properties.DataSource = null;
        BindCountries();
    }

    private void UpdateCountryEditButton()
    {
        lueCountry.EditButtonEnabled = canUpdateCountries && lueCountry.EditValue is not null;
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
