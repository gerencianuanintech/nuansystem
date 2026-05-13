using NuanSystem.Application.Features.ConfigurationSettings.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IConfigurationSettingRepository : IRepository
{
    Task<IReadOnlyCollection<ConfigurationSettingDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ConfigurationSettingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByKeyAsync(string key, CancellationToken cancellationToken = default);

    Task<bool> ExistsByKeyAsync(string key, int excludingId, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateConfigurationSettingData setting, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateConfigurationSettingData setting, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
}
