using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapSyncExecutionContext(
    int CompanyId,
    string CompanyCode,
    string EntityCode,
    SapSyncDirection Direction,
    SapSyncOperation Operation,
    string WorkerInstance,
    string CorrelationId,
    int AttemptCount,
    DateTime StartedAtUtc);
