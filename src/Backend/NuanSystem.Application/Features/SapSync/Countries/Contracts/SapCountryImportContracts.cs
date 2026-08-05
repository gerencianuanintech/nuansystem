namespace NuanSystem.Application.Features.SapSync.Countries.Contracts;

public sealed record SapCountryPreviewItemDto(
    string SapCountryCode,
    string SapCountryName,
    string? Iso2,
    string? Iso3,
    string Status,
    string StatusName,
    int? LocalCountryId,
    string? LocalCode,
    string? LocalName,
    string? DifferenceSummary,
    string? ResultCode);

public sealed record SapCountryImportResultDto(
    int TotalRead,
    int Created,
    int Updated,
    int Unchanged,
    int ApprovalRequired,
    int Conflicts,
    int Skipped,
    int Failed,
    IReadOnlyCollection<SapCountryImportItemResultDto> Items);

public sealed record SapCountryImportItemResultDto(
    string SapCountryCode,
    string SapCountryName,
    string Status,
    string Message,
    int? LocalCountryId,
    Guid? LocalGlobalId,
    string ResultCode);

public interface ISapCountryImportService
{
    Task<IReadOnlyCollection<SapCountryPreviewItemDto>> PreviewAsync(
        int companyId,
        CancellationToken cancellationToken = default);

    Task<SapCountryImportResultDto> ImportAsync(
        int companyId,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default);
}
