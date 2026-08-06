using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Lookups;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.General.Cities;
using NuanSystem.WinForms.Services.Definitions.General.Common;
using NuanSystem.WinForms.Services.Definitions.General.Countries;
using NuanSystem.WinForms.Services.Definitions.General.Provinces;

namespace NuanSystem.WinForms.Forms.Definitions.General.Cities;

public sealed partial class CityEditForm : BaseEditForm
{
    private readonly List<GeographyLookupItem> countries;
    private readonly List<GeographyLookupItem> provinces;
    private readonly bool canCreateCountries;
    private readonly bool canUpdateCountries;
    private readonly bool canCreateProvinces;
    private readonly bool canUpdateProvinces;
    private bool suppressCountryChange;
    private bool managingLookup;
    private int provinceLoadVersion;

    public CityEditForm()
        : this(Array.Empty<GeographyLookupItem>(), Array.Empty<GeographyLookupItem>())
    {
    }

    public CityEditForm(
        IReadOnlyCollection<GeographyLookupItem> countries,
        IReadOnlyCollection<GeographyLookupItem> provinces,
        bool canCreateCountries = false,
        bool canUpdateCountries = false,
        bool canCreateProvinces = false,
        bool canUpdateProvinces = false)
    {
        this.countries = countries.ToList();
        this.provinces = provinces.ToList();
        this.canCreateCountries = canCreateCountries;
        this.canUpdateCountries = canUpdateCountries;
        this.canCreateProvinces = canCreateProvinces;
        this.canUpdateProvinces = canUpdateProvinces;
        InitializeComponent();
        ConfigureForm();
        BindCountries();
        BindProvinces();
    }

    public CityEditForm(
        IReadOnlyCollection<GeographyLookupItem> countries,
        IReadOnlyCollection<GeographyLookupItem> provinces,
        CityItem item,
        bool copyMode = false,
        bool canCreateCountries = false,
        bool canUpdateCountries = false,
        bool canCreateProvinces = false,
        bool canUpdateProvinces = false)
        : this(
            countries,
            provinces,
            canCreateCountries,
            canUpdateCountries,
            canCreateProvinces,
            canUpdateProvinces)
    {
        LoadCity(item, copyMode);
    }

    public event Func<CityEditForm, string, Task<IReadOnlyCollection<GeographyLookupItem>>>? LoadProvincesRequested;

    public event Func<CityEditForm, Task<CountryItem?>>? CreateCountryRequested;

    public event Func<CityEditForm, int, Task<CountryItem?>>? EditCountryRequested;

    public event Func<CityEditForm, int, Task<ProvinceItem?>>? CreateProvinceRequested;

