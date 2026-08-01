using System.ComponentModel;
using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Sap;
using NuanSystem.WinForms.Services.Sap.Models;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Sap;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.GridColumnSettings.Models;
using DevExpress.XtraGrid.Columns;

namespace NuanSystem.WinForms.Forms.Sap;

public sealed partial class SapSyncExecutionListForm : BaseCrudListForm
{
    public const string FormKey = "sap-sync-executions";
    private SapSyncExecutionsViewModel? viewModel;
    private ISapSyncManagementClient? client;
    private ApiSession? session;
    private long? fixedProfileId;
    private IGridColumnSettingsClient? columnSettingsClient;
    private bool refreshing;

    public SapSyncExecutionListForm()
    {
        InitializeComponent();
        FormStyler.ApplyBase(this);
    }

    public SapSyncExecutionListForm(
        SapSyncExecutionsViewModel viewModel,
        ISapSyncManagementClient client,
        ApiSession session,
        IGridColumnSettingsClient? columnSettingsClient = null,
        long? profileId = null) : this()
    {
        this.viewModel = viewModel;
        this.client = client;
        this.session = session;
        this.columnSettingsClient = columnSettingsClient;
        fixedProfileId = profileId;
        ConfigureCrudPermissions(session, new(
            PermissionCodes.SapSyncExecutionsView,
            "__SAP_EXEC_CREATE__",
            "__SAP_EXEC_EDIT__",
            "__SAP_EXEC_DELETE__"));
        ConfigureGrid();
        executionGrid.PageRequested += async (_, args) => await GoToPageAsync(args);
        executionGrid.GridView.DoubleClick += async (_, _) => await ExecuteConsultAsync();
        pollingTimer.Tick += async (_, _) =>
        {
            if (ViewModel.Executions.Any(item => SapSyncExecutionPolicy.IsActive(item.Status)))
            {
                await LoadDataAsync();
            }
        };
    }

    private SapSyncExecutionsViewModel ViewModel =>
        viewModel ?? throw new InvalidOperationException("El formulario requiere inyeccion de dependencias.");

    private ISapSyncManagementClient Client =>
        client ?? throw new InvalidOperationException("El formulario requiere inyeccion de dependencias.");

