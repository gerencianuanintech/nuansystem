using NuanSystem.WinForms.Services.ConfigurationSettings;
using NuanSystem.WinForms.Services.ConfigurationSettings.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.ConfigurationSettings;

public sealed class ConfigurationSettingsViewModel(IConfigurationSettingClient settingClient) : ViewModelBase
{
    private IReadOnlyCollection<ConfigurationSettingItem> items = Array.Empty<ConfigurationSettingItem>();

    public IReadOnlyCollection<ConfigurationSettingItem> Items
    {
        get => items;
        private set => SetProperty(ref items, value);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Items = await settingClient.GetAllAsync(cancellationToken);
    }

    public Task<ConfigurationSettingItem> CreateAsync(SaveConfigurationSettingRequest request, CancellationToken cancellationToken = default)
    {
        return settingClient.CreateAsync(request, cancellationToken);
    }

    public Task<ConfigurationSettingItem> UpdateAsync(int id, SaveConfigurationSettingRequest request, CancellationToken cancellationToken = default)
    {
        return settingClient.UpdateAsync(id, request, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return settingClient.DeleteAsync(id, cancellationToken);
    }
}
