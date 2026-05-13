namespace NuanSystem.WinForms.Services.Companies.Models;

public sealed record CompanyAdminItem(
    int Id,
    string Code,
    string CommercialName,
    string? LegalName,
    string? TaxIdentification,
    int DatabaseEngine,
    string Server,
    int? Port,
    string DatabaseName,
    string DatabaseUser,
    bool IsActive,
    int SapIntegrationMode);
