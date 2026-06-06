namespace NuanSystem.Application.Features.SecurityAccess.Dtos;

public sealed class SecurityDocumentSeriesAccessDto
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
    public bool IsSelected { get; set; }
}

public sealed record SecurityDocumentSeriesOperationAccessDto(
    int? SecurityRoleDocumentSeriesId,
    int OperationId,
    string OperationCode,
    string OperationName,
    string? OperationDescription,
    string? ActionKey,
    string? IconLarge,
    string? IconSmall,
    int DisplayOrder,
    bool IsAllowed,
    int? UpdatedByUserId,
    string? UpdatedByUserName,
    DateTime? UpdatedAt,
    int? CreatedByUserId,
    string? CreatedByUserName,
    DateTime? CreatedAt);

public sealed record SaveSecurityDocumentSeriesOperationAccessData(
    int? OperationId,
    string ActionKey,
    bool IsAllowed);

public sealed record SaveSecurityDocumentSeriesAccessRequest(
    bool IsSelected,
    IReadOnlyCollection<SaveSecurityDocumentSeriesOperationAccessData> Operations);
