using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapSyncInboxItemDto(
    long Id,
    int CompanyId,
    string EntityCode,
    string SapEntityId,
    string? PayloadJson,
    SapSyncStatus Status,
    int AttemptCount,
    DateTime? NextAttemptAtUtc,
    string? WorkerInstance,
    string? CorrelationId,
    DateTime CreatedAt,
    DateTime? LockedAt,
    DateTime? ExpiresAt);
