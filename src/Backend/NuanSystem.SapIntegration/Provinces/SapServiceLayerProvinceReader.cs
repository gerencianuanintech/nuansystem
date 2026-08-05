using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.SapIntegration.ServiceLayer;

namespace NuanSystem.SapIntegration.Provinces;

public sealed class SapServiceLayerProvinceReader(
    SapServiceLayerQueryClient queryClient) : ISapProvinceReader
{
    public async Task<IReadOnlyCollection<SapProvinceRecord>> GetProvincesAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        var rows = await queryClient.ReadAllAsync(
            companyId,
            SapProvinceQuery.Full,
            SapProvinceQuery.ReadOptions,
            cancellationToken);

        return rows.Select(SapProvinceMapper.Map).ToArray();
    }
}
