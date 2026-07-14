using DevExpress.XtraEditors;
using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Sync;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Controls.Kpi;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.Sync.Models;
using NuanSystem.WinForms.ViewModels.Sync;

namespace NuanSystem.WinForms.Forms.Sync;

public sealed partial class SyncMonitorForm : XtraForm
{
    public const string FormKey = "sync-monitor";

    private readonly SyncMonitorViewModel monitorViewModel;
    private readonly SyncOutboxListViewModel outboxViewModel;
    private readonly SyncOutboxDetailViewModel detailViewModel;
    private readonly bool canViewAudit;

    public SyncMonitorForm()
    {
        monitorViewModel = null!;
        outboxViewModel = null!;
        detailViewModel = null!;
        InitializeComponent();
        ApplyDashboardTileStyles();
        ApplyFilterActionIcons();
    }

    public SyncMonitorForm(
        SyncMonitorViewModel monitorViewModel,
        SyncOutboxListViewModel outboxViewModel,
        SyncOutboxDetailViewModel detailViewModel,
        SyncAuditViewModel auditViewModel,
        ApiSession session)
    {
        this.monitorViewModel = monitorViewModel;
        this.outboxViewModel = outboxViewModel;
        this.detailViewModel = detailViewModel;
        canViewAudit = session.HasPermission(PermissionCodes.SyncAuditView);

        InitializeComponent();
        ApplyDashboardTileStyles();
        ApplyFilterActionIcons();
        ConfigureOutboxGrid();
        WireEvents();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await RefreshAllAsync();
    }

    private void WireEvents() {
        btnApplyFilters.Click += async (_, _) => await RefreshListsAsync();
        btnClearFilters.Click += async (_, _) =>
        {
            ClearFilters();
            await RefreshListsAsync();
        };
        outboxGrid.RowDoubleClick += async (_, _) => await OpenSelectedDetailAsync();
    }

    private async Task RefreshAllAsync()
    {
        await RunWithBusyStateAsync(async () =>
        {
            ApplyFiltersToViewModels();
            await monitorViewModel.LoadAsync();
            await outboxViewModel.LoadAsync();

            BindDashboard();
            BindOutbox();
        });
    }

    private async Task RefreshListsAsync()
    {
        await RunWithBusyStateAsync(async () =>
        {
            ApplyFiltersToViewModels();
            await outboxViewModel.LoadAsync();

            BindOutbox();
        });
    }

    private void BindDashboard()
    {
        var dashboard = monitorViewModel.Dashboard;
        var summary = monitorViewModel.Summary;
        cardPending.ValueText = (dashboard?.TotalPending ?? summary?.TotalPending ?? 0).ToString("N0");
        cardInProcess.ValueText = (dashboard?.TotalInProcess ?? summary?.TotalInProcess ?? 0).ToString("N0");
        cardApplied.ValueText = (dashboard?.TotalApplied ?? summary?.TotalApplied ?? 0).ToString("N0");
        cardError.ValueText = (dashboard?.TotalErrors ?? summary?.TotalErrors ?? 0).ToString("N0");
        cardDeadLetter.ValueText = (dashboard?.TotalDeadLetter ?? summary?.TotalDeadLetter ?? 0).ToString("N0");
        cardIgnored.ValueText = (dashboard?.TotalIgnored ?? summary?.TotalIgnored ?? 0).ToString("N0");
    }

