using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Features.Companies.Dtos;

public sealed record CompanyDto(
    int Id,
    string Code,
    string CommercialName,
    string? LegalName,
    string? TaxIdentification,
    DatabaseEngine DatabaseEngine,
    string Server,
    int? Port,
    string DatabaseName,
    string DatabaseUser,
    bool IsActive,
    SapIntegrationMode SapIntegrationMode);
