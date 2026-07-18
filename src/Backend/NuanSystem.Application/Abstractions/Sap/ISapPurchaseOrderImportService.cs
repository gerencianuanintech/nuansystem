using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapPurchaseOrderImportService
{
    Task<SapPurchaseOrderImportResultDto> ImportAsync(int companyId, DateTime? modifiedSince, int? auditUserId, string? auditUserName, CancellationToken cancellationToken = default);
}
