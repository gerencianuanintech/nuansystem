using NuanSystem.WinForms.Services.Sap.Models;

namespace NuanSystem.WinForms.Services.Sap;

public interface ISapIntegrationSettingsClient
{
    Task<SapServiceLayerSettingsItem> GetAsync(CancellationToken cancellationToken = default);
    Task<SapServiceLayerSettingsItem> SaveAsync(SaveSapServiceLayerSettingsRequest request, CancellationToken cancellationToken = default);
    Task<SapConnectionTestResult> TestConnectionAsync(CancellationToken cancellationToken = default);
}
