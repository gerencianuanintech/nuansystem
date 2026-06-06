namespace NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Dtos;

public sealed class SecurityDocumentSeriesDto
{
    public int Id { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Prefix { get; set; } = string.Empty;
    public string Establishment { get; set; } = string.Empty;
    public string EmissionPoint { get; set; } = string.Empty;
    public int InitialNumber { get; set; }
    public int CurrentNumber { get; set; }
    public int NextNumber { get; set; }
    public int NumberLength { get; set; }
    public string NextNumberFormatted { get; set; } = string.Empty;
    public string? SapObjectType { get; set; }
    public int? SapSeriesId { get; set; }
    public string? SapSeriesName { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public bool IsSapIntegrationActive { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class SecurityDocumentSeriesLookupDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public string Establishment { get; set; } = string.Empty;
    public string EmissionPoint { get; set; } = string.Empty;
    public int NextNumber { get; set; }
    public int NumberLength { get; set; }
    public string NextNumberFormatted { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}

public sealed record SecurityDocumentSeriesFilterData(
    string? Search,
    string? DocumentType,
    bool? IsActive);

public sealed record CreateSecurityDocumentSeriesData(
    string DocumentType,
    string Code,
    string Name,
    string? Description,
    string Prefix,
    string Establishment,
    string EmissionPoint,
    int InitialNumber,
    int CurrentNumber,
    int NextNumber,
    int NumberLength,
    string? SapObjectType,
    int? SapSeriesId,
    string? SapSeriesName,
    bool IsDefault,
    bool IsActive,
    bool IsSapIntegrationActive,
    int? CreatedByUserId,
    string? CreatedByUserName);

public sealed record UpdateSecurityDocumentSeriesData(
    int Id,
    string DocumentType,
    string Code,
    string Name,
    string? Description,
    string Prefix,
    string Establishment,
    string EmissionPoint,
    int InitialNumber,
    int CurrentNumber,
    int NextNumber,
    int NumberLength,
    string? SapObjectType,
    int? SapSeriesId,
    string? SapSeriesName,
    bool IsDefault,
    bool IsActive,
    bool IsSapIntegrationActive,
    int? UpdatedByUserId,
    string? UpdatedByUserName);

public sealed record ReserveSecurityDocumentNumberResult(
    bool Success,
    int? ReservedNumber,
    string? FormattedNumber,
    string? DisplayNumber,
    string Message);
