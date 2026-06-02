namespace NuanSystem.Application.Abstractions.Authentication;

public interface IJwtTokenService
{
    JwtTokenResult CreateToken(
        int userId,
        string userName,
        string displayName,
        bool mustChangePassword,
        string securityStamp,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions);
}
