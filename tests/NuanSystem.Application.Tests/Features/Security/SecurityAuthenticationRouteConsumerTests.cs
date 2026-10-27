using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NuanSystem.Api.Middleware;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Contracts.Auth;
using NuanSystem.WinForms.Services.Authentication;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Session;

namespace NuanSystem.Application.Tests.Features.Security;

public sealed class SecurityAuthenticationRouteConsumerTests
{
    [Fact]
    public async Task Authentication_client_uses_only_canonical_security_auth_routes()
    {
        var apiClient = Substitute.For<INuanApiClient>();
        var session = new ApiSession();
        var loginResponse = new LoginResponse(
            7,
            "student",
            "Student",
            "token",
            DateTime.UtcNow.AddHours(1),
            true,
            [],
            [],
            []);
        apiClient.PostAsync<LoginRequest, LoginResponse>(
                Arg.Any<string>(),
                Arg.Any<LoginRequest>(),
                Arg.Any<CancellationToken>())
            .Returns(loginResponse);
        apiClient.PostAsync<ChangePasswordRequest, object>(
                Arg.Any<string>(),
                Arg.Any<ChangePasswordRequest>(),
                Arg.Any<CancellationToken>())
            .Returns(new object());
        var client = new AuthenticationClient(apiClient, session);

        await client.LoginAsync("student", "secret");
        await client.ChangePasswordAsync("secret", "new-secret");

        await apiClient.Received(1).PostAsync<LoginRequest, LoginResponse>(
            "/api/security/auth/login",
            Arg.Any<LoginRequest>(),
            Arg.Any<CancellationToken>());
        await apiClient.Received(1).PostAsync<ChangePasswordRequest, object>(
            "/api/security/auth/change-password",
            Arg.Any<ChangePasswordRequest>(),
            Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("/api/security/auth/login")]
    [InlineData("/api/security/auth/change-password")]
    public async Task Company_context_skips_canonical_authentication_routes(string path)
    {
        var nextCalled = false;
        var middleware = new CompanyContextMiddleware(
            _ => { nextCalled = true; return Task.CompletedTask; },
            Substitute.For<ILogger<CompanyContextMiddleware>>());
        var resolver = Substitute.For<ICompanyResolver>();
        var context = Context(path);

        await middleware.InvokeAsync(context, resolver, Substitute.For<ICompanyContext>());

        nextCalled.Should().BeTrue();
        await resolver.DidNotReceiveWithAnyArgs()
            .ResolveByCodeForUserAsync(default!, default, default);
    }

    [Theory]
    [InlineData("/api/security/auth/login")]
    [InlineData("/api/security/auth/change-password")]
    public async Task Required_password_change_allows_canonical_authentication_routes(string path)
    {
        var nextCalled = false;
        var middleware = new RequiredPasswordChangeMiddleware(
            _ => { nextCalled = true; return Task.CompletedTask; });
        var context = Context(path);
        context.User = MustChangePasswordPrincipal();

        await middleware.InvokeAsync(context);

        nextCalled.Should().BeTrue();
        context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Theory]
    [InlineData("/api/auth/login")]
    [InlineData("/api/auth/change-password")]
    public async Task Removed_authentication_routes_are_not_middleware_bypasses(string path)
    {
        var companyNextCalled = false;
        var companyMiddleware = new CompanyContextMiddleware(
            _ => { companyNextCalled = true; return Task.CompletedTask; },
            Substitute.For<ILogger<CompanyContextMiddleware>>());
        var companyContext = Context(path);

        await companyMiddleware.InvokeAsync(
            companyContext,
            Substitute.For<ICompanyResolver>(),
            Substitute.For<ICompanyContext>());

        companyNextCalled.Should().BeFalse();
        companyContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var passwordNextCalled = false;
        var passwordMiddleware = new RequiredPasswordChangeMiddleware(
            _ => { passwordNextCalled = true; return Task.CompletedTask; });
        var passwordContext = Context(path);
        passwordContext.User = MustChangePasswordPrincipal();

        await passwordMiddleware.InvokeAsync(passwordContext);

        passwordNextCalled.Should().BeFalse();
        passwordContext.Response.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    private static DefaultHttpContext Context(string path)
    {
        var context = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection()
                .AddLogging()
                .BuildServiceProvider()
        };
        context.Request.Path = path;
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static ClaimsPrincipal MustChangePasswordPrincipal() =>
        new(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, "7"),
            new Claim(AuthClaimNames.MustChangePassword, "true")
        ],
        authenticationType: "test"));
}
