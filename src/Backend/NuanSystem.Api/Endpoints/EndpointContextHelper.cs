using System.Security.Claims;
using NuanSystem.Api.Extensions;

namespace NuanSystem.Api.Endpoints;

internal static class EndpointContextHelper
{
    public static (int? UserId, string? UserName) GetAuditUser(ClaimsPrincipal user)
    {
        return (user.TryGetUserId(out var userId) ? userId : null, user.FindFirstValue(ClaimTypes.Name) ?? user.Identity?.Name);
    }

    public static bool TryGetUserId(ClaimsPrincipal user, out int userId)
    {
        return user.TryGetUserId(out userId);
    }

    public static string? Trim(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
