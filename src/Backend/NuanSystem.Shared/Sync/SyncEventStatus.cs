namespace NuanSystem.Shared.Sync;

public enum SyncEventStatus
{
    Pending = 0,
    InProcess = 1,
    Applied = 2,
    Error = 3,
    Ignored = 4,
    DeadLetter = 5
}
