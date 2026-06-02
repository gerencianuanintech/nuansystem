using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System.ComponentModel;
using NuanSystem.Shared.Contracts.Auth;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.ViewModels.Auth;
using NuanSystem.WinForms.ViewModels.Companies;

namespace NuanSystem.WinForms.Forms.Auth;

public sealed partial class LoginForm : XtraForm
{
    private static readonly Point CredentialsButtonLocation = new(37, 335);
    private static readonly Point CredentialsStatusLocation = new(37, 391);
    private static readonly Point CompanyButtonLocation = new(37, 383);
    private static readonly Point CompanyChangeUserLocation = new(37, 431);
    private static readonly Point CompanyStatusLocation = new(37, 468);

    private readonly LoginViewModel viewModel;
    private readonly CompanySelectionViewModel companyViewModel;
    private readonly INuanApiClient apiClient;
    private bool credentialsAccepted;

    public LoginForm()
    {
        viewModel = null!;
        companyViewModel = null!;
        apiClient = null!;
        InitializeComponent();
        FormStyler.ApplyBase(this);
        WireEvents();
    }

    public LoginForm(LoginViewModel viewModel, CompanySelectionViewModel companyViewModel, INuanApiClient apiClient)
    {
        this.viewModel = viewModel;
        this.companyViewModel = companyViewModel;
        this.apiClient = apiClient;
        InitializeComponent();
        FormStyler.ApplyBase(this);
        WireEvents();

        this.txtUsuario.Text = "admin";
        this.txtPassword.Text = "Abc1234*";
    }

    private void WireEvents()
    {
        btnEstadoApi.Click += ApiStatusButton_Click;
        btnContinuar.Click += LoginButton_Click;
        btnCambiarUsuario.Click += ChangeUserButton_Click;
    }

    private async void ApiStatusButton_Click(object? sender, EventArgs e)
    {
        await CheckApiAsync();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (IsInDesignMode)
        {
            return;
        }

        txtUsuario.Focus();
        await CheckApiAsync();
    }

    private async Task CheckApiAsync()
    {
        if (IsInDesignMode || apiClient is null)
        {
            return;
        }

        SetApiStatus("Verificando", Color.FromArgb(235, 235, 225), BrandResources.MutedText);

        try
        {
            if (await apiClient.IsAvailableAsync(cancellationToken: CancellationToken.None))
            {
                SetApiStatus("API activa", BrandResources.SuccessBack, BrandResources.SuccessText);
                return;
            }
        }
        catch
        {
        }

        SetApiStatus("API error", BrandResources.ErrorBack, BrandResources.ErrorText);
    }

    private void SetApiStatus(string text, Color backColor, Color foreColor)
    {
        btnEstadoApi.Text = text;
        btnEstadoApi.Appearance.BackColor = backColor;
        btnEstadoApi.Appearance.ForeColor = foreColor;
        btnEstadoApi.Appearance.Options.UseBackColor = true;
        btnEstadoApi.Appearance.Options.UseForeColor = true;
    }

    private async void LoginButton_Click(object? sender, EventArgs e)
    {
        if (viewModel is null || companyViewModel is null || IsInDesignMode)
        {
            return;
        }

        if (credentialsAccepted)
        {
            ConfirmCompanyAndClose();
            return;
        }

        viewModel.UserNameOrEmail = txtUsuario.Text;
        viewModel.Password = txtPassword.Text;
        btnContinuar.Enabled = false;
        btnContinuar.Text = "Autenticando...";
        lblStatus.Text = "Validando credenciales...";

        try
        {
            var response = await viewModel.LoginAsync();
            if (response.MustChangePassword && !await PromptRequiredPasswordChangeAsync())
            {
                return;
            }

            await LoadCompaniesAsync();
        }
        catch (Exception exception)
        {
            lblStatus.Text = UiExceptionHandler.GetUserMessage(exception);
            UiExceptionHandler.ShowError(this, "Login", exception);
        }
        finally
        {
            btnContinuar.Enabled = true;
            btnContinuar.Text = credentialsAccepted ? "Ingresar al sistema" : "Continuar";
        }
    }

