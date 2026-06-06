namespace NuanSystem.WinForms.Services.Documents.SecurityDocumentSeries.Models;

public sealed class SecurityDocumentSeriesLookupItem
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

    public string DisplayText => $"{Code} - {Name}";
}
