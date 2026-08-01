using System.ComponentModel;
using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Sap;
using NuanSystem.WinForms.Services.Sap.Models;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Sap;

namespace NuanSystem.WinForms.Forms.Sap;

public sealed partial class SapSyncExecutionListForm : BaseGridCrudListForm
{
    public const string FormKey = "sap-sync-executions";
    private SapSyncExecutionsViewModel? viewModel;
    private ISapSyncManagementClient? client;
    private ApiSession? session;
    private long? fixedProfileId;
    private bool refreshing;

    public SapSyncExecutionListForm() { InitializeComponent(); FormStyler.ApplyBase(this); }
    public SapSyncExecutionListForm(SapSyncExecutionsViewModel viewModel, ISapSyncManagementClient client, ApiSession session, long? profileId = null) : this()
    {
        this.viewModel = viewModel; this.client = client; this.session = session; fixedProfileId = profileId;
        ConfigureCrudPermissions(session, new(PermissionCodes.SapSyncExecutionsView, "__SAP_EXEC_CREATE__", "__SAP_EXEC_EDIT__", "__SAP_EXEC_DELETE__"));
        GridView.DoubleClick += async (_, _) => await ExecuteConsultAsync();
        pollingTimer.Tick += async (_, _) => { if (ViewModel.Executions.Any(item => SapSyncExecutionPolicy.IsActive(item.Status))) await LoadDataAsync(); };
    }
    private SapSyncExecutionsViewModel ViewModel => viewModel ?? throw new InvalidOperationException("El formulario requiere inyeccion de dependencias.");
    private ISapSyncManagementClient Client => client ?? throw new InvalidOperationException("El formulario requiere inyeccion de dependencias.");
    private ApiSession Session => session ?? throw new InvalidOperationException("El formulario requiere inyeccion de dependencias.");

    protected override async void OnShown(EventArgs e) { base.OnShown(e); if (!IsInDesignMode() && viewModel is not null) { await LoadDataAsync(); pollingTimer.Start(); } }
    protected override void OnFormClosed(FormClosedEventArgs e) { pollingTimer.Stop(); base.OnFormClosed(e); }
    protected override async Task LoadDataAsync()
    {
        if (refreshing || viewModel is null) return;
        refreshing = true;
        try { ViewModel.Filter.ProfileId = fixedProfileId; await ViewModel.LoadAsync(); SetGridData(ViewModel.Executions); }
        finally { refreshing = false; }
    }
    protected override async Task ConsultAsync()
    {
        if (Selected() is not { } item) { ShowWarning("Seleccione una ejecucion SAP."); return; }
        using var form = new SapSyncExecutionDetailForm(new SapSyncExecutionDetailViewModel(Client), Session, item.ExecutionUid);
        form.ShowDialog(this); await LoadDataAsync();
    }
    public override bool CanExecuteCustomOperation(string operationKey) => Normalize(operationKey) switch
    {
        "filter" => Session.HasPermission(PermissionCodes.SapSyncExecutionsView),
        "retry" => Session.HasPermission(PermissionCodes.SapSyncExecutionsRetry),
        "cancel" => Session.HasPermission(PermissionCodes.SapSyncExecutionsCancel),
        "releaseexpiredlock" => Session.HasPermission(PermissionCodes.SapSyncExecutionsReleaseExpiredLock),
        _ => base.CanExecuteCustomOperation(operationKey)
    };
    public override Task ExecuteCustomOperationAsync(string operationKey) => Normalize(operationKey) switch
    {
        "filter" => OpenFilterAsync(),
        "retry" => OpenDetailActionAsync(),
        "cancel" => OpenDetailActionAsync(),
        "releaseexpiredlock" => OpenDetailActionAsync(),
        _ => base.ExecuteCustomOperationAsync(operationKey)
    };
    private async Task OpenFilterAsync()
    {
        using var dialog = new SapSyncExecutionFilterDialog(ViewModel.Filter, fixedProfileId.HasValue);
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        ViewModel.Filter.EntityCode = dialog.EntityCode; ViewModel.Filter.Direction = dialog.Direction; ViewModel.Filter.Status = dialog.Status;
        ViewModel.Filter.TriggerType = dialog.TriggerType; ViewModel.Filter.DateFromUtc = dialog.DateFromUtc; ViewModel.Filter.DateToUtc = dialog.DateToUtc;
        await LoadDataAsync();
    }
    private async Task OpenDetailActionAsync()
    {
        if (Selected() is not { } item) { ShowWarning("Seleccione una ejecucion SAP."); return; }
        using var form = new SapSyncExecutionDetailForm(new SapSyncExecutionDetailViewModel(Client), Session, item.ExecutionUid);
        form.ShowDialog(this); await LoadDataAsync();
    }
    private SapSyncExecutionListItem? Selected() => SelectedGridItem<SapSyncExecutionListItem>();
    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns(); foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns) column.Visible = false;
        Column(nameof(SapSyncExecutionListItem.Id), "Id", 0, 70); Column(nameof(SapSyncExecutionListItem.ProfileCode), "Perfil SAP", 1, 110);
        Column(nameof(SapSyncExecutionListItem.EntityCode), "Entidad", 2, 120); Column(nameof(SapSyncExecutionListItem.Direction), "Direccion", 3, 110);
        Column(nameof(SapSyncExecutionListItem.TriggerType), "Origen", 4, 90); Column(nameof(SapSyncExecutionListItem.Status), "Estado", 5, 135);
        Column(nameof(SapSyncExecutionListItem.RequestedAtUtc), "Solicitada", 6, 145); Column(nameof(SapSyncExecutionListItem.FinishedAtUtc), "Finalizada", 7, 145);
        Column(nameof(SapSyncExecutionListItem.TotalRecords), "Registros", 8, 85); Column(nameof(SapSyncExecutionListItem.SucceededRecords), "Correctos", 9, 85);
        Column(nameof(SapSyncExecutionListItem.WarningRecords), "Avisos", 10, 75); Column(nameof(SapSyncExecutionListItem.FailedRecords), "Errores", 11, 75);
    }
    private void Column(string field, string caption, int index, int width) { if (GridView.Columns[field] is not { } column) return; column.Caption = caption; column.Visible = true; column.VisibleIndex = index; column.Width = width; }
    private static string Normalize(string value) => value.Replace("ACTION.", "", StringComparison.OrdinalIgnoreCase).Replace("SAP_SYNC_EXECUTIONS.", "", StringComparison.OrdinalIgnoreCase).Replace("-", "").Replace("_", "").ToLowerInvariant();
    private bool IsInDesignMode() => LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode || Site?.DesignMode == true;
}
