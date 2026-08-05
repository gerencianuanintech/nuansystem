using NuanSystem.Api.Endpoints.Definitions.General.Cities;
using NuanSystem.Api.Endpoints.Definitions.General.Countries;
using NuanSystem.Api.Endpoints.Definitions.General.Provinces;
using NuanSystem.Api.OpenApi;

namespace NuanSystem.Api.Endpoints.Definitions.General;

public static class GeographyDefinitionEndpoints
{
    public static IEndpointRouteBuilder MapGeographyEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/api/geography")
            .WithTags(SwaggerTags.DefinitionsGeneralCountries)
            .MapCountryEndpoints();

        app.MapGroup("/api/geography")
            .WithTags(SwaggerTags.DefinitionsGeneralProvinces)
            .MapProvinceEndpoints();

        app.MapGroup("/api/geography")
            .WithTags(SwaggerTags.DefinitionsGeneralCities)
            .MapCityEndpoints();

        return app;
    }
}
