namespace NuanSystem.Shared.Sync;

public enum SyncAuditAction
{
    Created = 0,
    TargetCreated = 1,
    Claimed = 2,
    Applied = 3,
    Failed = 4,
    Ignored = 5,
    Retried = 6,
    DeadLetter = 7,
    DryRun = 8,
    RetriedFromDeadLetter = 9,
    LockReleased = 10
}
