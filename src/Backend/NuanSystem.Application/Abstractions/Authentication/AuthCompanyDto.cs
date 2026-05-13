namespace NuanSystem.Application.Abstractions.Authentication;

public sealed record AuthCompanyDto(
    int Id,
    string Code,
    string CommercialName,
    byte[]? LogoImage,
    string? LogoImageContentType,
    string? LogoImageFileName);
