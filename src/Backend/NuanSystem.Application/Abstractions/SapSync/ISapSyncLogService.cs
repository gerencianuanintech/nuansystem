using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncLogService
{
    Task WriteAsync(SapSyncLogWriteDto log, CancellationToken cancellationToken = default);
}
