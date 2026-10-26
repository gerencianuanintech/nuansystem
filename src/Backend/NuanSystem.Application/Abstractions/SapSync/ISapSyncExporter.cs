using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncExporter
{
    string EntityCode { get; }
    IReadOnlyCollection<string> SupportedModes { get; }
    Task<SapSyncExecutionResult> ExportToSapAsync(SapSyncExecutionContext context, CancellationToken cancellationToken = default);
}
