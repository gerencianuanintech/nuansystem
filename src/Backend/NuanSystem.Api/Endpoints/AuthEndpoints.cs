using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Authentication;
using NuanSystem.Application.Features.Auth.Commands;
using NuanSystem.Shared.Contracts.Auth;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            LoginRequest request,
            IAuthService authService,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.UserNameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Results.BadRequest(ApiResponse<object>.Fail("Usuario y contrasena son requeridos."));
            }

            var result = await authService.LoginAsync(
                request.UserNameOrEmail,
                request.Password,
                cancellationToken);

            if (result is null)
            {
                return Results.Unauthorized();
            }

            var response = new LoginResponse(
                result.UserId,
                result.UserName,
                result.DisplayName,
                result.AccessToken,
                result.ExpiresAtUtc,
                result.MustChangePassword,
                result.Roles,
                result.Permissions,
                result.Companies.Select(company => new UserCompanyResponse(
                    company.Id,
                    company.Code,
                    company.CommercialName,
                    company.LogoImage,
                    company.LogoImageContentType,
                    company.LogoImageFileName)).ToArray());

            return Results.Ok(ApiResponse<LoginResponse>.Ok(response, "Login correcto."));
        })
        .RequireRateLimiting("auth-login")
        .AllowAnonymous();

        app.MapPost("/api/auth/change-password", async (
            ChangePasswordRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(
                new ChangePasswordCommand(
                    userId,
                    request.CurrentPassword,
                    request.NewPassword),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequireAuthorization();

        return app;
    }
}
