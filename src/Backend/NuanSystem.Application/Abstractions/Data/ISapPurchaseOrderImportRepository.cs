using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISapPurchaseOrderImportRepository : IRepository
{
    Task<SapPurchaseOrderImportApplyResult> UpsertAsync(SapPurchaseOrderImportData data, CancellationToken cancellationToken = default);
}
