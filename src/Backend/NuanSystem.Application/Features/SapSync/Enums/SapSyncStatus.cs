namespace NuanSystem.Application.Features.SapSync.Enums;

public enum SapSyncStatus
{
    Pending = 1,
    Processing = 2,
    Synced = 3,
    Failed = 4,
    RetryScheduled = 5,
    DeadLetter = 6,
    Skipped = 7,
    NotImplemented = 8
}
