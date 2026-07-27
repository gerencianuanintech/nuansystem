namespace NuanSystem.WinForms.Services.SriTxtImports.Models;

public sealed class SriTxtImportFilter
{
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? Status { get; set; }
    public string? FileName { get; set; }
    public string? Environment { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public sealed class SriTxtImportSummary
{
    public long TotalRows { get; set; }
    public long ValidRows { get; set; }
    public long InvalidRows { get; set; }
    public long LinkedRows { get; set; }
    public long StagedRows { get; set; }
    public long PendingRows { get; set; }
}

public sealed class SriTxtImportListItem
{
    public long Id { get; set; }
    public Guid GlobalId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string FileSha256Hex { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string EncodingCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TotalRows { get; set; }
    public int ValidRows { get; set; }
    public int InvalidRows { get; set; }
    public int LinkedRows { get; set; }
    public int StagedRows { get; set; }
    public int PendingRows { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];
}

public sealed class SriTxtImportPage
{
    public IReadOnlyCollection<SriTxtImportListItem> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public SriTxtImportSummary Summary { get; set; } = new();
}

public sealed class SriTxtImportDetail
{
    public long Id { get; set; }
    public Guid GlobalId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string FileSha256Hex { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string EncodingCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TotalRows { get; set; }
    public int ValidRows { get; set; }
    public int InvalidRows { get; set; }
    public int DuplicateRows { get; set; }
    public int StagedRows { get; set; }
    public int PendingRows { get; set; }
    public int LinkedRows { get; set; }
    public int EnqueuedRows { get; set; }
    public int ConflictRows { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];
}

public sealed class SriTxtImportRow
{
    public long Id { get; set; }
    public Guid RowGlobalId { get; set; }
    public int LineNumber { get; set; }
    public string RowSha256Hex { get; set; } = string.Empty;
    public string? AccessKeySha256Hex { get; set; }
    public string? MaskedAccessKey { get; set; }
    public string? IssuerRuc { get; set; }
    public string? IssuerLegalName { get; set; }
    public string? DocumentTypeCode { get; set; }
    public string? DocumentTypeName { get; set; }
    public string? DocumentSeries { get; set; }
    public string? Environment { get; set; }
    public DateTime? AuthorizationAt { get; set; }
    public DateTime? EmissionDate { get; set; }
    public string? ReceiverIdentification { get; set; }
    public decimal? ValueWithoutTaxes { get; set; }
    public decimal? VatAmount { get; set; }
    public decimal? TotalAmount { get; set; }
    public string? ModifiedDocumentNumber { get; set; }
    public string ValidationStatus { get; set; } = string.Empty;
    public string EnqueueStatus { get; set; } = string.Empty;
    public long? QueueId { get; set; }
    public string? QueueStatus { get; set; }
    public string? QueueStatusDisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class SriTxtImportRowPage
{
    public IReadOnlyCollection<SriTxtImportRow> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
