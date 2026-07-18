using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapWarehouseImportService
{
    Task<IReadOnlyCollection<SapWarehousePreviewItemDto>> PreviewAsync(
        int companyId,
        CancellationToken cancellationToken = default);

    Task<SapWarehouseImportResultDto> ImportAsync(
        int companyId,
        IReadOnlyCollection<SapWarehouseBranchMappingDto> mappings,
        int? auditUserId,
        string? auditUserName,
        CancellationToken cancellationToken = default);
}
