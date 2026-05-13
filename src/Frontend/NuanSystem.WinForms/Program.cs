using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Auth;
using NuanSystem.WinForms.Forms.ConfigurationCompanies;
using NuanSystem.WinForms.Forms.Customers;
using NuanSystem.WinForms.Forms.Documents;
using NuanSystem.WinForms.Forms.InventoryItems;
using NuanSystem.WinForms.Forms.Roles;
using NuanSystem.WinForms.Forms.Sap;
using NuanSystem.WinForms.Forms.SecurityOperations;
using NuanSystem.WinForms.Forms.SecurityMenus;
using NuanSystem.WinForms.Forms.SecurityForms;
using NuanSystem.WinForms.Forms.SecurityFields;
using NuanSystem.WinForms.Forms.SecurityAccess;
using NuanSystem.WinForms.Forms.SecurityRoles;
using NuanSystem.WinForms.Forms.ConfigurationSettings;
using NuanSystem.WinForms.Forms.Shell;
using NuanSystem.WinForms.Forms.SecurityUsers;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.GeneralInventory.ItemGroups;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.Authentication;
using NuanSystem.WinForms.Services.Companies;
using NuanSystem.WinForms.Services.ConfigurationCompanies;
using NuanSystem.WinForms.Services.ConfigurationSettings;
using NuanSystem.WinForms.Services.Configuration;
using NuanSystem.WinForms.Services.Customers;
using NuanSystem.WinForms.Services.Documents;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups;
using NuanSystem.WinForms.Services.InventoryItems;
using NuanSystem.WinForms.Services.Roles;
using NuanSystem.WinForms.Services.Sap;
using NuanSystem.WinForms.Services.SecurityOperations;
using NuanSystem.WinForms.Services.SecurityMenus;
using NuanSystem.WinForms.Services.SecurityForms;
using NuanSystem.WinForms.Services.SecurityFields;
using NuanSystem.WinForms.Services.SecurityAccess;
using NuanSystem.WinForms.Services.SecurityRoles;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.Settings;
using NuanSystem.WinForms.Services.SecurityUsers;
using NuanSystem.WinForms.ViewModels.Audit;
using NuanSystem.WinForms.ViewModels.Auth;
using NuanSystem.WinForms.ViewModels.Companies;
using NuanSystem.WinForms.ViewModels.ConfigurationCompanies;
using NuanSystem.WinForms.ViewModels.ConfigurationSettings;
using NuanSystem.WinForms.ViewModels.Customers;
using NuanSystem.WinForms.ViewModels.Documents;
using NuanSystem.WinForms.ViewModels.GeneralInventory.ItemGroups;
using NuanSystem.WinForms.ViewModels.InventoryItems;
using NuanSystem.WinForms.ViewModels.Roles;
using NuanSystem.WinForms.ViewModels.Sap;
using NuanSystem.WinForms.ViewModels.SecurityOperations;
using NuanSystem.WinForms.ViewModels.SecurityMenus;
using NuanSystem.WinForms.ViewModels.SecurityForms;
using NuanSystem.WinForms.ViewModels.SecurityFields;
using NuanSystem.WinForms.ViewModels.SecurityAccess;
using NuanSystem.WinForms.ViewModels.SecurityRoles;
using NuanSystem.WinForms.ViewModels.Settings;
using NuanSystem.WinForms.ViewModels.Shell;
using NuanSystem.WinForms.ViewModels.SecurityUsers;

namespace NuanSystem.WinForms;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        using var composition = BuildComposition();
        composition.ConfigureGlobalErrorHandling();

        while (true)
        {
            composition.ClearSession();
            using var loginForm = composition.CreateLoginForm();
            if (loginForm.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            using var mainForm = composition.CreateMainForm();
            var mainResult = mainForm.ShowDialog();
            if (mainResult != DialogResult.Retry)
            {
                return;
            }
        }
    }    

    private static FrontendComposition BuildComposition()
    {
        var baseUrl = Environment.GetEnvironmentVariable("NUANSYSTEM_API_URL") ?? "https://localhost:7293";
        var options = new ApiClientOptions
        {
            BaseUrl = baseUrl
        };

        return new FrontendComposition(options);
    }
}

internal sealed class FrontendComposition : IDisposable
{
    private readonly HttpClient httpClient;
    private readonly ApiSession session = new();
    private readonly NuanApiClient apiClient;
    private readonly AuthenticationClient authenticationClient;
    private readonly CompanyClient companyClient;
    private readonly ConfigurationCompanyClient configurationCompanyClient;
    private readonly ConfigurationSettingClient configurationSettingClient;
    private readonly CustomerClient customerClient;
    private readonly ItemGroupClient itemGroupClient;
    private readonly ItemClient itemClient;
    private readonly DocumentClient documentClient;
    private readonly SapClient sapClient;
    private readonly AuditClient auditClient;
    private readonly SettingsClient settingsClient;
    private readonly UserClient userClient;
    private readonly RoleClient roleClient;
    private readonly SecurityRoleClient securityRoleClient;
    private readonly SecurityOperationClient securityOperationClient;
    private readonly SecurityMenuClient securityMenuClient;
    private readonly SecurityFormClient securityFormClient;
    private readonly SecurityFieldClient securityFieldClient;
    private readonly SecurityAccessClient securityAccessClient;
    private readonly GridColumnSettingsClient gridColumnSettingsClient;

