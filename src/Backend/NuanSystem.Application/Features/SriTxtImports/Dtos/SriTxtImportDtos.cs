using System.Text.Json.Serialization;

namespace NuanSystem.Application.Features.SriTxtImports.Dtos;

public sealed class SriTxtImportDetailDto
{
    public long Id { get; set; }
    public Guid GlobalId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string FileSha256Hex { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string EncodingCode { get; set; } = string.Empty;
    [JsonIgnore]
    public string HeaderLine { get; set; } = string.Empty;
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
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];
}

public sealed record SriTxtImportFilter(
    DateTime? CreatedFrom,
    DateTime? CreatedTo,
    string? Status,
    string? FileName,
    string? Environment,
    int Page = 1,
    int PageSize = 50);

public sealed class SriTxtImportListItemDto
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

public sealed class SriTxtImportSummaryDto
{
    public long TotalRows { get; set; }
    public long ValidRows { get; set; }
    public long InvalidRows { get; set; }
    public long LinkedRows { get; set; }
    public long StagedRows { get; set; }
    public long PendingRows { get; set; }
}

public sealed record SriTxtImportPageDto(
    IReadOnlyCollection<SriTxtImportListItemDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    SriTxtImportSummaryDto Summary);

public sealed record SriTxtImportRowFilter(
    string Validity = SriTxtRowValidityCodes.All,
    int Page = 1,
    int PageSize = 100);

public sealed class SriTxtImportRowDto
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
    public string? ValidationCode { get; set; }
    public string? ValidationMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];
}

public sealed record SriTxtImportRowPageDto(
    IReadOnlyCollection<SriTxtImportRowDto> Items,
    int TotalCount,
    int Page,
    int PageSize);

public sealed record SriTxtParsedFile(
    byte[] FileSha256,
    string EncodingCode,
    string HeaderLine,
    IReadOnlyCollection<SriTxtParsedRow> Rows);

public sealed record SriTxtParsedRow(
    Guid RowGlobalId,
    int LineNumber,
    byte[] RowSha256,
    [property: JsonIgnore] string? AccessKey,
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
