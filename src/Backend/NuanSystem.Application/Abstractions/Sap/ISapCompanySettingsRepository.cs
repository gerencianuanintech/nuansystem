using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapCompanySettingsRepository
{
    Task<SapCompanySettingsDto?> GetByCompanyIdAsync(
        int companyId,
        CancellationToken cancellationToken = default);

    Task<SapCompanySettingsDto?> GetByCompanyCodeAsync(
        string companyCode,
        CancellationToken cancellationToken = default);

    Task<int> UpsertServiceLayerAsync(
        UpdateSapServiceLayerSettingsData settings,
        CancellationToken cancellationToken = default);

    Task<int> UpdateCitiesSelectQueryAsync(
        UpdateSapCityQuerySettingsData settings,
        CancellationToken cancellationToken = default);
}
