using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapSyncLogRepository
{
    Task<IReadOnlyCollection<SapSyncLogDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<long> CreateAsync(CreateSapSyncLogData log, CancellationToken cancellationToken = default);
}
