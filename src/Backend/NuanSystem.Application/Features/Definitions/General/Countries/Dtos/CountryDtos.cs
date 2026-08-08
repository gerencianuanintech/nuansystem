namespace NuanSystem.Application.Features.Definitions.General.Countries.Dtos;

public sealed class CountryDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Iso2 { get; set; }
    public string? Iso3 { get; set; }
    public string? PhonePrefix { get; set; }
    public string? ExternalSystem { get; set; }
    public string? ExternalCode { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed record CountryListFilter(
    string? Search,
    int PageNumber = 1,
    int PageSize = 50);

public sealed record CountryPageDto(
    IReadOnlyCollection<CountryDto> Items,
    int TotalCount,
    int PageNumber,
    int PageSize);

public sealed record CountrySyncPayload(
    Guid GlobalId,
    string Code,
    string Name,
    string? Iso2,
    string? Iso3,
    string? PhonePrefix,
    bool IsActive,
    bool IsDeleted,
    string? ExternalSystem,
    string? ExternalCode,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record SaveCountryData(
    int? Id,
    Guid GlobalId,
    string Code,
    string Name,
    string? Iso2,
    string? Iso3,
    string? PhonePrefix,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName,
    string? ExternalSystem = null,
    string? ExternalCode = null);
