using FluentAssertions;
using NSubstitute;
using NuanSystem.WinForms.Services.Definitions.General.Common;
using NuanSystem.WinForms.Services.Security.Access;
using NuanSystem.WinForms.Services.Security.Access.Models;
using NuanSystem.WinForms.ViewModels.Definitions.General.Cities;

namespace NuanSystem.Application.Tests.Features.Definitions.General.Common;

public sealed class GeographyEditFormContractTests
{
    [Fact]
    public void GeographyEditors_ShouldUseCorporateControlsAndCompactVerticalRhythm()
    {
        var countryDesigner = Read(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Definitions", "General",
            "Countries", "CountryEditForm.Designer.cs");
        var provinceDesigner = Read(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Definitions", "General",
            "Provinces", "ProvinceEditForm.Designer.cs");
        var cityDesigner = Read(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Definitions", "General",
            "Cities", "CityEditForm.Designer.cs");

        countryDesigner.Should().Contain("txtCode.Location = new Point(170, 26)")
            .And.Contain("txtName.Location = new Point(170, 54)")
            .And.Contain("txtIso2.Location = new Point(170, 82)")
            .And.Contain("txtPhonePrefix.Location = new Point(170, 110)")
            .And.NotContain("private SimpleButton btnSave")
            .And.NotContain("private SimpleButton btnCancel");

        provinceDesigner.Should().Contain("lueCountry = new NuanLookupEdit()")
            .And.Contain("lueCountry.Location = new Point(170, 26)")
            .And.Contain("txtCode.Location = new Point(170, 54)")
            .And.Contain("txtName.Location = new Point(170, 82)")
            .And.NotContain("private LookUpEdit lueCountry")
            .And.NotContain("private SimpleButton btnSave");

        cityDesigner.Should().Contain("lueCountry = new NuanLookupEdit()")
            .And.Contain("lueProvince = new NuanLookupEdit()")
            .And.Contain("lueCountry.Location = new Point(170, 26)")
            .And.Contain("lueProvince.Location = new Point(170, 54)")
            .And.Contain("txtCode.Location = new Point(170, 82)")
            .And.Contain("txtName.Location = new Point(170, 110)")
            .And.NotContain("private LookUpEdit lueCountry")
            .And.NotContain("private LookUpEdit lueProvince")
            .And.NotContain("private SimpleButton btnSave");
    }

    [Fact]
    public void GeographyLookups_ShouldSupportPermissionAwareCreateEditAndCountryFiltering()
    {
        var lookup = Read(
            "src", "Frontend", "NuanSystem.WinForms.Controls", "Lookups", "NuanLookupEdit.cs");
        var provinceEditor = Read(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Definitions", "General",
            "Provinces", "ProvinceEditForm.cs");
        var cityEditor = Read(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Definitions", "General",
            "Cities", "CityEditForm.cs");
        var citiesForm = Read(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Definitions", "General",
            "Cities", "CitiesForm.cs");

        lookup.Should().Contain("public event EventHandler? EditButtonClick")
            .And.Contain("public bool EditButtonEnabled")
            .And.Contain("ButtonPredefines.Ellipsis")
            .And.Contain("Editar seleccionado");

        provinceEditor.Should().Contain("CreateCountryRequested")
            .And.Contain("EditCountryRequested")
            .And.Contain("lueCountry.ClearButtonEnabled = false")
            .And.Contain("lueCountry.CreateButtonEnabled = canCreateCountries")
            .And.Contain("lueCountry.EditButtonEnabled = canUpdateCountries");

        cityEditor.Should().Contain("LoadProvincesRequested(this, selectedCountry.Code)")
            .And.Contain("provinceLoadVersion")
            .And.Contain("CreateCountryRequested")
            .And.Contain("EditCountryRequested")
            .And.Contain("CreateProvinceRequested")
            .And.Contain("EditProvinceRequested")
            .And.Contain("lueProvince.EditValue = null");

        citiesForm.Should().Contain("viewModel.LoadProvincesAsync(countryCode)")
            .And.Contain("viewModel.CanCreateCountries")
            .And.Contain("viewModel.CanUpdateCountries")
            .And.Contain("viewModel.CanCreateProvinces")
            .And.Contain("viewModel.CanUpdateProvinces")
            .And.NotContain("HasPermission(PermissionCodes.GeographyCountriesManage)")
            .And.NotContain("HasPermission(PermissionCodes.GeographyProvincesManage)")
            .And.Contain("CreateProvinceEditor");
    }

