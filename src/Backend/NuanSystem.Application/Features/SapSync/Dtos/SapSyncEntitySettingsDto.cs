using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapSyncEntitySettingsDto(
    long Id,
    int CompanyId,
    string CompanyCode,
    string EntityCode,
    SapSyncDirection Direction,
    bool IsEnabled,
    int BatchSize,
    int MaxRetryCount,
    int ExecutionOrder,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
