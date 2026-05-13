namespace NuanSystem.Application.Abstractions.Authentication;

public sealed record AuthResult(
    int UserId,
    string UserName,
    string DisplayName,
    string AccessToken,
    DateTime ExpiresAtUtc,
    bool MustChangePassword,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions,
    IReadOnlyCollection<AuthCompanyDto> Companies);
