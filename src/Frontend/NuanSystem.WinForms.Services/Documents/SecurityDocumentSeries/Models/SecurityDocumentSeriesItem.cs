namespace NuanSystem.WinForms.Services.Documents.SecurityDocumentSeries.Models;

public sealed class SecurityDocumentSeriesItem
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

    public string DocumentTypeName => SecurityDocumentSeriesCatalogs.GetDocumentTypeName(DocumentType);

    public string DisplayNumber => string.IsNullOrWhiteSpace(NextNumberFormatted)
        ? NextNumber.ToString().PadLeft(Math.Max(1, NumberLength), '0')
        : NextNumberFormatted;
}
