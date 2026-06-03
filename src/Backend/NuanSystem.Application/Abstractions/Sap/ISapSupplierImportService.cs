using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapSupplierImportService
{
    Task<IReadOnlyCollection<SapSupplierPreviewItemDto>> PreviewAsync(
        int companyId,
        CancellationToken cancellationToken = default);

    Task<SapSupplierImportResultDto> ImportAsync(
        int companyId,
        SapSupplierImportOptions options,
        CancellationToken cancellationToken = default);

    Task<SapSupplierImportItemResultDto> ImportOneAsync(
        SapSupplierRecord supplier,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default);
}
