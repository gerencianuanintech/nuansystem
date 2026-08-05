using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapCountryReader
{
    Task<IReadOnlyCollection<SapCountryRecord>> GetCountriesAsync(
        int companyId,
        CancellationToken cancellationToken = default);
}
