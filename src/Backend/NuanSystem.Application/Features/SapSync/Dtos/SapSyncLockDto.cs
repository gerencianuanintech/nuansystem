using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapSyncLockDto(
    long Id,
    int CompanyId,
    string EntityCode,
    SapSyncDirection Direction,
    string WorkerInstance,
    string CorrelationId,
    Guid? ExecutionUid,
    string OwnerToken,
    DateTime LockedAtUtc,
    DateTime? RenewedAtUtc,
    DateTime LockExpiresAtUtc);
