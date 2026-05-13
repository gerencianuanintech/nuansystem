using NuanSystem.WinForms.Services.ConfigurationCompanies.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.ConfigurationCompanies;

public sealed class ConfigurationCompanyClient(INuanApiClient apiClient) : IConfigurationCompanyClient
{
    public async Task<IReadOnlyCollection<ConfigurationCompanyItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<ConfigurationCompanyItem>>("/api/configuration/companies", cancellationToken);
    }

    public Task<ConfigurationCompanyItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<ConfigurationCompanyItem>($"/api/configuration/companies/{id}", cancellationToken);
    }

    public Task<ConfigurationCompanyItem> CreateAsync(SaveConfigurationCompanyRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveConfigurationCompanyRequest, ConfigurationCompanyItem>("/api/configuration/companies", request, cancellationToken);
    }

    public Task<ConfigurationCompanyItem> UpdateAsync(int id, SaveConfigurationCompanyRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveConfigurationCompanyRequest, ConfigurationCompanyItem>($"/api/configuration/companies/{id}", request, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.DeleteAsync<bool>($"/api/configuration/companies/{id}", cancellationToken);
    }
}