    [Fact]
    public async Task CitiesViewModel_ShouldLoadOnlyProvincesForSelectedCountryCode()
    {
        var client = Substitute.For<IGeographyClient>();
        var expected = new[]
        {
            new GeographyLookupItem { Id = 9, Code = "09", Name = "Guayas", IsActive = true }
        };
        client.GetProvinceLookupAsync("EC", Arg.Any<CancellationToken>())
            .Returns(expected);
        var accessClient = Substitute.For<ISecurityAccessClient>();
        var viewModel = new CitiesViewModel(client, accessClient);

        var result = await viewModel.LoadProvincesAsync("EC");

        result.Should().BeEquivalentTo(expected);
        viewModel.Provinces.Should().BeEquivalentTo(expected);
        await client.Received(1).GetProvinceLookupAsync("EC", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CitiesViewModel_ShouldUseRelatedFormOperationsAndFailClosed()
    {
        var client = Substitute.For<IGeographyClient>();
        client.GetCountryLookupAsync(Arg.Any<CancellationToken>())
            .Returns(Array.Empty<GeographyLookupItem>());
        client.GetCitiesAsync(Arg.Any<CancellationToken>())
            .Returns(Array.Empty<NuanSystem.WinForms.Services.Definitions.General.Cities.CityItem>());
        var accessClient = Substitute.For<ISecurityAccessClient>();
        accessClient.GetFormOperationsAsync("countries", Arg.Any<CancellationToken>())
            .Returns(
            [
                Operation("new", isAllowed: false),
                Operation("edit", isAllowed: true)
            ]);
        accessClient.GetFormOperationsAsync("provinces", Arg.Any<CancellationToken>())
            .Returns(
            [
                Operation("new", isAllowed: true),
                Operation("edit", isAllowed: false)
            ]);
        var viewModel = new CitiesViewModel(client, accessClient);

        await viewModel.LoadAsync();

        viewModel.CanCreateCountries.Should().BeFalse();
        viewModel.CanUpdateCountries.Should().BeTrue();
        viewModel.CanCreateProvinces.Should().BeTrue();
        viewModel.CanUpdateProvinces.Should().BeFalse();
    }

    [Fact]
    public void ShellAndGeographyApi_ShouldFailClosedAndRequireFormOperations()
    {
        var shell = Read(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Shell", "MainForm.cs");
        var baseCrud = Read(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Common", "BaseCrudListForm.cs");

        shell.Should().Contain("crudForm.ConfigureCrudOperationAccess(Array.Empty<string>())")
            .And.Contain("await crudForm.ExecuteRefreshAsync()")
            .And.Contain("ResolveCrudOperations(operations)")
            .And.Contain("ResolveOperation(operations, \"create\", \"crear\", \"new\", \"nuevo\", \"post\")")
            .And.Contain("CreateRibbonButton(\"Consultar\", \"Operaciones/consultar_32.svg\"")
            .And.Contain("await RefreshActiveRibbonOperationsAsync();")
            .And.Contain("ApplyOperationAccessAsync(activeModule, activeCrudForm, refreshData: false)")
            .And.Contain(
                "ApplyRibbonActionState(customButton.Button, canExecuteCustomOperation);")
            .And.Contain("UpdateRibbonGroupVisibility();")
            .And.Contain("Operaciones/consultar_32.svg");
        baseCrud.Replace("\r\n", "\n", StringComparison.Ordinal).Should().Contain(
            "protected override async void OnShown(EventArgs e)\n"
            + "    {\n"
            + "        base.OnShown(e);\n"
            + "        await ExecuteRefreshAsync();\n"
            + "    }");
    }

    private static FormOperationAccessItem Operation(string actionKey, bool isAllowed)
    {
        return new FormOperationAccessItem(
            1,
            actionKey,
            actionKey,
            null,
            actionKey,
            null,
            null,
            null,
            null,
            1,
            isAllowed);
    }

    private static string Read(params string[] parts)
    {
        var root = FindRepositoryRoot();
        return File.ReadAllText(Path.Combine(new[] { root }.Concat(parts).ToArray()));
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, "database")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("No se encontró la raíz del repositorio.");
    }
}
