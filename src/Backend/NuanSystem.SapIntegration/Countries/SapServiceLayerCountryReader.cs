using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.SapIntegration.ServiceLayer;

namespace NuanSystem.SapIntegration.Countries;

public sealed class SapServiceLayerCountryReader(
    SapServiceLayerQueryClient queryClient) : ISapCountryReader
{
    public async Task<IReadOnlyCollection<SapCountryRecord>> GetCountriesAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        var rows = await queryClient.ReadAllAsync(
            companyId,
            SapCountryQuery.Full,
            SapCountryQuery.ReadOptions,
            cancellationToken);

        return rows.Select(SapCountryMapper.Map).ToArray();
    }
}
