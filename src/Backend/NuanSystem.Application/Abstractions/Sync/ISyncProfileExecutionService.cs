using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Sync.Execution.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncProfileExecutionService
{
    Task<Result<CreateSyncProfileExecutionResultDto>> RequestExecutionAsync(
        int syncProfileId,
        SyncProfileExecutionRequest request,
        int? auditUserId = null,
        string? auditUserName = null,
        CancellationToken cancellationToken = default);

    Task ProcessPendingAsync(CancellationToken cancellationToken = default);

    Task<Result<CancelSyncProfileExecutionResultDto>> CancelAsync(
        int executionId,
        string? requestedBy,
        CancellationToken cancellationToken = default);

    Task<Result<RetrySyncProfileExecutionResultDto>> RetryAsync(
        int executionId,
        string? requestedBy,
        CancellationToken cancellationToken = default);
}
