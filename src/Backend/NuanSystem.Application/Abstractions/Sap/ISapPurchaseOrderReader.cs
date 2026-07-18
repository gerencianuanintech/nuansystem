using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapPurchaseOrderReader
{
    Task<IReadOnlyCollection<SapPurchaseOrderRecord>> GetPurchaseOrdersAsync(int companyId, DateTime? modifiedSince, CancellationToken cancellationToken = default);
}
