namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapSendResultDto(
    bool Success,
    string Status,
    string? ErrorMessage,
    int? SapDocEntry,
    int? SapDocNum,
    long SyncLogId);
