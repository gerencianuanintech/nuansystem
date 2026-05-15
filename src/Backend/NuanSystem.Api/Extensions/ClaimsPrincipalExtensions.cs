using System.Security.Claims;

namespace NuanSystem.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static (int? UserId, string? UserName) GetAuditUser(this ClaimsPrincipal user)
    {
        var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = user.FindFirstValue(ClaimTypes.Name)
            ?? user.FindFirstValue("name")
            ?? user.Identity?.Name;

        return (int.TryParse(userIdValue, out var userId) ? userId : null, Trim(userName, 120));
    }

    private static string? Trim(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        value = value.Trim();
        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