    private void ApplyDashboardTileStyles()
    {
        cardPending.ApplyStyle(NuanKpiCardStyle.Pending());
        cardPending.Description = "Pendientes de procesamiento";
        cardPending.FallbackIconText = "P";
        cardPending.UseSvgIcon = true;
        cardPending.SvgIcon = OperationButtonIcons.LoadOperationIcon("dashboard_32.svg", Color.FromArgb(226, 232, 240));

        cardInProcess.ApplyStyle(NuanKpiCardStyle.InProcess());
        cardInProcess.Description = "En proceso actualmente";
        cardInProcess.FallbackIconText = "R";
        cardInProcess.UseSvgIcon = true;
        cardInProcess.SvgIcon = OperationButtonIcons.LoadOperationIcon("actualizar_32.svg", Color.White);

        cardApplied.ApplyStyle(NuanKpiCardStyle.Applied());
        cardApplied.Description = "Aplicados correctamente";
        cardApplied.FallbackIconText = "OK";
        cardApplied.UseSvgIcon = true;
        cardApplied.SvgIcon = OperationButtonIcons.LoadOperationIcon("aprobar_32.svg", Color.White);

        cardError.ApplyStyle(NuanKpiCardStyle.Error());
        cardError.Description = "Con errores";
        cardError.FallbackIconText = "!";
        cardError.UseSvgIcon = true;
        cardError.SvgIcon = OperationButtonIcons.LoadOperationIcon("rechazar_32.svg", Color.White);

        cardDeadLetter.ApplyStyle(NuanKpiCardStyle.DeadLetter());
        cardDeadLetter.Description = "Movidos a DeadLetter";
        cardDeadLetter.FallbackIconText = "DL";
        cardDeadLetter.UseSvgIcon = true;
        cardDeadLetter.SvgIcon = OperationButtonIcons.LoadOperationIcon("rechazar_32.svg", Color.White);

        cardIgnored.ApplyStyle(NuanKpiCardStyle.Ignored());
        cardIgnored.Description = "Ignorados por reglas";
        cardIgnored.FallbackIconText = "I";
        cardIgnored.UseSvgIcon = true;
        cardIgnored.SvgIcon = OperationButtonIcons.LoadOperationIcon("ver_detalle_32.svg", Color.FromArgb(229, 231, 235));
    }

    private void ApplyFilterActionIcons()
    {
        btnApplyFilters.ImageOptions.SvgImage = OperationButtonIcons.LoadOperationIcon("buscar_32.svg", Color.White);
        btnApplyFilters.ImageOptions.SvgImageSize = new Size(32, 32);
        btnClearFilters.ImageOptions.SvgImage = OperationButtonIcons.LoadOperationIcon("limpiar_filtros_32.svg", Color.White);
        btnClearFilters.ImageOptions.SvgImageSize = new Size(32, 32);
    }

