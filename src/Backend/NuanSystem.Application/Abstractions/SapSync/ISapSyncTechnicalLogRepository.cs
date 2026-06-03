using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncTechnicalLogRepository
{
    Task<long> WriteAsync(SapSyncLogWriteDto log, CancellationToken cancellationToken = default);
}
