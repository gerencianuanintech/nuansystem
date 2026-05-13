using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.ConfigurationCompanies.Dtos;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Features.ConfigurationCompanies.Commands;

public sealed record UpdateConfigurationCompanyCommand(
    int Id,
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
    string? DatabasePassword,
    bool ValidateConnection,
    bool IsActive,
    SapIntegrationMode SapIntegrationMode,
    int DisplayOrder,
    bool IsDefault,
    string TimeZoneId,
    string CultureCode,
    string CurrencyCode,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<ConfigurationCompanyDto>;
