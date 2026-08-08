using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Lookups;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.General.Cities;
using NuanSystem.WinForms.Services.Definitions.General.Common;
using NuanSystem.WinForms.Services.Definitions.General.Countries;
using NuanSystem.WinForms.Services.Definitions.General.Provinces;
using NuanSystem.WinForms.Services.GeneralInventory.Warehouses.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.Warehouses;

public sealed partial class WarehouseEditForm : BaseEditForm
{
    private readonly List<GeographyLookupItem> countries;
    private readonly List<GeographyLookupItem> provinces;
    private readonly List<GeographyLookupItem> cities;
    private readonly bool canCreateCountries;
    private readonly bool canUpdateCountries;
    private readonly bool canCreateProvinces;
    private readonly bool canUpdateProvinces;
    private readonly bool canCreateCities;
    private readonly bool canUpdateCities;
    private bool suppressGeographyChange;
    private bool managingLookup;
    private int provinceLoadVersion;
    private int cityLoadVersion;
    private string? legacyCountry;
    private string? legacyProvince;
    private string? legacyCity;

    public WarehouseEditForm()
        : this([], [], [])
    {
    }

    public WarehouseEditForm(
        IReadOnlyCollection<GeographyLookupItem> countries,
        IReadOnlyCollection<GeographyLookupItem> provinces,
        IReadOnlyCollection<GeographyLookupItem> cities,
        bool canCreateCountries = false,
        bool canUpdateCountries = false,
        bool canCreateProvinces = false,
        bool canUpdateProvinces = false,
        bool canCreateCities = false,
        bool canUpdateCities = false)
    {
        this.countries = countries.ToList();
        this.provinces = provinces.ToList();
        this.cities = cities.ToList();
        this.canCreateCountries = canCreateCountries;
        this.canUpdateCountries = canUpdateCountries;
        this.canCreateProvinces = canCreateProvinces;
        this.canUpdateProvinces = canUpdateProvinces;
        this.canCreateCities = canCreateCities;
        this.canUpdateCities = canUpdateCities;
        InitializeComponent();
        ConfigureLookups();
        BindLookups();
        chkIsActive.Checked = true;
        chkAllowsSales.Checked = true;
        chkAllowsPurchases.Checked = true;
        chkAllowsTransfers.Checked = true;
        UpdateLookupState();
    }

    public WarehouseEditForm(
        IReadOnlyCollection<GeographyLookupItem> countries,
        IReadOnlyCollection<GeographyLookupItem> provinces,
        IReadOnlyCollection<GeographyLookupItem> cities,
        WarehouseItem item,
        bool copyMode = false,
        bool canCreateCountries = false,
        bool canUpdateCountries = false,
        bool canCreateProvinces = false,
        bool canUpdateProvinces = false,
        bool canCreateCities = false,
        bool canUpdateCities = false)
        : this(
            countries,
            provinces,
            cities,
            canCreateCountries,
            canUpdateCountries,
            canCreateProvinces,
            canUpdateProvinces,
            canCreateCities,
            canUpdateCities)
    {
        EnsureLinkedGeography(item);
        LoadWarehouse(item, copyMode);
    }

    public event Func<WarehouseEditForm, string, Task<IReadOnlyCollection<GeographyLookupItem>>>? LoadProvincesRequested;

    public event Func<WarehouseEditForm, string, string, Task<IReadOnlyCollection<GeographyLookupItem>>>? LoadCitiesRequested;

