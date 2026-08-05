namespace NuanSystem.Application.Features.SapSync.Countries.Contracts;

public static class SapCountryResultCodes
{
    public const string Created = "SAP_COUNTRY_CREATED";
    public const string Updated = "SAP_COUNTRY_UPDATED";
    public const string Unchanged = "SAP_COUNTRY_UNCHANGED";
    public const string CodeCollisionApprovalRequired = "SAP_COUNTRY_CODE_COLLISION_APPROVAL_REQUIRED";
    public const string Invalid = "SAP_COUNTRY_INVALID";
    public const string IdentityConflict = "SAP_COUNTRY_IDENTITY_CONFLICT";
    public const string SaveFailed = "SAP_COUNTRY_SAVE_FAILED";
    public const string SnapshotInvalid = "SAP_COUNTRY_SNAPSHOT_INVALID";
}

public sealed record SapCountryRecordProcessResult(
    string Action,
    string Status,
    int? LocalCountryId,
    Guid? LocalGlobalId,
    string ResultCode,
    string SafeMessage);
