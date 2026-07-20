namespace NuanSystem.Application.Features.Sync.Dtos;

public sealed record ReferenceCatalogSyncPayload(
    Guid GlobalId,
    string Code,
    string Name,
    string? Description,
    decimal? Rate,
    string? CurrencyCode,
    string? AppliesTo,
    bool IsDefault,
    bool IsActive,
    string? ExternalSystem,
    string? ExternalCode,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    int? Days = null,
    bool? IsCredit = null);

public sealed record ReferenceCatalogSyncApplyResult(
    bool Applied,
    bool AlreadyApplied,
    int? LocalId,
    string Message);
