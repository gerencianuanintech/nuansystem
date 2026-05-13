using NuanSystem.Shared.Contracts.Auth;
using NuanSystem.WinForms.Services.Companies;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Companies;

public sealed class CompanySelectionViewModel : ViewModelBase
{
    private readonly ICompanyClient companyClient;
    private readonly ApiSession session;
    private IReadOnlyCollection<UserCompanyResponse> companies = Array.Empty<UserCompanyResponse>();
    private UserCompanyResponse? selectedCompany;
    private bool isBusy;

    public CompanySelectionViewModel(ICompanyClient companyClient, ApiSession session)
    {
        this.companyClient = companyClient;
        this.session = session;
    }

    public IReadOnlyCollection<UserCompanyResponse> Companies
    {
        get => companies;
        private set => SetProperty(ref companies, value);
    }

    public UserCompanyResponse? SelectedCompany
    {
        get => selectedCompany;
        set => SetProperty(ref selectedCompany, value);
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
            var loginCompanies = session.CurrentUser?.Companies;
            Companies = loginCompanies is { Count: > 0 }
                ? loginCompanies
                : await companyClient.GetMyCompaniesAsync(cancellationToken);

            SelectedCompany = Companies.FirstOrDefault();
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void ConfirmSelection()
    {
        if (SelectedCompany is null)
        {
            throw new InvalidOperationException("Seleccione una empresa.");
        }

        session.SelectCompany(SelectedCompany);
    }
}
