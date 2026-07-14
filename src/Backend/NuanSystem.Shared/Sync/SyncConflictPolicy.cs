namespace NuanSystem.Shared.Sync;

public enum SyncConflictPolicy
{
    MasterWins = 0,
    BranchWins = 1,
    RejectOnConflict = 2,
    ManualReview = 3
}
