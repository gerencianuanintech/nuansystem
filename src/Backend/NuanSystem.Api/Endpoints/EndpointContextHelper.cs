using System.Security.Claims;

namespace NuanSystem.Api.Endpoints;

internal static class EndpointContextHelper
{
    public static (int? UserId, string? UserName) GetAuditUser(ClaimsPrincipal user)
    {
        var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = int.TryParse(userIdValue, out var parsedUserId) ? parsedUserId : (int?)null;

        return (userId, user.FindFirstValue(ClaimTypes.Name) ?? user.Identity?.Name);
    }

    public static bool TryGetUserId(ClaimsPrincipal user, out int userId)
    {
        var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out userId);
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
