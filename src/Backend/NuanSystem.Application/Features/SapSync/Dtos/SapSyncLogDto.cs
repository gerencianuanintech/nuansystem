namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapSyncLogDto(
    long Id,
    int CompanyId,
    string EntityType,
    string EntityId,
    string SapObjectType,
    string? Status,
    string? ErrorMessage,
    int? SapDocEntry,
    int? SapDocNum,
    DateTime CreatedAt,
    DateTime? SyncedAt);
