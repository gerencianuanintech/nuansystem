using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Lookups;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.General.Common;
using NuanSystem.WinForms.Services.GeneralInventory.Warehouses.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.Warehouses;

public sealed partial class WarehouseEditForm : BaseEditForm
{
    private readonly List<GeographyLookupItem> countries;
    private readonly List<GeographyLookupItem> provinces;
    private readonly List<GeographyLookupItem> cities;
    private bool suppressGeographyChange;
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
        IReadOnlyCollection<GeographyLookupItem> cities)
    {
        this.countries = countries.ToList();
        this.provinces = provinces.ToList();
        this.cities = cities.ToList();
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
        bool copyMode = false)
        : this(countries, provinces, cities)
    {
        EnsureLinkedGeography(item);
        LoadWarehouse(item, copyMode);
    }

    public event Func<WarehouseEditForm, string, Task<IReadOnlyCollection<GeographyLookupItem>>>? LoadProvincesRequested;

    public event Func<WarehouseEditForm, string, string, Task<IReadOnlyCollection<GeographyLookupItem>>>? LoadCitiesRequested;

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
        ConfigureLookup(lueCountry);
        ConfigureLookup(lueProvince);
        ConfigureLookup(lueCity);
        lueCountry.EditValueChanged += CountryLookupEditValueChanged;
        lueProvince.EditValueChanged += ProvinceLookupEditValueChanged;
        lueCity.EditValueChanged += CityLookupEditValueChanged;
    }

    private static void ConfigureLookup(NuanLookupEdit lookup)
    {
        lookup.RefreshButtons();
        lookup.ClearButtonEnabled = true;
        lookup.CreateButtonEnabled = false;
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
        await RunWithUiExceptionHandlingAsync(ReloadProvincesAsync);
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
        await RunWithUiExceptionHandlingAsync(ReloadCitiesAsync);
    }

    private void CityLookupEditValueChanged(object? sender, EventArgs e)
    {
        if (!suppressGeographyChange)
        {
            legacyCity = Selected(cities, lueCity)?.Name;
            ClearLegacyDisplay(lueCity);
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

    private async Task ReloadProvincesAsync()
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
        UpdateLookupState();
    }

    private async Task ReloadCitiesAsync()
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
        lueProvince.Enabled = lueCountry.EditValue is not null;
        lueCity.Enabled = lueCountry.EditValue is not null && lueProvince.EditValue is not null;
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
