using NuanSystem.Application.Abstractions.Tenancy;

namespace NuanSystem.Persistence.Tenancy;

public sealed class CompanyContext : ICompanyContext
{
    public bool HasActiveCompany => CurrentCompany is not null;

    public CompanyConnectionInfo? CurrentCompany { get; private set; }

    public void SetCurrentCompany(CompanyConnectionInfo company)
    {
        if (CurrentCompany is not null && CurrentCompany.CompanyCode != company.CompanyCode)
        {
            throw new InvalidOperationException("La empresa activa ya fue establecida para esta solicitud.");
        }

        CurrentCompany = company;
    }
}
