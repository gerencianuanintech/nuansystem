namespace NuanSystem.WinForms.Services.Companies.Models;

public sealed record ValidateCompanyConnectionRequest(
    int DatabaseEngine,
    string Server,
    int? Port,
    string DatabaseName,
    string DatabaseUser,
    string DatabasePassword);
