using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapSyncOutboxItemDto(
    long Id,
    int CompanyId,
    string EntityCode,
    string OperationCode,
    string LocalEntityId,
    string? PayloadJson,
    SapSyncStatus Status,
    int AttemptCount,
    DateTime? NextAttemptAtUtc,
    string? WorkerInstance,
    string? CorrelationId,
    DateTime CreatedAt,
    DateTime? LockedAt,
    DateTime? ExpiresAt);
