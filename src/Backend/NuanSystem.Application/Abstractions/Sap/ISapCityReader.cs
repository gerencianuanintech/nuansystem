using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapCityReader
{
    Task<IReadOnlyCollection<SapCityRecord>> GetCitiesAsync(
        int companyId,
        CancellationToken cancellationToken = default);
}
