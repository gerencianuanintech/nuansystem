using NuanSystem.Application.Features.SapSync.Scheduling;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncScheduleRepository
{
    Task<SapSyncScheduleCandidatePage> GetCandidatesAsync(
        SapSyncScheduleCursor cursor,
        int pageSize,
        DateTime utcNow,
        CancellationToken cancellationToken = default);

    Task<bool> TryReserveAsync(
        SapSyncScheduleReservation reservation,
        CancellationToken cancellationToken = default);
}