    public FrontendComposition(ApiClientOptions options)
    {
        httpClient = new HttpClient
        {
            BaseAddress = new Uri(options.BaseUrl),
            Timeout = options.Timeout
        };

        apiClient = new NuanApiClient(httpClient, session);
        authenticationClient = new AuthenticationClient(apiClient, session);
        companyClient = new CompanyClient(apiClient);
        configurationCompanyClient = new ConfigurationCompanyClient(apiClient);
        configurationSettingClient = new ConfigurationSettingClient(apiClient);
        customerClient = new CustomerClient(apiClient);
        itemGroupClient = new ItemGroupClient(apiClient);
        itemClient = new ItemClient(apiClient);
        documentClient = new DocumentClient(apiClient);
        sapClient = new SapClient(apiClient);
        auditClient = new AuditClient(apiClient);
        settingsClient = new SettingsClient(apiClient);
        userClient = new UserClient(apiClient);
        roleClient = new RoleClient(apiClient);
        securityRoleClient = new SecurityRoleClient(apiClient);
        securityOperationClient = new SecurityOperationClient(apiClient);
        securityMenuClient = new SecurityMenuClient(apiClient);
        securityFormClient = new SecurityFormClient(apiClient);
        securityFieldClient = new SecurityFieldClient(apiClient);
        securityAccessClient = new SecurityAccessClient(apiClient);
        gridColumnSettingsClient = new GridColumnSettingsClient(apiClient);
    }

    public void ConfigureGlobalErrorHandling()
    {
        GlobalUiExceptionHandler.Configure(auditClient, session);
        GlobalUiExceptionHandler.RegisterApplicationHandlers();
    }

    public LoginForm CreateLoginForm()
    {
        return new LoginForm(
            new LoginViewModel(authenticationClient),
            new CompanySelectionViewModel(companyClient, session),
            httpClient.BaseAddress?.ToString() ?? string.Empty);
    }

    public MainForm CreateMainForm()
    {
        var shellViewModel = new ShellViewModel(session, securityAccessClient);
        shellViewModel.LoadNavigationAsync().GetAwaiter().GetResult();

        return new MainForm(
            shellViewModel,
            CreateConfigurationCompaniesForm,
            CreateUsersForm,
            CreateSecurityRolesForm,
            CreateOperationsForm,
            CreateMenusForm,
            CreateFormsForm,
            CreateFieldsForm,
            CreateRoleAccessForm,
            CreateCustomersForm,
            CreateItemGroupsForm,
            CreateItemsForm,
            CreateDocumentsForm,
            CreateSapSyncLogForm,
            CreateAuditLogsForm,
            CreateSettingsForm);
    }

    public void ClearSession()
    {
        session.Clear();
    }

    public CustomersForm CreateCustomersForm()
    {
        return new CustomersForm(new CustomersViewModel(customerClient), session);
    }

    public ItemGroupsForm CreateItemGroupsForm()
    {
        return new ItemGroupsForm(new ItemGroupsViewModel(itemGroupClient), session, auditClient, gridColumnSettingsClient);
    }

    public ConfigurationCompaniesForm CreateConfigurationCompaniesForm()
    {
        return new ConfigurationCompaniesForm(new ConfigurationCompaniesViewModel(configurationCompanyClient), session, auditClient, gridColumnSettingsClient);
    }

    public UsersForm CreateUsersForm()
    {
        return new UsersForm(new UsersViewModel(userClient, companyClient), session, auditClient, gridColumnSettingsClient);
    }

    public SecurityRolesForm CreateSecurityRolesForm()
    {
        return new SecurityRolesForm(new SecurityRolesViewModel(securityRoleClient), session, auditClient, gridColumnSettingsClient);
    }

    public OperationsForm CreateOperationsForm()
    {
        return new OperationsForm(new OperationsViewModel(securityOperationClient), session, auditClient, gridColumnSettingsClient);
    }

    public MenusForm CreateMenusForm()
    {
        return new MenusForm(new MenusViewModel(securityMenuClient), session, auditClient, gridColumnSettingsClient);
    }

    public FormsForm CreateFormsForm()
    {
        return new FormsForm(new FormsViewModel(securityFormClient), session, auditClient, gridColumnSettingsClient);
    }

    public FieldsForm CreateFieldsForm()
    {
        return new FieldsForm(new FieldsViewModel(securityFieldClient, securityFormClient), session, auditClient, gridColumnSettingsClient);
    }

    public RoleAccessForm CreateRoleAccessForm()
    {
        return new RoleAccessForm(new RoleAccessViewModel(roleClient, securityAccessClient));
    }

    public ItemsForm CreateItemsForm()
    {
        return new ItemsForm(new ItemsViewModel(itemClient), session, auditClient, gridColumnSettingsClient);
    }

    public DocumentsForm CreateDocumentsForm()
    {
        return new DocumentsForm(new DocumentsViewModel(documentClient, customerClient, itemClient, sapClient));
    }

    public SapSyncLogForm CreateSapSyncLogForm()
    {
        return new SapSyncLogForm(new SapSyncLogViewModel(sapClient));
    }

    public AuditLogsForm CreateAuditLogsForm()
    {
        return new AuditLogsForm(new AuditLogsViewModel(auditClient));
    }

    public SettingsForm CreateSettingsForm()
    {
        return new SettingsForm(new ConfigurationSettingsViewModel(configurationSettingClient), session, auditClient, gridColumnSettingsClient);
    }

    public void Dispose()
    {
        httpClient.Dispose();
    }
}

