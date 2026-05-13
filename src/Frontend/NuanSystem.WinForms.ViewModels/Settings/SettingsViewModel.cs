using NuanSystem.WinForms.Services.Settings;
using NuanSystem.WinForms.Services.Settings.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Settings;

public sealed class SettingsViewModel(ISettingsClient settingsClient) : ViewModelBase
{
    private IReadOnlyCollection<CompanyParameterItem> parameters = Array.Empty<CompanyParameterItem>();
    private bool isBusy;

    public IReadOnlyCollection<CompanyParameterItem> Parameters
    {
        get => parameters;
        private set => SetProperty(ref parameters, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        private set => SetProperty(ref isBusy, value);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try
        {
            Parameters = await settingsClient.GetParametersAsync(cancellationToken);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public Task<CompanyParameterItem> SaveAsync(
        SaveCompanyParameterRequest request,
        CancellationToken cancellationToken = default)
    {
        return settingsClient.SaveParameterAsync(request, cancellationToken);
    }
}