    private async Task<bool> PromptRequiredPasswordChangeAsync()
    {
        XtraMessageBox.Show(
            this,
            "Debe cambiar su clave antes de continuar.",
            "Cambio de clave requerido",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        while (true)
        {
            using var form = new ChangePasswordForm();
            if (form.ShowDialog(this) != DialogResult.OK)
            {
                lblStatus.Text = "Cambio de clave requerido.";
                return false;
            }

            try
            {
                await viewModel.ChangePasswordAsync(form.CurrentPassword, form.NewPassword);
                lblStatus.Text = "Clave actualizada correctamente.";
                return true;
            }
            catch (Exception exception)
            {
                UiExceptionHandler.ShowError(this, "Cambio de clave", exception);
            }
        }
    }

    private async Task LoadCompaniesAsync()
    {
        lblStatus.Text = "Cargando empresas...";
        await companyViewModel.LoadAsync();

        lueEmpresa.Properties.Columns.Clear();
        lueEmpresa.Properties.Columns.Add(new LookUpColumnInfo(nameof(UserCompanyResponse.CommercialName), "Nombre comercial"));
        lueEmpresa.Properties.DataSource = companyViewModel.Companies.ToList();
        lueEmpresa.Properties.DisplayMember = nameof(UserCompanyResponse.CommercialName);
        lueEmpresa.Properties.ValueMember = nameof(UserCompanyResponse.Id);
        lueEmpresa.Properties.ShowHeader = false;
        lueEmpresa.Properties.NullText = "Seleccione una empresa";
        lueEmpresa.EditValue = companyViewModel.SelectedCompany?.Id;

        credentialsAccepted = true;
        txtUsuario.Enabled = false;
        txtPassword.Enabled = false;
        lblStep.Text = "PASO 2 DE 2 - SELECCIONAR EMPRESA";
        lblTitle.Text = "Empresa de trabajo";
        lblSubtitle.Text = "Selecciona la empresa con la que vas a operar.";
        lblEmpresa.Visible = true;
        lueEmpresa.Visible = true;
        btnCambiarUsuario.Visible = true;
        btnContinuar.Location = CompanyButtonLocation;
        btnCambiarUsuario.Location = CompanyChangeUserLocation;
        lblStatus.Location = CompanyStatusLocation;
        btnContinuar.Text = "Ingresar al sistema";
        lblStatus.Text = $"{companyViewModel.Companies.Count} empresa(s) disponibles.";
        lueEmpresa.Focus();
    }

    private void ConfirmCompanyAndClose()
    {
        if (lueEmpresa.GetSelectedDataRow() is not UserCompanyResponse company)
        {
            XtraMessageBox.Show(this, "Seleccione una empresa.", "Empresas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        companyViewModel.SelectedCompany = company;
        companyViewModel.ConfirmSelection();
        DialogResult = DialogResult.OK;
        Close();
    }

    private void ChangeUserButton_Click(object? sender, EventArgs e)
    {
        credentialsAccepted = false;
        txtUsuario.Enabled = true;
        txtPassword.Enabled = true;
        txtPassword.Text = string.Empty;
        lblStep.Text = "PASO 1 DE 2 - AUTENTICACION";
        lblTitle.Text = "Bienvenido";
        lblSubtitle.Text = "Ingresa tus credenciales para continuar.";
        lblEmpresa.Visible = false;
        lueEmpresa.Visible = false;
        btnCambiarUsuario.Visible = false;
        btnContinuar.Location = CredentialsButtonLocation;
        lblStatus.Location = CredentialsStatusLocation;
        btnContinuar.Text = "Continuar";
        lblStatus.Text = string.Empty;
        txtUsuario.Focus();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }



    private bool IsInDesignMode => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;
}
