using NuanSystem.Shared.Contracts.Auth;
using NuanSystem.WinForms.Services.Companies.Models;

namespace NuanSystem.WinForms.Services.Companies;

public interface ICompanyClient
{
    Task<IReadOnlyCollection<UserCompanyResponse>> GetMyCompaniesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<CompanyAdminItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CompanyConnectionTestItem> ValidateConnectionAsync(ValidateCompanyConnectionRequest request, CancellationToken cancellationToken = default);
    Task<CompanyAdminItem> CreateAsync(CreateCompanyRequest request, CancellationToken cancellationToken = default);
    Task<bool> AssignUserAsync(AssignUserCompanyRequest request, CancellationToken cancellationToken = default);
}
