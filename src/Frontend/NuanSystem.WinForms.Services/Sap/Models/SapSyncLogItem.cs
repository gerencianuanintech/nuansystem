namespace NuanSystem.WinForms.Services.Sap.Models;

public sealed record SapSyncLogItem(
    long Id,
    string EntityType,
    long EntityId,
    string SapObjectType,
    string Status,
    string? ErrorMessage,
    int? SapDocEntry,
    int? SapDocNum,
    DateTime CreatedAtUtc,
    DateTime? SyncedAtUtc);
