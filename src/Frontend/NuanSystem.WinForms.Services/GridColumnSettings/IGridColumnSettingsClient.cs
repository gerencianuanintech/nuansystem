using NuanSystem.WinForms.Services.GridColumnSettings.Models;

namespace NuanSystem.WinForms.Services.GridColumnSettings;

public interface IGridColumnSettingsClient
{
    Task<IReadOnlyCollection<GridColumnSettingItem>> GetAsync(string formKey, string gridName, CancellationToken cancellationToken = default);

    Task<bool> SaveAsync(string formKey, string gridName, IReadOnlyCollection<SaveGridColumnSettingRequest> columns, CancellationToken cancellationToken = default);
}
