using FluentAssertions;

namespace NuanSystem.Application.Tests.Features.Definitions.General.Common;

public sealed class DefinitionGeneralBackendStructureTests
{
    [Fact]
    public void Catalogs_HaveIndependentApplicationApiPersistenceAndTestVerticals()
    {
        foreach (var catalog in new[] { "Countries", "Provinces", "Cities" })
        {
            Directory.Exists(PathInRoot("src", "Backend", "NuanSystem.Application", "Features", "Definitions", "General", catalog)).Should().BeTrue();
            Directory.Exists(PathInRoot("src", "Backend", "NuanSystem.Api", "Endpoints", "Definitions", "General", catalog)).Should().BeTrue();
            Directory.Exists(PathInRoot("src", "Backend", "NuanSystem.Persistence", "Repositories", "Definitions", "General", catalog)).Should().BeTrue();
            Directory.Exists(PathInRoot("tests", "NuanSystem.Application.Tests", "Features", "Definitions", "General", catalog)).Should().BeTrue();
        }

        File.Exists(PathInRoot("src", "Backend", "NuanSystem.Application", "Features", "Geography", "Commands", "GeographyCommands.cs")).Should().BeFalse();
        File.Exists(PathInRoot("src", "Backend", "NuanSystem.Api", "Endpoints", "GeographyEndpoints.cs")).Should().BeFalse();
        File.Exists(PathInRoot("src", "Backend", "NuanSystem.Persistence", "Repositories", "Geography", "GeographyRepository.cs")).Should().BeFalse();
    }

    [Fact]
    public void EndpointSplit_PreservesPublicRoutesAndPermissions()
    {
        var country = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "Definitions", "General", "Countries", "CountryEndpoints.cs");
        var province = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "Definitions", "General", "Provinces", "ProvinceEndpoints.cs");
        var city = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "Definitions", "General", "Cities", "CityEndpoints.cs");
        var common = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "Geography", "Common", "GeographyCommonEndpoints.cs");
        var root = Read("src", "Backend", "NuanSystem.Api", "Endpoints", "Definitions", "General", "GeographyDefinitionEndpoints.cs");
        var tags = Read("src", "Backend", "NuanSystem.Api", "OpenApi", "SwaggerTags.cs");

        root.Should().Contain("MapGroup(\"/api/geography\")");
        root.Should().Contain("SwaggerTags.DefinitionsGeneralCountries")
            .And.Contain("SwaggerTags.DefinitionsGeneralProvinces")
            .And.Contain("SwaggerTags.DefinitionsGeneralCities");
        AssertCrudAuthorization(country, "countries", "GeographyCountriesRead", "GeographyCountriesManage");
        AssertCrudAuthorization(province, "provinces", "GeographyProvincesRead", "GeographyProvincesManage");
        AssertCrudAuthorization(city, "cities", "GeographyCitiesRead", "GeographyCitiesManage");
        common.Should().Contain("\"/reverse-geocode\"")
            .And.Contain("\"/static-map\"")
            .And.Contain("SwaggerTags.GeographyMaps");
        tags.Should().Contain("Definitions - General - Countries")
            .And.Contain("Definitions - General - Provinces")
            .And.Contain("Definitions - General - Cities")
            .And.Contain("Geography - Maps");
    }

    private static void AssertCrudAuthorization(
        string endpoint,
        string formKey,
        string readPermission,
        string managePermission)
    {
        endpoint.Should().Contain($"\"/{formKey}\"")
            .And.Contain(readPermission)
            .And.Contain(managePermission)
            .And.Contain($"private const string FormKey = \"{formKey}\"")
            .And.Contain("RequireFormOperation(FormKey, \"refresh\")")
            .And.Contain("RequireFormOperation(FormKey, \"consult\")")
            .And.Contain("RequireFormOperation(FormKey, \"create\")")
            .And.Contain("RequireFormOperation(FormKey, \"update\")")
            .And.Contain("RequireFormOperation(FormKey, \"delete\")");
    }

    private static string Read(params string[] parts) => File.ReadAllText(PathInRoot(parts));

    private static string PathInRoot(params string[] parts) => Path.Combine([FindRepositoryRoot(), .. parts]);

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !Directory.Exists(Path.Combine(directory.FullName, "src"))) directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("No se encontro la raiz del repositorio.");
    }
}
