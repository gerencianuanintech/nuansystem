namespace NuanSystem.Shared.Contracts.Auth;

public sealed record UserCompanyResponse(
    int Id,
    string Code,
    string CommercialName,
    byte[]? LogoImage,
    string? LogoImageContentType,
    string? LogoImageFileName);
