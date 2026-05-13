namespace NuanSystem.Application.Abstractions.Tenancy;

public interface ICompanyContext
{
    bool HasActiveCompany { get; }

    CompanyConnectionInfo? CurrentCompany { get; }

    void SetCurrentCompany(CompanyConnectionInfo company);
}
