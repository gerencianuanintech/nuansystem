namespace NuanSystem.Application.Features.SriTxtImports.Dtos;

public sealed class SriTxtImportDetailDto
{
    public long Id { get; set; }
    public Guid GlobalId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string FileSha256Hex { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string EncodingCode { get; set; } = string.Empty;
    public string HeaderLine { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TotalRows { get; set; }
    public int ValidRows { get; set; }
    public int InvalidRows { get; set; }
    public int DuplicateRows { get; set; }
    public int StagedRows { get; set; }
    public int LinkedRows { get; set; }
    public int EnqueuedRows { get; set; }
    public int ConflictRows { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];
}

public sealed record SriTxtParsedFile(
    byte[] FileSha256,
    string EncodingCode,
    string HeaderLine,
    IReadOnlyCollection<SriTxtParsedRow> Rows);

public sealed record SriTxtParsedRow(
    Guid RowGlobalId,
    int LineNumber,
    byte[] RowSha256,
    string? AccessKey,
    byte[]? AccessKeySha256,
    string? MaskedAccessKey,
    string? IssuerRuc,
    string? IssuerLegalName,
    string? DocumentTypeCode,
    string? DocumentTypeName,
    string? DocumentSeries,
    string? Environment,
    DateTime? AuthorizationAt,
    DateTime? EmissionDate,
    string? ReceiverIdentification,
    decimal? ValueWithoutTaxes,
    decimal? VatAmount,
    decimal? TotalAmount,
    string? ModifiedDocumentNumber,
    string ValidationStatus,
    string? ValidationCode,
    string? ValidationMessage);

public sealed record RegisterValidatedSriTxtImportData(
    Guid GlobalId,
    string OriginalFileName,
    byte[] FileSha256,
    long FileSizeBytes,
    string EncodingCode,
    string HeaderLine,
    IReadOnlyCollection<SriTxtParsedRow> Rows,
    Guid TraceId,
    int? AuditUserId,
    string? AuditUserName);

public sealed record SriTxtImportPersistenceResult(
    SriTxtImportDetailDto Import,
    bool IsCreated);

public sealed record EnqueueSriTxtImportData(
    long ImportId,
    byte[] RowVersion,
    Guid TraceId,
    int? AuditUserId,
    string? AuditUserName);

public enum SriTxtImportEnqueueCode
{
    Updated = 1,
    NotFound = 0,
    ConcurrencyConflict = -2,
    InvalidState = -3
}

public sealed record SriTxtImportEnqueuePersistenceResult(
    SriTxtImportEnqueueCode Code,
    SriTxtImportDetailDto? Import);
