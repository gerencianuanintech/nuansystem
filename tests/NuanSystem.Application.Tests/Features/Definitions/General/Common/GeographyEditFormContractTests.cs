using FluentAssertions;
using NSubstitute;
using NuanSystem.WinForms.Services.Definitions.General.Common;
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
            .And.Contain("lueCountry.EditButtonEnabled = canManageCountries");

        cityEditor.Should().Contain("LoadProvincesRequested(this, selectedCountry.Code)")
            .And.Contain("provinceLoadVersion")
            .And.Contain("CreateCountryRequested")
            .And.Contain("EditCountryRequested")
            .And.Contain("CreateProvinceRequested")
            .And.Contain("EditProvinceRequested")
            .And.Contain("lueProvince.EditValue = null");

        citiesForm.Should().Contain("viewModel.LoadProvincesAsync(countryCode)")
            .And.Contain("PermissionCodes.GeographyCountriesManage")
            .And.Contain("PermissionCodes.GeographyProvincesManage")
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
        var viewModel = new CitiesViewModel(client);

        var result = await viewModel.LoadProvincesAsync("EC");

        result.Should().BeEquivalentTo(expected);
        viewModel.Provinces.Should().BeEquivalentTo(expected);
        await client.Received(1).GetProvinceLookupAsync("EC", Arg.Any<CancellationToken>());
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
