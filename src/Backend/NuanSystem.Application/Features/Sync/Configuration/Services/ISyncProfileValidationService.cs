using NuanSystem.Application.Features.Sync.Configuration.Dtos;

namespace NuanSystem.Application.Features.Sync.Configuration.Services;

public interface ISyncProfileValidationService
{
    Task<SyncProfileValidationResultDto> ValidateAsync(
        SaveSyncProfileRequest request,
        int? profileId,
        int? userId,
        CancellationToken cancellationToken = default);

    Task<SyncProfileValidationResultDto> ValidatePersistedAsync(
        int profileId,
        int? userId,
        CancellationToken cancellationToken = default);
}
