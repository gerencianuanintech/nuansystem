using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.SapIntegration.ServiceLayer;

namespace NuanSystem.SapIntegration.Warehouses;

public sealed class SapServiceLayerWarehouseReader(
    SapServiceLayerQueryClient queryClient) : ISapWarehouseReader
{
    public async Task<IReadOnlyCollection<SapWarehouseRecord>> GetWarehousesAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        var rows = await queryClient.ReadAllAsync(
            companyId,
            SapWarehouseQuery.Full,
            SapWarehouseQuery.ReadOptions,
            cancellationToken);

        return rows.Select(SapWarehouseMapper.Map).ToArray();
    }
}
