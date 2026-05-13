namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record CreateSapSyncLogData(
    int CompanyId,
    string EntityType,
    string EntityId,
    string SapObjectType,
    string? RequestJson,
    string? ResponseJson,
    string Status,
    string? ErrorMessage,
    int? SapDocEntry,
    int? SapDocNum,
    DateTime? SyncedAt);
