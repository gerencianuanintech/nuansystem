using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Authentication;
using NuanSystem.Application.Features.Companies.Commands;
using NuanSystem.Application.Features.Companies.Queries;
using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Contracts.Auth;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Endpoints;

public static class CompanyEndpoints
{
    public static IEndpointRouteBuilder MapCompanyEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/companies/my-companies", async (
            ClaimsPrincipal user,
            IAuthService authService,
            CancellationToken cancellationToken) =>
        {
            var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdValue, out var userId))
            {
                return Results.Unauthorized();
            }

            var companies = await authService.GetCompaniesForUserAsync(userId, cancellationToken);
            var response = companies
                .Select(company => new UserCompanyResponse(
                    company.Id,
                    company.Code,
                    company.CommercialName,
                    company.LogoImage,
                    company.LogoImageContentType,
                    company.LogoImageFileName))
                .ToArray();

            return Results.Ok(ApiResponse<IReadOnlyCollection<UserCompanyResponse>>.Ok(response));
        })
        .RequirePermission(PermissionCodes.UsersManage);

        app.MapGet("/api/companies", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCompaniesQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.CompaniesManage);

        app.MapPost("/api/companies", async (
            CreateCompanyCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.CompaniesManage);

        app.MapPost("/api/companies/validate-connection", async (
            ValidateCompanyConnectionCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.CompaniesManage);

        app.MapPost("/api/companies/assign-user", async (
            AssignUserCompanyCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.CompaniesManage);

        return app;
    }
}
