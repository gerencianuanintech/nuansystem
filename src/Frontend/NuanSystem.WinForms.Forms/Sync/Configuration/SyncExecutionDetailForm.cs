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
    private bool isActionInProgress;
    private bool allowActions = true;

    public SyncExecutionDetailForm()
    {
        InitializeComponent();
        FormStyler.ApplyBase(this);
    }

    public SyncExecutionDetailForm(
        SyncProfileExecutionDetailViewModel viewModel,
        ApiSession session,
        int executionId,
        bool allowActions = true)
        : this()
    {
        this.viewModel = viewModel;
        this.session = session;
        this.executionId = executionId;
        this.allowActions = allowActions;
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
        UpdatePollingState();
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
        UpdateActionState();
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
            UpdateActionState();
            UpdatePollingState();
        }
    }

    private async Task RefreshIfActiveAsync()
    {
        if (!IsDisposed
            && !Disposing
            && !isActionInProgress
            && (ViewModel.Detail is null || SyncExecutionStatusPolicy.IsActive(ViewModel.Detail.Status)))
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
        UpdateActionState();
    }

    private async Task CancelAsync()
    {
        if (!CanCancelCurrentExecution())
        {
            return;
        }

        if (XtraMessageBox.Show(this, $"Cancelar la ejecucion {executionId}?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        isActionInProgress = true;
        UpdateActionState();
        try
        {
            await UiExceptionHandler.RunAsync(this, Text, async () =>
            {
                await ViewModel.CancelAsync(executionId);
                await RefreshAsync();
            });
        }
        finally
        {
            isActionInProgress = false;
            UpdateActionState();
        }
    }

    private async Task RetryAsync()
    {
        if (!CanRetryCurrentExecution())
        {
            return;
        }

        if (XtraMessageBox.Show(this, $"Reintentar la ejecucion {executionId}?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        isActionInProgress = true;
        UpdateActionState();
        try
        {
            await UiExceptionHandler.RunAsync(this, Text, async () =>
            {
                var retry = await ViewModel.RetryAsync(executionId);
                executionId = retry.NewExecutionId;
                Text = $"Detalle de ejecucion {executionId}";
                await RefreshAsync();
            });
        }
        finally
        {
            isActionInProgress = false;
            UpdateActionState();
        }
    }

    private bool CanCancelCurrentExecution()
    {
        return allowActions
            && !isRefreshing
            && !isActionInProgress
            && Session.HasPermission(PermissionCodes.SyncConfigurationCancel)
            && ViewModel.Detail is { } detail
            && SyncExecutionStatusPolicy.CanCancel(detail.Status);
    }

    private bool CanRetryCurrentExecution()
    {
        return allowActions
            && !isRefreshing
            && !isActionInProgress
            && Session.HasPermission(PermissionCodes.SyncConfigurationRetry)
            && ViewModel.Detail is { } detail
            && SyncExecutionStatusPolicy.CanRetry(detail.Status);
    }

    private void UpdateActionState()
    {
        refreshButton.Enabled = !isRefreshing && !isActionInProgress;
        cancelButton.Enabled = CanCancelCurrentExecution();
        retryButton.Enabled = CanRetryCurrentExecution();
    }

    private void UpdatePollingState()
    {
        if (IsDisposed || Disposing)
        {
            pollingTimer.Stop();
            return;
        }

        if (ViewModel.Detail is null || SyncExecutionStatusPolicy.IsActive(ViewModel.Detail.Status))
        {
            pollingTimer.Start();
            return;
        }

        pollingTimer.Stop();
    }

    private bool IsInDesignMode()
    {
        return LicenseManager.UsageMode == LicenseUsageMode.Designtime
            || DesignMode
            || Site?.DesignMode == true;
    }
}
