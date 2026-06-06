namespace NuanSystem.WinForms.Services.Documents.SecurityDocumentSeries.Models;

public sealed class ReserveSecurityDocumentNumberResult
{
    public bool Success { get; set; }
    public int? ReservedNumber { get; set; }
    public string? FormattedNumber { get; set; }
    public string? DisplayNumber { get; set; }
    public string Message { get; set; } = string.Empty;
}
