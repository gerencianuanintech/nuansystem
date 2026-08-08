namespace NuanSystem.Application.Features.Definitions.General.Provinces.Dtos;

public sealed class ProvinceDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public int CountryId { get; set; }
    public Guid CountryGlobalId { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ExternalSystem { get; set; }
    public string? ExternalCode { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed record ProvinceListFilter(
    string? Search,
    int PageNumber = 1,
    int PageSize = 50);

public sealed record ProvincePageDto(
    IReadOnlyCollection<ProvinceDto> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);

public sealed record ProvinceSyncPayload(
    Guid GlobalId,
    Guid CountryGlobalId,
    string CountryCode,
    string Code,
    string Name,
    bool IsActive,
    bool IsDeleted,
    string? ExternalSystem,
    string? ExternalCode,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record SaveProvinceData(
    int? Id,
    Guid GlobalId,
    int CountryId,
    string Code,
    string Name,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName,
    string? ExternalSystem = null,
    string? ExternalCode = null);
