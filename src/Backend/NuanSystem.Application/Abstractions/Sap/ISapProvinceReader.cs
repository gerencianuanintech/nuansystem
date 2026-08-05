using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapProvinceReader
{
    Task<IReadOnlyCollection<SapProvinceRecord>> GetProvincesAsync(
        int companyId,
        CancellationToken cancellationToken = default);
}
