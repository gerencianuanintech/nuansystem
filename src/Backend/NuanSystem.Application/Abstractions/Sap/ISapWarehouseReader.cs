using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapWarehouseReader
{
    Task<IReadOnlyCollection<SapWarehouseRecord>> GetWarehousesAsync(
        int companyId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SapWarehouseRecord>> GetWarehousesAsync(
        int companyId,
        SapWarehouseFilter filter,
        CancellationToken cancellationToken = default);
}

public sealed record SapWarehouseFilter(
    string? NameContains = null,
    string? ExactName = null);
