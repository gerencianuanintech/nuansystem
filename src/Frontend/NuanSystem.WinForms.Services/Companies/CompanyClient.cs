using NuanSystem.Shared.Contracts.Auth;
using NuanSystem.WinForms.Services.Companies.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Companies;

public sealed class CompanyClient : ICompanyClient
{
    private readonly INuanApiClient apiClient;

    public CompanyClient(INuanApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<IReadOnlyCollection<UserCompanyResponse>> GetMyCompaniesAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<UserCompanyResponse>>(
            "/api/companies/my-companies",
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<CompanyAdminItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<CompanyAdminItem>>(
            "/api/companies",
            cancellationToken);
    }

    public Task<CompanyConnectionTestItem> ValidateConnectionAsync(
        ValidateCompanyConnectionRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<ValidateCompanyConnectionRequest, CompanyConnectionTestItem>(
            "/api/companies/validate-connection",
            request,
            cancellationToken);
    }

    public Task<CompanyAdminItem> CreateAsync(
        CreateCompanyRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<CreateCompanyRequest, CompanyAdminItem>(
            "/api/companies",
            request,
            cancellationToken);
    }

    public Task<bool> AssignUserAsync(
        AssignUserCompanyRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<AssignUserCompanyRequest, bool>(
            "/api/companies/assign-user",
            request,
            cancellationToken);
    }
}
