using NuanSystem.WinForms.Services.Settings.Models;

namespace NuanSystem.WinForms.Services.Settings;

public interface ISettingsClient
{
    Task<IReadOnlyCollection<CompanyParameterItem>> GetParametersAsync(CancellationToken cancellationToken = default);
    Task<CompanyParameterItem> SaveParameterAsync(SaveCompanyParameterRequest request, CancellationToken cancellationToken = default);
}
