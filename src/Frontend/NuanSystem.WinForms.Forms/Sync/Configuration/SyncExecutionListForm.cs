using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.ViewModels.Sync;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

public sealed partial class SyncExecutionListForm : BaseGridCrudListForm
{
    public const string FormKey = "sync-executions";

    private SyncExecutionsViewModel? viewModel;
    private SyncProfileExecutionDetailViewModel? detailViewModel;
    private ApiSession? session;
    private int? fixedProfileId;
    private bool isRefreshing;

    public SyncExecutionListForm()
    {
        InitializeComponent();
        ConfigureDesignerSafeVisuals();
    }

    public SyncExecutionListForm(
        SyncExecutionsViewModel viewModel,
        SyncProfileExecutionDetailViewModel detailViewModel,
        ApiSession session,
        int? profileId = null)
        : this()
    {
        this.viewModel = viewModel;
        this.detailViewModel = detailViewModel;
        this.session = session;
        fixedProfileId = profileId;
        Text = profileId.HasValue ? "Ejecuciones del perfil" : "Ejecuciones de sincronizacion";
        ConfigureCrudPermissions(session, new CrudOperationPermissions(
            PermissionCodes.SyncConfigurationViewExecutions,
            "__SYNC.EXECUTIONS.CREATE__",
            "__SYNC.EXECUTIONS.UPDATE__",
            "__SYNC.EXECUTIONS.DELETE__"));

        WireEvents();
    }

    private SyncExecutionsViewModel ViewModel =>
        viewModel ?? throw new InvalidOperationException("El formulario debe abrirse mediante inyeccion de dependencias.");

    private SyncProfileExecutionDetailViewModel DetailViewModel =>
        detailViewModel ?? throw new InvalidOperationException("El formulario debe abrirse mediante inyeccion de dependencias.");

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

    private void ConfigureDesignerSafeVisuals()
    {
        FormStyler.ApplyBase(this);
    }

    private void WireEvents()
    {
        GridView.DoubleClick += async (_, _) => await ExecuteConsultAsync();
        pollingTimer.Tick += async (_, _) => await RefreshActiveOnlyAsync();
    }

    protected override Task LoadDataAsync()
    {
        return RefreshAsync();
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
                ViewModel.Filter.ProfileId = fixedProfileId;
                ViewModel.Filter.Status = null;
                ViewModel.Filter.ExecutionType = null;
                ViewModel.Filter.DateFrom = null;
                ViewModel.Filter.DateTo = null;
                await ViewModel.LoadAsync();
                if (IsDisposed || Disposing)
                {
                    return;
                }

                SetGridData(ViewModel.Executions);
            });
        }
        finally
        {
            isRefreshing = false;
        }
    }

    private async Task RefreshActiveOnlyAsync()
    {
        if (!IsDisposed && !Disposing && ViewModel.Executions.Any(IsActiveStatus))
        {
            await RefreshAsync();
        }
    }

    protected override async Task ConsultAsync()
    {
        if (GetSelectedExecution() is not { } execution)
        {
            ShowWarning("Seleccione una ejecucion.");
            return;
        }

        using var form = new SyncExecutionDetailForm(DetailViewModel, Session, execution.Id);
        form.ShowDialog(this);
        await RefreshAsync();
    }

    private async Task CancelSelectedAsync()
    {
        if (GetSelectedExecution() is not { } execution)
        {
            return;
        }

        if (XtraMessageBox.Show(this, $"Cancelar la ejecucion {execution.Id}?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        await UiExceptionHandler.RunAsync(this, Text, async () =>
        {
            await ViewModel.CancelAsync(execution.Id);
            await RefreshAsync();
        });
    }

    private async Task RetrySelectedAsync()
    {
        if (GetSelectedExecution() is not { } execution)
        {
            return;
        }

        if (XtraMessageBox.Show(this, $"Reintentar la ejecucion {execution.Id}?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        await UiExceptionHandler.RunAsync(this, Text, async () =>
        {
            await ViewModel.RetryAsync(execution.Id);
            await RefreshAsync();
        });
    }

    private SyncProfileExecutionListItem? GetSelectedExecution()
    {
        return SelectedGridItem<SyncProfileExecutionListItem>();
    }

    public override bool CanExecuteCustomOperation(string operationKey)
    {
        return IsCustomOperation(operationKey, "cancel")
            ? Session.HasPermission(PermissionCodes.SyncConfigurationCancel)
            : IsCustomOperation(operationKey, "retry")
                ? Session.HasPermission(PermissionCodes.SyncConfigurationRetry)
                : base.CanExecuteCustomOperation(operationKey);
    }

    public override Task ExecuteCustomOperationAsync(string operationKey)
    {
        if (IsCustomOperation(operationKey, "cancel"))
        {
            return CancelSelectedAsync();
        }

        if (IsCustomOperation(operationKey, "retry"))
        {
            return RetrySelectedAsync();
        }

        return base.ExecuteCustomOperationAsync(operationKey);
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();

        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns)
        {
            column.Visible = false;
        }

        ConfigureColumn(nameof(SyncProfileExecutionListItem.Id), "Id", 0, 70);
        ConfigureColumn(nameof(SyncProfileExecutionListItem.ProfileCode), "Perfil", 1, 110);
        ConfigureColumn(nameof(SyncProfileExecutionListItem.ProfileName), "Nombre", 2, 200);
        ConfigureColumn(nameof(SyncProfileExecutionListItem.Status), "Estado", 3, 110);
        ConfigureColumn(nameof(SyncProfileExecutionListItem.ExecutionType), "Tipo", 4, 100);
        ConfigureColumn(nameof(SyncProfileExecutionListItem.RequestedAt), "Solicitado", 5, 145);
        ConfigureColumn(nameof(SyncProfileExecutionListItem.StartedAt), "Inicio", 6, 145);
        ConfigureColumn(nameof(SyncProfileExecutionListItem.FinishedAt), "Fin", 7, 145);
        ConfigureColumn(nameof(SyncProfileExecutionListItem.TotalEventsPublished), "Eventos", 8, 85);
        ConfigureColumn(nameof(SyncProfileExecutionListItem.TotalErrors), "Errores", 9, 85);
        ConfigureColumn(nameof(SyncProfileExecutionListItem.Message), "Mensaje", 10, 240);
    }

    private void ConfigureColumn(string fieldName, string caption, int visibleIndex, int width)
    {
        if (GridView.Columns[fieldName] is not { } column)
        {
            return;
        }

        column.Caption = caption;
        column.Visible = true;
        column.VisibleIndex = visibleIndex;
        column.Width = width;
    }

    private static bool IsCustomOperation(string operationKey, params string[] aliases)
    {
        var normalized = NormalizeOperation(operationKey);
        return aliases.Select(NormalizeOperation).Any(alias => string.Equals(normalized, alias, StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeOperation(string operationKey)
    {
        return operationKey
            .Replace("ACTION.", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsActiveStatus(SyncProfileExecutionListItem execution)
    {
        return string.Equals(execution.Status, "Pending", StringComparison.OrdinalIgnoreCase)
            || string.Equals(execution.Status, "Running", StringComparison.OrdinalIgnoreCase)
            || string.Equals(execution.Status, "Cancelling", StringComparison.OrdinalIgnoreCase);
    }

    private bool IsInDesignMode()
    {
        return LicenseManager.UsageMode == LicenseUsageMode.Designtime
            || DesignMode
            || Site?.DesignMode == true;
    }
}
