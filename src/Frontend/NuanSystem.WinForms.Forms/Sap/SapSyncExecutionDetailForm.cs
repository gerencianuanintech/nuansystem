using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Sap.Models;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Sap;
using NuanSystem.WinForms.Controls.Grids;

namespace NuanSystem.WinForms.Forms.Sap;

public sealed partial class SapSyncExecutionDetailForm : XtraForm
{
    private SapSyncExecutionDetailViewModel? viewModel; private ApiSession? session; private Guid executionUid; private bool busy;
    public SapSyncExecutionDetailForm() { InitializeComponent(); FormStyler.ApplyBase(this); }
    public SapSyncExecutionDetailForm(SapSyncExecutionDetailViewModel viewModel, ApiSession session, Guid executionUid) : this()
    {
        this.viewModel = viewModel; this.session = session; this.executionUid = executionUid;
        ConfigureGrid();
        refreshButton.Click += async (_, _) => await RefreshAsync(); retryButton.Click += async (_, _) => await RetryAsync(); cancelButton.Click += async (_, _) => await CancelAsync(); releaseButton.Click += async (_, _) => await ReleaseAsync();
        detailGrid.PageRequested += async (_, args) => await GoToPageAsync(args);
        detailGrid.FocusedRowChanged += (_, _) => UpdateActions();
        pollingTimer.Tick += async (_, _) => { if (ViewModel.Execution is null || SapSyncExecutionPolicy.IsActive(ViewModel.Execution.Status)) await RefreshAsync(); };
    }
    private SapSyncExecutionDetailViewModel ViewModel => viewModel ?? throw new InvalidOperationException("El formulario requiere inyeccion de dependencias.");
    private ApiSession Session => session ?? throw new InvalidOperationException("El formulario requiere inyeccion de dependencias.");
    protected override async void OnShown(EventArgs e) { base.OnShown(e); if (!IsInDesignMode() && viewModel is not null) { await RefreshAsync(); pollingTimer.Start(); } }
    protected override void OnFormClosed(FormClosedEventArgs e) { pollingTimer.Stop(); base.OnFormClosed(e); }
    private async Task RefreshAsync()
    {
        if (busy) return; busy = true; UpdateActions();
        try { await UiExceptionHandler.RunAsync(this, Text, async () => { await ViewModel.LoadAsync(executionUid); Bind(); }); }
        finally { busy = false; UpdateActions(); }
    }
    private void Bind()
    {
        if (ViewModel.Execution is not { } item) return;
        summaryEdit.Text = $"Perfil: {item.ProfileCode} - {item.ProfileName}{Environment.NewLine}Empresa: {item.CompanyCode} | Entidad: {item.EntityCode} | Direccion: {item.Direction}{Environment.NewLine}Estado: {item.Status} | Origen: {item.TriggerType}{Environment.NewLine}Solicitada: {item.RequestedAtUtc.ToLocalTime():yyyy-MM-dd HH:mm:ss} | Inicio: {item.StartedAtUtc?.ToLocalTime():yyyy-MM-dd HH:mm:ss} | Fin: {item.FinishedAtUtc?.ToLocalTime():yyyy-MM-dd HH:mm:ss}{Environment.NewLine}Total: {item.TotalRecords:N0} | Creados: {item.CreatedRecords:N0} | Actualizados: {item.UpdatedRecords:N0} | Sin cambios: {item.UnchangedRecords:N0} | Aprobacion: {item.ApprovalRequiredRecords:N0} | Conflictos: {item.ConflictRecords:N0} | Errores: {item.FailedRecords + item.DeadLetterRecords:N0}{Environment.NewLine}Mensaje: {item.LastSafeErrorMessage ?? "Sin novedades"}";
        var filter = ViewModel.Filter ?? new SapSyncExecutionDetailFilter();
        detailGrid.SetPagedData(ViewModel.Details, filter.PageNumber, filter.PageSize, ViewModel.DetailTotalCount);
        UpdateActions();
    }
    private async Task RetryAsync() { if (!CanRetry()) return; var reason = AskReason("Motivo del reintento"); if (reason is null) return; await ExecuteAsync(() => ViewModel.RetryAsync(reason)); }
    private async Task CancelAsync() { if (!CanCancel()) return; if (XtraMessageBox.Show(this, "¿Solicitar la cancelacion de esta ejecucion SAP?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return; await ExecuteAsync(() => ViewModel.CancelAsync()); }
    private async Task ReleaseAsync() { if (!CanRelease() || SelectedDetail() is not { } detail) return; var reason = AskReason("Motivo para liberar el lock vencido"); if (reason is null) return; await ExecuteAsync(() => ViewModel.ReleaseExpiredLockAsync(detail, reason)); }
    private async Task ExecuteAsync(Func<Task> action) { busy = true; UpdateActions(); try { await UiExceptionHandler.RunAsync(this, Text, async () => { await action(); await ViewModel.LoadAsync(executionUid); Bind(); }); } finally { busy = false; UpdateActions(); } }
    private string? AskReason(string caption) { var value = XtraInputBox.Show("Ingrese un motivo auditable:", caption, string.Empty); return string.IsNullOrWhiteSpace(value) ? null : value.Trim(); }
    private SapSyncExecutionDetailItem? SelectedDetail() => detailGrid.GetFocusedRow<SapSyncExecutionDetailItem>();
    private bool CanRetry() => !busy && Session.HasPermission(PermissionCodes.SapSyncExecutionsRetry) && ViewModel.Execution is { } item && SapSyncExecutionPolicy.CanRetry(item.Status);
    private bool CanCancel() => !busy && Session.HasPermission(PermissionCodes.SapSyncExecutionsCancel) && ViewModel.Execution is { } item && SapSyncExecutionPolicy.CanCancel(item.Status);
    private bool CanRelease() => !busy && Session.HasPermission(PermissionCodes.SapSyncExecutionsReleaseExpiredLock) && SelectedDetail() is { } item && SapSyncExecutionPolicy.CanRelease(item.Status);
    private void UpdateActions() { refreshButton.Enabled = !busy; retryButton.Enabled = CanRetry(); cancelButton.Enabled = CanCancel(); releaseButton.Enabled = CanRelease(); }
    private void ConfigureGrid()
    {
        detailGrid.ConfigureColumns(
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionDetailItem.SourceRecordKey), Caption = "Clave SAP", VisibleIndex = 0, Width = 135 },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionDetailItem.Action), Caption = "Accion", VisibleIndex = 1, Width = 90 },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionDetailItem.Status), Caption = "Estado", VisibleIndex = 2, Width = 130, Format = NuanGridColumnFormat.StatusBadge },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionDetailItem.AttemptCount), Caption = "Intentos", VisibleIndex = 3, Width = 70, Format = NuanGridColumnFormat.Number },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionDetailItem.ResultCode), Caption = "Resultado", VisibleIndex = 4, Width = 120 },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionDetailItem.SafeMessage), Caption = "Mensaje", VisibleIndex = 5, Width = 300 },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionDetailItem.NextAttemptAtUtc), Caption = "Proximo intento", VisibleIndex = 6, Width = 145, Format = NuanGridColumnFormat.DateTime },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionDetailItem.FinishedAtUtc), Caption = "Finalizado", VisibleIndex = 7, Width = 145, Format = NuanGridColumnFormat.DateTime });
    }
    private async Task GoToPageAsync(NuanGridPageRequestEventArgs args)
    {
        if (busy) return;
        busy = true;
        UpdateActions();
        try
        {
            await UiExceptionHandler.RunAsync(this, Text, async () =>
            {
                await ViewModel.GoToDetailPageAsync(executionUid, args.Page, args.PageSize);
                Bind();
            });
        }
        finally
        {
            busy = false;
            UpdateActions();
        }
    }
    private bool IsInDesignMode() => LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode || Site?.DesignMode == true;
}
