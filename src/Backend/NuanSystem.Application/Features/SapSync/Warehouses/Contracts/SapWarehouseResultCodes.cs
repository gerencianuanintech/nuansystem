namespace NuanSystem.Application.Features.SapSync.Warehouses.Contracts;

public static class SapWarehouseResultCodes
{
    public const string Created = "SAP_WAREHOUSE_CREATED";
    public const string Updated = "SAP_WAREHOUSE_UPDATED";
    public const string Unchanged = "SAP_WAREHOUSE_UNCHANGED";
    public const string ApprovalRequired = "SAP_WAREHOUSE_APPROVAL_REQUIRED";
    public const string CodeCollisionApprovalRequired = "SAP_WAREHOUSE_CODE_COLLISION_APPROVAL_REQUIRED";
    public const string Inactive = "SAP_WAREHOUSE_INACTIVE";
    public const string Invalid = "SAP_WAREHOUSE_INVALID";
    public const string IdentityConflict = "SAP_WAREHOUSE_IDENTITY_CONFLICT";
    public const string SaveFailed = "SAP_WAREHOUSE_SAVE_FAILED";
    public const string SnapshotInvalid = "SAP_WAREHOUSE_SNAPSHOT_INVALID";
}

public sealed record SapWarehouseRecordProcessResult(
    string Action,
    string Status,
    int? LocalWarehouseId,
    Guid? LocalGlobalId,
    string ResultCode,
    string SafeMessage);
