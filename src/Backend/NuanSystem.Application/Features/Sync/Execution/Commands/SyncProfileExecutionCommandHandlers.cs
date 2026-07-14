using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Sync.Execution.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Sync.Execution.Commands;

public sealed class ExecuteSyncProfileCommandHandler(ISyncProfileExecutionService executionService)
    : ICommandHandler<ExecuteSyncProfileCommand, CreateSyncProfileExecutionResultDto>
{
    public Task<Result<CreateSyncProfileExecutionResultDto>> Handle(
        ExecuteSyncProfileCommand request,
        CancellationToken cancellationToken)
    {
        var executionRequest = new SyncProfileExecutionRequest
        {
            ExecutionType = "Manual",
            RequestedBy = request.AuditUserName ?? "Usuario",
            EntityCodes = request.Request.EntityCodes,
            FromKey = request.Request.FromKey,
            MaxRecords = request.Request.MaxRecords
        };

        return executionService.RequestExecutionAsync(
            request.SyncProfileId,
            executionRequest,
            request.AuditUserId,
            request.AuditUserName,
            cancellationToken);
    }
}

public sealed class CancelSyncProfileExecutionCommandHandler(ISyncProfileExecutionService executionService)
    : ICommandHandler<CancelSyncProfileExecutionCommand, CancelSyncProfileExecutionResultDto>
{
    public Task<Result<CancelSyncProfileExecutionResultDto>> Handle(
        CancelSyncProfileExecutionCommand request,
        CancellationToken cancellationToken)
    {
        return executionService.CancelAsync(
            request.ExecutionId,
            request.AuditUserName ?? request.AuditUserId?.ToString(),
            cancellationToken);
    }
}

public sealed class RetrySyncProfileExecutionCommandHandler(ISyncProfileExecutionService executionService)
    : ICommandHandler<RetrySyncProfileExecutionCommand, RetrySyncProfileExecutionResultDto>
{
    public Task<Result<RetrySyncProfileExecutionResultDto>> Handle(
        RetrySyncProfileExecutionCommand request,
        CancellationToken cancellationToken)
    {
        return executionService.RetryAsync(
            request.ExecutionId,
            request.AuditUserName ?? request.AuditUserId?.ToString(),
            cancellationToken);
    }
}
