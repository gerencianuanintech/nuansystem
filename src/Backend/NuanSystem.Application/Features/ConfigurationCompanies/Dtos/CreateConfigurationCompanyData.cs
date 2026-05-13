using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Features.ConfigurationCompanies.Dtos;

public sealed record CreateConfigurationCompanyData(
    string Code,
    string CommercialName,
    string? LegalName,
    string? TaxIdentification,
    string? Address,
    string? Phone,
    string? Email,
    byte[]? LogoImage,
    string? LogoImageContentType,
    string? LogoImageFileName,
    DatabaseEngine DatabaseEngine,
    string Server,
    int? Port,
    string DatabaseName,
    string DatabaseUser,
    string DatabasePasswordEncrypted,
    bool IsActive,
    SapIntegrationMode SapIntegrationMode,
    int DisplayOrder,
    bool IsDefault,
    string TimeZoneId,
    string CultureCode,
    string CurrencyCode,
    int? CreatedByUserId,
    string? CreatedByUserName);
