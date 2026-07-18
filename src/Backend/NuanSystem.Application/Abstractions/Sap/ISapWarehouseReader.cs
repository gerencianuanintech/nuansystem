using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapWarehouseReader
{
    Task<IReadOnlyCollection<SapWarehouseRecord>> GetWarehousesAsync(
        int companyId,
        CancellationToken cancellationToken = default);
}
