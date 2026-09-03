namespace NuanSystem.Application.Features.BusinessPartners.Sync;

public static class BusinessPartnerSyncSchemaVersions
{
    public const int Proposal = 1;
    public const int Canonical = 2;
    public const int ProposalResult = 1;
}

public sealed record BusinessPartnerProposalPayloadV1(
    int SchemaVersion,
    Guid GlobalId,
    string Code,
    string PartnerType,
    string IdentificationTypeCode,
    string IdentificationNumber,
    string NormalizedIdentificationNumber,
    long BaseCanonicalVersion,
    int? OriginUserId,
    string? OriginUserName,
    BusinessPartnerCanonicalSnapshot? Base,
    BusinessPartnerCanonicalSnapshot Proposed,
    IReadOnlyCollection<string> ChangedFields);

public sealed record BusinessPartnerCanonicalPayloadV2(
    int SchemaVersion,
    long CanonicalVersion,
    int? OriginCompanyId,
    Guid? CausationEventId,
    BusinessPartnerCanonicalSnapshot Partner);

public sealed record BusinessPartnerProposalResultPayloadV1(
    int SchemaVersion,
    Guid GlobalId,
    Guid ProposalEventId,
    string Status,
    string? Message,
    long CanonicalVersion,
    BusinessPartnerCanonicalSnapshot? Canonical);

public sealed record BusinessPartnerCanonicalSnapshot(
    Guid GlobalId,
    string Code,
    string Name,
    string? CommercialName,
    string PartnerType,
    string IdentificationTypeCode,
    string IdentificationNumber,
    string NormalizedIdentificationNumber,
    string? Email,
    string? Phone,
    string? SapCardCode,
    bool IsActive,
    IReadOnlyCollection<BusinessPartnerAddressSnapshot> Addresses,
    IReadOnlyCollection<BusinessPartnerContactSnapshot> Contacts);

public sealed record BusinessPartnerAddressSnapshot(
    Guid GlobalId,
    string AddressType,
    string Line1,
    string? Line2,
    string? CountryCode,
    string? ProvinceCode,
    string? CityCode,
    string? PostalCode,
    decimal? Latitude,
    decimal? Longitude,
    bool IsPrimary,
    bool IsActive);

public sealed record BusinessPartnerContactSnapshot(
    Guid GlobalId,
    string? ContactTypeCode,
    string? ContactChannelCode,
    string Name,
    string? Position,
    string? Department,
    string? Phone,
    string? Extension,
    string? Mobile,
    string? Email,
    string? Language,
    bool ReceivesNotifications,
    bool IsPrimary,
    bool IsActive,
    string? Notes);

public enum BusinessPartnerMergeStatus
{
    Accepted,
    Conflict,
    Rejected
}

public sealed record BusinessPartnerMergeResult(
    BusinessPartnerMergeStatus Status,
    BusinessPartnerCanonicalSnapshot? Merged,
    IReadOnlyCollection<string> ConflictFields,
    string? ErrorCode = null);
