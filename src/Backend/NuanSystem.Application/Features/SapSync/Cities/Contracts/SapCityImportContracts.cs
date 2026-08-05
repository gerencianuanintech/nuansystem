namespace NuanSystem.Application.Features.SapSync.Cities.Contracts;

public sealed record SapCityPreviewItemDto(
    string SapCountryCode,
    string SapProvinceCode,
    string SapCityCode,
    string SapCityName,
    string Status,
    string StatusName,
    int? LocalCityId,
    string? LocalCountryCode,
    string? LocalProvinceCode,
    string? LocalCode,
    string? LocalName,
    string? DifferenceSummary,
    string? ResultCode);

public sealed record SapCityImportResultDto(
    int TotalRead,
    int Created,
    int Updated,
    int Unchanged,
    int ApprovalRequired,
    int Conflicts,
    int Skipped,
    int Failed,
    IReadOnlyCollection<SapCityImportItemResultDto> Items);

public sealed record SapCityImportItemResultDto(
    string SapCountryCode,
    string SapProvinceCode,
    string SapCityCode,
    string SapCityName,
    string Status,
    string Message,
    int? LocalCityId,
    Guid? LocalGlobalId,
    string ResultCode);

public interface ISapCityImportService
{
    Task<IReadOnlyCollection<SapCityPreviewItemDto>> PreviewAsync(
        int companyId,
        CancellationToken cancellationToken = default);

    Task<SapCityImportResultDto> ImportAsync(
        int companyId,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default);
}
