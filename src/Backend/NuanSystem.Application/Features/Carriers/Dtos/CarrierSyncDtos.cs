namespace NuanSystem.Application.Features.Carriers.Dtos;

public sealed record CarrierSyncPayloadV1(
    Guid GlobalId,
    string Code,
    string Name,
    string IdentificationTypeCode,
    string IdentificationNumber,
    string? Description,
    bool IsActive,
    bool IsDeleted,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