    public event Func<WarehouseEditForm, Task<CountryItem?>>? CreateCountryRequested;
    public event Func<WarehouseEditForm, int, Task<CountryItem?>>? EditCountryRequested;
    public event Func<WarehouseEditForm, int, Task<ProvinceItem?>>? CreateProvinceRequested;
    public event Func<WarehouseEditForm, int, Task<ProvinceItem?>>? EditProvinceRequested;
    public event Func<WarehouseEditForm, int, int, Task<CityItem?>>? CreateCityRequested;
    public event Func<WarehouseEditForm, int, Task<CityItem?>>? EditCityRequested;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveWarehouseRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= Validator.RequireText(txtCode, "Ingrese el codigo.");
        isValid &= Validator.RequireText(txtName, "Ingrese el nombre.");
        return isValid;
    }

    protected override void BuildRequest()
    {
        var selectedCountry = Selected(countries, lueCountry);
        var selectedProvince = Selected(provinces, lueProvince);
        var selectedCity = Selected(cities, lueCity);
        Request = new SaveWarehouseRequest(
            GetGlobalId(),
            txtCode.Text.Trim(),
            txtName.Text.Trim(),
            NormalizeText(memDescription.Text),
            NormalizeText(txtBranchCode.Text),
            NormalizeText(txtAddress.Text),
            selectedCity?.Name ?? legacyCity,
            selectedProvince?.Name ?? legacyProvince,
            selectedCountry?.Name ?? legacyCountry,
            NormalizeText(txtPhone.Text),
            NormalizeText(txtEmail.Text),
            NormalizeText(txtManagerName.Text),
            chkAllowsSales.Checked,
            chkAllowsPurchases.Checked,
            chkAllowsTransfers.Checked,
            chkAllowsProduction.Checked,
            chkIsDefault.Checked,
            NormalizeText(txtExternalSystem.Text),
            NormalizeText(txtExternalCode.Text),
            NormalizeText(txtSapCode.Text),
            chkIsActive.Checked,
            selectedCountry?.Id,
            selectedProvince?.Id,
            selectedCity?.Id);
    }

    private void ConfigureLookups()
    {
        ConfigureLookup(lueCountry, canCreateCountries);
        ConfigureLookup(lueProvince, canCreateProvinces);
        ConfigureLookup(lueCity, canCreateCities);
        lueCountry.EditValueChanged += CountryLookupEditValueChanged;
        lueProvince.EditValueChanged += ProvinceLookupEditValueChanged;
        lueCity.EditValueChanged += CityLookupEditValueChanged;
        lueCountry.CreateButtonClick += CountryLookupCreateButtonClick;
        lueCountry.EditButtonClick += CountryLookupEditButtonClick;
        lueProvince.CreateButtonClick += ProvinceLookupCreateButtonClick;
        lueProvince.EditButtonClick += ProvinceLookupEditButtonClick;
        lueCity.CreateButtonClick += CityLookupCreateButtonClick;
        lueCity.EditButtonClick += CityLookupEditButtonClick;
    }

    private static void ConfigureLookup(NuanLookupEdit lookup, bool canCreate)
    {
        lookup.RefreshButtons();
        lookup.ClearButtonEnabled = true;
        lookup.CreateButtonEnabled = canCreate;
        lookup.EditButtonEnabled = false;
        lookup.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lookup.Properties.SearchMode = SearchMode.AutoSearch;
        lookup.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
    }

    private void BindLookups()
    {
        BindLookup(lueCountry, countries);
        BindLookup(lueProvince, provinces);
        BindLookup(lueCity, cities);
    }

    private static void BindLookup(NuanLookupEdit lookup, IReadOnlyCollection<GeographyLookupItem> items)
    {
        lookup.Properties.DataSource = items;
        lookup.Properties.DisplayMember = nameof(GeographyLookupItem.Name);
        lookup.Properties.ValueMember = nameof(GeographyLookupItem.Id);
        lookup.Properties.Columns.Clear();
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(GeographyLookupItem.Code), "Codigo", 80));
        lookup.Properties.Columns.Add(new LookUpColumnInfo(nameof(GeographyLookupItem.Name), "Nombre", 180));
    }

    private void LoadWarehouse(WarehouseItem item, bool copyMode)
    {
        Text = copyMode ? "Copiar bodega" : "Editar bodega";
        legacyCountry = item.Country;
        legacyProvince = item.Province;
        legacyCity = item.City;
        UpdateLegacyDisplay(item);
        suppressGeographyChange = true;
        lueCountry.EditValue = item.CountryId;
        lueProvince.EditValue = item.ProvinceId;
        lueCity.EditValue = item.CityId;
        suppressGeographyChange = false;
        txtGlobalId.Text = copyMode ? string.Empty : item.GlobalId.ToString();
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        txtBranchCode.Text = item.BranchCode;
        txtAddress.Text = item.Address;
        txtPhone.Text = item.Phone;
        txtEmail.Text = item.Email;
        txtManagerName.Text = item.ManagerName;
        chkAllowsSales.Checked = item.AllowsSales;
        chkAllowsPurchases.Checked = item.AllowsPurchases;
        chkAllowsTransfers.Checked = item.AllowsTransfers;
        chkAllowsProduction.Checked = item.AllowsProduction;
        chkIsDefault.Checked = item.IsDefault;
        txtExternalSystem.Text = item.ExternalSystem;
        txtExternalCode.Text = item.ExternalCode;
        txtSapCode.Text = item.SapCode;
        chkIsActive.Checked = item.IsActive;
        UpdateLookupState();
    }

    private async void CountryLookupEditValueChanged(object? sender, EventArgs e)
    {
        UpdateLookupState();
        if (suppressGeographyChange)
        {
            return;
        }

        legacyCountry = Selected(countries, lueCountry)?.Name;
        legacyProvince = null;
        legacyCity = null;
        ClearLegacyDisplay(lueCountry);
        ClearLegacyDisplay(lueProvince);
        ClearLegacyDisplay(lueCity);
        await RunWithUiExceptionHandlingAsync(() => ReloadProvincesAsync());
    }

    private async void ProvinceLookupEditValueChanged(object? sender, EventArgs e)
    {
        UpdateLookupState();
        if (suppressGeographyChange)
        {
            return;
        }

        legacyProvince = Selected(provinces, lueProvince)?.Name;
        legacyCity = null;
        ClearLegacyDisplay(lueProvince);
        ClearLegacyDisplay(lueCity);
        await RunWithUiExceptionHandlingAsync(() => ReloadCitiesAsync());
    }

    private void CityLookupEditValueChanged(object? sender, EventArgs e)
    {
        if (!suppressGeographyChange)
        {
            legacyCity = Selected(cities, lueCity)?.Name;
            ClearLegacyDisplay(lueCity);
        }
    }

    private async void CountryLookupCreateButtonClick(object? sender, EventArgs e) =>
        await ManageCountryAsync(null);

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

    private async void CityLookupCreateButtonClick(object? sender, EventArgs e)
    {
        if (lueCountry.EditValue is not null && lueProvince.EditValue is not null)
        {
            await ManageCityAsync(
                null,
                Convert.ToInt32(lueCountry.EditValue),
                Convert.ToInt32(lueProvince.EditValue));
        }
    }

    private async void CityLookupEditButtonClick(object? sender, EventArgs e)
    {
        if (lueCountry.EditValue is not null
            && lueProvince.EditValue is not null
            && lueCity.EditValue is not null)
        {
            await ManageCityAsync(
                Convert.ToInt32(lueCity.EditValue),
                Convert.ToInt32(lueCountry.EditValue),
                Convert.ToInt32(lueProvince.EditValue));
        }
    }

    private async Task ManageCountryAsync(int? countryId)
    {
        var hasAccess = countryId.HasValue ? canUpdateCountries : canCreateCountries;
        if (managingLookup || !hasAccess
            || (!countryId.HasValue && CreateCountryRequested is null)
            || (countryId.HasValue && EditCountryRequested is null))
        {
            return;
        }

        managingLookup = true;
        UpdateLookupState();
        try
        {
            CountryItem? saved = null;
            await RunWithUiExceptionHandlingAsync(async () =>
            {
                saved = countryId.HasValue
                    ? await EditCountryRequested!(this, countryId.Value)
                    : await CreateCountryRequested!(this);
            });
            if (saved is null)
            {
                return;
            }

            var preferredProvinceId = Selected(provinces, lueProvince)?.Id;
            var preferredCityId = Selected(cities, lueCity)?.Id;
            Upsert(countries, ToLookup(saved));
            RebindLookup(lueCountry, countries);
            suppressGeographyChange = true;
            lueCountry.EditValue = saved.Id;
            suppressGeographyChange = false;
            legacyCountry = saved.Name;
            ClearLegacyDisplay(lueCountry);
            await RunWithUiExceptionHandlingAsync(
                () => ReloadProvincesAsync(preferredProvinceId, preferredCityId));
        }
        finally
        {
            managingLookup = false;
            UpdateLookupState();
        }
    }

    private async Task ManageProvinceAsync(int? provinceId, int countryId)
    {
        var hasAccess = provinceId.HasValue ? canUpdateProvinces : canCreateProvinces;
        if (managingLookup || !hasAccess
            || (!provinceId.HasValue && CreateProvinceRequested is null)
            || (provinceId.HasValue && EditProvinceRequested is null))
        {
            return;
        }

        managingLookup = true;
        UpdateLookupState();
        try
        {
            ProvinceItem? saved = null;
            await RunWithUiExceptionHandlingAsync(async () =>
            {
                saved = provinceId.HasValue
                    ? await EditProvinceRequested!(this, provinceId.Value)
                    : await CreateProvinceRequested!(this, countryId);
            });
            if (saved is null)
            {
                return;
            }

            var preferredCityId = Selected(cities, lueCity)?.Id;
            await RunWithUiExceptionHandlingAsync(() => SelectProvinceAsync(saved, preferredCityId));
        }
        finally
        {
            managingLookup = false;
            UpdateLookupState();
        }
    }

    private async Task ManageCityAsync(int? cityId, int countryId, int provinceId)
    {
        var hasAccess = cityId.HasValue ? canUpdateCities : canCreateCities;
        if (managingLookup || !hasAccess
            || (!cityId.HasValue && CreateCityRequested is null)
            || (cityId.HasValue && EditCityRequested is null))
        {
            return;
        }

        managingLookup = true;
        UpdateLookupState();
        try
        {
            CityItem? saved = null;
            await RunWithUiExceptionHandlingAsync(async () =>
            {
                saved = cityId.HasValue
                    ? await EditCityRequested!(this, cityId.Value)
                    : await CreateCityRequested!(this, countryId, provinceId);
            });
            if (saved is null)
            {
                return;
            }

            await RunWithUiExceptionHandlingAsync(() => SelectCityAsync(saved));
        }
        finally
        {
            managingLookup = false;
            UpdateLookupState();
        }
    }

    private static GeographyLookupItem ToLookup(CountryItem item) =>
        new() { Id = item.Id, Code = item.Code, Name = item.Name, IsActive = item.IsActive };

    private static GeographyLookupItem ToLookup(ProvinceItem item) =>
        new() { Id = item.Id, Code = item.Code, Name = item.Name, IsActive = item.IsActive };

    private static GeographyLookupItem ToLookup(CityItem item) =>
        new() { Id = item.Id, Code = item.Code, Name = item.Name, IsActive = item.IsActive };

    private async Task SelectProvinceAsync(ProvinceItem province, int? preferredCityId)
    {
        AddMissing(countries, province.CountryId, province.CountryCode, province.CountryName);
        RebindLookup(lueCountry, countries);
        suppressGeographyChange = true;
        lueCountry.EditValue = province.CountryId;
        suppressGeographyChange = false;
        await ReloadProvincesAsync();
        Upsert(provinces, ToLookup(province));
        RebindLookup(lueProvince, provinces);
        suppressGeographyChange = true;
        lueProvince.EditValue = province.Id;
        suppressGeographyChange = false;
        legacyCountry = province.CountryName;
        legacyProvince = province.Name;
        ClearLegacyDisplay(lueCountry);
        ClearLegacyDisplay(lueProvince);
        await ReloadCitiesAsync(preferredCityId);
    }

    private async Task SelectCityAsync(CityItem city)
    {
        AddMissing(countries, city.CountryId, city.CountryCode, city.CountryName);
        RebindLookup(lueCountry, countries);
        suppressGeographyChange = true;
        lueCountry.EditValue = city.CountryId;
        suppressGeographyChange = false;
        await ReloadProvincesAsync();
        Upsert(provinces, new GeographyLookupItem
        {
            Id = city.ProvinceId,
            Code = city.ProvinceCode,
            Name = city.ProvinceName,
            IsActive = true
        });
        RebindLookup(lueProvince, provinces);
        suppressGeographyChange = true;
        lueProvince.EditValue = city.ProvinceId;
        suppressGeographyChange = false;
        await ReloadCitiesAsync();
        Upsert(cities, ToLookup(city));
        RebindLookup(lueCity, cities);
        suppressGeographyChange = true;
        lueCity.EditValue = city.Id;
        suppressGeographyChange = false;
        legacyCountry = city.CountryName;
        legacyProvince = city.ProvinceName;
        legacyCity = city.Name;
        ClearLegacyDisplay(lueCountry);
        ClearLegacyDisplay(lueProvince);
        ClearLegacyDisplay(lueCity);
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

    private void UpdateLegacyDisplay(WarehouseItem item)
    {
        SetLegacyDisplay(lueCountry, item.CountryId, item.Country);
        SetLegacyDisplay(lueProvince, item.ProvinceId, item.Province);
        SetLegacyDisplay(lueCity, item.CityId, item.City);
    }

    private static void SetLegacyDisplay(NuanLookupEdit lookup, int? linkedId, string? legacyValue)
    {
        if (!linkedId.HasValue && !string.IsNullOrWhiteSpace(legacyValue))
        {
            lookup.Properties.NullText = legacyValue;
            lookup.ToolTip = "Valor historico o externo pendiente de homologacion.";
        }
    }

    private static void ClearLegacyDisplay(NuanLookupEdit lookup)
    {
        lookup.Properties.NullText = string.Empty;
        lookup.ToolTip = string.Empty;
    }

    private void EnsureLinkedGeography(WarehouseItem item)
    {
        AddMissing(countries, item.CountryId, item.CountryCode, item.Country);
        AddMissing(provinces, item.ProvinceId, item.ProvinceCode, item.Province);
        AddMissing(cities, item.CityId, item.CityCode, item.City);
        BindLookups();
    }

    private static void AddMissing(
        List<GeographyLookupItem> items,
        int? id,
        string? code,
        string? name)
    {
        if (!id.HasValue || items.Any(item => item.Id == id.Value))
        {
            return;
        }

        items.Add(new GeographyLookupItem
        {
            Id = id.Value,
            Code = code ?? string.Empty,
            Name = name ?? code ?? id.Value.ToString(),
            IsActive = false
        });
    }

    private async Task ReloadProvincesAsync(int? preferredProvinceId = null, int? preferredCityId = null)
    {
        var version = ++provinceLoadVersion;
        ++cityLoadVersion;
        var country = Selected(countries, lueCountry);
        ClearLookup(lueProvince, provinces);
        ClearLookup(lueCity, cities);
        UpdateLookupState();
        if (country is null || LoadProvincesRequested is null)
        {
            return;
        }

        lueProvince.Enabled = false;
        var loaded = await LoadProvincesRequested(this, country.Code);
        if (version != provinceLoadVersion || Selected(countries, lueCountry)?.Id != country.Id)
        {
            return;
        }

        provinces.AddRange(loaded);
        RebindLookup(lueProvince, provinces);
        if (preferredProvinceId.HasValue && provinces.Any(item => item.Id == preferredProvinceId.Value))
        {
            suppressGeographyChange = true;
            lueProvince.EditValue = preferredProvinceId.Value;
            suppressGeographyChange = false;
            await ReloadCitiesAsync(preferredCityId);
        }
        UpdateLookupState();
    }

    private async Task ReloadCitiesAsync(int? preferredCityId = null)
    {
        var version = ++cityLoadVersion;
        var country = Selected(countries, lueCountry);
        var province = Selected(provinces, lueProvince);
        ClearLookup(lueCity, cities);
        UpdateLookupState();
        if (country is null || province is null || LoadCitiesRequested is null)
        {
            return;
        }

        lueCity.Enabled = false;
        var loaded = await LoadCitiesRequested(this, country.Code, province.Code);
        if (version != cityLoadVersion
            || Selected(countries, lueCountry)?.Id != country.Id
            || Selected(provinces, lueProvince)?.Id != province.Id)
        {
            return;
        }

        cities.AddRange(loaded);
        RebindLookup(lueCity, cities);
        if (preferredCityId.HasValue && cities.Any(item => item.Id == preferredCityId.Value))
        {
            suppressGeographyChange = true;
            lueCity.EditValue = preferredCityId.Value;
            suppressGeographyChange = false;
        }
        UpdateLookupState();
    }

    private static void ClearLookup(NuanLookupEdit lookup, List<GeographyLookupItem> items)
    {
        lookup.EditValue = null;
        items.Clear();
        RebindLookup(lookup, items);
    }

    private static void RebindLookup(NuanLookupEdit lookup, IReadOnlyCollection<GeographyLookupItem> items)
    {
        lookup.Properties.DataSource = null;
        BindLookup(lookup, items);
    }

    private void UpdateLookupState()
    {
        var hasCountry = lueCountry.EditValue is not null;
        var hasProvince = lueProvince.EditValue is not null;
        var hasCity = lueCity.EditValue is not null;
        lueCountry.Enabled = !managingLookup;
        lueProvince.Enabled = !managingLookup && hasCountry;
        lueCity.Enabled = !managingLookup && hasCountry && hasProvince;
        lueCountry.CreateButtonEnabled = canCreateCountries && !managingLookup;
        lueCountry.EditButtonEnabled = canUpdateCountries && !managingLookup && hasCountry;
        lueProvince.CreateButtonEnabled = canCreateProvinces && !managingLookup && hasCountry;
        lueProvince.EditButtonEnabled = canUpdateProvinces && !managingLookup && hasCountry && hasProvince;
        lueCity.CreateButtonEnabled = canCreateCities && !managingLookup && hasCountry && hasProvince;
        lueCity.EditButtonEnabled = canUpdateCities && !managingLookup && hasCountry && hasProvince && hasCity;
    }

    private static GeographyLookupItem? Selected(List<GeographyLookupItem> items, NuanLookupEdit lookup) =>
        lookup.EditValue is null
            ? null
            : items.FirstOrDefault(item => item.Id == Convert.ToInt32(lookup.EditValue));

    private Guid? GetGlobalId() =>
        Guid.TryParse(txtGlobalId.Text, out var globalId) && globalId != Guid.Empty ? globalId : null;

    private static string? NormalizeText(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static SaveWarehouseRequest EmptyRequest() =>
        new(null, string.Empty, string.Empty, null, null, null, null, null, null, null, null, null, true, true, true, false, false, null, null, null, true);
}
