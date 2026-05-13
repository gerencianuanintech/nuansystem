using NuanSystem.WinForms.Services.ConfigurationSettings.Models;

namespace NuanSystem.WinForms.Services.ConfigurationSettings;

public interface IConfigurationSettingClient
{
    Task<IReadOnlyCollection<ConfigurationSettingItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ConfigurationSettingItem> CreateAsync(SaveConfigurationSettingRequest request, CancellationToken cancellationToken = default);
    Task<ConfigurationSettingItem> UpdateAsync(int id, SaveConfigurationSettingRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
