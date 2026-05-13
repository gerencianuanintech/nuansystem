using NuanSystem.WinForms.Services.Companies;
using NuanSystem.WinForms.Services.Companies.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Companies;

public sealed class CompaniesAdminViewModel(ICompanyClient companyClient) : ViewModelBase
{
    private IReadOnlyCollection<CompanyAdminItem> companies = Array.Empty<CompanyAdminItem>();
    private bool isBusy;

    public IReadOnlyCollection<CompanyAdminItem> Companies
    {
        get => companies;
        private set => SetProperty(ref companies, value);
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
            Companies = await companyClient.GetAllAsync(cancellationToken);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public Task<CompanyConnectionTestItem> ValidateConnectionAsync(
        ValidateCompanyConnectionRequest request,
        CancellationToken cancellationToken = default)
    {
        return companyClient.ValidateConnectionAsync(request, cancellationToken);
    }

    public Task<CompanyAdminItem> CreateAsync(
        CreateCompanyRequest request,
        CancellationToken cancellationToken = default)
    {
        return companyClient.CreateAsync(request, cancellationToken);
    }
}
