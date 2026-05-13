using NuanSystem.Application.Features.ConfigurationCompanies.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IConfigurationCompanyRepository : IRepository
{
    Task<IReadOnlyCollection<ConfigurationCompanyDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ConfigurationCompanyDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateConfigurationCompanyData company, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateConfigurationCompanyData company, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
}
