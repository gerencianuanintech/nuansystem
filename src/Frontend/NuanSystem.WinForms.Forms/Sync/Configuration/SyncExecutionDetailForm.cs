using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.ViewModels.Sync;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

public sealed partial class SyncExecutionDetailForm : XtraForm
{
    private SyncProfileExecutionDetailViewModel? viewModel;
    private ApiSession? session;
    private int executionId;
    private bool isRefreshing;

    public SyncExecutionDetailForm()
    {
        InitializeComponent();
        FormStyler.ApplyBase(this);
    }

    public SyncExecutionDetailForm(SyncProfileExecutionDetailViewModel viewModel, ApiSession session, int executionId)
        : this()
    {
        this.viewModel = viewModel;
        this.session = session;
        this.executionId = executionId;
        Text = $"Detalle de ejecucion {executionId}";
        WireEvents();
        ApplyPermissions();
    }

    private SyncProfileExecutionDetailViewModel ViewModel =>
        viewModel ?? throw new InvalidOperationException("El formulario debe abrirse mediante inyeccion de dependencias.");

    private ApiSession Session =>
        session ?? throw new InvalidOperationException("El formulario debe abrirse mediante inyeccion de dependencias.");

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (IsInDesignMode() || viewModel is null)
        {
            return;
        }

        await RefreshAsync();
        pollingTimer.Start();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        pollingTimer.Stop();
        base.OnFormClosed(e);
    }

    private void WireEvents()
    {
        refreshButton.Click += async (_, _) => await RefreshAsync();
        cancelButton.Click += async (_, _) => await CancelAsync();
        retryButton.Click += async (_, _) => await RetryAsync();
        pollingTimer.Tick += async (_, _) => await RefreshIfActiveAsync();
    }

    private void ApplyPermissions()
    {
        cancelButton.Enabled = Session.HasPermission(PermissionCodes.SyncConfigurationCancel);
        retryButton.Enabled = Session.HasPermission(PermissionCodes.SyncConfigurationRetry);
    }

    private async Task RefreshAsync()
    {
        if (isRefreshing || IsDisposed || Disposing)
        {
            return;
        }

        isRefreshing = true;
        try
        {
            await UiExceptionHandler.RunAsync(this, Text, async () =>
            {
                await ViewModel.LoadAsync(executionId);
                if (IsDisposed || Disposing)
                {
                    return;
                }

                BindDetail();
            });
        }
        finally
        {
            isRefreshing = false;
        }
    }

    private async Task RefreshIfActiveAsync()
    {
        if (!IsDisposed && !Disposing && (ViewModel.Detail is null || IsActiveStatus(ViewModel.Detail.Status)))
        {
            await RefreshAsync();
        }
    }

    private void BindDetail()
    {
        if (ViewModel.Detail is not { } detail)
        {
            return;
        }

        summaryEdit.Text =
            $"Perfil: {detail.ProfileCode} - {detail.ProfileName}{Environment.NewLine}" +
            $"Estado: {detail.Status} | Tipo: {detail.ExecutionType} | Correlacion: {detail.CorrelationId}{Environment.NewLine}" +
            $"Solicitado: {detail.RequestedAt} | Inicio: {detail.StartedAt} | Fin: {detail.FinishedAt}{Environment.NewLine}" +
            $"Entidades: {detail.TotalEntities:N0} | Leidos: {detail.TotalRecordsRead:N0} | Publicados: {detail.TotalEventsPublished:N0} | Omitidos: {detail.TotalSkipped:N0} | Errores: {detail.TotalErrors:N0}{Environment.NewLine}" +
            $"Mensaje: {detail.Message}";

        detailGrid.DataSource = detail.Details;
    }

    private async Task CancelAsync()
    {
        if (XtraMessageBox.Show(this, $"Cancelar la ejecucion {executionId}?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        await UiExceptionHandler.RunAsync(this, Text, async () =>
        {
            await ViewModel.CancelAsync(executionId);
            await RefreshAsync();
        });
    }

    private async Task RetryAsync()
    {
        if (XtraMessageBox.Show(this, $"Reintentar la ejecucion {executionId}?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        await UiExceptionHandler.RunAsync(this, Text, async () =>
        {
            await ViewModel.RetryAsync(executionId);
            await RefreshAsync();
        });
    }

    private static bool IsActiveStatus(string status)
    {
        return string.Equals(status, "Pending", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "Running", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "Cancelling", StringComparison.OrdinalIgnoreCase);
    }

    private bool IsInDesignMode()
    {
        return LicenseManager.UsageMode == LicenseUsageMode.Designtime
            || DesignMode
            || Site?.DesignMode == true;
    }
}
