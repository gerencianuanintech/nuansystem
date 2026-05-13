using NuanSystem.WinForms.Services.ConfigurationCompanies;
using NuanSystem.WinForms.Services.ConfigurationCompanies.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.ConfigurationCompanies;

public sealed class ConfigurationCompaniesViewModel(IConfigurationCompanyClient companyClient) : ViewModelBase
{
    private IReadOnlyCollection<ConfigurationCompanyItem> items = Array.Empty<ConfigurationCompanyItem>();
    private bool isBusy;

    public IReadOnlyCollection<ConfigurationCompanyItem> Items
    {
        get => items;
        private set => SetProperty(ref items, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        private set => SetProperty(ref isBusy, value);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try
        {
            Items = await companyClient.GetAllAsync(cancellationToken);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public Task<ConfigurationCompanyItem> CreateAsync(SaveConfigurationCompanyRequest request, CancellationToken cancellationToken = default)
    {
        return companyClient.CreateAsync(request, cancellationToken);
    }

    public Task<ConfigurationCompanyItem> UpdateAsync(int id, SaveConfigurationCompanyRequest request, CancellationToken cancellationToken = default)
    {
        return companyClient.UpdateAsync(id, request, cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return companyClient.DeleteAsync(id, cancellationToken);
    }
}
