namespace NuanSystem.Application.Features.SapSync.Provinces.Contracts;

public sealed record SapProvincePreviewItemDto(
    string SapCountryCode,
    string SapProvinceCode,
    string SapProvinceName,
    string Status,
    string StatusName,
    int? LocalProvinceId,
    string? LocalCountryCode,
    string? LocalCode,
    string? LocalName,
    string? DifferenceSummary,
    string? ResultCode);

public sealed record SapProvinceImportResultDto(
    int TotalRead,
    int Created,
    int Updated,
    int Unchanged,
    int ApprovalRequired,
    int Conflicts,
    int Skipped,
    int Failed,
    IReadOnlyCollection<SapProvinceImportItemResultDto> Items);

public sealed record SapProvinceImportItemResultDto(
    string SapCountryCode,
    string SapProvinceCode,
    string SapProvinceName,
    string Status,
    string Message,
    int? LocalProvinceId,
    Guid? LocalGlobalId,
    string ResultCode);

public interface ISapProvinceImportService
{
    Task<IReadOnlyCollection<SapProvincePreviewItemDto>> PreviewAsync(
        int companyId,
        CancellationToken cancellationToken = default);

    Task<SapProvinceImportResultDto> ImportAsync(
        int companyId,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default);
}
