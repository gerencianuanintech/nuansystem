using FluentAssertions;
using NuanSystem.Application.Features.Definitions.General.Cities.Queries;
using NuanSystem.Application.Features.Definitions.General.Provinces.Queries;

namespace NuanSystem.Application.Tests.Features.Definitions.General.Common;

public sealed class ProvinceCityPagedSearchTests
{
    [Fact]
    public void Validators_ShouldRejectInvalidPagingAndOversizedSearch()
    {
        var provinceResult = new SearchProvincesQueryValidator()
            .Validate(new SearchProvincesQuery(new string('x', 121), 0, 101));
        var cityResult = new SearchCitiesQueryValidator()
            .Validate(new SearchCitiesQuery(new string('x', 121), 0, 101));

        provinceResult.Errors.Select(error => error.ErrorCode).Should().BeEquivalentTo(
            "GEOGRAPHY_PROVINCE_SEARCH_MAX_LENGTH",
            "GEOGRAPHY_PROVINCE_PAGE_NUMBER_INVALID",
            "GEOGRAPHY_PROVINCE_PAGE_SIZE_INVALID");
        cityResult.Errors.Select(error => error.ErrorCode).Should().BeEquivalentTo(
            "GEOGRAPHY_CITY_SEARCH_MAX_LENGTH",
            "GEOGRAPHY_CITY_PAGE_NUMBER_INVALID",
            "GEOGRAPHY_CITY_PAGE_SIZE_INVALID");
    }

    [Fact]
    public void Migrations181And182_ShouldProvideRelationalGlobalSearchAndBeRegistered()
    {
        var provinceSql = Read("database", "sql", "181_tenant_province_paged_search.sql");
        var citySql = Read("database", "sql", "182_tenant_city_paged_search.sql");
        var initializer = Read(
            "src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerTenantDatabaseInitializer.cs");

        provinceSql.Should().Contain("SP_NA_GET_PROVINCES_BUSCARPAGINADO")
            .And.Contain("country.Name LIKE")
            .And.Contain("province.Name LIKE")
            .And.Contain("OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY")
            .And.Contain("SELECT COUNT(1)")
            .And.Contain("N'20260807.181'");
        citySql.Should().Contain("SP_NA_GET_CITIES_BUSCARPAGINADO")
            .And.Contain("country.Name LIKE")
            .And.Contain("province.Name LIKE")
            .And.Contain("city.Name LIKE")
            .And.Contain("province.CountryId = country.CountryId")
            .And.Contain("OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY")
            .And.Contain("SELECT COUNT(1)")
            .And.Contain("N'20260807.182'");
        initializer.Should().Contain("181_tenant_province_paged_search.sql")
            .And.Contain("182_tenant_city_paged_search.sql");
    }

    [Theory]
    [InlineData("Provinces", "ProvinceEndpoints.cs", "/provinces/page", "ProvincesForm.cs")]
    [InlineData("Cities", "CityEndpoints.cs", "/cities/page", "CitiesForm.cs")]
    public void Forms_ShouldUseInheritedRemoteFind(
        string feature,
        string endpointFile,
        string route,
        string formFile)
    {
        var endpoint = Read(
            "src", "Backend", "NuanSystem.Api", "Endpoints", "Definitions", "General",
            feature, endpointFile);
        var form = Read(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Definitions", "General",
            feature, formFile);

        endpoint.Should().Contain(route)
            .And.Contain("RequireFormOperation(FormKey, \"refresh\")");
        form.Should().Contain("EnableServerPaging(50)")
            .And.Contain("EnableServerFind(ApplyServerFindAsync)")
            .And.Contain("viewModel.PageNumber = 1")
            .And.Contain("SetPagedGridData(")
            .And.NotContain("ColumnFilterChanged +=")
            .And.NotContain("findDebounceTimer");
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
