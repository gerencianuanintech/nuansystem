namespace NuanSystem.WinForms.Services.Companies.Models;

public sealed record CreateCompanyRequest(
    string Code,
    string CommercialName,
    string? LegalName,
    string? TaxIdentification,
    int DatabaseEngine,
    string Server,
    int? Port,
    string DatabaseName,
    string DatabaseUser,
    string DatabasePassword,
    bool ValidateConnection,
    bool IsActive,
    int SapIntegrationMode);
