namespace NuanSystem.Shared.Contracts.Auth;

public sealed record LoginRequest(string UserNameOrEmail, string Password);
