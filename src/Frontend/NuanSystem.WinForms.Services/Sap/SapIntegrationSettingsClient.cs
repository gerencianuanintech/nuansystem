using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Sap.Models;

namespace NuanSystem.WinForms.Services.Sap;

public sealed class SapIntegrationSettingsClient(INuanApiClient apiClient) : ISapIntegrationSettingsClient
{
    private const string Path = "/api/sap/settings/service-layer";

    public Task<SapServiceLayerSettingsItem> GetAsync(CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<SapServiceLayerSettingsItem>(Path, cancellationToken);

    public Task<SapServiceLayerSettingsItem> SaveAsync(SaveSapServiceLayerSettingsRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync<SaveSapServiceLayerSettingsRequest, SapServiceLayerSettingsItem>(Path, request, cancellationToken);

    public Task<SapConnectionTestResult> TestConnectionAsync(CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<object, SapConnectionTestResult>(Path + "/test", new { }, cancellationToken);
}
