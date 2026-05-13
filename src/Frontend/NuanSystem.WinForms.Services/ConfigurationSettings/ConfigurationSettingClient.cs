using NuanSystem.WinForms.Services.ConfigurationSettings.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.ConfigurationSettings;

public sealed class ConfigurationSettingClient(INuanApiClient apiClient) : IConfigurationSettingClient
{
    public async Task<IReadOnlyCollection<ConfigurationSettingItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<ConfigurationSettingItem>>("/api/configuration/settings", cancellationToken);
    }

    public Task<ConfigurationSettingItem> CreateAsync(SaveConfigurationSettingRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveConfigurationSettingRequest, ConfigurationSettingItem>("/api/configuration/settings", request, cancellationToken);
    }

    public Task<ConfigurationSettingItem> UpdateAsync(int id, SaveConfigurationSettingRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveConfigurationSettingRequest, ConfigurationSettingItem>($"/api/configuration/settings/{id}", request, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.DeleteAsync<bool>($"/api/configuration/settings/{id}", cancellationToken);
    }
}