    private ApiSession Session =>
        session ?? throw new InvalidOperationException("El formulario requiere inyeccion de dependencias.");

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (!IsInDesignMode() && viewModel is not null)
        {
            pollingTimer.Start();
        }
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        pollingTimer.Stop();
        base.OnFormClosed(e);
    }

    protected override async Task LoadDataAsync()
    {
        if (refreshing || viewModel is null)
        {
            return;
        }

        refreshing = true;
        try
        {
            await RunWithBusyStateAsync(async () =>
            {
                ViewModel.Filter.ProfileId = fixedProfileId;
                await ViewModel.LoadAsync();
                executionGrid.SetPagedData(
                    ViewModel.Executions,
                    ViewModel.Filter.PageNumber,
                    ViewModel.Filter.PageSize,
                    ViewModel.TotalCount);
                await ApplyColumnSettingsAsync();
            });
        }
        finally
        {
            refreshing = false;
        }
    }

    protected override async Task ConsultAsync()
    {
        if (Selected() is not { } item)
        {
            ShowWarning("Seleccione una ejecucion SAP.");
            return;
        }

        using var form = new SapSyncExecutionDetailForm(
            new SapSyncExecutionDetailViewModel(Client),
            Session,
            item.ExecutionUid);
        form.ShowDialog(this);
        await LoadDataAsync();
    }

    protected override async Task CustomizeColumnsAsync()
    {
        if (columnSettingsClient is null)
        {
            await base.CustomizeColumnsAsync();
            return;
        }

        var current = executionGrid.GridView.Columns
            .Cast<GridColumn>()
            .Where(column => !string.IsNullOrWhiteSpace(column.FieldName))
            .Select(column => new GridColumnSettingItem(
                column.FieldName,
                column.Caption,
                column.Caption,
                column.Visible,
                column.VisibleIndex,
                column.Width))
            .ToArray();
        using var form = new GridColumnSettingsForm(current);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await columnSettingsClient.SaveAsync(FormKey, "MainGrid", form.Request);
        ApplyColumnSettings(form.Request.Select(item => new GridColumnSettingItem(
            item.FieldName,
            item.DefaultCaption,
            item.Caption,
            item.IsVisible,
            item.VisibleIndex,
            item.Width)));
        ShowSuccess("Columnas guardadas correctamente.");
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
        "retry" or "cancel" or "releaseexpiredlock" => OpenDetailActionAsync(),
        _ => base.ExecuteCustomOperationAsync(operationKey)
    };

    private void ConfigureGrid()
    {
        executionGrid.ConfigureColumns(
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionListItem.Id), Caption = "Id", VisibleIndex = 0, Width = 70, Format = NuanGridColumnFormat.Number },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionListItem.ProfileCode), Caption = "Perfil SAP", VisibleIndex = 1, Width = 110 },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionListItem.EntityCode), Caption = "Entidad", VisibleIndex = 2, Width = 120 },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionListItem.Direction), Caption = "Direccion", VisibleIndex = 3, Width = 110 },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionListItem.TriggerType), Caption = "Origen", VisibleIndex = 4, Width = 90 },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionListItem.Status), Caption = "Estado", VisibleIndex = 5, Width = 135, Format = NuanGridColumnFormat.StatusBadge },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionListItem.RequestedAtUtc), Caption = "Solicitada", VisibleIndex = 6, Width = 145, Format = NuanGridColumnFormat.DateTime },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionListItem.FinishedAtUtc), Caption = "Finalizada", VisibleIndex = 7, Width = 145, Format = NuanGridColumnFormat.DateTime },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionListItem.TotalRecords), Caption = "Registros", VisibleIndex = 8, Width = 85, Format = NuanGridColumnFormat.Number },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionListItem.SucceededRecords), Caption = "Correctos", VisibleIndex = 9, Width = 85, Format = NuanGridColumnFormat.Number },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionListItem.WarningRecords), Caption = "Avisos", VisibleIndex = 10, Width = 75, Format = NuanGridColumnFormat.Number },
            new NuanGridColumnDefinition { FieldName = nameof(SapSyncExecutionListItem.FailedRecords), Caption = "Errores", VisibleIndex = 11, Width = 75, Format = NuanGridColumnFormat.Number });
    }

    private async Task ApplyColumnSettingsAsync()
    {
        if (columnSettingsClient is null)
        {
            return;
        }

        ApplyColumnSettings(await columnSettingsClient.GetAsync(FormKey, "MainGrid"));
    }

    private void ApplyColumnSettings(IEnumerable<GridColumnSettingItem> settings)
    {
        foreach (var setting in settings)
        {
            if (executionGrid.GridView.Columns[setting.FieldName] is not { } column)
            {
                continue;
            }

            column.Caption = string.IsNullOrWhiteSpace(setting.Caption) ? setting.DefaultCaption : setting.Caption;
            column.Visible = setting.IsVisible;
            column.VisibleIndex = setting.VisibleIndex;
            column.Width = Math.Max(40, setting.Width);
        }
    }

    private async Task GoToPageAsync(NuanGridPageRequestEventArgs args)
    {
        ViewModel.Filter.PageNumber = args.Page;
        ViewModel.Filter.PageSize = args.PageSize;
        await LoadDataAsync();
    }

    private async Task OpenFilterAsync()
    {
        using var dialog = new SapSyncExecutionFilterDialog(ViewModel.Filter, fixedProfileId.HasValue);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        ViewModel.Filter.EntityCode = dialog.EntityCode;
        ViewModel.Filter.Direction = dialog.Direction;
        ViewModel.Filter.Status = dialog.Status;
        ViewModel.Filter.TriggerType = dialog.TriggerType;
        ViewModel.Filter.DateFromUtc = dialog.DateFromUtc;
        ViewModel.Filter.DateToUtc = dialog.DateToUtc;
        ViewModel.Filter.PageNumber = 1;
        await LoadDataAsync();
    }

    private async Task OpenDetailActionAsync()
    {
        if (Selected() is not { } item)
        {
            ShowWarning("Seleccione una ejecucion SAP.");
            return;
        }

        using var form = new SapSyncExecutionDetailForm(
            new SapSyncExecutionDetailViewModel(Client),
            Session,
            item.ExecutionUid);
        form.ShowDialog(this);
        await LoadDataAsync();
    }

    private SapSyncExecutionListItem? Selected() => executionGrid.GetFocusedRow<SapSyncExecutionListItem>();

    private static string Normalize(string value) => value
        .Replace("ACTION.", string.Empty, StringComparison.OrdinalIgnoreCase)
        .Replace("SAP_SYNC_EXECUTIONS.", string.Empty, StringComparison.OrdinalIgnoreCase)
        .Replace("-", string.Empty)
        .Replace("_", string.Empty)
        .ToLowerInvariant();

    private bool IsInDesignMode() =>
        LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode || Site?.DesignMode == true;
}
