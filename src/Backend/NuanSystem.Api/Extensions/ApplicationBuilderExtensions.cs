using NuanSystem.Api.Middleware;

namespace NuanSystem.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }

    public static IApplicationBuilder UseCompanyContext(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CompanyContextMiddleware>();
    }

    public static IApplicationBuilder UseRequiredPasswordChange(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequiredPasswordChangeMiddleware>();
    }

    public static IApplicationBuilder UseAuditLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuditLoggingMiddleware>();
    }
}
