using NuanSystem.Application.Features.Settings.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ICompanyParameterRepository
{
    Task<IReadOnlyCollection<CompanyParameterDto>> GetForCurrentCompanyAsync(CancellationToken cancellationToken = default);
    Task<CompanyParameterDto> UpsertForCurrentCompanyAsync(UpsertCompanyParameterData parameter, CancellationToken cancellationToken = default);
}
