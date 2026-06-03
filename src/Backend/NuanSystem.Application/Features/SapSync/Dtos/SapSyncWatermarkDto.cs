using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapSyncWatermarkDto(
    long Id,
    int CompanyId,
    string EntityCode,
    SapSyncDirection Direction,
    DateTime? LastSuccessfulSyncAtUtc,
    string? LastSapKey,
    string? LastLocalKey,
    string? MetadataJson,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
