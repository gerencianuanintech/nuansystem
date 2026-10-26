using DevExpress.XtraEditors;
using NuanSystem.WinForms.Controls.Editors;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Sap.Models;
using NuanSystem.WinForms.ViewModels.Sap;

namespace NuanSystem.WinForms.Forms.Sap;

public sealed partial class SapIntegrationSettingsForm : BaseEditForm
{
    public const string FormKey = "sap-integration-settings";
    private readonly SapIntegrationSettingsViewModel? viewModel;
    private readonly Func<SapIdentityPolicyForm>? identityPolicyFormFactory;

    public SapIntegrationSettingsForm()
    {
        InitializeComponent();
        FormStyler.ApplyBase(this);
    }

    public SapIntegrationSettingsForm(
        SapIntegrationSettingsViewModel viewModel,
        Func<SapIdentityPolicyForm> identityPolicyFormFactory) : this()
    {
        this.viewModel = viewModel;
        this.identityPolicyFormFactory = identityPolicyFormFactory;
        Shown += async (_, _) => await LoadSettingsAsync();
        btnIdentityPolicy.Click += (_, _) => OpenIdentityPolicy();
        btnTestConnection.Click += async (_, _) => await TestConnectionAsync();
    }

    protected override bool ValidateForm()
    {
        var valid = true;
        valid &= Validator.RequireText(txtServiceLayerUrl, "Ingrese la URL del Service Layer.");
        valid &= Validator.RequireText(txtSapCompanyDb, "Ingrese la base de datos SAP.");
        valid &= Validator.RequireText(txtSapUser, "Ingrese el usuario técnico SAP.");
        if (spnMaxRetryCount.Value < 0 || spnMaxRetryCount.Value > 20)
        {
            Validator.SetError(spnMaxRetryCount, "Los reintentos deben estar entre 0 y 20.");
            valid = false;
        }
        return valid;
    }

    protected override async Task<bool> PersistAsync()
    {
        if (viewModel is null) return false;
        var password = string.IsNullOrWhiteSpace(txtSapPassword.Text) ? null : txtSapPassword.Text;
        await viewModel.SaveAsync(new SaveSapServiceLayerSettingsRequest(
            tglIsEnabled.IsOn,
            txtServiceLayerUrl.Text.Trim(),
            txtSapCompanyDb.Text.Trim(),
            txtSapUser.Text.Trim(),
            password,
            Convert.ToInt32(spnMaxRetryCount.Value)));
        statusLabel.Text = "Configuración guardada";
        ShowSuccess("La configuración SAP se guardó correctamente.");
        return true;
    }

    private async Task LoadSettingsAsync()
    {
        if (viewModel is null) return;
        await viewModel.LoadAsync();
        if (viewModel.Settings is not { } settings) return;
        lblCompanyValue.Text = $"{settings.CompanyCode} (empresa activa)";
        tglIsEnabled.IsOn = settings.IsEnabled;
        txtServiceLayerUrl.Text = settings.ServiceLayerUrl;
        txtSapCompanyDb.Text = settings.SapCompanyDb;
        txtSapUser.Text = settings.SapUser;
        spnMaxRetryCount.Value = Math.Clamp(settings.MaxRetryCount, 0, 20);
        txtSapPassword.Properties.NullText = settings.HasPassword ? "Dejar vacío para conservar" : string.Empty;
        statusLabel.Text = settings.UpdatedAt is null ? "Sin configurar" : $"Última actualización: {settings.UpdatedAt:yyyy-MM-dd HH:mm}";
    }

    private void OpenIdentityPolicy()
    {
        if (identityPolicyFormFactory is null) return;
        using var form = identityPolicyFormFactory();
        form.ShowDialog(this);
    }

    private async Task TestConnectionAsync()
    {
        if (viewModel is null || viewModel.IsBusy) return;
        try
        {
            btnTestConnection.Enabled = false;
            statusLabel.Text = "Probando conexión...";
            var result = await viewModel.TestConnectionAsync();
            statusLabel.Text = result.Message;
            XtraMessageBox.Show(this, result.Message, Text, MessageBoxButtons.OK, result.IsSuccessful ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }
        catch (Exception exception)
        {
            statusLabel.Text = "Error al probar conexión";
            UiExceptionHandler.ShowError(this, Text, exception);
        }
        finally
        {
            btnTestConnection.Enabled = true;
        }
    }
}
