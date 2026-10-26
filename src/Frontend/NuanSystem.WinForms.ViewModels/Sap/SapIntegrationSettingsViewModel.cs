using NuanSystem.WinForms.Services.Sap;
using NuanSystem.WinForms.Services.Sap.Models;

namespace NuanSystem.WinForms.ViewModels.Sap;

public sealed class SapIntegrationSettingsViewModel(ISapIntegrationSettingsClient client)
{
    public SapServiceLayerSettingsItem? Settings { get; private set; }
    public bool IsBusy { get; private set; }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try { Settings = await client.GetAsync(cancellationToken); }
        finally { IsBusy = false; }
    }

    public async Task SaveAsync(SaveSapServiceLayerSettingsRequest request, CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try { Settings = await client.SaveAsync(request, cancellationToken); }
        finally { IsBusy = false; }
    }

    public async Task<SapConnectionTestResult> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try { return await client.TestConnectionAsync(cancellationToken); }
        finally { IsBusy = false; }
    }
}
