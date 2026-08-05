using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Api.OpenApi;
using NuanSystem.Application.Features.Geography.Common.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Geography.Common;

public static class GeographyCommonEndpoints
{
    public static IEndpointRouteBuilder MapGeographyCommonEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/geography")
            .WithTags(SwaggerTags.GeographyMaps);
        group.MapGet("/reverse-geocode", async (decimal latitude, decimal longitude, ISender sender, CancellationToken cancellationToken) => (await sender.Send(new ReverseGeocodeQuery(latitude, longitude), cancellationToken)).ToHttpResult()).RequirePermission(PermissionCodes.GeographyCitiesRead);
        group.MapGet("/static-map", async (decimal latitude, decimal longitude, ISender sender, CancellationToken cancellationToken) => (await sender.Send(new GetStaticMapQuery(latitude, longitude), cancellationToken)).ToHttpResult()).RequirePermission(PermissionCodes.GeographyCitiesRead);
        return app;
    }
}
