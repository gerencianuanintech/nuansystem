namespace NuanSystem.WinForms.Forms.BusinessPartners;

internal sealed class SupplierAttachmentViewModel
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string DocumentType { get; init; } = string.Empty;

    public string FileName { get; init; } = string.Empty;

    public DateTime UploadDate { get; init; }

    public string User { get; init; } = string.Empty;

    public string FileSize { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string FilePath { get; init; } = string.Empty;

    public string Category { get; init; } = string.Empty;

    public DateTime? ExpirationDate { get; init; }

    public string Description { get; init; } = string.Empty;
}
