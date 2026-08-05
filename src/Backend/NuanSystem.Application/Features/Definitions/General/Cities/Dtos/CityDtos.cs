namespace NuanSystem.Application.Features.Definitions.General.Cities.Dtos;

public sealed class CityDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public int CountryId { get; set; }
    public Guid CountryGlobalId { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public int ProvinceId { get; set; }
    public Guid ProvinceGlobalId { get; set; }
    public string ProvinceCode { get; set; } = string.Empty;
    public string ProvinceName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ExternalSystem { get; set; }
    public string? ExternalCode { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed record CitySyncPayload(
    Guid GlobalId,
    Guid CountryGlobalId,
    string CountryCode,
    Guid ProvinceGlobalId,
    string ProvinceCode,
    string Code,
    string Name,
    bool IsActive,
    bool IsDeleted,
    string? ExternalSystem,
    string? ExternalCode,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record SaveCityData(
    int? Id,
    Guid GlobalId,
    int CountryId,
    int ProvinceId,
    string Code,
    string Name,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName,
    string? ExternalSystem = null,
    string? ExternalCode = null);
