namespace NuanSystem.Application.Abstractions.Authentication;

public sealed record JwtTokenResult(string AccessToken, DateTime ExpiresAtUtc);
