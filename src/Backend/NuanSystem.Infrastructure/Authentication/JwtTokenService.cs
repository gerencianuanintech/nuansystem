using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NuanSystem.Application.Abstractions.Authentication;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Infrastructure.Authentication;

public sealed class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public JwtTokenResult CreateToken(
        int userId,
        string userName,
        string displayName,
        bool mustChangePassword,
        string securityStamp,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions)
    {
        var options = ReadOptions();
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(options.ExpirationMinutes);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userName),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, userName),
            new(AuthClaimNames.DisplayName, displayName),
            new(AuthClaimNames.MustChangePassword, mustChangePassword ? "true" : "false"),
            new(AuthClaimNames.SecurityStamp, securityStamp)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(permissions.Select(permission => new Claim(AuthClaimNames.Permission, permission)));

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new JwtTokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }

    private JwtOptions ReadOptions()
    {
        var options = new JwtOptions
        {
            Issuer = configuration[$"{JwtOptions.SectionName}:Issuer"] ?? "NuanSystem",
            Audience = configuration[$"{JwtOptions.SectionName}:Audience"] ?? "NuanSystem.Client",
            SigningKey = configuration[$"{JwtOptions.SectionName}:SigningKey"] ?? string.Empty
        };

        if (int.TryParse(configuration[$"{JwtOptions.SectionName}:ExpirationMinutes"], out var minutes))
        {
            options.ExpirationMinutes = minutes;
        }

        if (string.IsNullOrWhiteSpace(options.SigningKey) || options.SigningKey.Length < 32)
        {
            throw new InvalidOperationException("Jwt:SigningKey debe tener al menos 32 caracteres.");
        }

        return options;
    }
}
