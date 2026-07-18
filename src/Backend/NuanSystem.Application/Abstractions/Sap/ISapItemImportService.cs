using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapItemImportService
{
    Task<IReadOnlyCollection<SapItemPreviewItemDto>> PreviewAsync(
        int companyId,
        int take,
        string? search,
        CancellationToken cancellationToken = default);

    Task<SapItemImportResultDto> ImportAsync(
        int companyId,
        IReadOnlyCollection<string>? sapItemCodes,
        int? auditUserId,
        string? auditUserName,
        bool writePublicSapLog = true,
        CancellationToken cancellationToken = default);
}
