using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Companies.Dtos;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Application.Features.Companies.Commands;

public sealed record CreateCompanyCommand(
    string Code,
    string CommercialName,
    string? LegalName,
    string? TaxIdentification,
    DatabaseEngine DatabaseEngine,
    string Server,
    int? Port,
    string DatabaseName,
    string DatabaseUser,
    string DatabasePassword,
    bool ValidateConnection,
    bool IsActive,
    SapIntegrationMode SapIntegrationMode) : ICommand<CompanyDto>;
