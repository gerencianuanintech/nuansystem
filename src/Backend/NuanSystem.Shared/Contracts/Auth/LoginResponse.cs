namespace NuanSystem.Shared.Contracts.Auth;

public sealed record LoginResponse(
    int UserId,
    string UserName,
    string DisplayName,
    string AccessToken,
    DateTime ExpiresAtUtc,
    bool MustChangePassword,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions,
    IReadOnlyCollection<UserCompanyResponse> Companies);
