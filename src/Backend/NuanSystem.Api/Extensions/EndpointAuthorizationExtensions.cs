using System.Security.Claims;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Extensions;

public static class EndpointAuthorizationExtensions
{
    public static RouteHandlerBuilder RequirePermission(
        this RouteHandlerBuilder builder,
        string permission)
    {
        return builder.RequireAuthorization(permission);
    }

    public static RouteHandlerBuilder RequireFormOperation(
        this RouteHandlerBuilder builder,
        string formKey,
        string actionKey)
    {
        return builder
            .RequireAuthorization()
            .AddEndpointFilter(async (context, next) =>
            {
                if (!TryGetUserId(context.HttpContext.User, out var userId))
                {
                    return Results.Unauthorized();
                }

                if (HasSecurityAdminBypass(context.HttpContext.User))
                {
                    return await next(context);
                }

                var resolvedFormKey = ResolveRouteValue(context, formKey);
                var repository = context.HttpContext.RequestServices.GetRequiredService<ISecurityAccessRepository>();
                var operations = await repository.GetFormOperationsAsync(userId, resolvedFormKey, context.HttpContext.RequestAborted);
                var allowedAliases = ResolveOperationAliases(actionKey);
                var isAllowed = operations.Any(operation =>
                    operation.IsAllowed &&
                    OperationMatches(operation.ActionKey, allowedAliases));

                return isAllowed
                    ? await next(context)
                    : Forbidden(
                        "No tienes permiso para ejecutar esta accion.",
                        "FormOperationDenied",
                        $"El rol actual no tiene la operacion '{actionKey}' habilitada para el formulario '{resolvedFormKey}'.");
            });
    }

    public static RouteHandlerBuilder RequireSecurityHistoryOperation(this RouteHandlerBuilder builder)
    {
        return builder
            .RequireAuthorization()
            .AddEndpointFilter(async (context, next) =>
            {
                if (!TryGetUserId(context.HttpContext.User, out var userId))
                {
                    return Results.Unauthorized();
                }

                if (HasSecurityAdminBypass(context.HttpContext.User))
                {
                    return await next(context);
                }

                var entityName = context.HttpContext.Request.Query["entityName"].ToString();
                var formKey = entityName switch
                {
                    "SecurityUsers" => "users",
                    "SecurityRoles" => "security-roles",
                    "SecurityOperations" => "security-operations",
                    "SecurityMenus" => "security-menus",
                    "SecurityForms" => "security-forms",
                    "ConfigurationCompanies" => "configuration-companies",
                    "ConfigurationSettings" => "configuration-settings",
                    _ => null
                };

                if (string.IsNullOrWhiteSpace(formKey))
                {
                    return Forbidden(
                        "No tienes permiso para consultar este historial.",
                        "SecurityHistoryFormUnsupported",
                        "La entidad solicitada no esta asociada a un formulario de seguridad.");
                }

                var repository = context.HttpContext.RequestServices.GetRequiredService<ISecurityAccessRepository>();
                var operations = await repository.GetFormOperationsAsync(userId, formKey, context.HttpContext.RequestAborted);
                var allowedAliases = ResolveOperationAliases("history");
                var isAllowed = operations.Any(operation =>
                    operation.IsAllowed &&
                    OperationMatches(operation.ActionKey, allowedAliases));

                return isAllowed
                    ? await next(context)
                    : Forbidden(
                        "No tienes permiso para consultar este historial.",
                        "SecurityHistoryOperationDenied",
                        $"El rol actual no tiene la operacion de historial habilitada para el formulario '{formKey}'.");
            });
    }

    private static bool TryGetUserId(ClaimsPrincipal user, out int userId)
    {
        var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out userId);
    }

    private static bool OperationMatches(string? actionKey, IReadOnlyCollection<string> allowedAliases)
    {
        if (string.IsNullOrWhiteSpace(actionKey))
        {
            return false;
        }

        var normalizedAction = NormalizeOperation(actionKey);
        return allowedAliases.Any(alias => NormalizeOperation(alias) == normalizedAction);
    }

    private static IReadOnlyCollection<string> ResolveOperationAliases(string actionKey)
    {
        return NormalizeOperation(actionKey) switch
        {
            "refresh" => ["refresh", "actualizar", "read", "consult", "consultar"],
            "create" => ["create", "new", "nuevo", "crear"],
            "update" => ["update", "edit", "editar", "modificar"],
            "delete" => ["delete", "eliminar", "borrar"],
            "consult" => ["consult", "consultar", "view", "ver"],
            "history" => ["history", "historial", "audit", "auditoria"],
            "customizecolumns" => ["customizecolumns", "columns", "columnas", "personalizarcolumnas", "configurarcolumnas"],
            _ => [actionKey]
        };
    }

    private static string ResolveRouteValue(EndpointFilterInvocationContext context, string value)
    {
        if (value.Length > 2 && value[0] == '{' && value[^1] == '}')
        {
            var routeKey = value[1..^1];
            return context.HttpContext.Request.RouteValues.TryGetValue(routeKey, out var routeValue)
                ? Convert.ToString(routeValue) ?? value
                : value;
        }

        return value;
    }

    private static bool HasSecurityAdminBypass(ClaimsPrincipal user)
    {
        return user.HasClaim(AuthClaimNames.Permission, PermissionCodes.SecurityAccessBypass);
    }

    private static IResult Forbidden(string message, string code, string detail)
    {
        return Results.Json(
            NuanSystem.Shared.Responses.ApiResponse<object>.Fail(
                message,
                [new NuanSystem.Shared.Responses.ApiError(code, detail)]),
            statusCode: StatusCodes.Status403Forbidden);
    }

    private static string NormalizeOperation(string operation)
    {
        return operation.Trim()
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(".", string.Empty, StringComparison.Ordinal)
            .ToLowerInvariant();
    }
}
