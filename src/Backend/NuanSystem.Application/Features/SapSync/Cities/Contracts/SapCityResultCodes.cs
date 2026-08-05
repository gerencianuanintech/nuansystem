namespace NuanSystem.Application.Features.SapSync.Cities.Contracts;

public static class SapCityResultCodes
{
    public const string Created = "SAP_CITY_CREATED";
    public const string Updated = "SAP_CITY_UPDATED";
    public const string Unchanged = "SAP_CITY_UNCHANGED";
    public const string CodeCollisionApprovalRequired = "SAP_CITY_CODE_COLLISION_APPROVAL_REQUIRED";
    public const string CountryNotFound = "SAP_CITY_COUNTRY_NOT_FOUND";
    public const string CountryIdentityConflict = "SAP_CITY_COUNTRY_IDENTITY_CONFLICT";
    public const string ProvinceNotFound = "SAP_CITY_PROVINCE_NOT_FOUND";
    public const string ProvinceIdentityConflict = "SAP_CITY_PROVINCE_IDENTITY_CONFLICT";
    public const string Invalid = "SAP_CITY_INVALID";
    public const string IdentityConflict = "SAP_CITY_IDENTITY_CONFLICT";
    public const string SaveFailed = "SAP_CITY_SAVE_FAILED";
    public const string SnapshotInvalid = "SAP_CITY_SNAPSHOT_INVALID";
}

public sealed record SapCityRecordProcessResult(
    string Action,
    string Status,
    int? LocalCityId,
    Guid? LocalGlobalId,
    string ResultCode,
    string SafeMessage);
