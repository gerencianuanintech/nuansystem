using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncEntityHandler
{
    string EntityCode { get; }
    Task<SapSyncExecutionResult> ImportFromSapAsync(SapSyncExecutionContext context, CancellationToken cancellationToken = default);
    Task<SapSyncExecutionResult> ExportToSapAsync(SapSyncExecutionContext context, CancellationToken cancellationToken = default);
}
