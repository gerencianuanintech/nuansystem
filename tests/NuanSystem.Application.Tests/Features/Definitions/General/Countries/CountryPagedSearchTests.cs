using FluentAssertions;
using NuanSystem.Application.Features.Definitions.General.Countries.Queries;

namespace NuanSystem.Application.Tests.Features.Definitions.General.Countries;

public sealed class CountryPagedSearchTests
{
    [Fact]
    public void Validator_ShouldRejectInvalidPagingAndOversizedSearch()
    {
        var validator = new SearchCountriesQueryValidator();

        var result = validator.Validate(new SearchCountriesQuery(new string('x', 121), 0, 101));

        result.Errors.Select(error => error.ErrorCode).Should().BeEquivalentTo(
            "GEOGRAPHY_COUNTRY_SEARCH_MAX_LENGTH",
            "GEOGRAPHY_COUNTRY_PAGE_NUMBER_INVALID",
            "GEOGRAPHY_COUNTRY_PAGE_SIZE_INVALID");
    }

    [Fact]
    public void Migration180_ShouldProvideFilteredPagedResultAndBeRegistered()
    {
        var sql = Read("database", "sql", "180_tenant_country_paged_search.sql");
        var initializer = Read(
            "src", "Backend", "NuanSystem.Persistence", "Services",
            "SqlServerTenantDatabaseInitializer.cs");

        sql.Should().Contain("SP_NA_GET_COUNTRIES_BUSCARPAGINADO")
            .And.Contain("@Search nvarchar(120)")
            .And.Contain("OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY")
            .And.Contain("SELECT COUNT(1)")
            .And.Contain("IsDeleted = 0")
            .And.Contain("N'20260807.180'");
        initializer.Should().Contain("180_tenant_country_paged_search.sql");
    }

    [Fact]
    public void CountriesForm_ShouldUseRemoteFindWithoutChangingSharedGrid()
    {
        var endpoint = Read(
            "src", "Backend", "NuanSystem.Api", "Endpoints", "Definitions", "General",
            "Countries", "CountryEndpoints.cs");
        var client = Read(
            "src", "Frontend", "NuanSystem.WinForms.Services", "Definitions", "General",
            "Common", "GeographyClient.cs");
        var form = Read(
            "src", "Frontend", "NuanSystem.WinForms.Forms", "Definitions", "General",
            "Countries", "CountriesForm.cs");
        var sharedGrid = Read(
            "src", "Frontend", "NuanSystem.WinForms.Controls", "Grids",
            "NuanDataGridControl.cs");

        endpoint.Should().Contain("/countries/page")
            .And.Contain("RequireFormOperation(FormKey, \"refresh\")");
        client.Should().Contain("/api/geography/countries/page?")
            .And.Contain("Uri.EscapeDataString(search.Trim())");
        form.Should().Contain("EnableServerPaging(50)")
            .And.Contain("GridView.ColumnFilterChanged += OnColumnFilterChanged")
            .And.Contain("findDebounceTimer")
            .And.Contain("viewModel.PageNumber = 1")
            .And.Contain("SetPagedGridData(");
        sharedGrid.Should().NotContain("SearchCountriesAsync");
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
