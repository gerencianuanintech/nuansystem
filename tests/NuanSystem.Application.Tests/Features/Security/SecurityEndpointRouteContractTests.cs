using System.Reflection;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NuanSystem.Api.Endpoints;
using NuanSystem.Application.Abstractions.Authentication;

namespace NuanSystem.Application.Tests.Features.Security;

public sealed class SecurityEndpointRouteContractTests
{
    [Fact]
    public void Registers_only_the_canonical_authentication_user_and_role_routes()
    {
        var endpoints = BuildEndpoints();
        var routes = endpoints
            .Select(endpoint => endpoint.RoutePattern.RawText)
            .ToArray();

        routes.Should().Contain(
        [
            "/api/security/auth/login",
            "/api/security/auth/change-password",
            "/api/security/users",
            "/api/security/users/{id:int}",
            "/api/security/users/roles",
            "/api/security/roles",
            "/api/security/roles/{id:int}"
        ]);

        routes.Should().NotContain(
        [
            "/api/auth/login",
            "/api/auth/change-password",
            "/api/users",
            "/api/users/roles"
        ]);
    }

    [Fact]
    public void Preserves_authentication_route_security_metadata()
    {
        var endpoints = BuildEndpoints();
        var login = endpoints.Single(endpoint =>
            endpoint.RoutePattern.RawText == "/api/security/auth/login");
        var changePassword = endpoints.Single(endpoint =>
            endpoint.RoutePattern.RawText == "/api/security/auth/change-password");

        login.Metadata.GetMetadata<IAllowAnonymous>().Should().NotBeNull();
        var rateLimit = login.Metadata.Single(metadata =>
            metadata.GetType().Name == "EnableRateLimitingAttribute");
        rateLimit.GetType().GetProperty("PolicyName")!.GetValue(rateLimit)
            .Should().Be("auth-login");

        changePassword.Metadata.GetOrderedMetadata<IAuthorizeData>()
            .Should().ContainSingle();
        changePassword.Metadata.GetMetadata<IAllowAnonymous>().Should().BeNull();
    }

    private static IReadOnlyCollection<RouteEndpoint> BuildEndpoints()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton(Substitute.For<IAuthService>());
        builder.Services.AddSingleton(Substitute.For<ISender>());
        var app = builder.Build();

        Map(app, "SecurityAuthEndpoints", "MapSecurityAuthEndpoints");
        Map(app, "SecurityUserEndpoints", "MapSecurityUserEndpoints");
        Map(app, "SecurityRoleEndpoints", "MapSecurityRoleEndpoints");

        return ((IEndpointRouteBuilder)app).DataSources
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .ToArray();
    }

    private static void Map(WebApplication app, string typeName, string methodName)
    {
        var type = typeof(PurchaseOrderEndpoints).Assembly.GetType(
            $"NuanSystem.Api.Endpoints.Seguridad.{typeName}");
        type.Should().NotBeNull($"{typeName} debe existir en el namespace de Seguridad");

        var method = type!.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
        method.Should().NotBeNull($"{typeName} debe exponer {methodName}");
        method!.Invoke(null, [app]);
    }
}
