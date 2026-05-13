namespace NuanSystem.Application.Abstractions.Authentication;

public interface IJwtTokenService
{
    JwtTokenResult CreateToken(
        int userId,
        string userName,
        string displayName,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissions);
}
