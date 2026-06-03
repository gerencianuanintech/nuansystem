using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncCompanyRepository
{
    Task<IReadOnlyCollection<SapSyncCompanyDto>> GetActiveSapCompaniesAsync(CancellationToken cancellationToken = default);
}
