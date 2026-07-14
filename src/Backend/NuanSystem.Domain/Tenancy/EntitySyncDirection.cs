namespace NuanSystem.Domain.Tenancy;

public enum EntitySyncDirection
{
    None = 0,
    SapToNuan = 1,
    NuanToSap = 2,
    Bidirectional = 3,
    MasterToBranch = 4,
    BranchToMaster = 5
}

