namespace NuanSystem.WinForms.Services.Companies.Models;

public sealed record AssignUserCompanyRequest(
    int UserId,
    int CompanyId);
