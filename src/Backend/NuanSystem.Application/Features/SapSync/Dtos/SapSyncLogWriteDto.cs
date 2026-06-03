using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapSyncLogWriteDto(
    int CompanyId,
    string CompanyCode,
    string EntityCode,
    SapSyncDirection Direction,
    SapSyncOperation Operation,
    SapSyncStatus Status,
    string CorrelationId,
    string WorkerInstance,
    int AttemptCount,
    long? QueueItemId,
    string? LocalEntityId,
    string? SapEntityId,
    int? SapDocEntry,
    int? SapDocNum,
    string? RequestJson,
    string? ResponseJson,
    string? ErrorCode,
    string? ErrorMessage,
    long DurationMs,
    DateTime StartedAtUtc,
    DateTime FinishedAtUtc);
