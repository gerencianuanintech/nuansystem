using System.Net;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Middleware;

public sealed class CompanyContextMiddleware(
    RequestDelegate next,
    ILogger<CompanyContextMiddleware> logger)
{
    public const string CompanyCodeHeaderName = "X-Company-Code";

    public async Task InvokeAsync(
        HttpContext context,
        ICompanyResolver companyResolver,
        ICompanyContext companyContext)
    {
        if (ShouldSkipCompanyResolution(context))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(CompanyCodeHeaderName, out var headerValue)
            || string.IsNullOrWhiteSpace(headerValue))
        {
            await WriteErrorAsync(
                context,
                HttpStatusCode.BadRequest,
                "La empresa activa es requerida.",
                "CompanyHeaderMissing",
                $"Debe enviar el header {CompanyCodeHeaderName}.");
            return;
        }

        var companyCode = headerValue.ToString().Trim();
        if (!context.User.TryGetUserId(out var userId))
        {
            await WriteErrorAsync(
                context,
                HttpStatusCode.Unauthorized,
                "La sesion autenticada es requerida.",
                "UserSessionMissing",
                "No fue posible identificar al usuario autenticado.");
            return;
        }

        var company = await companyResolver.ResolveByCodeForUserAsync(companyCode, userId, context.RequestAborted);
        if (company is null)
        {
            logger.LogWarning("Empresa no encontrada, inactiva o no disponible: {CompanyCode}", companyCode);

            await WriteErrorAsync(
                context,
                HttpStatusCode.Forbidden,
                "La empresa indicada no esta disponible.",
                "CompanyUnavailable",
                "La empresa no existe, esta inactiva o el usuario no tiene acceso.");
            return;
        }

        companyContext.SetCurrentCompany(company);
        context.Items["CompanyId"] = company.CompanyId;
        context.Items["CompanyCode"] = company.CompanyCode;

        await next(context);
    }

    private static bool ShouldSkipCompanyResolution(HttpContext context)
    {
        var path = context.Request.Path;

        return path == "/"
            || path.StartsWithSegments("/health")
            || path.StartsWithSegments("/swagger")
            || path.StartsWithSegments("/api/auth")
            || path.StartsWithSegments("/api/companies");
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string message,
        string code,
        string detail)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = ApiResponse<object>.Fail(message, new[]
        {
            new ApiError(code, detail)
        });

        await context.Response.WriteAsJsonAsync(response);
    }
}