    public event Func<CityEditForm, int, Task<ProvinceItem?>>? EditProvinceRequested;

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
        ConfigureLookup(lueCountry, canCreateCountries);
        ConfigureLookup(lueProvince, canCreateProvinces);
        lueCountry.EditValueChanged += CountryLookupEditValueChanged;
        lueCountry.CreateButtonClick += CountryLookupCreateButtonClick;
        lueCountry.EditButtonClick += CountryLookupEditButtonClick;
        lueProvince.CreateButtonClick += ProvinceLookupCreateButtonClick;
        lueProvince.EditButtonClick += ProvinceLookupEditButtonClick;
        lueProvince.EditValueChanged += ProvinceLookupEditValueChanged;
        UpdateLookupButtons();
    }

    private static void ConfigureLookup(NuanLookupEdit lookup, bool canCreate)
    {
        lookup.RefreshButtons();
        lookup.ClearButtonEnabled = false;
        lookup.CreateButtonEnabled = canCreate;
        lookup.EditButtonEnabled = false;
        lookup.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lookup.Properties.SearchMode = SearchMode.AutoSearch;
        lookup.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
    }

    private void BindCountries()
    {
        BindLookup(lueCountry, countries);
    }

    private void BindProvinces()
    {
        BindLookup(lueProvince, provinces);
    }

    private static void BindLookup(NuanLookupEdit lookup, IReadOnlyCollection<GeographyLookupItem> items)
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
        suppressCountryChange = true;
        lueCountry.EditValue = item.CountryId;
        lueProvince.EditValue = item.ProvinceId;
        suppressCountryChange = false;
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        chkIsActive.Checked = item.IsActive;
        UpdateLookupButtons();
    }

    private async void CountryLookupEditValueChanged(object? sender, EventArgs e)
    {
        UpdateLookupButtons();
        if (suppressCountryChange)
        {
            return;
        }

        await RunWithUiExceptionHandlingAsync(() => ReloadProvincesAsync());
    }

    private void ProvinceLookupEditValueChanged(object? sender, EventArgs e)
    {
        UpdateLookupButtons();
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

    private async void ProvinceLookupCreateButtonClick(object? sender, EventArgs e)
    {
        if (lueCountry.EditValue is not null)
        {
            await ManageProvinceAsync(null, Convert.ToInt32(lueCountry.EditValue));
        }
    }

    private async void ProvinceLookupEditButtonClick(object? sender, EventArgs e)
    {
        if (lueCountry.EditValue is not null && lueProvince.EditValue is not null)
        {
            await ManageProvinceAsync(
                Convert.ToInt32(lueProvince.EditValue),
                Convert.ToInt32(lueCountry.EditValue));
        }
    }

    private async Task ManageCountryAsync(int? countryId)
    {
        var hasAccess = countryId.HasValue ? canUpdateCountries : canCreateCountries;
        if (managingLookup || !hasAccess)
        {
            return;
        }

        if ((!countryId.HasValue && CreateCountryRequested is null)
            || (countryId.HasValue && EditCountryRequested is null))
        {
            return;
        }

        managingLookup = true;
        SetLookupsEnabled(false);
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
                int? preferredProvinceId = lueProvince.EditValue is null
                    ? null
                    : Convert.ToInt32(lueProvince.EditValue);
                suppressCountryChange = true;
                UpsertCountry(saved);
                lueCountry.EditValue = saved.Id;
                suppressCountryChange = false;
                await RunWithUiExceptionHandlingAsync(() => ReloadProvincesAsync(preferredProvinceId));
            }
        }
        finally
        {
            managingLookup = false;
            SetLookupsEnabled(true);
            UpdateLookupButtons();
        }
    }

    private async Task ManageProvinceAsync(int? provinceId, int countryId)
    {
        var hasAccess = provinceId.HasValue ? canUpdateProvinces : canCreateProvinces;
        if (managingLookup || !hasAccess)
        {
            return;
        }

        if ((!provinceId.HasValue && CreateProvinceRequested is null)
            || (provinceId.HasValue && EditProvinceRequested is null))
        {
            return;
        }

        managingLookup = true;
        SetLookupsEnabled(false);
        try
        {
            ProvinceItem? saved = null;
            await RunWithUiExceptionHandlingAsync(async () =>
            {
                saved = provinceId.HasValue
                    ? await EditProvinceRequested!(this, provinceId.Value)
                    : await CreateProvinceRequested!(this, countryId);
            });

            if (saved is not null)
            {
                await SelectProvinceAsync(saved);
            }
        }
        finally
        {
            managingLookup = false;
            SetLookupsEnabled(true);
            UpdateLookupButtons();
        }
    }

    private async Task ReloadProvincesAsync(int? preferredProvinceId = null)
    {
        var loadVersion = ++provinceLoadVersion;
        var selectedCountry = SelectedCountry();
        provinces.Clear();
        lueProvince.Properties.DataSource = null;
        BindProvinces();
        lueProvince.EditValue = null;
        UpdateLookupButtons();

        if (selectedCountry is null || LoadProvincesRequested is null)
        {
            return;
        }

        lueProvince.Enabled = false;
        try
        {
            var loaded = await LoadProvincesRequested(this, selectedCountry.Code);
            if (loadVersion != provinceLoadVersion
                || Convert.ToInt32(lueCountry.EditValue) != selectedCountry.Id)
            {
                return;
            }

            provinces.AddRange(loaded);
            lueProvince.Properties.DataSource = null;
            BindProvinces();
            if (preferredProvinceId.HasValue && provinces.Any(item => item.Id == preferredProvinceId.Value))
            {
                lueProvince.EditValue = preferredProvinceId.Value;
            }
        }
        finally
        {
            if (loadVersion == provinceLoadVersion)
            {
                lueProvince.Enabled = lueCountry.EditValue is not null;
            }

            UpdateLookupButtons();
        }
    }

    private async Task SelectProvinceAsync(ProvinceItem province)
    {
        if (countries.All(item => item.Id != province.CountryId))
        {
            suppressCountryChange = true;
            countries.Add(new GeographyLookupItem
            {
                Id = province.CountryId,
                Code = province.CountryCode,
                Name = province.CountryName,
                IsActive = true
            });
            lueCountry.Properties.DataSource = null;
            BindCountries();
            suppressCountryChange = false;
        }

        suppressCountryChange = true;
        lueCountry.EditValue = province.CountryId;
        suppressCountryChange = false;
        await RunWithUiExceptionHandlingAsync(() => ReloadProvincesAsync(province.Id));
        UpsertProvince(province);
        lueProvince.EditValue = province.Id;
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
        Upsert(countries, lookup);
        lueCountry.Properties.DataSource = null;
        BindCountries();
    }

    private void UpsertProvince(ProvinceItem province)
    {
        Upsert(provinces, new GeographyLookupItem
        {
            Id = province.Id,
            Code = province.Code,
            Name = province.Name,
            IsActive = province.IsActive
        });
        lueProvince.Properties.DataSource = null;
        BindProvinces();
    }

    private static void Upsert(List<GeographyLookupItem> items, GeographyLookupItem value)
    {
        var index = items.FindIndex(item => item.Id == value.Id);
        if (index >= 0)
        {
            items[index] = value;
        }
        else
        {
            items.Add(value);
        }
    }

    private GeographyLookupItem? SelectedCountry()
    {
        return lueCountry.EditValue is null
            ? null
            : countries.FirstOrDefault(item => item.Id == Convert.ToInt32(lueCountry.EditValue));
    }

    private void SetLookupsEnabled(bool enabled)
    {
        lueCountry.Enabled = enabled;
        lueProvince.Enabled = enabled && lueCountry.EditValue is not null;
    }

    private void UpdateLookupButtons()
    {
        var hasCountry = lueCountry.EditValue is not null;
        lueProvince.Enabled = !managingLookup && hasCountry;
        lueCountry.CreateButtonEnabled = canCreateCountries && !managingLookup;
        lueCountry.EditButtonEnabled = canUpdateCountries && !managingLookup && hasCountry;
        lueProvince.CreateButtonEnabled = canCreateProvinces && !managingLookup && hasCountry;
        lueProvince.EditButtonEnabled = canUpdateProvinces
            && !managingLookup
            && hasCountry
            && lueProvince.EditValue is not null;
    }
}
