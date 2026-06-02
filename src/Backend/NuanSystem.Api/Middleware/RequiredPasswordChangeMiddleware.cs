using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Middleware;

public sealed class RequiredPasswordChangeMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldSkip(context) || context.User.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        var mustChangePassword = context.User.FindFirst(AuthClaimNames.MustChangePassword)?.Value;
        if (string.Equals(mustChangePassword, "true", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.Fail(
                "Debe cambiar su clave antes de continuar.",
                [new ApiError("PasswordChangeRequired", "La sesion solo puede usar el endpoint de cambio de clave.")]);

            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        await next(context);
    }

    private static bool ShouldSkip(HttpContext context)
    {
        var path = context.Request.Path;

        return path == "/"
            || path.StartsWithSegments("/health")
            || path.StartsWithSegments("/swagger")
            || path.StartsWithSegments("/api/auth/login")
            || path.StartsWithSegments("/api/auth/change-password");
    }
}
