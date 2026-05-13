using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Features.Companies.Dtos;

public sealed record CreateCompanyData(
    string Code,
    string CommercialName,
    string? LegalName,
    string? TaxIdentification,
    DatabaseEngine DatabaseEngine,
    string Server,
    int? Port,
    string DatabaseName,
    string DatabaseUser,
    string DatabasePasswordEncrypted,
    bool IsActive,
    SapIntegrationMode SapIntegrationMode);
