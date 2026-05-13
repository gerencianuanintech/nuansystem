namespace NuanSystem.Application.Abstractions.Tenancy;

public interface ICompanyResolver
{
    Task<CompanyConnectionInfo?> ResolveByCodeAsync(
        string companyCode,
        CancellationToken cancellationToken = default);
}
