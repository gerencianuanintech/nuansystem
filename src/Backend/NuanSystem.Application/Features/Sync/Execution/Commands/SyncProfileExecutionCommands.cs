using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Sync.Execution.Dtos;

namespace NuanSystem.Application.Features.Sync.Execution.Commands;

public sealed record ExecuteSyncProfileCommand(
    int SyncProfileId,
    ExecuteSyncProfileRequest Request,
    int? AuditUserId,
    string? AuditUserName) : ICommand<CreateSyncProfileExecutionResultDto>;

public sealed record CancelSyncProfileExecutionCommand(
    int ExecutionId,
    int? AuditUserId,
    string? AuditUserName) : ICommand<CancelSyncProfileExecutionResultDto>;

public sealed record RetrySyncProfileExecutionCommand(
    int ExecutionId,
    int? AuditUserId,
    string? AuditUserName) : ICommand<RetrySyncProfileExecutionResultDto>;
