using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace NuanSystem.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static (int? UserId, string? UserName) GetAuditUser(this ClaimsPrincipal user)
    {
        var userName = user.FindFirstValue(ClaimTypes.Name)
            ?? user.FindFirstValue("name")
            ?? user.Identity?.Name;

        return (user.TryGetUserId(out var userId) ? userId : null, Trim(userName, 120));
    }

    public static bool TryGetUserId(this ClaimsPrincipal user, out int userId)
    {
        var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? user.FindFirstValue("sub")
            ?? user.FindFirstValue("nameid");

        return int.TryParse(userIdValue, out userId);
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
