using NuanSystem.WinForms.Services.GridColumnSettings.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.GridColumnSettings;

public sealed class GridColumnSettingsClient(INuanApiClient apiClient) : IGridColumnSettingsClient
{
    public async Task<IReadOnlyCollection<GridColumnSettingItem>> GetAsync(string formKey, string gridName, CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<GridColumnSettingItem>>(
            $"/api/security/grid-columns/{Uri.EscapeDataString(formKey)}/{Uri.EscapeDataString(gridName)}/me",
            cancellationToken);
    }

    public Task<bool> SaveAsync(string formKey, string gridName, IReadOnlyCollection<SaveGridColumnSettingRequest> columns, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<IReadOnlyCollection<SaveGridColumnSettingRequest>, bool>(
            $"/api/security/grid-columns/{Uri.EscapeDataString(formKey)}/{Uri.EscapeDataString(gridName)}/me",
            columns,
            cancellationToken);
    }
}
