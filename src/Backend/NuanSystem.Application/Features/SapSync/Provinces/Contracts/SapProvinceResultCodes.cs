namespace NuanSystem.Application.Features.SapSync.Provinces.Contracts;

public static class SapProvinceResultCodes
{
    public const string Created = "SAP_PROVINCE_CREATED";
    public const string Updated = "SAP_PROVINCE_UPDATED";
    public const string Unchanged = "SAP_PROVINCE_UNCHANGED";
    public const string CodeCollisionApprovalRequired = "SAP_PROVINCE_CODE_COLLISION_APPROVAL_REQUIRED";
    public const string CountryNotFound = "SAP_PROVINCE_COUNTRY_NOT_FOUND";
    public const string CountryIdentityConflict = "SAP_PROVINCE_COUNTRY_IDENTITY_CONFLICT";
    public const string Invalid = "SAP_PROVINCE_INVALID";
    public const string IdentityConflict = "SAP_PROVINCE_IDENTITY_CONFLICT";
    public const string SaveFailed = "SAP_PROVINCE_SAVE_FAILED";
    public const string SnapshotInvalid = "SAP_PROVINCE_SNAPSHOT_INVALID";
}

public sealed record SapProvinceRecordProcessResult(
    string Action,
    string Status,
    int? LocalProvinceId,
    Guid? LocalGlobalId,
    string ResultCode,
    string SafeMessage);
