using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncSettingsRepository
{
    Task<IReadOnlyCollection<SapSyncEntitySettingsDto>> GetEnabledEntitiesAsync(int companyId, CancellationToken cancellationToken = default);
}