    private void ConfigureOutboxGrid()
    {
        outboxGrid.ShowPagination = true;
        outboxGrid.PageSize = 50;
        outboxGrid.ShowFindPanel = true;
        outboxGrid.MultiSelect = false;
        outboxGrid.ShowSelectionCheckBox = false;
        outboxGrid.SetStatusBadgeProvider(value => Convert.ToString(value) switch
        {
            nameof(SyncEventStatus.InProcess) => NuanGridBadgeStyle.Info,
            nameof(SyncEventStatus.Applied) => NuanGridBadgeStyle.Success,
            nameof(SyncEventStatus.Error) => NuanGridBadgeStyle.Error,
            nameof(SyncEventStatus.DeadLetter) => NuanGridBadgeStyle.Critical,
            nameof(SyncEventStatus.Pending) => NuanGridBadgeStyle.Neutral,
            nameof(SyncEventStatus.Ignored) => NuanGridBadgeStyle.Neutral,
            _ => NuanGridBadgeStyle.Neutral
        });
        outboxGrid.ConfigureColumns(
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxListItem.Id),
                Caption = "Id",
                VisibleIndex = 0,
                Width = 80,
                Format = NuanGridColumnFormat.Number
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxListItem.Status),
                Caption = "Estado",
                VisibleIndex = 1,
                Width = 130,
                Format = NuanGridColumnFormat.StatusBadge
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxListItem.EntityName),
                Caption = "Entidad",
                VisibleIndex = 2,
                Width = 160,
                Format = NuanGridColumnFormat.Text
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxListItem.EntityCode),
                Caption = "Codigo",
                VisibleIndex = 3,
                Width = 130,
                Format = NuanGridColumnFormat.Text
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxListItem.Operation),
                Caption = "Operacion",
                VisibleIndex = 4,
                Width = 120,
                Format = NuanGridColumnFormat.Text
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxListItem.AttemptCount),
                Caption = "Intentos",
                VisibleIndex = 5,
                Width = 90,
                Format = NuanGridColumnFormat.Number
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxListItem.CreatedAt),
                Caption = "Creado",
                VisibleIndex = 6,
                Width = 150,
                Format = NuanGridColumnFormat.DateTime
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxListItem.ProcessedAt),
                Caption = "Procesado",
                VisibleIndex = 7,
                Width = 150,
                Format = NuanGridColumnFormat.DateTime
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxListItem.NextRetryAt),
                Caption = "Proximo retry",
                VisibleIndex = 8,
                Width = 150,
                Format = NuanGridColumnFormat.DateTime
            },
            new NuanGridColumnDefinition
            {
                FieldName = nameof(SyncOutboxListItem.LastErrorMessage),
                Caption = "Error",
                VisibleIndex = 9,
                Width = 260,
                Format = NuanGridColumnFormat.Text
            });
    }

    private void BindOutbox()
    {
        outboxGrid.SetData(outboxViewModel.Items.ToList());
    }

    private void ApplyFiltersToViewModels()
    {
        var status = ResolveSelectedStatus();
        var entityText = txtEntity.Text.Trim();
        var entityName = string.IsNullOrWhiteSpace(entityText) || entityText == "(Todas)" ? null : entityText;
        var from = dateFrom.EditValue as DateTime?;
        var to = dateTo.EditValue as DateTime?;

        outboxViewModel.Status = status;
        outboxViewModel.EntityName = entityName;
        outboxViewModel.CreatedFrom = from;
        outboxViewModel.CreatedTo = to;
        outboxViewModel.DeadLetterOnly = chkDeadLetterOnly.Checked;
        outboxViewModel.HasErrors = chkHasErrors.Checked;
    }

    private void ClearFilters()
    {
        cmbStatus.SelectedIndex = 0;
        txtEntity.Text = "(Todas)";
        txtBranch.Text = "(Todas)";
        dateFrom.EditValue = null;
        dateTo.EditValue = null;
        chkDeadLetterOnly.Checked = false;
        chkHasErrors.Checked = false;
    }

    private SyncEventStatus? ResolveSelectedStatus()
    {
        return cmbStatus.SelectedItem?.ToString() switch
        {
            "Pending" => SyncEventStatus.Pending,
            "InProcess" => SyncEventStatus.InProcess,
            "Applied" => SyncEventStatus.Applied,
            "Error" => SyncEventStatus.Error,
            "DeadLetter" => SyncEventStatus.DeadLetter,
            "Ignored" => SyncEventStatus.Ignored,
            _ => null
        };
    }

    private SyncOutboxListItem? SelectedOutboxItem()
    {
        return outboxGrid.GetFocusedRow<SyncOutboxListItem>();
    }

    private async Task OpenSelectedDetailAsync()
    {
        if (SelectedOutboxItem() is not { } item)
        {
            XtraMessageBox.Show(this, "Seleccione un evento de SyncOutbox.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        await RunWithBusyStateAsync(async () =>
        {
            await detailViewModel.LoadAsync(item.Id, canViewAudit);
            using var form = new SyncOutboxDetailForm(detailViewModel, canViewAudit);
            form.ShowDialog(this);
            if (form.ManualActionCompleted)
            {
                await RefreshAllAsync();
            }
        });
    }

    private async Task RunWithBusyStateAsync(Func<Task> action)
    {
        ToggleBusy(false);
        try
        {
            await UiExceptionHandler.RunAsync(this, Text, action);
        }
        finally
        {
            ToggleBusy(true);
        }
    }

    private void ToggleBusy(bool enabled) {
        btnApplyFilters.Enabled = enabled;
        btnClearFilters.Enabled = enabled;
        Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
    }
}
