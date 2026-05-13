using NuanSystem.WinForms.Services.ConfigurationCompanies.Models;

namespace NuanSystem.WinForms.Services.ConfigurationCompanies;

public interface IConfigurationCompanyClient
{
    Task<IReadOnlyCollection<ConfigurationCompanyItem>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ConfigurationCompanyItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<ConfigurationCompanyItem> CreateAsync(SaveConfigurationCompanyRequest request, CancellationToken cancellationToken = default);

    Task<ConfigurationCompanyItem> UpdateAsync(int id, SaveConfigurationCompanyRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
