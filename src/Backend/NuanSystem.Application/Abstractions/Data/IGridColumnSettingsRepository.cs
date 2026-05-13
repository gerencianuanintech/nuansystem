using NuanSystem.Application.Features.GridColumnSettings.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IGridColumnSettingsRepository
{
    Task<IReadOnlyCollection<GridColumnSettingDto>> GetUserSettingsAsync(
        int userId,
        string formKey,
        string gridName,
        CancellationToken cancellationToken = default);

    Task SaveUserSettingsAsync(
        int userId,
        string formKey,
        string gridName,
        IReadOnlyCollection<SaveGridColumnSettingData> columns,
        int? updatedByUserId,
        string? updatedByUserName,
        CancellationToken cancellationToken = default);
}
