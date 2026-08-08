using FluentAssertions;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;

namespace NuanSystem.Application.Tests.Features.Warehouses;

public sealed class WarehouseGeographyRelationshipTests
{
    [Fact]
    public void Validator_ShouldRequireParentSelectorsForProvinceAndCity()
    {
        var provinceWithoutCountry = new CreateWarehouseCommandValidator()
            .Validate(CreateCommand(countryId: null, provinceId: 10, cityId: null));
        var cityWithoutProvince = new CreateWarehouseCommandValidator()
            .Validate(CreateCommand(countryId: 1, provinceId: null, cityId: 20));

        provinceWithoutCountry.Errors.Should().Contain(error => error.PropertyName == nameof(CreateWarehouseCommand.ProvinceId));
        cityWithoutProvince.Errors.Should().Contain(error => error.PropertyName == nameof(CreateWarehouseCommand.CityId));
    }

    [Fact]
    public void Migration183_ShouldAddOptionalRelationsSafeBackfillAndHierarchyDefense()
    {
        var migration = ReadSource("database", "sql", "183_tenant_warehouse_geography_relationships.sql");
        var initializer = ReadSource(
            "src", "Backend", "NuanSystem.Persistence", "Services", "SqlServerTenantDatabaseInitializer.cs");

        migration.Should().Contain("ALTER TABLE dbo.Warehouses ADD CountryId int NULL")
            .And.Contain("ALTER TABLE dbo.Warehouses ADD ProvinceId int NULL")
            .And.Contain("ALTER TABLE dbo.Warehouses ADD CityId int NULL")
            .And.Contain("HAVING COUNT(*) = 1")
            .And.Contain("FK_Warehouses_Countries_CountryId")
            .And.Contain("FK_Warehouses_Provinces_ProvinceId")
            .And.Contain("FK_Warehouses_Cities_CityId")
            .And.Contain("REFERENCES dbo.Countries(CountryId)")
            .And.Contain("REFERENCES dbo.Provinces(ProvinceId)")
            .And.Contain("REFERENCES dbo.Cities(CityId)")
            .And.Contain("country.CountryId=warehouse.CountryId")
            .And.Contain("province.ProvinceId=warehouse.ProvinceId")
            .And.Contain("city.CityId=warehouse.CityId")
            .And.Contain("ProvinceId does not belong to CountryId")
            .And.Contain("CityId does not belong to CountryId and ProvinceId")
            .And.Contain("COALESCE(city.Name, warehouse.City)")
            .And.Contain("N'20260808.183'");
        initializer.Should().Contain("183_tenant_warehouse_geography_relationships.sql");
    }

    [Fact]
    public void Geography_ShouldStayLocalAndWinFormsShouldUseCorporateCascadeLookups()
    {
        typeof(WarehouseSyncPayload).GetProperty("CountryId").Should().BeNull();
        typeof(WarehouseSyncPayload).GetProperty("ProvinceId").Should().BeNull();
        typeof(WarehouseSyncPayload).GetProperty("CityId").Should().BeNull();

        var editor = ReadSource(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "GeneralInventory", "Warehouses",
            "WarehouseEditForm.cs");
        var designer = ReadSource(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "GeneralInventory", "Warehouses",
            "WarehouseEditForm.Designer.cs");
        var form = ReadSource(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "GeneralInventory", "Warehouses",
            "WarehousesForm.cs");
        var viewModel = ReadSource(
            "src", "Frontend", "NuanSystem.WinForms.ViewModels", "GeneralInventory", "Warehouses",
            "WarehousesViewModel.cs");

        designer.Should().Contain("NuanLookupEdit lueCountry")
            .And.Contain("NuanLookupEdit lueProvince")
            .And.Contain("NuanLookupEdit lueCity")
            .And.NotContain("TextEdit txtCountry")
            .And.NotContain("TextEdit txtProvince")
            .And.NotContain("TextEdit txtCity");
        editor.Should().Contain("ReloadProvincesAsync")
            .And.Contain("ReloadCitiesAsync")
            .And.Contain("ClearButtonEnabled = true")
            .And.Contain("CreateButtonEnabled = canCreate")
            .And.Contain("canCreateCountries && !managingLookup")
            .And.Contain("canCreateProvinces && !managingLookup && hasCountry")
            .And.Contain("canCreateCities && !managingLookup && hasCountry && hasProvince")
            .And.Contain("CreateCountryRequested")
            .And.Contain("CreateProvinceRequested")
            .And.Contain("CreateCityRequested");
        viewModel.Should().Contain("GeographyRelatedFormAccess.LoadAsync(securityAccessClient, \"countries\"")
            .And.Contain("GeographyRelatedFormAccess.LoadAsync(securityAccessClient, \"provinces\"")
            .And.Contain("GeographyRelatedFormAccess.LoadAsync(securityAccessClient, \"cities\"")
            .And.Contain("CanCreateCountries = countryAccess.CanCreate")
            .And.Contain("CanCreateProvinces = provinceAccess.CanCreate")
            .And.Contain("CanCreateCities = cityAccess.CanCreate");
        form.Should().Contain("form.CreateCountryRequested +=")
            .And.Contain("form.CreateProvinceRequested +=")
            .And.Contain("form.CreateCityRequested +=")
            .And.Contain("new CountryEditForm")
            .And.Contain("new ProvinceEditForm")
            .And.Contain("new CityEditForm");
    }

    private static CreateWarehouseCommand CreateCommand(int? countryId, int? provinceId, int? cityId) =>
        new(
            null, "B01", "Bodega", null, null, null, null, null, null, null, null, null,
            true, true, true, false, false, null, null, null, true, null, null,
            countryId, provinceId, cityId);

    private static string ReadSource(params string[] pathParts)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var path = Path.Combine([directory.FullName, .. pathParts]);
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException(Path.Combine(pathParts));
    }
}
